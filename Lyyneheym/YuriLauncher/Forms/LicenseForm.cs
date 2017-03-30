using System;
using System.Windows.Forms;

namespace Yuri.YuriLauncher.Forms
{
    public partial class LicenseForm : Form
    {
        public LicenseForm()
        {
            InitializeComponent();
            this.textBox1.Text = this.LicenseChs;
            this.textBox1.Select(0, 0);
        }

        /// <summary>
        /// 按钮：语言切换
        /// </summary>
        private void button1_Click(object sender, EventArgs e)
        {
            if (this.button1.Text == "English")
            {
                this.label1.Text = "Yuri Engine is licensed under the:";
                this.label2.Text = "Microsoft Public License (MS-PL)";
                this.textBox1.Text = this.LicenseEng;
                this.button1.Text = "中文";
            }
            else
            {
                this.label1.Text = "优理引擎采用：";
                this.label2 .Text = "Microsoft公共许可证（MS-PL）";
                this.textBox1.Text = this.LicenseChs;
                this.button1.Text = "English";
            }
            this.textBox1.Select(0, 0);
        }

        /// <summary>
        /// 英文许可证
        /// </summary>
        private readonly string LicenseEng = "This license governs use of the accompanying software. If you use the software, you accept this license. If you do not accept the license, do not use the software." +
                                             Environment.NewLine +
                                             Environment.NewLine +
                                             "1. Definitions" +
                                             Environment.NewLine +
                                             "The terms \"reproduce,\" \"reproduction,\" \"derivative works,\" and \"distribution\" have the same meaning here as under U.S. copyright law." +
                                             Environment.NewLine +
                                             "A \"contribution\" is the original software, or any additions or changes to the software." +
                                             Environment.NewLine +
                                             "A \"contributor\" is any person that distributes its contribution under this license." +
                                             Environment.NewLine +
                                             "\"Licensed patents\" are a contributor\'s patent claims that read directly on its contribution." +
                                             Environment.NewLine +
                                             Environment.NewLine +
                                             "2. Grant of Rights" +
                                             Environment.NewLine +
                                             "(A) Copyright Grant- Subject to the terms of this license, including the license conditions and limitations in section 3, each contributor grants you a non-exclusive, worldwide, royalty-free copyright license to reproduce its contribution, prepare derivative works of its contribution, and distribute its contribution or any derivative works that you create." +
                                             Environment.NewLine +
                                             "(B) Patent Grant- Subject to the terms of this license, including the license conditions and limitations in section 3, each contributor grants you a non-exclusive, worldwide, royalty-free license under its licensed patents to make, have made, use, sell, offer for sale, import, and/or otherwise dispose of its contribution in the software or derivative works of the contribution in the software." +
                                             Environment.NewLine +
                                             Environment.NewLine +
                                             "3. Conditions and Limitations" +
                                             Environment.NewLine +
                                             "(A) No Trademark License- This license does not grant you rights to use any contributors\' name, logo, or trademarks." +
                                             Environment.NewLine +
                                             "(B) If you bring a patent claim against any contributor over patents that you claim are infringed by the software, your patent license from such contributor to the software ends automatically." +
                                             Environment.NewLine +
                                             "(C) If you distribute any portion of the software, you must retain all copyright, patent, trademark, and attribution notices that are present in the software." +
                                             Environment.NewLine +
                                             "(D) If you distribute any portion of the software in source code form, you may do so only under this license by including a complete copy of this license with your distribution. If you distribute any portion of the software in compiled or object code form, you may only do so under a license that complies with this license." +
                                             Environment.NewLine +
                                             "(E) The software is licensed \"AS-IS.\" You bear the risk of using it. The contributors give no express warranties, guarantees or conditions. You may have additional consumer rights under your local laws which this license cannot change. To the extent permitted under your local laws, the contributors exclude the implied warranties of merchantability, fitness for a particular purpose and non-infringement.";

        /// <summary>
        /// 中文许可证
        /// </summary>
        private readonly string LicenseChs = "对随附软件的使用受本许可证的制约。使用该软件即表示您接受本许可证。如果您不接受本许可证，请不要使用该软件。" + Environment.NewLine + Environment.NewLine +
                                             "1.定义" +
                                             Environment.NewLine +
                                             "术语“复制”、“衍生作品”和“分发”与美国版权法中的定义相同。" + 
                                             Environment.NewLine +
                                             "“稿件”是指原始软件或对该软件进行的任何添加或更改。" +
                                             Environment.NewLine +
                                             "“撰稿人”是指依据此许可证分发其稿件的任何人。" + 
                                             Environment.NewLine +
                                             "“许可专利”是指撰稿人在其稿件上直接注明的专利主张。" +
                                             Environment.NewLine +
                                             Environment.NewLine +
                                             "2.权利的授予" +
                                             Environment.NewLine +
                                             "(A)版权授予 - 根据本许可证的条款，包括第3节中的许可条件和限制，每个撰稿人授予您复制其稿件、准备其稿件的衍生作品以及分发其稿件或您创作的任何衍生作品的非排他性的、世界范围的、免版权税的版权许可。" +
                                             Environment.NewLine +
                                             "(B) 专利授予 - 根据本许可证的条款，包括第3节中的许可条件和限制，每个撰稿人授予您依据许可专利制作、请人制作、使用、出售、推销、导入和/或以其他方式处理软件中的稿件或软件中稿件的衍生作品的非排他性的、世界范围的、免版权税的许可。" +
                                             Environment.NewLine +
                                             Environment.NewLine +
                                             "3.条件和限制" +
                                             Environment.NewLine +
                                             "(A) 无商标许可证 - 本许可证不授予您使用任何撰稿人的姓名、徽标或商标的权利。" +
                                             Environment.NewLine +
                                             "(B) 如果您对任何撰稿人就软件侵犯您的专利权提出专利主张，则您从该撰稿人处获得的、对于该软件的专利许可将自动终止。" +
                                             Environment.NewLine +
                                             "(C) 如果您分发该软件的任何部分，则必须保留该软件上现有的所有版权、专利、商标和归属声明。" +
                                             Environment.NewLine +
                                             "(D) 如果您以源代码形式分发该软件的任何部分，则只能通过在分发时包含本许可证的完整副本来依据本许可证进行分发。如果您以编译的或目标代码的形式分发该软件的任何部分，则只能依据一个符合本许可证的许可证进行分发。" +
                                             Environment.NewLine +
                                             "(E) 该软件按“原样”授予许可。使用该软件的风险需要您自己承担。撰稿人不提供任何明示的担保、保证或条件。根据所在地区的法律，您可能拥有其他本许可证无法更改的消费者权利。在您当地法律允许的范围内，撰稿人排除有关适销性、针对特定目的的适用性和不侵权的默示担保。";
    }
}
