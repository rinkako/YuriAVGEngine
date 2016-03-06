using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Yuri;
using Yuri.YuriHalation.ScriptPackage;

namespace YuriHalation.YuriForms
{
    public partial class FunctionCallForm : Form
    {
        public FunctionCallForm()
        {
            InitializeComponent();
            foreach (var sp in Halation.project.GetScene())
            {
                foreach (var fp in sp.GetFunc())
                {
                    this.comboBox1.Items.Add(fp.functionCallName);
                }
            }
            //if (Halation.currentCodePackage is ScenePackage)
            //{
            //    this.activeScene = (ScenePackage)Halation.currentCodePackage;
            //    foreach (var f in this.activeScene.GetFunc())
            //    {
            //        this.comboBox1.Items.Add(f.functionName);
            //    }
            //}
            //else
            //{
            //    this.activeScene = (Halation.currentCodePackage as FunctionPackage).parent;
            //    foreach (var f in this.activeScene.GetFunc())
            //    {
            //        this.comboBox1.Items.Add(f.functionName);
            //    }
            //}
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.comboBox1.SelectedIndex != -1)
            {
                this.button1.Enabled = true;
            }
            else
            {
                this.button1.Enabled = false;
                return;
            }
            string fullName = this.comboBox1.SelectedItem.ToString();
            string funcName = fullName.Split('@')[0];
            string sceneName = fullName.Split('@')[1];
            FunctionPackage fp = Halation.project.GetScene(sceneName).GetFunc(funcName);
            // 处理参数列表
            this.argsGridDataView.Rows.Clear();
            if (fp.Argv.Count > 0)
            {
                for (int i = 0; i < fp.Argv.Count; i++)
                {
                    this.argsGridDataView.Rows.Add();
                    this.argsGridDataView.Rows[i].Cells[0].Value = fp.Argv[i];
                }
            }
        }

        /// <summary>
        /// 按钮：确定
        /// </summary>
        private void button1_Click(object sender, EventArgs e)
        {
            // 检查空值并处理参数
            string argStr = "";
            for (int i = 0; i < this.argsGridDataView.Rows.Count; i++)
            {
                if (this.argsGridDataView.Rows[i].Cells[1].Value == null ||
                    (string)(this.argsGridDataView.Rows[i].Cells[1].Value) == "")
                {
                    MessageBox.Show("请完整填写", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                else
                {
                    argStr += String.Format(",{0}:{1}", this.argsGridDataView.Rows[i].Cells[0].Value.ToString(),
                        this.argsGridDataView.Rows[i].Cells[1].Value.ToString());
                }
            }
            if (argStr.Length > 0)
            {
                argStr = argStr.Substring(1);
            }
            // 提交给后台
            Halation.GetInstance().DashFuncall(this.comboBox1.SelectedItem.ToString(), argStr);
            // 关闭
            this.Close();
        }

    }
}
