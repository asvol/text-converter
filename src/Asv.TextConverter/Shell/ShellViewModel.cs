﻿using System;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using Caliburn.Micro;
using NLog;
using LogManager = NLog.LogManager;

namespace Asv.TextConverter
{
    public class ShellViewModelConfig
    {
        public string LastOpenFileFolder { get; set; } = string.Empty;
        public string LastFile { get; set; }
    }

    [Export(typeof(IShell))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class ShellViewModel : Conductor<TextLineViewModel>.Collection.OneActive, IShell
    {
        private readonly IConfiguration _cfgService;
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();
        private TextLineViewModel _selectedItem;
        private readonly ShellViewModelConfig _cfg;
        private bool _isSettingsOpened;

        [ImportingConstructor]
        public ShellViewModel(IConfiguration cfgService)
        {
            _cfgService = cfgService;
            Rules = new RuleListViewModel(_cfgService);
            Rules.ActivateWith(this);
            Rules.DeactivateWith(this);
            Rules.Parent = this;
            _cfg = _cfgService.Get<ShellViewModelConfig>();
            LoadConfig();
            DisplayName = "Text converter";

        }

        public void OpenRulesFolder()
        {
            try
            {
                var folder = (_cfgService as JsonOneFileConfiguration)?.FilePath;
                if (folder.IsNullOrWhiteSpace()) return;
                var dir = Path.GetFullPath(folder);
                dir = Path.GetDirectoryName(dir);
                Process.Start(new ProcessStartInfo("cmd", $"/c start {dir}") );
            }
            catch (Exception e)
            {
                IoC.Get<IWindowManager>().ShowError("Error occured to open folder", e.Message, e);
                _logger.Error(e, $"Error occured to open folder:{e.Message}");
            }
        }

        #region Settings

        public void OpenSettings()
        {
            IsSettingsOpened = !IsSettingsOpened;
        }

        public bool IsSettingsOpened
        {
            get { return _isSettingsOpened; }
            set
            {
                if (value == _isSettingsOpened) return;
                _isSettingsOpened = value;
                NotifyOfPropertyChange(() => IsSettingsOpened);
            }
        }

        #endregion

        #region Config

        private void LoadConfig()
        {
            try
            {
                if (!_cfg.LastFile.IsNullOrWhiteSpace() && File.Exists(_cfg.LastFile))
                {
                    Open(_cfg.LastFile);
                }
            }
            catch (Exception e)
            {
                IoC.Get<IWindowManager>().ShowError("Error occured to load config", e.Message, e);
                _logger.Error(e, $"Error occured to load config:{e.Message}");
            }
        }

        private void SaveConfig()
        {
            try
            {
                _cfgService.Set(_cfg);
            }
            catch (Exception e)
            {
                IoC.Get<IWindowManager>().ShowError("Error occured to save config", e.Message, e);
                _logger.Error(e, $"Error occured to save config:{e.Message}");
            }
        }

        #endregion

        public void Save()
        {
            try
            {
                string fileName = null;
                string ext = null;
                if (!_cfg.LastFile.IsNullOrWhiteSpace())
                {
                    fileName = Path.GetFileNameWithoutExtension(_cfg.LastFile);
                    ext = Path.GetExtension(_cfg.LastFile);
                }

                fileName = IoC.Get<IWindowManager>().ShowSaveFileDialog("Save file", initialDirectory:_cfg.LastOpenFileFolder, fileName: fileName+"-convert"+ext);
                if (fileName == null) return;
                File.WriteAllLines(fileName,Items.Select(_=>_.Result), Encoding.GetEncoding(1251));
            }
            catch (Exception e)
            {
                _cfg.LastFile = null;
                _cfg.LastOpenFileFolder = null;
                IoC.Get<IWindowManager>().ShowError("Error occured to save file", e.Message, e);
                _logger.Error(e, $"Error occured to save file:{e.Message}");
            }
        }

        public void Open()
        {
            try
            {
                var path = IoC.Get<IWindowManager>().ShowOpenFileDialog("Source file", "All files (*.*)|*.*", _cfg.LastOpenFileFolder);
                if (path.IsNullOrWhiteSpace()) return;
                _cfg.LastOpenFileFolder = Path.GetDirectoryName(path);
                _cfgService.Set(_cfg);
                Open(path);
            }
            catch (Exception e)
            {
                _cfg.LastFile = null;
                _cfg.LastOpenFileFolder = null;
                IoC.Get<IWindowManager>().ShowError("Error occured to Open file", e.Message, e);
                _logger.Error(e, $"Error occured to open file:{e.Message}");
            }
        }

        public void Open(string path)
        {
            try
            {
                var lines = File.ReadAllLines(path, Encoding.GetEncoding(1251));

                Items.Clear();

                foreach (var line in lines)
                {
                    ActivateItem(new TextLineViewModel(line));
                }

                _cfg.LastFile = path;
            }
            catch (Exception e)
            {
                IoC.Get<IWindowManager>().ShowError("Error occured to open file", e.Message, e);
                _logger.Error(e, $"Error occured to open file:{e.Message}");
            }
        }

        public void Update()
        {
            try
            {
                foreach (var item in Items)
                {
                    item.Convert(Rules.Items.Where(_=>_.IsEnabled));
                }
            }
            catch (Exception e)
            {
                IoC.Get<IWindowManager>().ShowError("Error occured to convert text", e.Message, e);
                _logger.Error(e, $"Error occured to convert text:{e.Message}");
            }
        }

        protected override void OnDeactivate(bool close)
        {
            if (close)
            {
                SaveConfig();
            }
            base.OnDeactivate(close);
        }

        protected override void OnViewLoaded(object view)
        {
            base.OnViewLoaded(view);
            var args = Environment.GetCommandLineArgs();
            var path = args.Length >= 2 ? args.Skip(1).First() : _cfg.LastFile;

            if (!path.IsNullOrWhiteSpace() && File.Exists(path))
            {
                Open(path);
            }
        }

        

        public RuleListViewModel Rules { get; }

        public TextLineViewModel SelectedItem
        {
            get { return _selectedItem; }
            set
            {
                if (Equals(value, _selectedItem)) return;
                _selectedItem = value;
                NotifyOfPropertyChange(() => SelectedItem);
            }
        }
    }
}
