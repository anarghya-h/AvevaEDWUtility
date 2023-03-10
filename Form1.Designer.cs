
namespace AvevaUtility
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.Windows.Forms.Label label1;
            textBox1 = new System.Windows.Forms.TextBox();
            button2 = new System.Windows.Forms.Button();
            button1 = new System.Windows.Forms.Button();
            textBox2 = new System.Windows.Forms.TextBox();
            label2 = new System.Windows.Forms.Label();
            button3 = new System.Windows.Forms.Button();
            button4 = new System.Windows.Forms.Button();
            button5 = new System.Windows.Forms.Button();
            textBox3 = new System.Windows.Forms.TextBox();
            label3 = new System.Windows.Forms.Label();
            label1 = new System.Windows.Forms.Label();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            label1.Location = new System.Drawing.Point(51, 29);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(89, 23);
            label1.TabIndex = 0;
            label1.Text = "Old Folder";
            // 
            // textBox1
            // 
            textBox1.Location = new System.Drawing.Point(161, 29);
            textBox1.Name = "textBox1";
            textBox1.Size = new System.Drawing.Size(531, 27);
            textBox1.TabIndex = 1;
            // 
            // button2
            // 
            button2.Location = new System.Drawing.Point(714, 29);
            button2.Name = "button2";
            button2.Size = new System.Drawing.Size(32, 27);
            button2.TabIndex = 3;
            button2.Text = "..";
            button2.UseVisualStyleBackColor = true;
            button2.Click += button2_Click;
            // 
            // button1
            // 
            button1.Location = new System.Drawing.Point(714, 77);
            button1.Name = "button1";
            button1.Size = new System.Drawing.Size(32, 27);
            button1.TabIndex = 6;
            button1.Text = "..";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click;
            // 
            // textBox2
            // 
            textBox2.Location = new System.Drawing.Point(161, 77);
            textBox2.Name = "textBox2";
            textBox2.Size = new System.Drawing.Size(531, 27);
            textBox2.TabIndex = 5;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            label2.Location = new System.Drawing.Point(44, 77);
            label2.Name = "label2";
            label2.Size = new System.Drawing.Size(96, 23);
            label2.TabIndex = 4;
            label2.Text = "New Folder";
            // 
            // button3
            // 
            button3.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            button3.Location = new System.Drawing.Point(242, 190);
            button3.Name = "button3";
            button3.Size = new System.Drawing.Size(100, 35);
            button3.TabIndex = 7;
            button3.Text = "Submit";
            button3.UseVisualStyleBackColor = true;
            button3.Click += button3_Click;
            // 
            // button4
            // 
            button4.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            button4.Location = new System.Drawing.Point(452, 190);
            button4.Name = "button4";
            button4.Size = new System.Drawing.Size(100, 35);
            button4.TabIndex = 8;
            button4.Text = "Exit";
            button4.UseVisualStyleBackColor = true;
            button4.Click += button4_Click;
            // 
            // button5
            // 
            button5.Location = new System.Drawing.Point(714, 128);
            button5.Name = "button5";
            button5.Size = new System.Drawing.Size(32, 27);
            button5.TabIndex = 11;
            button5.Text = "..";
            button5.UseVisualStyleBackColor = true;
            button5.Click += button5_Click;
            // 
            // textBox3
            // 
            textBox3.Location = new System.Drawing.Point(161, 128);
            textBox3.Name = "textBox3";
            textBox3.Size = new System.Drawing.Size(531, 27);
            textBox3.TabIndex = 10;
            // 
            // label3
            // 
            label3.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            label3.Location = new System.Drawing.Point(12, 132);
            label3.MinimumSize = new System.Drawing.Size(150, 50);
            label3.Name = "label3";
            label3.Size = new System.Drawing.Size(168, 50);
            label3.TabIndex = 9;
            label3.Text = "Select Destination \r\nFolder";
            // 
            // Form1
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(800, 253);
            Controls.Add(button5);
            Controls.Add(textBox3);
            Controls.Add(label3);
            Controls.Add(button4);
            Controls.Add(button3);
            Controls.Add(button1);
            Controls.Add(textBox2);
            Controls.Add(label2);
            Controls.Add(button2);
            Controls.Add(textBox1);
            Controls.Add(label1);
            MinimumSize = new System.Drawing.Size(18, 250);
            Name = "Form1";
            Text = "Aveva Utility";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.Button button5;
        private System.Windows.Forms.TextBox textBox3;
        private System.Windows.Forms.Label label3;
    }
}

