using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using LuceneSearchClient.Model;
using LuceneSearchLibrary;
using LuceneSearchLibrary.Model;


namespace LuceneSearchClient.ViewModel
{
    public class MainViewModel : ViewModelBase
    {        
        #region Consts
        public const string NavigationUriPropertyName = "NavigationUri";
        #endregion
        #region Fields
        private Uri _navigationUri;   
        #endregion
        #region Properties    
        public Uri NavigationUri
        {
            get
            {
                return _navigationUri;
            }
            set
            {
                if (_navigationUri == value)
                {
                    return;
                }                
                _navigationUri = value;
                RaisePropertyChanged(NavigationUriPropertyName);
            }
        }
        #endregion
        #region Ctors and Methods        
        public MainViewModel()
        {    
            NavigationUri=new Uri("../Views/SearchView.xaml",UriKind.RelativeOrAbsolute);
            Messenger.Default.Register<NotificationMessage>(this, (msg) =>
            {
                if (msg.Notification == "navigateToSearch")
                    NavigationUri = new Uri("../Views/SearchView.xaml", UriKind.RelativeOrAbsolute);
            }); 
        }

        #endregion
        #region Commands
        private RelayCommand _newSearchCommand;
        public RelayCommand NewSearchCommand
        {
            get
            {
                return _newSearchCommand
                    ?? (_newSearchCommand = new RelayCommand(
                                          () =>
                                          {
                                              NavigationUri=new Uri("../Views/SearchView.xaml",UriKind.RelativeOrAbsolute);  
                                          }));
            }
        }
        private RelayCommand _settingsCommand;  
        public RelayCommand SettingsCommand
        {
            get
            {
                return _settingsCommand
                    ?? (_settingsCommand = new RelayCommand(
                        () =>
                        {
                            NavigationUri = new Uri("../Views/SettingView.xaml", UriKind.RelativeOrAbsolute); 
                        }));
            }
        }
        private RelayCommand _allSimulationCommand;      
        public RelayCommand AllSimulationCommand
        {
            get
            {
                return _allSimulationCommand
                    ?? (_allSimulationCommand = new RelayCommand(
                                          () =>
                                          {
                                              NavigationUri=new Uri("../Views/SimilatorView.xaml",UriKind.RelativeOrAbsolute);  
                                          }));
            }
        }
        private RelayCommand _pageRankSettingsCommand;
        public RelayCommand PageRankCommand
        {
            get
            {
                return  _pageRankSettingsCommand
                    ?? ( _pageRankSettingsCommand = new RelayCommand(
                                          () =>
                                          {
                                               NavigationUri=new Uri("../Views/PageRankView.xaml",UriKind.RelativeOrAbsolute);  
                                          }));
            }
        }
        private RelayCommand _allPagesCommand;    
        public RelayCommand AllPagesCommand
        {
            get
            {
                return _allPagesCommand
                    ?? (_allPagesCommand = new RelayCommand(
                                          () =>
                                          {
                                              NavigationUri = new Uri("../Views/AllPagesView.xaml", UriKind.RelativeOrAbsolute);
                                              Messenger.Default.Send<NotificationMessage>(new NotificationMessage("ShowAll"));
                                          }));
            }
        }
        private RelayCommand _showDefineMatricePaneCommand;
        public RelayCommand ShowDefineMatricePaneCommand
        {
            get
            {
                return _showDefineMatricePaneCommand
                    ?? (_showDefineMatricePaneCommand = new RelayCommand(
                        () =>
                        {
                            NavigationUri = new Uri("../Views/SimilatorView.xaml", UriKind.RelativeOrAbsolute); 
                            Messenger.Default.Send<NotificationMessage>(new NotificationMessage("ShowDefineMatricePane"));
                        }));
            }
        }
        private RelayCommand _showVisualWebGraphPaneCommand;
        public RelayCommand ShowVisualWebGraphPaneCommand
        {
            get
            {
                return _showVisualWebGraphPaneCommand
                    ?? (_showVisualWebGraphPaneCommand = new RelayCommand(
                                          () =>
                                          {
                                              NavigationUri = new Uri("../Views/SimilatorView.xaml", UriKind.RelativeOrAbsolute); 
                                              Messenger.Default.Send<NotificationMessage>(
                                                  new NotificationMessage("ShowVisualWebGraphPane"));

                                          }));
            }
        }
        private RelayCommand _showCalculationPaneCommand;

