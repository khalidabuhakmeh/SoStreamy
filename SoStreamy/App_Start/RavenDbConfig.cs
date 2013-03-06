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
            DocumentStore = new EmbeddableDocumentStore
            {
                ConnectionStringName = "RavenDb",
                //UseEmbeddedHttpServer = true,
                Conventions = { IdentityPartsSeparator = "-" }
            }.Initialize();

            IndexCreation.CreateIndexes(typeof(RavenDbConfig).Assembly, DocumentStore);
        }

        public static IDocumentStore Start()
        {
            return DocumentStore;
        }
    }
}