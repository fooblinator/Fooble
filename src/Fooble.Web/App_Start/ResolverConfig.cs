using Autofac;
using Autofac.Integration.Mvc;
using Fooble.Core.Infrastructure;
using System.Web.Mvc;

namespace Fooble.Web
{
    static class ResolverConfig
    {
        public static void RegisterResolver()
        {
            var builder = new ContainerBuilder();
            builder.RegisterModule<AutofacModule>();
            builder.RegisterControllers(typeof(MvcApplication).Assembly);
            var container = builder.Build();

            DependencyResolver.SetResolver(new AutofacDependencyResolver(container));
        }
    }
}
