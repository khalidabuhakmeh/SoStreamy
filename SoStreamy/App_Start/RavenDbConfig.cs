using System.Configuration;
using System.Diagnostics;
using System.Web.Hosting;
using Microsoft.Isam.Esent.Interop;
using Raven.Client;
using Raven.Client.Embedded;
using Raven.Client.Indexes;

namespace SoStreamy.App_Start
{
    public static class RavenDbConfig
    {
        public static IDocumentStore DocumentStore { get; set; }

        static RavenDbConfig()
        {
            try
            {
                TryToInitialize();
            }
            catch (EsentSecondaryIndexCorruptedException e)
            {
                var process = new Process
                {
                    StartInfo =
                    {
                        FileName = "esentutl",
                        WorkingDirectory = HostingEnvironment.MapPath(ConfigurationManager.AppSettings["RavenDirectory"]),
                        Arguments = "/d Data",
                    }
                };
                process.Start();
                TryToInitialize();
            }
        }

        private static void TryToInitialize()
        {
            DocumentStore = new EmbeddableDocumentStore
            {
                ConnectionStringName = "RavenDb",
                //UseEmbeddedHttpServer = true,
                Conventions = {IdentityPartsSeparator = "-"}
            }.Initialize();

            IndexCreation.CreateIndexes(typeof (RavenDbConfig).Assembly, DocumentStore);
        }

        public static IDocumentStore Start()
        {
            return DocumentStore;
        }
    }
}