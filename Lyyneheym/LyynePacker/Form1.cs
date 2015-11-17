using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using LyyneheymCore.SlyviaPile;

namespace LyynePacker
{
    public partial class Form1 : Form
    {
        public Form1()
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
                    //if (fname.EndsWith(".jpg") || fname.EndsWith(".bmp") || fname.EndsWith(".png"))
                    //{
                    this.pendingList.Add(fname);
                    this.listBox1.Items.Add(fname);
                    //}
                    //else
                    //{
                    //    MessageBox.Show("文件 " + fname + " 格式不匹配，必须是jpg、bmp或png格式。", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    //}
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
                SlyviaPackageUtil.pack(flist, fwindow.FileName);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string s1 = "", s2 = "";
            FileDialog fwindow = new OpenFileDialog();
            fwindow.Filter = "dat文件|*.dat";
            if (fwindow.ShowDialog() == DialogResult.OK)
            {
                s1 = fwindow.FileName;
            }
            FileDialog fwindow2 = new SaveFileDialog();
            if (fwindow2.ShowDialog() == DialogResult.OK)
            {
                s2 = fwindow2.FileName;
            }
            SlyviaPackageUtil.unpack(s1, this.textBox1.Text, s2);
        }


    }
}
