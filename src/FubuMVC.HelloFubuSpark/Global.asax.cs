using System.Web;
using System.Web.Routing;
using Bottles;
using FubuMVC.Core;
using FubuMVC.HelloFubuSpark.Services;
using FubuMVC.Spark;
using StructureMap;
using FubuMVC.StructureMap;

namespace FubuMVC.HelloFubuSpark
{
    public class Global : HttpApplication
    {
        protected void Application_Start()
        {
            FubuApplication
                .For<HelloWorldFubuRegistry>()
                .StructureMap(() => new Container(SetupContainer))
                .ModifyRegistry(x => x.UseSpark(spark => spark.Include("QueryTemplate.htm")))
                .Bootstrap(RouteTable.Routes);

            // If there is an error during bootstrapping, it will not automatically be considered
            // fatal and there will be no YSOD.  This is to help during initial debugging and 
            // troubleshooting package loading. Normally, however, you want a YSOD if there is
            // a bootstrapping failure or a package-loading failure. This next line ensures that.
            PackageRegistry.AssertNoFailures(); 
        }

        private static void SetupContainer(ConfigurationExpression x)
        {
            x.For<IHttpSession>().Use<CurrentHttpContextSession>();
            x.Scan(i =>
            {
                i.TheCallingAssembly();
                i.Convention<SettingsScanner>();
            });
            x.SetAllProperties(s => s.Matching(p => p.Name.EndsWith("Settings")));
        }
    }
}