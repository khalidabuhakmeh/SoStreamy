using System;
using System.Linq;
using Raven.Abstractions.Indexing;
using Raven.Client.Indexes;

namespace SoStreamy.Models
{
    public class Thoughts_All : AbstractIndexCreationTask<Thought,Thoughts_All.Result>
    {
        public class Result
        {
            public string Id { get; set; }
            public string Text { get; set; }
            public string Name { get; set; }
            public DateTime Created { get; set; }
        }

        public Thoughts_All()
        {
            Map = thoughts => from thought in thoughts
                              select new Result
                              {
                                  Id = thought.Id,
                                  Text = thought.Text,
                                  Name = thought.Name,
                                  Created = thought.Created
                              };

            Store(x => x.Created, FieldStorage.Yes);
        }
    }
}