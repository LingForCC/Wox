using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Wox.Core.Resource;
using Wox.Core.Updater;
using Wox.Core.UserSettings;

namespace Wox.ViewModel
{
    public class AboutTabItemViewModel : BaseSettingTabItemViewModel
    {

        #region Override Methods

        protected override BaseViewModel GetContent()
        {
            return new AboutSettingViewModel();
        }

        protected override string GetHeaderResourceKey()
        {
            return "about";
        }

        #endregion
    }

    public class AboutSettingViewModel : BaseViewModel
    {

        public AboutSettingViewModel()
        {
            OpenWebsiteCommand = new RelayCommand((parameter) =>
            {
                Process.Start(parameter.ToString());
            });
        }

        #region ViewModel Properties

        public string ActiveTimes
        {
            get
            {
                return string.Format(InternationalizationManager.Instance.GetTranslation("about_activate_times"),
                    UserSettingStorage.Instance.ActivateTimes);
            }
        }

        public string Version
        {
            get
            {
                return UpdaterManager.Instance.CurrentVersion.ToString();
            }
        }

        public ICommand OpenWebsiteCommand { get; private set; }

        #endregion

    }
}
