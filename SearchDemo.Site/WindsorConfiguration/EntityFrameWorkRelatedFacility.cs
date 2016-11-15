using Castle.MicroKernel.Facilities;
using Castle.MicroKernel.Registration;
using SearchDemo.Repositories.IRepositories.DemoDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;


namespace TravelOnline.Internal.WindsorConfiguration
{
    public class EntityFrameWorkRelatedFacility : AbstractFacility
    {
        protected override void Init()
        {
            //Repositories
            Kernel.Register(Classes.FromAssembly(Assembly.GetAssembly(typeof(IFileRepository))).
                    InSameNamespaceAs<IFileRepository>().WithService.DefaultInterfaces().LifestylePerWebRequest());
        }
    }
}