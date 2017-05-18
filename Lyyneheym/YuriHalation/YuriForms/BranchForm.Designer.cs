namespace Yuri.YuriHalation.YuriForms
{
    partial class BranchForm
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            this.switchDataGridView = new System.Windows.Forms.DataGridView();
            this.文本 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.跳转标签 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.button1 = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.switchDataGridView)).BeginInit();
            this.SuspendLayout();
            // 
            // switchDataGridView
            // 
            this.switchDataGridView.AllowUserToOrderColumns = true;
            this.switchDataGridView.AllowUserToResizeRows = false;
            this.switchDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.switchDataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.文本,
            this.跳转标签});
            this.switchDataGridView.Location = new System.Drawing.Point(12, 12);
            this.switchDataGridView.MultiSelect = false;
            this.switchDataGridView.Name = "switchDataGridView";
            this.switchDataGridView.RowTemplate.Height = 23;
            this.switchDataGridView.Size = new System.Drawing.Size(380, 297);
            this.switchDataGridView.TabIndex = 3;
            // 
            // 文本
            // 
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.White;
            this.文本.DefaultCellStyle = dataGridViewCellStyle1;
            this.文本.HeaderText = "文本";
            this.文本.Name = "文本";
            this.文本.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.文本.Width = 200;
            // 
            // 跳转标签
            // 
            this.跳转标签.HeaderText = "跳转标签";
            this.跳转标签.Name = "跳转标签";
            this.跳转标签.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.跳转标签.Width = 120;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(398, 286);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 4;
            this.button1.Text = "确定";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // BranchForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(483, 317);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.switchDataGridView);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "BranchForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "选择支";
            ((System.ComponentModel.ISupportInitialize)(this.switchDataGridView)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView switchDataGridView;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.DataGridViewTextBoxColumn 文本;
        private System.Windows.Forms.DataGridViewTextBoxColumn 跳转标签;

    }
}