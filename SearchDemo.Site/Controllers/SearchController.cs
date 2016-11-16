using NHunspell;
using SearchDemo.BLL.Services.ServiceImpl;
using SearchDemo.Repositories.IRepositories.DemoDB;
using SearchDemo.ViewModels.Site;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SearchDemo.Site.Controllers
{
    public class SearchController : Controller
    {
        #region Fields
        private readonly IFileRepository _fileRepository;
        #endregion

        #region Constructor
        public SearchController(IFileRepository fileRepository)
        {
            _fileRepository = fileRepository;
            var files = _fileRepository.GetAll();
            // init Lucene Search Index
            LuceneFileSearch.AddUpdateLuceneIndex(files);
            LuceneFileSearch.Optimize();
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Return list of files to show
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            var files = LuceneFileSearch.GetAllIndexRecords();
            LuceneFileSearch.Optimize();
            var fileViewModel = new FileViewModel();
            var fileViewModels = fileViewModel.ConvertFromFileDB(files);
            return View(fileViewModels);
        }

        public ActionResult SearchResult(string searchString)
        {
            var files = LuceneFileSearch.Search(searchString);
            LuceneFileSearch.Optimize();
            var fileViewModel = new FileViewModel();
            var fileViewModels = fileViewModel.ConvertFromFileDB(files);
            return View(fileViewModels);
        }
        [HttpPost]
        public JsonResult GetSuggestions(string searchString)
        {
            var suggestions = new List<string>();
            using (Hunspell hunspell = new Hunspell(System.Web.HttpContext.Current.Server.MapPath("~/Assets/dictionaries/en_us.aff"),
    System.Web.HttpContext.Current.Server.MapPath("~/Assets/dictionaries/en_us.dic")))
            {
                bool correct = hunspell.Spell(searchString);
                if (!correct)
                {
                    suggestions = hunspell.Suggest(searchString);
                }
            }
            return Json(suggestions, JsonRequestBehavior.AllowGet);
        }
        #endregion
    }
}