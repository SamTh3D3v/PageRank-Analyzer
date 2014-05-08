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
        }

        #endregion
        #region Commands
      
        #endregion            
    }
}