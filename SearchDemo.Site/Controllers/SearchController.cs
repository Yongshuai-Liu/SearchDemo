using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.QueryParsers;
using Lucene.Net.Search;
using Lucene.Net.Store;
using Version = Lucene.Net.Util.Version;
using System.Web.Mvc;

namespace SearchDemo.Site.Controllers
{
    public class SearchController : Controller
    {
       
        // GET: Search
        public ActionResult Index()
        {
            return View();
        }
    }
}