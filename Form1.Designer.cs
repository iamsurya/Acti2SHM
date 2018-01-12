namespace Acti2SHM
{
    partial class Frm_MainForm
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
            this.BTN_PickFile = new System.Windows.Forms.Button();
            this.TB_infile = new System.Windows.Forms.TextBox();
            this.LB_File = new System.Windows.Forms.Label();
            this.LB_of = new System.Windows.Forms.Label();
            this.TB_outfile = new System.Windows.Forms.TextBox();
            this.PB_convert = new System.Windows.Forms.ProgressBar();
            this.LB_prog = new System.Windows.Forms.Label();
            this.BTN_go = new System.Windows.Forms.Button();
            this.LB_stat = new System.Windows.Forms.Label();
            this.lb_Status = new System.Windows.Forms.Label();
            this.LB_DateLabel = new System.Windows.Forms.Label();
            this.dtp_dateTimePicker = new System.Windows.Forms.DateTimePicker();
            this.SuspendLayout();
            // 
            // BTN_PickFile
            // 
            this.BTN_PickFile.Location = new System.Drawing.Point(441, 33);
            this.BTN_PickFile.Name = "BTN_PickFile";
            this.BTN_PickFile.Size = new System.Drawing.Size(75, 23);
            this.BTN_PickFile.TabIndex = 0;
            this.BTN_PickFile.Text = "Browse...";
            this.BTN_PickFile.UseVisualStyleBackColor = true;
            this.BTN_PickFile.Click += new System.EventHandler(this.btn_PickFile_Click);
            // 
            // TB_infile
            // 
            this.TB_infile.Location = new System.Drawing.Point(80, 34);
            this.TB_infile.Name = "TB_infile";
            this.TB_infile.Size = new System.Drawing.Size(355, 20);
            this.TB_infile.TabIndex = 1;
            // 
            // LB_File
            // 
            this.LB_File.AutoSize = true;
            this.LB_File.Location = new System.Drawing.Point(13, 38);
            this.LB_File.Name = "LB_File";
            this.LB_File.Size = new System.Drawing.Size(53, 13);
            this.LB_File.TabIndex = 2;
            this.LB_File.Text = "Input File:";
            // 
            // LB_of
            // 
            this.LB_of.AutoSize = true;
            this.LB_of.Location = new System.Drawing.Point(13, 64);
            this.LB_of.Name = "LB_of";
            this.LB_of.Size = new System.Drawing.Size(61, 13);
            this.LB_of.TabIndex = 5;
            this.LB_of.Text = "Output File:";
            // 
            // TB_outfile
            // 
            this.TB_outfile.Location = new System.Drawing.Point(80, 60);
            this.TB_outfile.Name = "TB_outfile";
            this.TB_outfile.Size = new System.Drawing.Size(355, 20);
            this.TB_outfile.TabIndex = 4;
            // 
            // PB_convert
            // 
            this.PB_convert.Location = new System.Drawing.Point(80, 180);
            this.PB_convert.Name = "PB_convert";
            this.PB_convert.Size = new System.Drawing.Size(355, 23);
            this.PB_convert.TabIndex = 6;
            // 
            // LB_prog
            // 
            this.LB_prog.AutoSize = true;
            this.LB_prog.Location = new System.Drawing.Point(13, 190);
            this.LB_prog.Name = "LB_prog";
            this.LB_prog.Size = new System.Drawing.Size(51, 13);
            this.LB_prog.TabIndex = 7;
            this.LB_prog.Text = "Progress:";
            // 
            // BTN_go
            // 
            this.BTN_go.Location = new System.Drawing.Point(222, 134);
            this.BTN_go.Name = "BTN_go";
            this.BTN_go.Size = new System.Drawing.Size(75, 23);
            this.BTN_go.TabIndex = 8;
            this.BTN_go.Text = "Go!";
            this.BTN_go.UseVisualStyleBackColor = true;
            this.BTN_go.Click += new System.EventHandler(this.Btn_go_Click);
            // 
            // LB_stat
            // 
            this.LB_stat.AutoSize = true;
            this.LB_stat.Location = new System.Drawing.Point(16, 232);
            this.LB_stat.Name = "LB_stat";
            this.LB_stat.Size = new System.Drawing.Size(40, 13);
            this.LB_stat.TabIndex = 9;
            this.LB_stat.Text = "Status:";
            // 
            // lb_Status
            // 
            this.lb_Status.AutoSize = true;
            this.lb_Status.Location = new System.Drawing.Point(77, 232);
            this.lb_Status.Name = "lb_Status";
            this.lb_Status.Size = new System.Drawing.Size(0, 13);
            this.lb_Status.TabIndex = 10;
            // 
            // LB_DateLabel
            // 
            this.LB_DateLabel.AutoSize = true;
            this.LB_DateLabel.Location = new System.Drawing.Point(16, 93);
            this.LB_DateLabel.Name = "LB_DateLabel";
            this.LB_DateLabel.Size = new System.Drawing.Size(80, 13);
            this.LB_DateLabel.TabIndex = 11;
            this.LB_DateLabel.Text = "Date to extract:";
            // 
            // dtp_dateTimePicker
            // 
            this.dtp_dateTimePicker.Location = new System.Drawing.Point(102, 86);
            this.dtp_dateTimePicker.Name = "dtp_dateTimePicker";
            this.dtp_dateTimePicker.Size = new System.Drawing.Size(200, 20);
            this.dtp_dateTimePicker.TabIndex = 12;
            // 
            // Frm_MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(538, 286);
            this.Controls.Add(this.dtp_dateTimePicker);
            this.Controls.Add(this.LB_DateLabel);
            this.Controls.Add(this.lb_Status);
            this.Controls.Add(this.LB_stat);
            this.Controls.Add(this.BTN_go);
            this.Controls.Add(this.LB_prog);
            this.Controls.Add(this.PB_convert);
            this.Controls.Add(this.LB_of);
            this.Controls.Add(this.TB_outfile);
            this.Controls.Add(this.LB_File);
            this.Controls.Add(this.TB_infile);
            this.Controls.Add(this.BTN_PickFile);
            this.Name = "Frm_MainForm";
            this.Text = "Acti2SHM";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button BTN_PickFile;
        private System.Windows.Forms.TextBox TB_infile;
        private System.Windows.Forms.Label LB_File;
        private System.Windows.Forms.Label LB_of;
        private System.Windows.Forms.TextBox TB_outfile;
        private System.Windows.Forms.ProgressBar PB_convert;
        private System.Windows.Forms.Label LB_prog;
        private System.Windows.Forms.Button BTN_go;
        private System.Windows.Forms.Label LB_stat;
        private System.Windows.Forms.Label lb_Status;
        private System.Windows.Forms.Label LB_DateLabel;
        private System.Windows.Forms.DateTimePicker dtp_dateTimePicker;
    }
}

