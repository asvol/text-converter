using System;

namespace Asv.TextConverter
{
    public class MessageBoxHelpButton
    {
        public MessageBoxHelpButton(string text,Action action)
        {
            if (text == null) throw new ArgumentNullException(nameof(text));
            if (action == null) throw new ArgumentNullException(nameof(action));
            Action = action;
            Text = text;
        }

        public string Text { get; set; }
        public Action Action { get; private set; }
    }
}