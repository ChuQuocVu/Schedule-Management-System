
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
            this.components = new System.ComponentModel.Container();
            this.txt_TimeIn = new System.Windows.Forms.TextBox();
            this.txt_TImeOut = new System.Windows.Forms.TextBox();
            this.txt_Status = new System.Windows.Forms.TextBox();
            this.txt_FilterName = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.timer2 = new System.Windows.Forms.Timer(this.components);
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
            // Filter
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.GradientActiveCaption;
            this.ClientSize = new System.Drawing.Size(469, 181);
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

        public System.Windows.Forms.TextBox txt_TimeIn;
        public System.Windows.Forms.TextBox txt_TImeOut;
        public System.Windows.Forms.TextBox txt_Status;
        public System.Windows.Forms.TextBox txt_FilterName;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Timer timer2;
    }
}