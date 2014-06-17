using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using GraphSharp.Controls;
using LuceneSearchClient.Model;

namespace LuceneSearchClient.Resources
{
    public partial class MyResourceDictionary : ResourceDictionary
    {
        public MyResourceDictionary()
        {
            InitializeComponent();
        }
        public void RemoveNode_ClickEvent(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Remove");
            var mi = (MenuItem)sender;
            VertexControl vc = null;
            if (mi != null)
            {
                vc = (VertexControl)((ContextMenu)mi.Parent).PlacementTarget;
                //my own class inherited from GraphSharp.Controls.VertexControl
                //you can use what you have directly
                var pv = (WebVertex)vc.Vertex;
                MessageBox.Show(pv.Label);
                //same thing with graph class
                //graph is stored globally
                //PocGraph gg = App.vm.Graph;

                //switch ((ContextCommands)mi.CommandParameter){
                //    case ContextCommands.REMOVE:
                //        gg.RemoveVertex(pv);
                //        break;
                //    ...

            }
        }
        public void StartArc_ClickEvent(object sender, RoutedEventArgs e)
        {

        }
        public void EndArc_ClickEvent(object sender, RoutedEventArgs e)
        {

        }

        private void Okey(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }
    }
}

