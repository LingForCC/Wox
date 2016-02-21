using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wox.ViewModel
{
    public class SettingViewModel : BaseViewModel
    {

        #region Constructor

        public SettingViewModel()
        {
            this.Tabs = new List<BaseSettingTabItemViewModel>();

        }

        #endregion

        #region ViewModel Properties

        public IEnumerable<BaseSettingTabItemViewModel> Tabs { get; private set; } 

        #endregion

    }
}
