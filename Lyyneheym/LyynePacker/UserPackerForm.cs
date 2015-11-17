using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using LyyneheymCore.Utils;

namespace LyynePacker
{
    public partial class UserPackerForm : Form
    {
        public UserPackerForm()
        {
            InitializeComponent();
        }

        private void listBox1_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effect = DragDropEffects.All;
            }
            else
            {
                e.Effect = DragDropEffects.None;
            }
        }

        private void listBox1_DragDrop(object sender, DragEventArgs e)
        {
            string[] fnameList = (e.Data.GetData(DataFormats.FileDrop, false) as String[]);
            try
            {
                this.pendingList = new List<string>();
                foreach (string fname in fnameList)
                {
                    this.pendingList.Add(fname);
                    this.listBox1.Items.Add(fname);
                }
            }
            catch
            {
                MessageBox.Show("文件格式不匹配，必须是规定的文件格式。", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private List<string> pendingList = null;

        private void button1_Click(object sender, EventArgs e)
        {
            if (this.listBox1.Items.Count == 0)
            {
                MessageBox.Show("先加入文件");
                return;
            }
            FileDialog fwindow = new SaveFileDialog();
            fwindow.Filter = "dat文件|*.dat";
            if (fwindow.ShowDialog() == DialogResult.OK)
            {
                List<string> flist = new List<string>();
                foreach (string s in this.listBox1.Items)
                {
                    flist.Add(s);
                }
                PackageUtils.pack(flist, fwindow.FileName);
            }
        }

        private void listBox2_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effect = DragDropEffects.All;
            }
            else
            {
                e.Effect = DragDropEffects.None;
            }
        }

        private string exPakName = "";
        private void button2_Click_1(object sender, EventArgs e)
        {
            if (exPakName == "")
            {
                MessageBox.Show("你还没拉入文件呢");
                return;
            }
            string savePath = "";
            FolderBrowserDialog fdg = new FolderBrowserDialog();
            fdg.Description = "选择资源提取到哪个目录";
            if (fdg.ShowDialog() == DialogResult.OK)
            {
                savePath = fdg.SelectedPath;
            }
            foreach (object ob in this.listBox2.SelectedItems)
            {
                string obs = ob.ToString();
                PackageUtils.unpack(this.exPakName, obs, savePath + "\\" + obs);
            }
        }

        private void listBox2_DragDrop(object sender, DragEventArgs e)
        {
            this.listBox2.Items.Clear();
            this.exPakName = (e.Data.GetData(DataFormats.FileDrop, false) as String[])[0];
            List<string> pakFileList = PackageUtils.getPackList(this.exPakName);
            foreach (string s in pakFileList)
            {
                this.listBox2.Items.Add(s);
            }
        }

        private void UserPackerForm_ResizeEnd(object sender, EventArgs e)
        {
            if (this.Width < 500)
            {
                this.Width = 500;
            }
            if (this.Height < 450)
            {
                this.Height = 450;
            }
        }

    }
}
