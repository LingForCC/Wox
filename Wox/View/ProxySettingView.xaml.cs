using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Wox.ViewModel;

namespace Wox.View
{
    /// <summary>
    /// Interaction logic for ProxySettingView.xaml
    /// </summary>
    public partial class ProxySettingView : UserControl
    {
        public ProxySettingView()
        {
            InitializeComponent();

            Password.KeyUp += (o, e) =>
            {
                var vm = DataContext as ProxySettingViewModel;
                if (vm != null)
                {
                    vm.Password = Password.Password;
                }
            };

            Loaded += (o, e) =>
            {
                var vm = DataContext as ProxySettingViewModel;
                if (null != vm)
                {
                    Password.Password = vm.Password;
                }
            };
        }
    }
}
