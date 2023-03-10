using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using ExcelDataReader;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Drawing.Design;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AvevaUtility
{
    class CalculateDelta
    {
        #region Private members
        private readonly string sourceFolderPath;
        private readonly string destFolderPath;
        private readonly string outputFolder;
        private StringBuilder expression;
        private string Filename;
        DataSet sourceFileData, destFileData;
        int lb, ub;
        readonly List<object[]> New, Updated, Deleted;
        readonly List<int> keyIndex;
        DataColumnCollection headers;
        #endregion

        #region Constructor
        public CalculateDelta(string sourcePath, string destPath, string opFolder)
        {
            sourceFolderPath = sourcePath;
            destFolderPath = destPath;
            outputFolder = opFolder;
            
            New = new();
            Updated = new();
            Deleted = new();
            keyIndex = new();
            
        }
        #endregion

        #region Private methods
        //Method to read the data from the identical files in 2 different folders
        void ReadDataFromFiles(string oldFile, string newFile)
        {
            try
            {
                Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
                IExcelDataReader reader1, reader2;
                List<Task> tasks = new();

                //Reading the list of identifiers from the config file
                dynamic fileIdentifiers;
                using (StreamReader r = new StreamReader("Identifiers.json"))
                {
                    string json = r.ReadToEnd();
                    fileIdentifiers = JsonConvert.DeserializeObject(json);
                }

                //Opening the files and reading using ExcelDataReader
                using var File1Stream = File.Open(oldFile, FileMode.Open, FileAccess.Read);
                using var File2Stream = File.Open(newFile, FileMode.Open, FileAccess.Read);

                //Checking the file extension
                if (Path.GetExtension(oldFile) == ".xlsx")
                {
                    reader1 = ExcelReaderFactory.CreateReader(File1Stream);
                    reader2 = ExcelReaderFactory.CreateReader(File2Stream);
                }
                else
                {
                    reader1 = ExcelReaderFactory.CreateCsvReader(File1Stream);
                    reader2 = ExcelReaderFactory.CreateCsvReader(File2Stream);
                }

                //Tasks to read the files simultaneously
                tasks.Add(Task.Run(() => sourceFileData = reader1.AsDataSet(new ExcelDataSetConfiguration()
                {
                    ConfigureDataTable = (tablereader) => new ExcelDataTableConfiguration()
                    {
                        UseHeaderRow = true
                    }
                })));

                tasks.Add(Task.Run(() => destFileData = reader2.AsDataSet(new ExcelDataSetConfiguration()
                {
                    ConfigureDataTable = (tablereader) => new ExcelDataTableConfiguration()
                    {
                        UseHeaderRow = true
                    }
                })));

                Task.WaitAll(tasks.ToArray());

                //Obtaining the identifier corresponding to the uploaded file
                var identifier = ((IEnumerable)fileIdentifiers).Cast<dynamic>().FirstOrDefault(x => Path.GetFileNameWithoutExtension(oldFile).ToUpper().Contains(x.Path.ToUpper())) ?? throw new ArgumentNullException(paramName: oldFile, message: "Identifier not available for the given file: " + Path.GetFileName(oldFile));
                var key = identifier.Value.ToString().Split(';');
                Filename = identifier.Path;

                //Obtaining the index of each of the identifiers
                foreach (var item in key)
                {
                    keyIndex.Add(sourceFileData.Tables[0].Columns.IndexOf(item));
                }

                //Creating placeholder for filter expression
                expression = new();
                expression.Append("[" + key[0] + "]='{0}'");
                if (key.Length > 1)
                {
                    for (int i = 1; i < key.Length; i++)
                    {
                        expression.Append(" and [" + key[i] + "]='{" + i + "}'");
                    }
                }

                //Obtaining the headers of the file
                headers = sourceFileData.Tables[0].Columns;
            }
            catch(Exception e)
            {
                throw new Exception("Could not read the files of " + Path.GetFileName(oldFile) + ": " + e.Message);
            }
            
        }

        //Method to find the updated and deleted records
        void getDeletedAndUpdatedRecords(int lb, int ub)
        {
            try
            {
                for (int i = lb; i < ub; i++)
                {
                    var Row = sourceFileData.Tables[0].Rows[i];

                    //Obtaining the values present in the identifier columns for the row
                    List<string> keyValues = new();
                    foreach (var j in keyIndex)
                    {
                        keyValues.Add(Row.ItemArray[j].ToString().Replace("'", @"\''"));
                    }

                    if (keyValues.Count != 0)
                    {
                        //Creating the filter expression
                        string filterExpression = string.Format(expression.ToString(), keyValues.ToArray());

                        //Selecting the record in second file using filter
                        var Row2 = destFileData.Tables[0].Select(filterExpression);

                        if (Row2 != null && Row2.Length == 0)
                        {
                            //Record not present in new file
                            Deleted.Add(Row.ItemArray);
                        }
                            
                        else if (Row2 != null)
                        {
                            //Iterating through each record in case of multiple records found
                            foreach (var row in Row2.Select(x => x.ItemArray))
                            {
                                if (!Enumerable.SequenceEqual(Row.ItemArray, row, new ItemArrayComparer()))
                                    Updated.Add(row);
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                throw new Exception("Could not find the updated and deleted records for file " + Filename + ": " + e.Message);
            }

        }

        void getNewRecords(int lb, int ub)
        {
            try
            {
                for (int i = lb; i < ub; i++)
                {
                    var Row = destFileData.Tables[0].Rows[i];

                    //Obtaining the values present in the identifier columns for the row
                    List<string> keyValues = new();
                    foreach (var j in keyIndex)
                    {
                        keyValues.Add(Row.ItemArray[j].ToString().Replace("'", @"\''"));
                    }

                    if (keyValues.Count != 0)
                    {
                        //Creating the filter expression
                        string s = string.Format(expression.ToString(), keyValues.ToArray());

                        //Selecting the record in second file using filter
                        var Row2 = sourceFileData.Tables[0].Select(s);

                        if (Row2.Length == 0)
                        {
                            //Record not present in old file
                            New.Add(Row.ItemArray);
                        }
                    }

                }
            }
            catch(Exception ex)
            {
                throw new Exception("Could not find the new records for file" + Filename + ": " + ex.Message);
            }
            
        }

        void WriteDeltaFileNewAndUpdated()
        {
            WorksheetPart worksheetPart;

            SpreadsheetDocument spreadsheetDocument = SpreadsheetDocument.Create(outputFolder + "\\Delta - " + Filename + " - New and Updated.xlsx", SpreadsheetDocumentType.Workbook);

            try
            {
                WorkbookPart workbookPart = spreadsheetDocument.AddWorkbookPart();

                worksheetPart = workbookPart.AddNewPart<WorksheetPart>();

                Workbook workbook = new();
                FileVersion fileVersion = new()
                {
                    ApplicationName = "Microsoft Office Excel"
                };

                Worksheet worksheet = new();

                SheetData sheetData = new();

                //Writing the headers
                Row headerRow = new();
                for (int i = 0; i < headers.Count; i++)
                {
                    Cell cell = new()
                    {
                        CellValue = new CellValue(headers[i].ToString()),
                        DataType = CellValues.String
                    };
                    headerRow.AppendChild(cell);
                }

                sheetData.AppendChild(headerRow);

                //Writing the new records
                if(New.Count != 0)
                {
                    foreach (var newRec in New)
                    {
                        Row row = new();
                        foreach (var r in newRec)
                        {
                            Cell c = new()
                            {
                                CellValue = new CellValue(r.ToString()),
                                DataType = CellValues.String
                            };
                            row.AppendChild(c);
                        }
                        sheetData.AppendChild(row);
                    }
                }
                

                //Writing the updated records
                if(Updated.Count != 0)
                {
                    foreach (var record in Updated)
                    {
                        Row row = new();
                        foreach (var r in record)
                        {
                            Cell c = new()
                            {
                                CellValue = new CellValue(r.ToString()),
                                DataType = CellValues.String
                            };
                            row.AppendChild(c);
                        }
                        sheetData.AppendChild(row);
                    }
                }
                
                worksheet.AppendChild(sheetData);
                worksheetPart.Worksheet = worksheet;
                worksheetPart.Worksheet.Save();
                Sheets sheets = new();
                Sheet sheet = new()
                {
                    Id = spreadsheetDocument.WorkbookPart.GetIdOfPart(worksheetPart),
                    SheetId = 1,
                    Name = "New and Updated"
                };
                sheets.AppendChild(sheet);
                workbook.AppendChild(fileVersion);
                workbook.AppendChild(sheets);
                spreadsheetDocument.WorkbookPart.Workbook = workbook;
                spreadsheetDocument.WorkbookPart.Workbook.Save();
                spreadsheetDocument.Save();
                spreadsheetDocument.Close();
            }
            catch (Exception ex)
            {
                spreadsheetDocument.Save();
                spreadsheetDocument.Close();
                throw new Exception("Could not write the updated records to the file: " + ex.Message);
            }
        }

        void WriteDeltaFileDeleted()
        {
            WorksheetPart worksheetPart;

            SpreadsheetDocument spreadsheetDocument = SpreadsheetDocument.Create(outputFolder + "\\Delta - " + Filename + " - Deleted.xlsx", SpreadsheetDocumentType.Workbook);

            try
            {
                WorkbookPart workbookPart = spreadsheetDocument.AddWorkbookPart();

                worksheetPart = workbookPart.AddNewPart<WorksheetPart>();

                Workbook workbook = new();
                FileVersion fileVersion = new()
                {
                    ApplicationName = "Microsoft Office Excel"
                };

                Worksheet worksheet = new();

                SheetData sheetData = new();

                //writing the headers
                Row headerRow = new();
                for (int i = 0; i < headers.Count; i++)
                {
                    Cell cell = new()
                    {
                        CellValue = new CellValue(headers[i].ToString()),
                        DataType = CellValues.String
                    };
                    headerRow.AppendChild(cell);
                }

                Cell c = new()
                {
                    CellValue = new CellValue("Action"),
                    DataType = CellValues.String
                };
                headerRow.AppendChild(c);

                sheetData.AppendChild(headerRow);

                //Writing the deleted records
                foreach (var record in Deleted)
                {
                    Row row = new();
                    foreach (var r in record)
                    {
                        Cell cell = new()
                        {
                            CellValue = new CellValue(r.ToString()),
                            DataType = CellValues.String
                        };
                        row.AppendChild(cell);
                    }
                    c = new()
                    {
                        CellValue = new CellValue("Delete"),
                        DataType = CellValues.String
                    };
                    row.AppendChild(c);
                    sheetData.AppendChild(row);
                }

                worksheet.AppendChild(sheetData);
                worksheetPart.Worksheet = worksheet;
                worksheetPart.Worksheet.Save();
                Sheets sheets = new();
                Sheet sheet = new()
                {
                    Id = spreadsheetDocument.WorkbookPart.GetIdOfPart(worksheetPart),
                    SheetId = 1,
                    Name = "Deleted"
                };
                sheets.AppendChild(sheet);
                workbook.AppendChild(fileVersion);
                workbook.AppendChild(sheets);
                spreadsheetDocument.WorkbookPart.Workbook = workbook;
                spreadsheetDocument.WorkbookPart.Workbook.Save();
                spreadsheetDocument.Save();
                spreadsheetDocument.Close();
            }
            catch (Exception ex)
            {
                spreadsheetDocument.Save();
                spreadsheetDocument.Close();
                throw new Exception("Could not write the deleted records to the file: " + ex.Message);
            }
        }
        #endregion

        #region Public Methods
        public void ProcessFiles()
        {
            try
            {
                //Obtaining the files in the old folder
                var listOfFiles = Directory.GetFiles(sourceFolderPath);
                foreach (var file in listOfFiles)
                {
                    //finding the same file in the new directory
                    var newFile = Directory.GetFiles(destFolderPath, "*" + Path.GetFileNameWithoutExtension(file) + "*");

                    if (newFile.Length == 0 || newFile.Length > 1)
                    {
                        continue;
                    }

                    //Reading data form files
                    ReadDataFromFiles(file, newFile[0]);

                    //Dividing the records in batches of 2500 and obtaining the delta
                    lb = 0;
                    ub = 2500;
                    if (sourceFileData.Tables[0].Rows.Count <= ub)
                    {
                        getDeletedAndUpdatedRecords(0, sourceFileData.Tables[0].Rows.Count);
                    }
                    else
                    {
                        //Creating tasks for each batch of 2500 records to find the updated and deleted records
                        List<Task> tasks = new();
                        var noOfTasks = (int)Math.Ceiling((double)sourceFileData.Tables[0].Rows.Count / 2500);
                        ThreadPool.SetMinThreads(8, 8);
                        ThreadPool.SetMaxThreads(8, 8);
                        for (int i = 0; i < noOfTasks; i++)
                        {
                            if (ub > sourceFileData.Tables[0].Rows.Count)
                                ub = sourceFileData.Tables[0].Rows.Count;

                            var l = lb;
                            var u = ub;

                            tasks.Add(Task.Run(() => getDeletedAndUpdatedRecords(l, u)));

                            if (ub != sourceFileData.Tables[0].Rows.Count)
                            {
                                lb = ub;
                                ub += 2500;
                            }
                        }

                        Task.WaitAll(tasks.ToArray());
                    }

                    Console.WriteLine("Finding new records started: " + DateTime.Now);
                    lb = 0;
                    ub = 2500;
                    if (destFileData.Tables[0].Rows.Count <= ub)
                    {
                        getNewRecords(0, destFileData.Tables[0].Rows.Count);
                    }
                    else
                    {
                        //Creating tasks for each batch of 2500 records to find the new records
                        List<Task> tasks = new();
                        var noOfTasks = (int)Math.Ceiling((double)destFileData.Tables[0].Rows.Count / 2500);
                        ThreadPool.SetMinThreads(8, 8);
                        ThreadPool.SetMaxThreads(8, 8);
                        for (int i = 0; i < noOfTasks; i++)
                        {
                            if (ub > destFileData.Tables[0].Rows.Count)
                                ub = destFileData.Tables[0].Rows.Count;

                            var l = lb;
                            var u = ub;

                            tasks.Add(Task.Run(() => getNewRecords(l, u)));

                            if (ub != destFileData.Tables[0].Rows.Count)
                            {
                                lb = ub;
                                ub += 2500;
                            }
                        }

                        Task.WaitAll(tasks.ToArray());
                    }

                    //Writing the new, updated and deleted records into the excel file
                    if (New.Count != 0 || Updated.Count != 0)
                        WriteDeltaFileNewAndUpdated();
                    if (Deleted.Count != 0)
                        WriteDeltaFileDeleted();

                    New.Clear();
                    Updated.Clear();
                    Deleted.Clear();
                    keyIndex.Clear();

                }
            }
            catch(Exception  ex)
            {
                throw new Exception(ex.Message);
            }
            
        }
        #endregion
    }
}
