﻿using System;
using Caliburn.Micro;

namespace Asv.TextConverter
{
    public class MessageBoxButtonViewModel:PropertyChangedBase,IHaveDisplayName
    {
        private readonly MessageBoxViewModel _owner;
        private readonly MessageBoxButtonExt _button;

        public MessageBoxButtonViewModel(MessageBoxViewModel owner, MessageBoxButtonExt button)
        {
            if (owner == null) throw new ArgumentNullException(nameof(owner));
            if (button == null) throw new ArgumentNullException(nameof(button));
            _owner = owner;
            _button = button;
        }

        public void Execute()
        {
            _owner.InternalExecuteButton(_button);
        }

        public bool IsDefault
        {
            get { return _button.IsDefault; }
        }

        public bool IsCancel
        {
            get { return _button.IsCancel; }
        }

        #region Implementation of IHaveDisplayName

        public string DisplayName
        {
            get { return _button.Text; }
            set {  }
        }

        #endregion
    }
}