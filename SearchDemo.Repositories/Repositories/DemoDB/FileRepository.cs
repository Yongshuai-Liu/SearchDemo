using SearchDemo.Data.DemoDB;
using SearchDemo.Repositories.BaseRepositories;
using SearchDemo.Repositories.IRepositories.DemoDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SearchDemo.Repositories.Repositories.DemoDB
{
    public class FileRepository : BaseRepository<File, DemoEntities>, IFileRepository
    {
        public FileRepository(DemoEntities dbContext) : base(dbContext)
        {

        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }
    }
}
