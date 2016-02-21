using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Wox.Core.Resource;
using Wox.Core.UserSettings;

namespace Wox.ViewModel
{
    public class ProxyTabItemViewModel : BaseSettingTabItemViewModel
    {
        #region Override Methods

        protected override BaseViewModel GetContent()
        {
            return new ProxySettingViewModel();
        }

        protected override string GetHeaderResourceKey()
        {
            return "proxy";
        }

        #endregion
    }

    public class ProxySettingViewModel : BaseViewModel
    {
        private bool _enableProxy;
        private bool _enableServer;
        private bool _enableUserName;
        private bool _enablePort;
        private bool _enablePassword;

        public ProxySettingViewModel()
        {
            TestProxyCommand = new RelayCommand(parameter =>
            {
                TestProxy();
            });

            SaveProxyCommand = new RelayCommand(parameter =>
            {
                SaveProxySetting();
            });

            Initialize();
        }

        #region ViewModel Properties

        public bool EnableProxy
        {
            get { return _enableProxy; }
            set
            {
                _enableProxy = value;
                OnPropertyChanged("EnableProxy");

                ConfigProxySettingFields(value);
            }
        }

        public bool EnableServer
        {
            get { return _enableServer; }
            set
            {
                _enableServer = value;
                OnPropertyChanged("EnableServer");
            }
        }

        public bool EnableUserName
        {
            get
            {
                return _enableUserName;
            }
            set
            {
                _enableUserName = value;
                OnPropertyChanged("EnableUserName");
            }
        }

        public bool EnablePort
        {
            get
            {
                return _enablePort;
            }
            set
            {
                _enablePort = value;
                OnPropertyChanged("EnablePort");
            }
        }

        public bool EnablePassword
        {
            get
            {
                return _enablePassword;
            }
            set
            {
                _enablePassword = value;
                OnPropertyChanged("EnablePassword");
            }
        }

        public string Server { get; set; }

        public string UserName { get; set; }

        public string Port { get; set; }

        public string Password { get; set; }

        public ICommand TestProxyCommand { get; private set; }

        public  ICommand SaveProxyCommand { get; private set; }

        #endregion

        #region Private Methods

        private void Initialize()
        {
            EnableProxy = UserSettingStorage.Instance.ProxyEnabled;
            Server = UserSettingStorage.Instance.ProxyServer;
            if (UserSettingStorage.Instance.ProxyPort != 0)
            {
                Port = UserSettingStorage.Instance.ProxyPort.ToString();
            }
            UserName = UserSettingStorage.Instance.ProxyUserName;
            Password = UserSettingStorage.Instance.ProxyPassword;
        }

        private void ConfigProxySettingFields(bool enable)
        {
            this.EnableServer = enable;
            this.EnablePort = enable;
            this.EnableUserName = enable;
            this.EnablePassword = enable;
        }

        private void TestProxy()
        {
            if (string.IsNullOrEmpty(Server))
            {
                MessageBox.Show(InternationalizationManager.Instance.GetTranslation("serverCantBeEmpty"));
                return;
            }
            if (string.IsNullOrEmpty(Port))
            {
                MessageBox.Show(InternationalizationManager.Instance.GetTranslation("portCantBeEmpty"));
                return;
            }
            int port;
            if (!int.TryParse(Port, out port))
            {
                MessageBox.Show(InternationalizationManager.Instance.GetTranslation("invalidPortFormat"));
                return;
            }

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create("http://www.baidu.com");
            request.Timeout = 1000 * 5;
            request.ReadWriteTimeout = 1000 * 5;
            if (string.IsNullOrEmpty(UserName))
            {
                request.Proxy = new WebProxy(Server, port);
            }
            else
            {
                request.Proxy = new WebProxy(Server, port);
                request.Proxy.Credentials = new NetworkCredential(UserName, Password);
            }
            try
            {
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    MessageBox.Show(InternationalizationManager.Instance.GetTranslation("proxyIsCorrect"));
                }
                else
                {
                    MessageBox.Show(InternationalizationManager.Instance.GetTranslation("proxyConnectFailed"));
                }
            }
            catch
            {
                MessageBox.Show(InternationalizationManager.Instance.GetTranslation("proxyConnectFailed"));
            }
        }

        private void SaveProxySetting()
        {
            UserSettingStorage.Instance.ProxyEnabled = EnableProxy;

            int port = 80;
            if (UserSettingStorage.Instance.ProxyEnabled)
            {
                if (string.IsNullOrEmpty(Server))
                {
                    MessageBox.Show(InternationalizationManager.Instance.GetTranslation("serverCantBeEmpty"));
                    return;
                }
                if (string.IsNullOrEmpty(Port))
                {
                    MessageBox.Show(InternationalizationManager.Instance.GetTranslation("portCantBeEmpty"));
                    return;
                }
                if (!int.TryParse(Port, out port))
                {
                    MessageBox.Show(InternationalizationManager.Instance.GetTranslation("invalidPortFormat"));
                    return;
                }
            }

            UserSettingStorage.Instance.ProxyServer = Server;
            UserSettingStorage.Instance.ProxyPort = port;
            UserSettingStorage.Instance.ProxyUserName = UserName;
            UserSettingStorage.Instance.ProxyPassword = Password;
            UserSettingStorage.Instance.Save();

            MessageBox.Show(InternationalizationManager.Instance.GetTranslation("saveProxySuccessfully"));
        }

        #endregion

    }

}
