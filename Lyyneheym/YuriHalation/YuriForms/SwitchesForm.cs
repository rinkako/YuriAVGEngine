using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Yuri.YuriHalation.YuriForms
{
    public partial class SwitchesForm : Form
    {
        /// <summary>
        /// 编辑状态位
        /// </summary>
        private readonly bool isEditing;

        /// <summary>
        /// 构造器
        /// </summary>
        public SwitchesForm(string title, bool isEdit, string id = "0", string dash = "off")
        {
            InitializeComponent();
            this.Text = title;
            if (title == "开关管理器")
            {
                this.comboBox1.Visible = false;
                this.label1.Visible = false;
            }
            else
            {
                // 选中默认开关值
                this.comboBox1.SelectedIndex = 0;
            }
            // 加载开关
            var switchVector = Halation.project.SwitchDescriptorList;
            // 加载开关列表
            for (int i = 0; i < switchVector.Count; i++)
            {
                this.switchDataGridView.Rows.Add();
                this.switchDataGridView.Rows[i].Cells[0].Value = i;
                this.switchDataGridView.Rows[i].Cells[1].Value = switchVector[i];
            }
            this.isEditing = isEdit;
            if (isEditing)
            {
                int tid = Convert.ToInt32(id);
                if (tid >= 0 && tid < switchVector.Count)
                {
                    this.switchDataGridView.Rows[tid].Cells[1].Selected = true;
                }
                this.comboBox1.SelectedIndex = dash == "off" ? 1 : 0;
            }
        }

        /// <summary>
        /// 按钮：确定
        /// </summary>
        private void button1_Click(object sender, EventArgs e)
        {
            // 开关操作时没有选择就不要应用
            if (this.Text != @"开关管理器" && this.switchDataGridView.SelectedCells.Count < 1)
            {
                return;
            }
            // 更新开关描述
            var desList = new List<string>();
            for (int i = 0; i < Halation.project.Config.GameMaxSwitchCount; i++)
            {
                if (this.switchDataGridView.Rows[i].Cells[1].Value == null)
                {
                    desList.Add(String.Empty);
                }
                else
                {
                    string descript = this.switchDataGridView.Rows[i].Cells[1].Value.ToString();
                    desList.Add(descript);
                }
            }
            Halation.project.SwitchDescriptorList = desList;
            // 开关操作
            if (this.Text != @"开关管理器")
            {
                // 提交到后台
                if (this.isEditing)
                {
                    Halation.GetInstance().DashEditSwitches(
                        this.switchDataGridView.SelectedCells[0].RowIndex.ToString(), this.comboBox1.SelectedItem.ToString());
                }
                else
                {
                    Halation.GetInstance().DashSwitches(
                        this.switchDataGridView.SelectedCells[0].RowIndex.ToString(), this.comboBox1.SelectedItem.ToString());
                }
            }
            this.Close();
        }
    }
}
