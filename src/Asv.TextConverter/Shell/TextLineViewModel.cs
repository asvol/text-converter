using System.Collections.Generic;
using Caliburn.Micro;

namespace Asv.TextConverter
{
    public class TextLineViewModel : PropertyChangedBase
    {
        private string _source;
        private string _result;
        private bool _isEnabled = true;

        public string Source
        {
            get { return _source; }
            set
            {
                if (value == _source) return;
                _source = value;
                NotifyOfPropertyChange(() => Source);
            }
        }

        public string Result
        {
            get { return _result; }
            set
            {
                if (value == _result) return;
                _result = value;
                NotifyOfPropertyChange(() => Result);
            }
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

        public TextLineViewModel(string source)
        {
            Source = source;
        }

        public void Convert(IEnumerable<RuleViewModel> rules)
        {
            if (!IsEnabled)
            {
                Result = Source;
            }
            else
            {
                var source = Source;
                foreach (var rule in rules)
                {
                    source = rule.Replace(source);
                }

                Result = source;
            }

            
        }

        
    }
}