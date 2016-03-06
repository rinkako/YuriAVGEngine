using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Yuri;

namespace YuriHalation.YuriForms
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
            if (this.textBox1.Text == "" ||
                !Halation.IsValidVarname(this.textBox1.Text))
            {
                MessageBox.Show("请使用字母正确填写函数名", "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            // 处理参数列表
            List<string> argvList = new List<string>();
            int nrows = this.argsGridDataView.Rows.Count - 1;
            for (int i = 0; i < nrows; i++)
            {
                string varname = this.argsGridDataView.Rows[i].Cells[0].Value.ToString();
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
            core.DashAddFunction(this.textBox1.Text, argvList);
            this.Close();
        }
    }
}
