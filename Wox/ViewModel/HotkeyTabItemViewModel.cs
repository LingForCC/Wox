using NHotkey;
using NHotkey.Wpf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using Wox.Core.Resource;
using Wox.Core.UserSettings;
using Wox.Infrastructure.Hotkey;
using Wox.Plugin;

namespace Wox.ViewModel
{
    public class HotkeyTabItemViewModel : BaseSettingTabItemViewModel
    {

        #region Override Methods

        protected override BaseViewModel GetContent()
        {
            return new HotkeySettingViewModel();
        }

        protected override string GetHeaderResourceKey()
        {
            return "hotkey";
        }

        #endregion

    }

    public class HotkeySettingViewModel : BaseViewModel
    {

        #region Private Fields

        private string _currentHotkey;
        private string _message;
        private Visibility _messageVisibility;
        private Brush _messageForegroundColor;
        private ObservableCollection<CustomPluginHotkeyViewModel> _customPluginHotkeys;

        #endregion

        #region Constructor

        public HotkeySettingViewModel()
        {
            _messageVisibility = Visibility.Hidden;
            _messageForegroundColor = new SolidColorBrush(Colors.Green);
            _customPluginHotkeys = new ObservableCollection<CustomPluginHotkeyViewModel>();

            Initialize();
        }

        #endregion

        #region ViewModel Properties

        public string CurrentHotkey
        {
            get { return _currentHotkey; }
            set
            {
                _currentHotkey = value;
                OnPropertyChanged("CurrentHotkey");
            }
        }

        public string Message
        {
            get { return _message; }
            set
            {
                _message = value;
                OnPropertyChanged("Message");
            }
        }

        public Visibility MessageVisibility
        {
            get { return _messageVisibility;}
            set
            {
                _messageVisibility = value;
                OnPropertyChanged("MessageVisibility");
            }
        }

        public Brush MessageForegroundColor
        {
            get { return _messageForegroundColor; }
            set
            {
                _messageForegroundColor = value;
                OnPropertyChanged("MessageForegroundColor");
            }
        }

        public ObservableCollection<CustomPluginHotkeyViewModel> CustomPluginHotkeys
        {
            get { return _customPluginHotkeys; }
        }

        public CustomPluginHotkeyViewModel SelectedCustomPluginHotkey { get; set; }

        public ICommand AddCommand { get; private set; }

        public ICommand EditCommand { get; private set; }

        public ICommand DeleteCommand { get; private set; }

        #endregion

        #region Public Methods

        public void SetHotkey(Key key)
        {

            MessageVisibility = Visibility.Hidden;

            SpecialKeyState specialKeyState = GlobalHotkey.Instance.CheckModifiers();

            var hotkeyModel = new HotkeyModel(
                specialKeyState.AltPressed,
                specialKeyState.ShiftPressed,
                specialKeyState.WinPressed,
                specialKeyState.CtrlPressed,
                key);

            var hotkeyString = hotkeyModel.ToString();

            if (hotkeyString == CurrentHotkey)
            {
                return;
            }

            CurrentHotkey = hotkeyString;

            Application.Current.Dispatcher.InvokeAsync(async () =>
            {
                await Task.Delay(500);
                SetHotkey(hotkeyModel);
            });
        }

        public void AddCustomPluginHotkey(CustomPluginHotkey hotkey)
        {
            _customPluginHotkeys.Add(new CustomPluginHotkeyViewModel(hotkey));
        }

        public void UpdateCustomPluginHotkey(string oldHotkey, CustomPluginHotkey newHotkey)
        {
            var index = _customPluginHotkeys.IndexOf(SelectedCustomPluginHotkey);
            if (index > -1)
            {
                var newCustomPluginHotkeyViewModel = new CustomPluginHotkeyViewModel(newHotkey);
                _customPluginHotkeys[index] = newCustomPluginHotkeyViewModel;
                SelectedCustomPluginHotkey = newCustomPluginHotkeyViewModel;
            }

        }

        #endregion

        #region Private Methods

        private void Initialize()
        {
            CurrentHotkey = UserSettingStorage.Instance.Hotkey;

            foreach (var hotkey in UserSettingStorage.Instance.CustomPluginHotkeys)
            {
                _customPluginHotkeys.Add(new CustomPluginHotkeyViewModel(hotkey));
            }

            DeleteCommand = new RelayCommand(parameter => DeleteCommandExe());

            AddCommand = new RelayCommand(parameter => AddCommandExe());

            EditCommand = new RelayCommand(parameter => EditCommandExe());
        }

