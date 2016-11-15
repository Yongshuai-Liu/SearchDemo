using System;
using System.Web.Http.Dispatcher;
using System.Net.Http;
using Castle.Windsor;
using System.Web.Http.Controllers;

namespace TravelOnline.Internal.WindsorConfiguration
{
    public class WindsorHttpControllerFactory : IHttpControllerActivator
    {
        private readonly IWindsorContainer container;

        public WindsorHttpControllerFactory(IWindsorContainer container)
        {
            this.container = container;
        }

        public IHttpController Create(HttpRequestMessage request,
            HttpControllerDescriptor controllerDescriptor, Type controllerType)
        {
            var controller =
                (IHttpController)this.container.Resolve(controllerType);

            request.RegisterForDispose(
                new Release(() => this.container.Release(controller)));

            return controller;
        }

        private class Release : IDisposable
        {
            private readonly Action release;

            public Release(Action release) { this.release = release; }

            public void Dispose()
            {
                this.release();
            }
        }
    }
}