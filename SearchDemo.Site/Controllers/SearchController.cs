using NHunspell;
using SearchDemo.BLL.Services.ServiceImpl;
using SearchDemo.Data.DemoDB;
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
        private readonly List<File> _files;
        #endregion

        #region Constructor
        public SearchController(IFileRepository fileRepository)
        {
            _fileRepository = fileRepository;
            _files = _fileRepository.GetAll().ToList();
        }
        #endregion

        #region Public Methods
        public ActionResult SearchResult(string searchString)
        {
            // init Lucene Search Index            
            LuceneFileSearch.AddUpdateLuceneIndex(_files);
            LuceneFileSearch.Optimize();
            var result = LuceneFileSearch.Search(searchString);            
            var fileViewModel = new FileViewModel();
            var fileViewModels = fileViewModel.ConvertFromFileDB(result);
            return View(fileViewModels);
        }
        [HttpPost]
        public JsonResult SpellCheck(string searchString)
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
        [HttpPost]
        public JsonResult GetSuggestedWords(string searchString)
        {
            var suggestions = LuceneFileSearch.FindSuggestions(searchString);
            return Json(suggestions, JsonRequestBehavior.AllowGet);
        }
        #endregion
    }
}