using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using GalaSoft.MvvmLight.Messaging;
using GraphSharp.Controls;
using LuceneSearchClient.Model;

namespace LuceneSearchClient.Views
{

    public partial class SimilatorView : Page
    {
        public SimilatorView()
        {
            InitializeComponent();                          
            Messenger.Default.Register<NotificationMessage>(this, (msg) =>
            {
                switch (msg.Notification)
                {
                    case "ShowDefineMatricePane":
                        ShowDefineMatricePane();
                        break;
                    //case "HideDefineMatricePane":
                    //    HideDefineMatricePane();
                    //    break;
                    case "ShowVisualWebGraphPane":
                        ShowVisualWebGraphPane();
                        break;
                    //case "HideVisualWebGraphPane":
                    //    HideVisualWebGraphPane();
                    //    break;
                    case "ShowCalculationPane":
                        ShowCalculationPane();
                        break;
                    //case "HideCalculationPane":
                    //    HideCalculationPane();
                    //    break;
                    case "ShowEignValuesPane":
                        ShowEignValuesPane();
                        break;
                    //case "HideEignValuesPane":
                    //    HideEignValuesPane();
                    //    break;
                    case "ShowPrDfPane":
                        ShowPrDfPane();
                        break;
                    //case "HidePrDfPane":
                    //    HidePrDfPane();
                    //    break;
                    case "ShowItDfPane":
                        ShowItDfPane();
                        break;
                    //case "HideItDfPane":
                    //    HideItDfPane();
                    //    break;
                    case "ShowPrAprPagePane":
                        ShowPrAprPagePane();
                        break;
                    //case "HidePrAprPagePane":
                    //    HidePrAprPagePane();
                    //    break;
                    case "ShowPrAprIteMatPane":
                        ShowPrAprIteMatPane();
                        break;
                    //case "HidePrAprIteMatPane":
                    //    HidePrAprIteMatPane();
                    //    break;
                    case "ShowPrAprTimeMatPane":
                        ShowPrAprTimeMatPane();
                        break;
                    //case "HidePrAprTimeMatPane":
                    //    HidePrAprTimeMatPane();
                    //    break;
                    case "ShowAll":
                        ShowDefineMatricePane();
                        ShowVisualWebGraphPane();
                        ShowCalculationPane();
                        ShowCalculationPane();
                        ShowEignValuesPane();
                        ShowEignValuesPane();
                        ShowPrDfPane();
                        ShowPrDfPane();
                        ShowItDfPane();
                        ShowItDfPane();
                        ShowPrAprPagePane();
                        ShowPrAprPagePane();
                        ShowPrAprIteMatPane();
                        ShowPrAprIteMatPane();
                        ShowPrAprTimeMatPane();
                        break;
                }
            });
        }
        private void OnLoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.Header = e.Row.GetIndex();
        }
        #region Show and Hide Panels : Implemented Inhere Due To an Issue In AvalonDock AssemBly
        private void ShowDefineMatricePane()
        {
            DefMatPane.Show();
        }
        private void HideDefineMatricePane()
        {
            DefMatPane.Hide();
        }
        private void ShowVisualWebGraphPane()
        {
            VisWebPane.Show();
        }
        private void HideVisualWebGraphPane()
        {
            VisWebPane.Hide();
        }
        private void ShowCalculationPane()
        {
            CalcPane.Show();
        }
        private void HideCalculationPane()
        {
            CalcPane.Hide();
        }
        private void ShowEignValuesPane()
        {
            EignPane.Show();
        }
        private void HideEignValuesPane()
        {
            EignPane.Hide();
        }
        private void ShowPrDfPane()
        {
            PrDfPane.Show();
        }
        private void HidePrDfPane()
        {
            PrDfPane.Hide();
        }
        private void ShowItDfPane()
        {
            IteDfPane.Show();
        }
        private void HideItDfPane()
        {
            IteDfPane.Hide();
        }
        private void ShowPrAprPagePane()
        {
            PrAprPagePane.Show();
        }
        private void HidePrAprPagePane()
        {
            PrAprPagePane.Hide();
        }
        private void ShowPrAprIteMatPane()
        {
            PrAprIteMatPane.Show();
        }
        private void HidePrAprIteMatPane()
        {
            PrAprIteMatPane.Hide();
        }
        private void ShowPrAprTimeMatPane()
        {
            PrAprTimeMatPane.Show();
        }
        private void HidePrAprTimeMatPane()
        {
            PrAprTimeMatPane.Hide();
        }

       
        #endregion

        
    }
}