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
    /// Interaction logic for HotkeySettingView.xaml
    /// </summary>
    public partial class HotkeySettingView : UserControl
    {
        public HotkeySettingView()
        {
            InitializeComponent();
        }

        private void TbHotkey_OnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            e.Handled = true;

            //when alt is pressed, the real key should be e.SystemKey
            var key = (e.Key == Key.System ? e.SystemKey : e.Key);

            var vm = this.DataContext as HotkeySettingViewModel;
            vm?.SetHotkey(key);
        }
    }
}
