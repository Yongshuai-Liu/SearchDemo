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
            var fileViewModels = new List<FileViewModel>();
            foreach (var file in files)
            {
                var fileViewModel = new FileViewModel()
                {
                    ID = file.ID,
                    Name = file.Name,
                    ContentType = file.ContentType,
                    CreateDateTime = file.CreatedDate,
                    FolderID = file.FolderID,
                    Dimension = file.Dimensions,
                    Link = file.Link,
                    Resolution = file.Resolution,
                    Size = file.Size.ToString(),
                };
                fileViewModels.Add(fileViewModel);
            }
            return View(fileViewModels);
        }
    }
}