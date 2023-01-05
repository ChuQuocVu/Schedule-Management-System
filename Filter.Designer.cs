
namespace Schedule_Management
{
    partial class Filter
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.txt_TimeIn = new System.Windows.Forms.TextBox();
            this.txt_TImeOut = new System.Windows.Forms.TextBox();
            this.txt_Status = new System.Windows.Forms.TextBox();
            this.txt_FilterName = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.btn_Filter = new System.Windows.Forms.Button();
            this.datePicker1 = new System.Windows.Forms.DateTimePicker();
            this.SuspendLayout();
            // 
            // txt_TimeIn
            // 
            this.txt_TimeIn.Location = new System.Drawing.Point(45, 123);
            this.txt_TimeIn.Name = "txt_TimeIn";
            this.txt_TimeIn.Size = new System.Drawing.Size(118, 22);
            this.txt_TimeIn.TabIndex = 0;
            this.txt_TimeIn.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // txt_TImeOut
            // 
            this.txt_TImeOut.Location = new System.Drawing.Point(169, 123);
            this.txt_TImeOut.Name = "txt_TImeOut";
            this.txt_TImeOut.Size = new System.Drawing.Size(118, 22);
            this.txt_TImeOut.TabIndex = 1;
            this.txt_TImeOut.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // txt_Status
            // 
            this.txt_Status.Location = new System.Drawing.Point(293, 123);
            this.txt_Status.Name = "txt_Status";
            this.txt_Status.Size = new System.Drawing.Size(118, 22);
            this.txt_Status.TabIndex = 2;
            this.txt_Status.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // txt_FilterName
            // 
            this.txt_FilterName.Location = new System.Drawing.Point(42, 41);
            this.txt_FilterName.Multiline = true;
            this.txt_FilterName.Name = "txt_FilterName";
            this.txt_FilterName.Size = new System.Drawing.Size(262, 40);
            this.txt_FilterName.TabIndex = 3;
            this.txt_FilterName.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(39, 14);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(124, 18);
            this.label1.TabIndex = 4;
            this.label1.Text = "Student\'s name";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(42, 102);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(63, 18);
            this.label5.TabIndex = 8;
            this.label5.Text = "Time in";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(169, 103);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(74, 18);
            this.label2.TabIndex = 9;
            this.label2.Text = "Time out";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(290, 102);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(56, 18);
            this.label3.TabIndex = 10;
            this.label3.Text = "Status";
            // 
            // btn_Filter
            // 
            this.btn_Filter.Location = new System.Drawing.Point(317, 41);
            this.btn_Filter.Name = "btn_Filter";
            this.btn_Filter.Size = new System.Drawing.Size(115, 40);
            this.btn_Filter.TabIndex = 11;
            this.btn_Filter.Text = "Search";
            this.btn_Filter.UseVisualStyleBackColor = true;
            this.btn_Filter.Click += new System.EventHandler(this.btn_Filter_Click);
            // 
            // datePicker1
            // 
            this.datePicker1.CustomFormat = "MM/dd/yyyy";
            this.datePicker1.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.datePicker1.Location = new System.Drawing.Point(183, 11);
            this.datePicker1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.datePicker1.Name = "datePicker1";
            this.datePicker1.Size = new System.Drawing.Size(121, 22);
            this.datePicker1.TabIndex = 55;
            this.datePicker1.ValueChanged += new System.EventHandler(this.datePicker1_ValueChanged);
            // 
            // Filter
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.GradientActiveCaption;
            this.ClientSize = new System.Drawing.Size(469, 181);
            this.Controls.Add(this.datePicker1);
            this.Controls.Add(this.btn_Filter);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txt_FilterName);
            this.Controls.Add(this.txt_Status);
            this.Controls.Add(this.txt_TImeOut);
            this.Controls.Add(this.txt_TimeIn);
            this.Name = "Filter";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Filter";
            this.Load += new System.EventHandler(this.Filter_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txt_TimeIn;
        private System.Windows.Forms.TextBox txt_TImeOut;
        private System.Windows.Forms.TextBox txt_Status;
        private System.Windows.Forms.TextBox txt_FilterName;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button btn_Filter;
        private System.Windows.Forms.DateTimePicker datePicker1;
    }
}