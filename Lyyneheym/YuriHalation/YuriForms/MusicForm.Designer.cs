namespace Yuri.YuriHalation.YuriForms
{
    partial class MusicForm
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
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.listBoxBGM = new System.Windows.Forms.ListBox();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.listBoxBGS = new System.Windows.Forms.ListBox();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.listBoxSE = new System.Windows.Forms.ListBox();
            this.tabPage4 = new System.Windows.Forms.TabPage();
            this.listBoxVocal = new System.Windows.Forms.ListBox();
            this.volTrackBar = new System.Windows.Forms.TrackBar();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.label5 = new System.Windows.Forms.Label();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.tabPage3.SuspendLayout();
            this.tabPage4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.volTrackBar)).BeginInit();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Controls.Add(this.tabPage3);
            this.tabControl1.Controls.Add(this.tabPage4);
            this.tabControl1.Location = new System.Drawing.Point(0, 4);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(244, 368);
            this.tabControl1.TabIndex = 0;
            this.tabControl1.SelectedIndexChanged += new System.EventHandler(this.tabControl1_SelectedIndexChanged);
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.listBoxBGM);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(236, 342);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "BGM";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // listBoxBGM
            // 
            this.listBoxBGM.FormattingEnabled = true;
            this.listBoxBGM.ItemHeight = 12;
            this.listBoxBGM.Location = new System.Drawing.Point(0, 0);
            this.listBoxBGM.Name = "listBoxBGM";
            this.listBoxBGM.Size = new System.Drawing.Size(236, 340);
            this.listBoxBGM.TabIndex = 0;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.listBoxBGS);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(236, 342);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "BGS";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // listBoxBGS
            // 
            this.listBoxBGS.FormattingEnabled = true;
            this.listBoxBGS.ItemHeight = 12;
            this.listBoxBGS.Location = new System.Drawing.Point(0, 0);
            this.listBoxBGS.Name = "listBoxBGS";
            this.listBoxBGS.Size = new System.Drawing.Size(236, 340);
            this.listBoxBGS.TabIndex = 2;
            // 
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.listBoxSE);
            this.tabPage3.Location = new System.Drawing.Point(4, 22);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Size = new System.Drawing.Size(236, 342);
            this.tabPage3.TabIndex = 2;
            this.tabPage3.Text = "SE";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // listBoxSE
            // 
            this.listBoxSE.FormattingEnabled = true;
            this.listBoxSE.ItemHeight = 12;
            this.listBoxSE.Location = new System.Drawing.Point(0, 0);
            this.listBoxSE.Name = "listBoxSE";
            this.listBoxSE.Size = new System.Drawing.Size(236, 340);
            this.listBoxSE.TabIndex = 2;
            // 
            // tabPage4
            // 
            this.tabPage4.Controls.Add(this.listBoxVocal);
            this.tabPage4.Location = new System.Drawing.Point(4, 22);
            this.tabPage4.Name = "tabPage4";
            this.tabPage4.Size = new System.Drawing.Size(236, 342);
            this.tabPage4.TabIndex = 3;
            this.tabPage4.Text = "Vocal";
            this.tabPage4.UseVisualStyleBackColor = true;
            // 
            // listBoxVocal
            // 
            this.listBoxVocal.FormattingEnabled = true;
            this.listBoxVocal.ItemHeight = 12;
            this.listBoxVocal.Location = new System.Drawing.Point(0, 0);
            this.listBoxVocal.Name = "listBoxVocal";
            this.listBoxVocal.Size = new System.Drawing.Size(236, 340);
            this.listBoxVocal.TabIndex = 2;
            // 
            // volTrackBar
            // 
            this.volTrackBar.LargeChange = 100;
            this.volTrackBar.Location = new System.Drawing.Point(250, 98);
            this.volTrackBar.Maximum = 1000;
            this.volTrackBar.Name = "volTrackBar";
            this.volTrackBar.Orientation = System.Windows.Forms.Orientation.Vertical;
            this.volTrackBar.Size = new System.Drawing.Size(45, 274);
            this.volTrackBar.SmallChange = 100;
            this.volTrackBar.TabIndex = 100;
            this.volTrackBar.TickFrequency = 50;
            this.volTrackBar.TickStyle = System.Windows.Forms.TickStyle.TopLeft;
            this.volTrackBar.Value = 800;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(287, 352);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(11, 12);
            this.label1.TabIndex = 101;
            this.label1.Text = "0";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(287, 105);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(29, 12);
            this.label2.TabIndex = 102;
            this.label2.Text = "1000";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(287, 230);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(23, 12);
            this.label3.TabIndex = 103;
            this.label3.Text = "500";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(287, 155);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(23, 12);
            this.label4.TabIndex = 104;
            this.label4.Text = "800";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(250, 26);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 105;
            this.button1.Text = "播放";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(250, 55);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 23);
            this.button2.TabIndex = 106;
            this.button2.Text = "停止";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(250, 83);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(29, 12);
            this.label5.TabIndex = 107;
            this.label5.Text = "音量";
            // 
            // MusicForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(336, 377);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.volTrackBar);
            this.Controls.Add(this.tabControl1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "MusicForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "音乐";
            this.Load += new System.EventHandler(this.MusicForm_Load);
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            this.tabPage3.ResumeLayout(false);
            this.tabPage4.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.volTrackBar)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.TrackBar volTrackBar;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.TabPage tabPage4;
        private System.Windows.Forms.ListBox listBoxBGM;
        private System.Windows.Forms.ListBox listBoxBGS;
        private System.Windows.Forms.ListBox listBoxSE;
        private System.Windows.Forms.ListBox listBoxVocal;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Label label5;
    }
}