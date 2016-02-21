using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms.VisualStyles;

namespace Wox.ViewModel
{
    public class SettingViewModel : BaseViewModel
    {

        private List<BaseSettingTabItemViewModel> _tabs; 
        private BaseSettingTabItemViewModel _selectedTabItemViewModel;

        #region Constructor

        public SettingViewModel()
        {
            _tabs = new List<BaseSettingTabItemViewModel>();
            _tabs.Add(new GeneralTabItemViewModel());

            this.SelectedTabItemViewModel = _tabs[0];
        }

        #endregion

        #region ViewModel Properties

        public IEnumerable<BaseSettingTabItemViewModel> Tabs
        {
            get { return _tabs; }
        }

        public BaseSettingTabItemViewModel SelectedTabItemViewModel
        {
            get { return _selectedTabItemViewModel;}
            set
            {
                _selectedTabItemViewModel = value;
                OnPropertyChanged("SelectedTabItemViewModel");
            }
        }

        #endregion

        public void SwitchTo(string tabName)
        {
            var toSelectTab = _tabs.Find(vm => vm.Header == tabName);
            if (toSelectTab != null)
            {
                SelectedTabItemViewModel = toSelectTab;
            }
        }

    }
}
