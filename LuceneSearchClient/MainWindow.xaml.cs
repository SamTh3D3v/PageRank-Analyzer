using System.Windows;
using GalaSoft.MvvmLight.Messaging;
using LuceneSearchClient.ViewModel;
using LuceneSearchClient.Views;

namespace LuceneSearchClient
{

    public partial class MainWindow : Window
    {
        private SettingView _settingsWindow;
        public MainWindow()
        {
            InitializeComponent();
            Closing += (s, e) => ViewModelLocator.Cleanup();


           
            //Messenger.Default.Register<NotificationMessage>(this, (msg) =>
            //{

            //    if (msg.Notification == "killsettingswindow" && _settingsWindow != null)
            //        _settingsWindow.Close();
            //});
        }
    }
}