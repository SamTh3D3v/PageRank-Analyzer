using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QuickGraph;

namespace LuceneSearchClient.Model
{
    public class WebGraph:BidirectionalGraph<WebVertex, WebEdge>
    {
         public WebGraph() { }

        public WebGraph(bool parallelEdgesAllowed)
             : base(parallelEdgesAllowed) { }

        public WebGraph(bool parallelEdgesAllowed, int vertexCapacity)
            : base(parallelEdgesAllowed, vertexCapacity) { }
    }
}
