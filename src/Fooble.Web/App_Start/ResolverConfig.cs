using Autofac;
using Autofac.Integration.Mvc;
using Fooble.Core.Infrastructure;
using System.Configuration;
using System.Web.Mvc;

namespace Fooble.Web
{
    static class ResolverConfig
    {
        public static void RegisterResolver()
        {
            var connectionString = ConfigurationManager.ConnectionStrings["FoobleContext"].ConnectionString;

            var builder = new ContainerBuilder();
            builder.RegisterModule(new AutofacModule { ConnectionString = connectionString });
            builder.RegisterControllers(typeof(MvcApplication).Assembly);
            var container = builder.Build();

            DependencyResolver.SetResolver(new AutofacDependencyResolver(container));
        }
    }
}
