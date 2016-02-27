using System;
using System.Collections.Generic;
using Yuri;
using Yuri.YuriForms;
using Yuri.YuriHalation;
using YuriHalation;

namespace Yuri.YuriHalation.Command
{
    static class HalationViewCommand
    {
        public static bool AddItemToCodeListbox(int insertLine, string text)
        {
            if (insertLine >= 0 && insertLine <= HalationViewCommand.LineCount())
            {
                Halation.mainView.codeListBox.Items.Insert(insertLine, text);
                return true;
            }
            else if (insertLine == -1)
            {
                Halation.mainView.codeListBox.Items.Add(text);
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
    }
}
