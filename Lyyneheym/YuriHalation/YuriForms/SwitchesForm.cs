using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Yuri;

namespace YuriHalation.YuriForms
{
    public partial class SwitchesForm : Form
    {
        /// <summary>
        /// 控制器实例
        /// </summary>
        Halation core = Halation.GetInstance();

        /// <summary>
        /// 构造器
        /// </summary>
        public SwitchesForm()
        {
            InitializeComponent();
            // 选中默认开关值
            this.comboBox1.SelectedIndex = 0;
            // 加载开关
            List<string> switchVector = Halation.project.SwitchDescriptorList;
            // 加载开关列表
            for (int i = 0; i < switchVector.Count; i++)
            {
                this.switchDataGridView.Rows.Add();
                this.switchDataGridView.Rows[i].Cells[0].Value = i;
                this.switchDataGridView.Rows[i].Cells[1].Value = switchVector[i];
            }
        }

        /// <summary>
        /// 按钮：确定
        /// </summary>
        private void button1_Click(object sender, EventArgs e)
        {
            if (this.switchDataGridView.SelectedCells.Count < 1)
            {
                return;
            }
            // 更新开关描述
            List<string> desList = new List<string>();
            for (int i = 0; i < Halation.project.Config.MaxSwitchCount; i++)
            {
                if (this.switchDataGridView.Rows[i].Cells[1].Value == null)
                {
                    desList.Add("");
                }
                else
                {
                    string descript = this.switchDataGridView.Rows[i].Cells[1].Value.ToString();
                    desList.Add(descript);
                }
            }
            Halation.project.SwitchDescriptorList = desList;
            // 提交命令
            Halation.GetInstance().DashSwitches(this.switchDataGridView.SelectedCells[0].RowIndex.ToString(), this.comboBox1.SelectedItem.ToString());
            this.Close();
        }
    }
}
