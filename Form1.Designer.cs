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
            this.TB_IP_file = new System.Windows.Forms.TextBox();
            this.LB_File = new System.Windows.Forms.Label();
            this.LB_of = new System.Windows.Forms.Label();
            this.TB_outfile = new System.Windows.Forms.TextBox();
            this.PB_convert = new System.Windows.Forms.ProgressBar();
            this.LB_prog = new System.Windows.Forms.Label();
            this.BTN_go = new System.Windows.Forms.Button();
            this.LB_stat = new System.Windows.Forms.Label();
            this.lb_Status = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // BTN_PickFile
            // 
            this.BTN_PickFile.Location = new System.Drawing.Point(588, 41);
            this.BTN_PickFile.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.BTN_PickFile.Name = "BTN_PickFile";
            this.BTN_PickFile.Size = new System.Drawing.Size(100, 28);
            this.BTN_PickFile.TabIndex = 0;
            this.BTN_PickFile.Text = "Browse...";
            this.BTN_PickFile.UseVisualStyleBackColor = true;
            this.BTN_PickFile.Click += new System.EventHandler(this.btn_PickFile_Click);
            // 
            // TB_IP_file
            // 
            this.TB_IP_file.Location = new System.Drawing.Point(107, 42);
            this.TB_IP_file.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.TB_IP_file.Name = "TB_IP_file";
            this.TB_IP_file.Size = new System.Drawing.Size(472, 22);
            this.TB_IP_file.TabIndex = 1;
            // 
            // LB_File
            // 
            this.LB_File.AutoSize = true;
            this.LB_File.Location = new System.Drawing.Point(17, 47);
            this.LB_File.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.LB_File.Name = "LB_File";
            this.LB_File.Size = new System.Drawing.Size(69, 17);
            this.LB_File.TabIndex = 2;
            this.LB_File.Text = "Input File:";
            // 
            // LB_of
            // 
            this.LB_of.AutoSize = true;
            this.LB_of.Location = new System.Drawing.Point(17, 79);
            this.LB_of.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.LB_of.Name = "LB_of";
            this.LB_of.Size = new System.Drawing.Size(81, 17);
            this.LB_of.TabIndex = 5;
            this.LB_of.Text = "Output File:";
            // 
            // TB_outfile
            // 
            this.TB_outfile.Location = new System.Drawing.Point(107, 74);
            this.TB_outfile.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.TB_outfile.Name = "TB_outfile";
            this.TB_outfile.Size = new System.Drawing.Size(472, 22);
            this.TB_outfile.TabIndex = 4;
            // 
            // PB_convert
            // 
            this.PB_convert.Location = new System.Drawing.Point(107, 174);
            this.PB_convert.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.PB_convert.Name = "PB_convert";
            this.PB_convert.Size = new System.Drawing.Size(473, 28);
            this.PB_convert.TabIndex = 6;
            // 
            // LB_prog
            // 
            this.LB_prog.AutoSize = true;
            this.LB_prog.Location = new System.Drawing.Point(17, 186);
            this.LB_prog.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.LB_prog.Name = "LB_prog";
            this.LB_prog.Size = new System.Drawing.Size(69, 17);
            this.LB_prog.TabIndex = 7;
            this.LB_prog.Text = "Progress:";
            // 
            // BTN_go
            // 
            this.BTN_go.Location = new System.Drawing.Point(296, 117);
            this.BTN_go.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.BTN_go.Name = "BTN_go";
            this.BTN_go.Size = new System.Drawing.Size(100, 28);
            this.BTN_go.TabIndex = 8;
            this.BTN_go.Text = "Go!";
            this.BTN_go.UseVisualStyleBackColor = true;
            this.BTN_go.Click += new System.EventHandler(this.Btn_go_Click);
            // 
            // LB_stat
            // 
            this.LB_stat.AutoSize = true;
            this.LB_stat.Location = new System.Drawing.Point(21, 238);
            this.LB_stat.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.LB_stat.Name = "LB_stat";
            this.LB_stat.Size = new System.Drawing.Size(52, 17);
            this.LB_stat.TabIndex = 9;
            this.LB_stat.Text = "Status:";
            // 
            // lb_Status
            // 
            this.lb_Status.AutoSize = true;
            this.lb_Status.Location = new System.Drawing.Point(103, 238);
            this.lb_Status.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lb_Status.Name = "lb_Status";
            this.lb_Status.Size = new System.Drawing.Size(0, 17);
            this.lb_Status.TabIndex = 10;
            // 
            // Frm_MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(717, 352);
            this.Controls.Add(this.lb_Status);
            this.Controls.Add(this.LB_stat);
            this.Controls.Add(this.BTN_go);
            this.Controls.Add(this.LB_prog);
            this.Controls.Add(this.PB_convert);
            this.Controls.Add(this.LB_of);
            this.Controls.Add(this.TB_outfile);
            this.Controls.Add(this.LB_File);
            this.Controls.Add(this.TB_IP_file);
            this.Controls.Add(this.BTN_PickFile);
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.Name = "Frm_MainForm";
            this.Text = "Acti2SHM";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button BTN_PickFile;
        private System.Windows.Forms.TextBox TB_IP_file;
        private System.Windows.Forms.Label LB_File;
        private System.Windows.Forms.Label LB_of;
        private System.Windows.Forms.TextBox TB_outfile;
        private System.Windows.Forms.ProgressBar PB_convert;
        private System.Windows.Forms.Label LB_prog;
        private System.Windows.Forms.Button BTN_go;
        private System.Windows.Forms.Label LB_stat;
        private System.Windows.Forms.Label lb_Status;
    }
}