        /// <summary>
        /// Gets the MyCommand.
        /// </summary>
        public RelayCommand ShowCalculationPaneCommand
        {
            get
            {
                return _showCalculationPaneCommand
                    ?? (_showCalculationPaneCommand = new RelayCommand(
                                          () =>
                                          {
                                              NavigationUri = new Uri("../Views/SimilatorView.xaml", UriKind.RelativeOrAbsolute); 
                                              Messenger.Default.Send<NotificationMessage>(
                                                  new NotificationMessage("ShowCalculationPane"));
                                          }));
            }
        }
        private RelayCommand _showEignValuesPaneCommand;
        public RelayCommand ShowEignValuesPaneCommand
        {
            get
            {
                return _showEignValuesPaneCommand
                    ?? (_showEignValuesPaneCommand = new RelayCommand(
                                          () =>
                                          {
                                              NavigationUri = new Uri("../Views/SimilatorView.xaml", UriKind.RelativeOrAbsolute); 
                                              Messenger.Default.Send<NotificationMessage>(
                                                  new NotificationMessage("ShowEignValuesPane"));
                                          }));
            }
        }
        private RelayCommand _showPrDfPaneCommand;
        public RelayCommand ShowPrDfPaneCommand
        {
            get
            {
                return _showPrDfPaneCommand
                    ?? (_showPrDfPaneCommand = new RelayCommand(
                                          () =>
                                          {
                                              NavigationUri = new Uri("../Views/SimilatorView.xaml", UriKind.RelativeOrAbsolute); 
                                              Messenger.Default.Send<NotificationMessage>(
                                                  new NotificationMessage("ShowPrDfPane"));
                                          }));
            }
        }
        private RelayCommand _showItDfPaneCommand;
        public RelayCommand ShowItDfPaneCommand
        {
            get
            {
                return  _showItDfPaneCommand
                    ?? ( _showItDfPaneCommand = new RelayCommand(
                                          () =>
                                          {
                                              NavigationUri = new Uri("../Views/SimilatorView.xaml", UriKind.RelativeOrAbsolute); 
                                              Messenger.Default.Send<NotificationMessage>(
                                                  new NotificationMessage("ShowItDfPane"));

                                          }));
            }
        }
        private RelayCommand _showPrAprPagePaneCommand;

        public RelayCommand ShowPrAprPagePaneCommand
        {
            get
            {
                return _showPrAprPagePaneCommand
                    ?? (_showPrAprPagePaneCommand = new RelayCommand(
                                          () =>
                                          {
                                              NavigationUri = new Uri("../Views/SimilatorView.xaml", UriKind.RelativeOrAbsolute); 
                                              Messenger.Default.Send<NotificationMessage>(
                                                  new NotificationMessage("ShowPrAprPagePane"));

                                          }));
            }
        }
        private RelayCommand _showPrAprIteMatPaneCommand;   
        public RelayCommand ShowPrAprIteMatPaneCommand
        {
            get
            {
                return _showPrAprIteMatPaneCommand
                    ?? (_showPrAprIteMatPaneCommand = new RelayCommand(
                                          () =>
                                          {
                                              NavigationUri = new Uri("../Views/SimilatorView.xaml", UriKind.RelativeOrAbsolute); 
                                              Messenger.Default.Send<NotificationMessage>(
                                                  new NotificationMessage("ShowPrAprIteMatPane"));
                                               
                                          }));
            }
        }
        private RelayCommand _showPrAprTimeMatPaneCommand;
        public RelayCommand ShowPrAprTimeMatPaneCommand
        {
            get
            {
                return _showPrAprTimeMatPaneCommand
                    ?? (_showPrAprTimeMatPaneCommand = new RelayCommand(
                                          () =>
                                          {
                                              NavigationUri = new Uri("../Views/SimilatorView.xaml", UriKind.RelativeOrAbsolute);  
                                              Messenger.Default.Send<NotificationMessage>(
                                                  new NotificationMessage("ShowPrAprTimeMatPane"));

                                          }));
            }
        }
        #endregion            
    }
}