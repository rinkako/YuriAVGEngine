using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Yuri.YuriHalation.YuriForms
{
    public partial class BranchForm : Form
    {
        public BranchForm()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 按钮：确定
        /// </summary>
        private void button1_Click(object sender, EventArgs e)
        {
            // 处理参数列表
            List<string> argvList = new List<string>();
            int nrows = this.switchDataGridView.Rows.Count - 1;
            for (int i = 0; i < nrows; i++)
            {
                // 文字不能为空
                if (this.switchDataGridView.Rows[i].Cells[0].Value == null)
                {
                    MessageBox.Show("文本不能为空", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                string branchName = this.switchDataGridView.Rows[i].Cells[0].Value.ToString();
                // 标签不能为空
                if (this.switchDataGridView.Rows[i].Cells[1].Value == null)
                {
                    MessageBox.Show(String.Format("{0} 的标签不能为空", branchName), "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                string vartype = this.switchDataGridView.Rows[i].Cells[1].Value.ToString();
                argvList.Add(String.Format("{0},{1}", branchName, vartype));
            }
            if (argvList.Count > 0)
            {
                Halation.GetInstance().DashBranch(argvList);
            }
            this.Close();
        }
    }
}
