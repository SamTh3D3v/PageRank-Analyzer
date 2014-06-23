using System.Diagnostics;
using System.Windows;
using GalaSoft.MvvmLight.Threading;

namespace LuceneSearchClient
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
            : base()
        {
            this.Dispatcher.UnhandledException += OnDispatcherUnhandledException;
        }

        void OnDispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            string errorMessage = string.Format("An unhandled exception occurred: {0}", e.Exception.Message);
            Debug.WriteLine("Error "+errorMessage);
            e.Handled = true;
        }
    }
}
