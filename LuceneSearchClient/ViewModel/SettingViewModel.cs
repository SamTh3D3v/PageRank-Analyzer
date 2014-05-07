using System;
using System.Windows.Forms;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using LuceneSearchClient.Model;
using OpenFileDialog = Microsoft.Win32.OpenFileDialog;


namespace LuceneSearchClient.ViewModel
{
 
    public class SettingViewModel : ViewModelBase
    {
        #region Consts
        public const string WebSiteUrlPropertyName = "WebSiteUrl";
        public const string WebSiteLocationPropertyName = "WebSiteLocation";
        public const string IndexLocationPropertyName = "IndexLocation";
        #endregion
        #region Fields
        private String _webSiteUrl = "";
        private String _webSiteLocation = "";
        private String _indexLocation = "";
        #endregion
        #region Properties         
        public String WebSiteUrl
        {
            get
            {
                return _webSiteUrl;
            }

            set
            {
                if (_webSiteUrl == value)
                {
                    return;
                }
                _webSiteUrl = value;
                RaisePropertyChanged(WebSiteUrlPropertyName);
            }
        }
        public String WebSiteLocation
        {
            get
            {
                return _webSiteLocation;
            }

            set
            {
                if (_webSiteLocation == value)
                {
                    return;
                }
                _webSiteLocation = value;
                RaisePropertyChanged(WebSiteLocationPropertyName);
            }
        }
        public String IndexLocation
        {
            get
            {
                return _indexLocation;
            }

            set
            {
                if (_indexLocation == value)
                {
                    return;
                }
                _indexLocation = value;
                RaisePropertyChanged(IndexLocationPropertyName);
            }
        }
        #endregion        
        #region  Constructors and Methods
        public SettingViewModel()
        {
        }
        #endregion
        #region Commands
        private RelayCommand _webSiteLocationCommand;     
        public RelayCommand WebSiteLocationCommand
        {
            get
            {
                return _webSiteLocationCommand
                    ?? (_webSiteLocationCommand = new RelayCommand(
                                          () =>
                                          {
                                              var fbd = new FolderBrowserDialog();
                                              fbd.ShowDialog();
                                              WebSiteLocation = fbd.SelectedPath;                                          
                                          }));
            }
        }
        private RelayCommand _indexLocationCommand;      
        public RelayCommand IndexLocationCommand
        {
            get
            {
                return _indexLocationCommand
                    ?? (_indexLocationCommand = new RelayCommand(
                                          () =>
                                          {
                                              var fbd = new FolderBrowserDialog();
                                              fbd.ShowDialog();
                                              IndexLocation = fbd.SelectedPath; 
                                          }));
            }
        }
        private RelayCommand _saveCommand;

        public RelayCommand SaveCommand
        {
            get
            {
                return _saveCommand
                       ?? (_saveCommand = new RelayCommand(
                           () =>
                           {
                               Messenger.Default.Send<WebSite>(new WebSite()
                               {
                                   WebSiteIndex = _indexLocation,
                                   WebSiteLocation = _webSiteLocation,
                                   WebSiteUrl = _webSiteUrl
                               }, "savesettings");
                               Messenger.Default.Send<NotificationMessage>(new NotificationMessage("killsettingswindow"));
                           }));
            }
        }

        #endregion
       
    }
}