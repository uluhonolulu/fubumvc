using System;
using System.Collections.Generic;
using System.Reflection;
using Bottles;
using FubuCore;
using FubuCore.Descriptions;
using FubuCore.Util.TextWriting;
using FubuMVC.Core.Packaging;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Registration.Routes;
using StringWriter = System.IO.StringWriter;

namespace FubuMVC.Core
{
    
    public class AboutEndpoint
    {
        public string get__about()
        {
            return FubuApplicationDescriber.WriteDescription();
        }
    }

    [Title("Register the _about endpoint")]
    public class RegisterAbout : IConfigurationAction
    {
        public void Configure(BehaviorGraph graph)
        {
            var action = ActionCall.For<AboutEndpoint>(x => x.get__about());
            var chain = new BehaviorChain();
            chain.AddToEnd(action);
            chain.Route = new RouteDefinition("_about");

            graph.AddChain(chain);
        }
    }

    public static class FubuApplicationDescriber
    {
        public static string WriteDescription()
        {
            var writer = new System.IO.StringWriter();

            writeProperties(writer);

            writeErrors(writer);

            writeAssemblies(writer);

            writeBottles(writer);

            return writer.ToString();
        }

        private static void writeBottles(StringWriter writer)
        {
            writer.WriteLine("------------------------------------------------------------------------------------------------");
            writer.WriteLine("Logs");
            writer.WriteLine("------------------------------------------------------------------------------------------------");

            PackageRegistry.Diagnostics.EachLog((o, log) =>
            {
                if (log.Success)
                {
                    writer.WriteLine(o.ToString());
                    writer.WriteLine(log.FullTraceText());
                    writer.WriteLine("------------------------------------------------------------------------------------------------");
                }
            });

            writer.WriteLine();
        }

        private static void writeErrors(StringWriter writer)
        {
            writer.WriteLine("------------------------------------------------------------------------------------------------");
            writer.WriteLine("Errors");
            writer.WriteLine("------------------------------------------------------------------------------------------------");

            PackageRegistry.Diagnostics.EachLog((o, log) =>
            {
                if (!log.Success)
                {
                    writer.WriteLine(o.ToString());
                    writer.WriteLine(log.FullTraceText());
                    writer.WriteLine("------------------------------------------------------------------------------------------------");
                }
            });

            writer.WriteLine();

        }

        

        private static void writeAssemblies(StringWriter writer)
        {
            var report = new TextReport();
            report.StartColumns(3);
            report.AddDivider('-');
            report.AddText("Assemblies");
            report.AddDivider('-');

            AppDomain.CurrentDomain.GetAssemblies().Each(assem => {
                var assemblyName = assem.GetName();
                var file = findCodebase(assem);
                report.AddColumnData(assemblyName.Name, assemblyName.Version.ToString(), file);
            });

            report.AddDivider('-');
            report.Write(writer);

            writer.WriteLine();
        }

        private static string findCodebase(Assembly assem)
        {
            try
            {
                var file = assem.CodeBase;
                return file ?? "None";
            }
            catch (Exception)
            {
                return "None";
            }
        }

        private static void writeProperties(StringWriter writer)
        {
            var report = new TextReport();
            report.StartColumns(2);

            if (FubuMvcPackageFacility.Restarted.HasValue)
                report.AddColumnData("Restarted", FubuMvcPackageFacility.Restarted.ToString());
            report.AddColumnData("Application Path", FubuMvcPackageFacility.GetApplicationPath());

            report.Write(writer);

            writer.WriteLine();
        }


    }
}