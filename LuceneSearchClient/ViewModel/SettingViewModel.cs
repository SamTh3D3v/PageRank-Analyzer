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
        public const string SaveButtonEnabledPropertyName = "SaveButtonEnabled";
        public const string ExistingIndexPropertyName = "NewIndex";
        #endregion
        #region Fields
        private String _webSiteUrl = "http://www.troyhunt.com/";
        private String _webSiteLocation = "";
        private String _indexLocation = "";
        private bool _saveButtonEnabled = false;
        private bool _newIndex = false;
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
                if (WebSiteLocation.Trim() != "" && IndexLocation.Trim() != "")
                    SaveButtonEnabled = true;
                else
                    SaveButtonEnabled = true;
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
                if (WebSiteLocation.Trim() != "" && IndexLocation.Trim() != "")
                    SaveButtonEnabled = true;
                else
                    SaveButtonEnabled = true;
            }
        }                    
        public bool SaveButtonEnabled
        {
            get
            {
                return _saveButtonEnabled;
            }

            set
            {
                if (_saveButtonEnabled == value)
                {
                    return;
                }                
                _saveButtonEnabled = value;
                RaisePropertyChanged(SaveButtonEnabledPropertyName);
            }
        }

        public bool NewIndex
        {
            get
            {
                return _newIndex;
            }

            set
            {
                if (_newIndex == value)
                {
                    return;
                }
                
                _newIndex = value;
                RaisePropertyChanged(ExistingIndexPropertyName);
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
                               Messenger.Default.Send<NotificationMessage>(new NotificationMessage("navigateToSearch"));
                               Messenger.Default.Send<WebSite>(new WebSite()
                               {
                                   WebSiteIndex = _indexLocation,
                                   WebSiteLocation = _webSiteLocation,
                                   WebSiteUrl = _webSiteUrl,
                                   NewIndex=NewIndex,
                               }, "savesettings");
                               
                           }));
            }

        }
        private RelayCommand _cancelSettingsCommand;
        public RelayCommand CancelSettingsCommand
        {
            get
            {
                return _cancelSettingsCommand
                    ?? (_cancelSettingsCommand = new RelayCommand(
                                          () => Messenger.Default.Send<NotificationMessage>(new NotificationMessage("navigateToSearch"))));
            }
        }
        #endregion
       
    }
}