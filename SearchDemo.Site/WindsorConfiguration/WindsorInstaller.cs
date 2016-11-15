using System.Web.Mvc;
using System.Reflection;
using System.Web.Http.Controllers;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using SearchDemo.Data.DemoDB;
using SearchDemo.Repositories.IRepositories.DemoDB;
using SearchDemo.Repositories.Repositories.DemoDB;

namespace TravelOnline.Internal.WindsorConfiguration
{
    public class WindsorInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {            
          
            //Register all controllers
            //Hello
            container.Register(
                 //All services
                 Classes.FromAssembly(Assembly.GetAssembly(typeof(IFileRepository))).
                    InSameNamespaceAs<FileRepository>().WithService.DefaultInterfaces().LifestylePerWebRequest(),

                //All MVC controllers
                Classes.FromThisAssembly().BasedOn<IController>().LifestyleTransient(),

                //All API Controllers
                Classes.FromThisAssembly().BasedOn<IHttpController>().LifestyleTransient(),

                //All DbContexts
                Component.For<DemoEntities>()
                                    .LifestylePerWebRequest().IsDefault()
                );
                
            //Register Facilities
            container.AddFacility<EntityFrameWorkRelatedFacility>();
            container.AddFacility<GeneralFacility>();


        }

    }
}