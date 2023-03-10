using System;
using System.Windows.Forms;

namespace AvevaUtility
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog outputFolderDlg = new()
            {
                ShowNewFolderButton = true
            };
            if (outputFolderDlg.ShowDialog() == DialogResult.OK)
            {
                textBox1.Text = outputFolderDlg.SelectedPath;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog outputFolderDlg = new()
            {
                ShowNewFolderButton = true,
            };
            if (outputFolderDlg.ShowDialog() == DialogResult.OK)
            {
                textBox2.Text = outputFolderDlg.SelectedPath;
            }

        }

        private void button5_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog outputFolderDlg = new()
            {
                ShowNewFolderButton = true
            };
            if (outputFolderDlg.ShowDialog() == DialogResult.OK)
            {
                textBox3.Text = outputFolderDlg.SelectedPath;
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
                     
            try
            {
                CalculateDelta calculateDelta = new(textBox1.Text, textBox2.Text, textBox3.Text);
                calculateDelta.ProcessFiles();
                MessageBox.Show("Files Processed Successfully", "Success");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Could not process the files: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private void button4_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}
