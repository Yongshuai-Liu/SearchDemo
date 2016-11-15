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
            LuceneFileSearch.AddUpdateLuceneIndex(files);
        }
        #endregion
        /// <summary>
        /// Return list of files to show
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            var files = LuceneFileSearch.GetAllIndexRecords();
            var fileViewModel = new FileViewModel();
            var fileViewModels = fileViewModel.ConvertFromFileDB(files);
            return View(fileViewModels);
        }

        public ActionResult SearchResult(string searchString)
        {
            var files = LuceneFileSearch.Search(searchString);
            var fileViewModel = new FileViewModel();
            var fileViewModels = fileViewModel.ConvertFromFileDB(files);
            return View(fileViewModels);
        }

    }
}