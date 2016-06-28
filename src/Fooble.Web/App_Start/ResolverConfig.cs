using Autofac;
using Autofac.Integration.Mvc;
using Fooble.Core.Infrastructure;
using Fooble.Persistence.Infrastructure;
using Fooble.Presentation.Infrastructure;
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
            builder.RegisterModule(new CoreRegistrations());
            builder.RegisterModule(new PersistenceRegistrations(connectionString));
            builder.RegisterModule(new PresentationRegistrations());
            builder.RegisterControllers(typeof(MvcApplication).Assembly);
            builder.RegisterModelBinderProvider();
            var container = builder.Build();

            DependencyResolver.SetResolver(new AutofacDependencyResolver(container));
        }
    }
}
