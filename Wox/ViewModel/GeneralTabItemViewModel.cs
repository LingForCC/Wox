using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Wox.Core.Resource;
using Application = System.Windows.Forms.Application;
using Wox.Core.UserSettings;

namespace Wox.ViewModel
{
    public class GeneralTabItemViewModel : BaseSettingTabItemViewModel
    {

        #region Override Methods

        protected override BaseViewModel GetContent()
        {
            return new GeneralSettingViewModel();
        }

        protected override string GetHeaderResourceKey()
        {
            return "general";
        }

        #endregion
    }

    public class GeneralSettingViewModel : BaseViewModel
    {

        #region Private Fields

        private bool _startWoxOnSystemStartup;
        private bool _hideWhenDeactive;
        private bool _dontPromptUpdateMsg;
        private bool _rememberLastLocation;
        private bool _ignoreHotkeysOnFullscreen;
        private List<LanguageOption> _languageOptions;
        private LanguageOption _selectedLanguageOption;
        private List<int> _maxResultToShowCollection;
        private int _maxResultToShow;

        #endregion

        #region Constructor

        public GeneralSettingViewModel()
        {
            _startWoxOnSystemStartup = CheckApplicationIsStartupWithWindow();
            _hideWhenDeactive = UserSettingStorage.Instance.HideWhenDeactive;
            _dontPromptUpdateMsg = UserSettingStorage.Instance.DontPromptUpdateMsg;
            _rememberLastLocation = UserSettingStorage.Instance.RememberLastLaunchLocation;
            _ignoreHotkeysOnFullscreen = UserSettingStorage.Instance.IgnoreHotkeysOnFullscreen;

            _languageOptions = new List<LanguageOption>();
            InitializeLanguageOptions();

            _maxResultToShowCollection = new List<int>();
            InitializeMaxResultToShow();
        }

        #endregion

        #region StartWoxOnSystemStartup

        public bool StartWoxOnSystemStartup
        {
            get { return _startWoxOnSystemStartup; }
            set
            {
                _startWoxOnSystemStartup = value;
                OnPropertyChanged("StartWoxOnSystemStartup");

                ConfigStartWoxOnSystemStartup(value);
            }
        }

        private void ConfigStartWoxOnSystemStartup(bool startUp)
        {
            if (startUp)
            {
                AddApplicationToStartup();
                UserSettingStorage.Instance.StartWoxOnSystemStartup = true;
            }
            else
            {
                RemoveApplicationFromStartup();
                UserSettingStorage.Instance.StartWoxOnSystemStartup = false;
            }

            UserSettingStorage.Instance.Save();
        }

        private void AddApplicationToStartup()
        {
            using (var key = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true))
            {
                key?.SetValue("Wox", "\"" + Application.ExecutablePath + "\" --hidestart");
            }
        }

        private void RemoveApplicationFromStartup()
        {
            using (var key = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true))
            {
                key?.DeleteValue("Wox", false);
            }
        }

        private bool CheckApplicationIsStartupWithWindow()
        {
            using (var key = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true))
            {
                return key != null && key.GetValue("Wox") != null;
            }
        }

        #endregion

        #region HideWhenDeactive

        public bool HideWhenDeactive
        {
            get { return _hideWhenDeactive; }
            set
            {
                _hideWhenDeactive = value;
                OnPropertyChanged("HideWhenDeactive");

                ConfigHideWhenDeactive(value);
            }
        }

        private void ConfigHideWhenDeactive(bool hideWhenDeactive)
        {
            UserSettingStorage.Instance.HideWhenDeactive = hideWhenDeactive;
            UserSettingStorage.Instance.Save();
        }

        #endregion

        #region DontPromptUpdateMsg

        public bool DontPromptUpdateMsg
        {
            get { return _dontPromptUpdateMsg; }
            set
            {
                _dontPromptUpdateMsg = value;
                OnPropertyChanged("DontPromptUpdateMsg");

                ConfigDontPromptUpdateMsg(value);
            }
        }

        private void ConfigDontPromptUpdateMsg(bool dontPromptUpdateMsg)
        {
            UserSettingStorage.Instance.DontPromptUpdateMsg = dontPromptUpdateMsg;
            UserSettingStorage.Instance.Save();
        }

        #endregion

        #region RememberLastLocation

        public bool RememberLastLocation
        {
            get { return _rememberLastLocation; }
            set
            {
                _rememberLastLocation = value;
                OnPropertyChanged("RememberLastLocation");

                ConfigRememberLastLocation(value);
            }
        }

        private void ConfigRememberLastLocation(bool rememberLastLocation)
        {
            UserSettingStorage.Instance.RememberLastLaunchLocation = rememberLastLocation;
            UserSettingStorage.Instance.Save();
        }

        #endregion

        #region IgnoreHotkeysOnFullscreen

        public bool IgnoreHotkeysOnFullscreen
        {
            get { return _ignoreHotkeysOnFullscreen; }
            set
            {
                _ignoreHotkeysOnFullscreen = value;
                OnPropertyChanged("IgnoreHotkeysOnFullscreen");

                ConfigIgnoreHotkeysOnFullscreen(value);
            }
        }

        private void ConfigIgnoreHotkeysOnFullscreen(bool ignoreHotkeysOnFullscreen)
        {
            UserSettingStorage.Instance.IgnoreHotkeysOnFullscreen = ignoreHotkeysOnFullscreen;
            UserSettingStorage.Instance.Save();
        }

        #endregion

        #region LanguageOptions

        private void InitializeLanguageOptions()
        {
            var languages = InternationalizationManager.Instance.LoadAvailableLanguages();
            foreach (var lan in languages)
            {
                _languageOptions.Add(new LanguageOption((lan)));       
            }

            _selectedLanguageOption = _languageOptions.Find(
                lo => { return lo.Language.LanguageCode == UserSettingStorage.Instance.Language; });
        }

        public IEnumerable<LanguageOption> LanguageOptions
        {
            get { return _languageOptions; }
        } 

        public LanguageOption SelectedLanguageOption
        {
            get { return _selectedLanguageOption; }
            set
            {
                _selectedLanguageOption = value;
                OnPropertyChanged("SelectedLanguageOption");

                ConfigLanguage(value);
            }
        }

        private void ConfigLanguage(LanguageOption languageOption)
        {
            InternationalizationManager.Instance.ChangeLanguage(languageOption.Language);
        }

        #endregion

        #region MaxResultToShow

        private void InitializeMaxResultToShow()
        {
            _maxResultToShowCollection.AddRange(Enumerable.Range(2, 16));
            _maxResultToShow = UserSettingStorage.Instance.MaxResultsToShow;
            _maxResultToShow = _maxResultToShow == 0 ? 2 : _maxResultToShow;
        }

        public IEnumerable<int> MaxResultToShowCollection
        {
            get
            {
                return _maxResultToShowCollection;
            }
        }

        public int MaxResultToShow
        {
            get
            {
                return _maxResultToShow;
            }
            set
            {
                _maxResultToShow = value;
                OnPropertyChanged("MaxResultToShow");

                ConfigMaxResultToShow(value);
            }
        }

        private void ConfigMaxResultToShow(int maxResultToShow)
        {
            UserSettingStorage.Instance.MaxResultsToShow = _maxResultToShow;
            UserSettingStorage.Instance.Save();
        }

        #endregion
    }

    public class LanguageOption : BaseViewModel
    {

        public LanguageOption(Language language)
        {
            Language = language;
        }

        public string Display
        {
            get { return Language?.Display; }
        }

        public Language Language
        {
            get;
            private set;
        }
    }

}
