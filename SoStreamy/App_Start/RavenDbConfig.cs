using System;
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
            catch (Exception e)
            {
                var dataDirectory = HostingEnvironment.MapPath(ConfigurationManager.AppSettings["RavenDirectory"]);
                throw new Exception(dataDirectory);

                var process = new Process
                {
                    StartInfo =
                    {
                        FileName = "esentutl",
                        WorkingDirectory = dataDirectory,
                        Arguments = "/d Data",
                    }
                };

                if (process.Start())
                    TryToInitialize();
            }

            IndexCreation.CreateIndexes(typeof(RavenDbConfig).Assembly, DocumentStore);
        }

        private static void TryToInitialize()
        {
            DocumentStore = new EmbeddableDocumentStore
            {
                ConnectionStringName = "RavenDb",
                //UseEmbeddedHttpServer = true,
                Conventions = { IdentityPartsSeparator = "-" }
            }.Initialize();

            
        }

        public static IDocumentStore Start()
        {
            return DocumentStore;
        }
    }
}