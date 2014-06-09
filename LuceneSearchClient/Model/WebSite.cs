using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuceneSearchClient.Model
{
   public  class WebSite
    {
        #region Properties

       public string WebSiteUrl
       {
           get; set;
       }
       public string WebSiteLocation
       {
           get;
           set;
       }
       public string WebSiteIndex
       {
           get;
           set;
       }

       public bool NewIndex
       {
           get;
           set;
       }

        #endregion
    }
}
