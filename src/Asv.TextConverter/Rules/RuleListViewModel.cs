using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel.Composition;
using System.Linq;
using Caliburn.Micro;
using NLog;
using LogManager = NLog.LogManager;

namespace Asv.TextConverter
{
    public class RuleListViewModelConfig
    {
        public RuleViewModelConfig[] Rules { get; set; }
    }

    public class RuleListViewModel:Conductor<RuleViewModel>.Collection.AllActive
    {
        private readonly IConfiguration _configService;
        private RuleListViewModelConfig _cfg;
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();

        [ImportingConstructor]
        public RuleListViewModel(IConfiguration configService)
        {
            _configService = configService;
            _cfg = configService.Get<RuleListViewModelConfig>();
            LoadConfig();
            DisplayName = "Правила";
            
        }

        public void RuleUp(RuleViewModel vm)
        {
            var index = Items.IndexOf(vm);
            if (index > 0)
            {
                Items.RemoveAt(index);
                Items.Insert(index-1,vm);
            }
            
        }

        public void RuleDown(RuleViewModel vm)
        {
            var index = Items.IndexOf(vm);
            if (index < (Items.Count -1))
            {
                Items.RemoveAt(index);
                Items.Insert(index + 1, vm);
            }

        }

        

        #region Config save\load

        public void SaveConfig()
        {
            try
            {
                _cfg.Rules = Items.Select(_ => _.SaveConfig()).ToArray();
                _configService.Set(_cfg);
            }
            catch (Exception e)
            {
                IoC.Get<IWindowManager>().ShowError("Error occured to save rules", e.Message, e);
                _logger.Error(e, $"Error occured to save rules:{e.Message}");
            }
        }


        private void LoadConfig()
        {
            try
            {
                Items.Clear();
                if (_cfg != null && _cfg.Rules != null)
                {
                    
                    foreach (var rule in _cfg.Rules)
                    {
                        var newRule = new RuleViewModel();
                        newRule.Load(rule);
                        Items.Add(newRule);
                    }
                }
            }
            catch (Exception e)
            {
                IoC.Get<IWindowManager>().ShowError("Error occured to load rules", e.Message, e);
                _logger.Error(e, $"Error occured to load rules:{e.Message}");
            }
        }

        #endregion


        protected override void OnDeactivate(bool close)
        {
            if (close)
            {
                SaveConfig();
            }
            base.OnDeactivate(close);
        }

        public void RemoveRule(RuleViewModel vm)
        {
            Items.Remove(vm);
        }
    }
}