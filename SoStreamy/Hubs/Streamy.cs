using System;
using System.Linq;
using Microsoft.AspNet.SignalR;
using Raven.Abstractions.Data;
using Raven.Client;
using SoStreamy.Models;

namespace SoStreamy.Hubs
{
    public class Streamy : Hub
    {
        static Streamy()
        {
            Application.DocumentStore
                       .Changes()
                       .ForDocumentsStartingWith("thoughts")
                       .Subscribe(new UpdateClientsWithNewThought());
        }

        public void Submit(dynamic thought)
        {
            string text = thought.thought;
            string name = thought.name;

            if (string.IsNullOrWhiteSpace(text) || string.IsNullOrWhiteSpace(name))
            {
                Clients.Caller.addError("A name and a throught are required");
                return;
            }

            using (var session = Application.DocumentStore.OpenSession())
            {
                var newThought = new Thought
                {
                    Name = name,
                    Text = text,
                    CallerId = Context.ConnectionId
                };
                session.Store(newThought);
                session.SaveChanges();
            }
        }
    }

    public class UpdateClientsWithNewThought : IObserver<DocumentChangeNotification>
    {
        public void OnNext(DocumentChangeNotification value)
        {
            var hub = GlobalHost.ConnectionManager.GetHubContext<Streamy>();

            if (value.Type == DocumentChangeTypes.Put)
            {
                using (var session = Application.DocumentStore.OpenSession())
                {
                    var thought = session.Load<Thought>(value.Id);
                    hub.Clients.All.addThought(thought);
                    hub.Clients.Client(thought.CallerId).addMessage("successfully added your thought");
                    hub.Clients.All.updateTotal(GetTotalCount(session));
                }
            }
        }

        private static int GetTotalCount(IDocumentSession session)
        {
            RavenQueryStatistics stats;
            session.Query<Thoughts_All.Result, Thoughts_All>()
                   .Statistics(out stats)
                   .OrderByDescending(x => x.Created)
                   .OfType<Thought>()
                   .Take(0)
                   .ToList();

            return stats.TotalResults + 1;
        }

        public void OnError(Exception error)
        { }

        public void OnCompleted()
        { }
    }
}