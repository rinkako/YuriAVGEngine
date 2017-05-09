using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Yuri.YuriHalation.YuriForms
{
    /// <summary>
    /// 窗体：添加函数
    /// </summary>
    public partial class AddFuncForm : Form
    {
        private Halation core = Halation.GetInstance();

        /// <summary>
        /// 构造器
        /// </summary>
        public AddFuncForm()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 按钮：确定
        /// </summary>
        private void button1_Click(object sender, EventArgs e)
        {
            // 检查函数名
            if (this.textBox1.Text == String.Empty ||
                !Halation.IsValidVarname(this.textBox1.Text))
            {
                MessageBox.Show("请使用字母正确填写函数名", "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            // 提示并行处理
            if (this.textBox1.Text.Trim().StartsWith("sync_", StringComparison.CurrentCultureIgnoreCase))
            {
                var dr = MessageBox.Show("Halation发现该函数名以 sync_ 开头，这将使引擎以并行处理的方式执行该函数，你确定要这么做吗？", "并行提示",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Information, MessageBoxDefaultButton.Button2);
                if (dr == DialogResult.No)
                {
                    return;
                }
            }
            // 处理参数列表
            List<string> argvList = new List<string>();
            int nrows = this.argsGridDataView.Rows.Count - 1;
            for (int i = 0; i < nrows; i++)
            {
                var tvarname = this.argsGridDataView.Rows[i].Cells[0].Value;
                if (tvarname == null)
                {
                    continue;
                }
                string varname = tvarname.ToString();
                // 符号合法性
                if (Halation.IsValidVarname(varname) == false)
                {
                    MessageBox.Show(String.Format("变量 {0} 命名不合法", varname), "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                // 符号唯一性
                if (argvList.Find((x) => x.Split('@')[0] == varname) != null)
                {
                    MessageBox.Show(String.Format("变量名 {0} 重复", varname), "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                argvList.Add(varname);
            }
            core.DashAddFunction(this.textBox1.Text.Trim(), argvList);
            this.Close();
        }
    }
}
