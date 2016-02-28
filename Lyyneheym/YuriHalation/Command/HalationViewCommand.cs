using System;
using System.Collections.Generic;
using System.Text;
using Yuri;
using Yuri.YuriForms;
using Yuri.YuriHalation;
using YuriHalation;

namespace Yuri.YuriHalation.Command
{
    static class HalationViewCommand
    {
        public static bool AddItemToCodeListbox(int insertLine, int indent, string text)
        {
            StringBuilder indentSb = new StringBuilder();
            for (int i = 0; i < indent; i++)
            {
                indentSb.Append(" ");
            }
            if (insertLine >= 0 && insertLine <= HalationViewCommand.LineCount())
            {
                Halation.mainView.codeListBox.Items.Insert(insertLine, indentSb.ToString() + text);
                return true;
            }
            else if (insertLine == -1)
            {
                Halation.mainView.codeListBox.Items.Add(indentSb.ToString() + text);
                return true;
            }
            return false;
        }

        public static bool RemoveItemFromCodeListbox(int removeLine)
        {
            if (removeLine >= 0 && removeLine < HalationViewCommand.LineCount())
            {
                Halation.mainView.codeListBox.Items.RemoveAt(removeLine);
                return true;
            }
            return false;
        }

        public static int LineCount()
        {
            return Halation.mainView.codeListBox.Items.Count;
        }

        public static void ClearAll()
        {
            Halation.mainView.codeListBox.Items.Clear();
        }
    }
}
