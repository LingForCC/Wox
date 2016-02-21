using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wox.Core.Resource;

namespace Wox.ViewModel
{
    public abstract class BaseSettingTabItemViewModel : BaseViewModel
    {

        #region ViewModel Properties

        public string Header
        {
            get { return InternationalizationManager.Instance.GetTranslation(GetHeaderResourceKey()); }
        }

        public BaseViewModel Content
        {
            get { return GetContent(); }
        }

        #endregion

        #region Abstract Methods

        protected abstract string GetHeaderResourceKey();

        protected abstract BaseViewModel GetContent();

        #endregion
    }
}
