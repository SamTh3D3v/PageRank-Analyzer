using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using GalaSoft.MvvmLight.Messaging;

namespace LuceneSearchClient.Resources
{
    public partial class GraphResourceDictionary : ResourceDictionary
    {
        public GraphResourceDictionary()
        {
            this.InitializeComponent();
        }

        private void RemoveNode_ClickEvent(object sender, RoutedEventArgs e)
        {
            Messenger.Default.Send<String>((sender as MenuItem).Tag.ToString(), "removenode");         
        }

        private void StartArc_ClickEvent(object sender, RoutedEventArgs e)
        {
            Messenger.Default.Send<String>((sender as MenuItem).Tag.ToString(), "startedge");
        }

        private void EndArc_ClickEvent(object sender, RoutedEventArgs e)
        {
            Messenger.Default.Send<String>((sender as MenuItem).Tag.ToString(), "endedge");
        }
    }
}