        private void DeleteCommandExe()
        {
            CustomPluginHotkey item = SelectedCustomPluginHotkey?.RawHotkey;
            if (item == null)
            {
                MessageBox.Show(InternationalizationManager.Instance.GetTranslation("pleaseSelectAnItem"));
                return;
            }

            string deleteWarning = string.Format(InternationalizationManager.Instance.GetTranslation("deleteCustomHotkeyWarning"), item.Hotkey);
            if (MessageBox.Show(deleteWarning, InternationalizationManager.Instance.GetTranslation("delete"), MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                UserSettingStorage.Instance.CustomPluginHotkeys.Remove(item);
                UserSettingStorage.Instance.Save();
                RemoveHotkey(item.Hotkey);

                CustomPluginHotkeys.Remove(SelectedCustomPluginHotkey);
                SelectedCustomPluginHotkey = null;
            }
        }

        private void EditCommandExe()
        {
            CustomPluginHotkey item = SelectedCustomPluginHotkey?.RawHotkey;
            if (item != null)
            {
                CustomQueryHotkeySetting window = new CustomQueryHotkeySetting(this);
                window.UpdateItem(item);
                window.ShowDialog();
            }
            else
            {
                MessageBox.Show(InternationalizationManager.Instance.GetTranslation("pleaseSelectAnItem"));
            }
        }

        private void AddCommandExe()
        {
            new CustomQueryHotkeySetting(this).ShowDialog();
        }

        private void SetHotkey(HotkeyModel keyModel)
        {
            var available = CheckHotkeyAvailability(keyModel);
            if (!available)
            {

                MessageForegroundColor = new SolidColorBrush(Colors.Red);
                Message = InternationalizationManager.Instance.GetTranslation("hotkeyUnavailable");
            }
            else
            {
                ConfigHotkeySettings(keyModel);

                MessageForegroundColor = new SolidColorBrush(Colors.Green);
                Message = InternationalizationManager.Instance.GetTranslation("succeed");
            }

            MessageVisibility = Visibility.Visible;
        }

        private bool CheckHotkeyAvailability(HotkeyModel keyModel)
        {
            try
            {
                HotkeyManager.Current.AddOrReplace("HotkeyAvailabilityTest", keyModel.CharKey, keyModel.ModifierKeys, (sender, e) => { });

                return true;
            }
            catch(Exception ex)
            {
            }
            finally
            {
                HotkeyManager.Current.Remove("HotkeyAvailabilityTest");
            }

            return false;
        }

        private void ConfigHotkeySettings(HotkeyModel hotkey)
        {
            try
            {
                HotkeyManager.Current.AddOrReplace(CurrentHotkey, hotkey.CharKey, hotkey.ModifierKeys, delegate
                {
                    if (!App.Window.IsVisible)
                    {
                        App.API.ShowApp();
                    }
                    else
                    {
                        App.API.HideApp();
                    }
                });

                RemoveHotkey(UserSettingStorage.Instance.Hotkey);
                UserSettingStorage.Instance.Hotkey = CurrentHotkey;
                UserSettingStorage.Instance.Save();
            }
            catch (Exception)
            {
                string errorMsg = string.Format(InternationalizationManager.Instance.GetTranslation("registerHotkeyFailed"), CurrentHotkey);
                MessageBox.Show(errorMsg);
            }
            
        }

        private void RemoveHotkey(string hotkeyStr)
        {
            if (!string.IsNullOrEmpty(hotkeyStr))
            {
                HotkeyManager.Current.Remove(hotkeyStr);
            }
        }

        #endregion

    }

    public class CustomPluginHotkeyViewModel : BaseViewModel
    {

        public CustomPluginHotkeyViewModel(CustomPluginHotkey customPluginHotkey)
        {
            RawHotkey = customPluginHotkey;
        }

        public string Hotkey
        {
            get { return RawHotkey.Hotkey; }
        }

        public string ActionKeywords
        {
            get { return RawHotkey.ActionKeyword; }
        }

        public CustomPluginHotkey RawHotkey { get; private set; }
    }
}
