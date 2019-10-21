using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Caliburn.Micro;

namespace Asv.TextConverter
{
    public class RuleViewModelConfig
    {
        public string DisplayName { get; set; }
        public bool IsEnabled { get; set; }
        public string RegexFrom { get; set; }
        public string RegexTo { get; set; }
    }

    public class RuleViewModel : Screen
    {
        private string _regexFrom;
        private string _regexTo;
        private bool _isEnabled = true;

        public RuleViewModel()
        {
            DisplayName = "Новое правило";
        }

        public bool IsEnabled
        {
            get { return _isEnabled; }
            set
            {
                if (value == _isEnabled) return;
                _isEnabled = value;
                NotifyOfPropertyChange(() => IsEnabled);
            }
        }

        public string RegexFrom
        {
            get { return _regexFrom; }
            set
            {
                if (value == _regexFrom) return;
                _regexFrom = value;
                NotifyOfPropertyChange(() => RegexFrom);
            }
        }

        public string RegexTo
        {
            get { return _regexTo; }
            set
            {
                if (value == _regexTo) return;
                _regexTo = value;
                
                NotifyOfPropertyChange(() => RegexTo);
            }
        }

        

        public void RuleUp(RuleViewModel vm)
        {
           (Parent as RuleListViewModel)?.RuleUp(vm);

        }

        public void RuleDown(RuleViewModel vm)
        {
            (Parent as RuleListViewModel)?.RuleDown(vm);
        }

        public void RemoveRule(RuleViewModel vm)
        {
            (Parent as RuleListViewModel)?.RemoveRule(vm);
        }

        

        public string Replace(string sourceText)
        {
            return Regex.Replace(sourceText, RegexFrom, RegexTo, RegexOptions.Compiled);
        }

        public void Load(RuleViewModelConfig cfg)
        {
            DisplayName = cfg.DisplayName;
            IsEnabled = cfg.IsEnabled;
            RegexFrom = cfg.RegexFrom;
            RegexTo = cfg.RegexTo;
        }

        public RuleViewModelConfig SaveConfig()
        {
            return new RuleViewModelConfig
            {
                DisplayName = DisplayName,
                IsEnabled = IsEnabled,
                RegexFrom = RegexFrom,
                RegexTo = RegexTo,
            };
        }
    }
}