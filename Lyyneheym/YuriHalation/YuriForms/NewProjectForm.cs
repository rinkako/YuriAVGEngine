using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using Yuri;
using Yuri.YuriHalation.HalationCore;

namespace Yuri.YuriForms
{
    public partial class NewProjectForm : Form
    {
        /// <summary>
        /// 构造器
        /// </summary>
        public NewProjectForm()
        {
            InitializeComponent();
            this.InitSuccess = false;
        }

        /// <summary>
        /// 选择文件夹
        /// </summary>
        private void button1_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fd = new FolderBrowserDialog();
            DialogResult dr = fd.ShowDialog(this);
            if (dr == System.Windows.Forms.DialogResult.OK)
            {
                this.textBox1.Text = fd.SelectedPath;
            }
        }

        /// <summary>
        /// 确定
        /// </summary>
        private void button2_Click(object sender, EventArgs e)
        {
            if (this.textBox1.Text != "" && this.textBox2.Text != "" && !this.textBox2.Text.Contains("\\"))
            {
                Halation.GetInstance().NewProject(this.textBox1.Text, this.textBox2.Text);
                this.Close();
                this.InitSuccess = true;
            }
            else
            {
                MessageBox.Show("请选择文件夹并正确填写工程名", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        /// <summary>
        /// 是否初始化成功
        /// </summary>
        public bool InitSuccess { get; private set; }
    }
}
