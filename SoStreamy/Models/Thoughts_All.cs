using System.Linq;
using Raven.Client.Indexes;

namespace SoStreamy.Models
{
    public class Thoughts_All : AbstractIndexCreationTask<Thought>
    {
        public Thoughts_All()
        {
            Map = thoughts => from thought in thoughts
                              select new
                              {
                                  thought.Id,
                                  thought.Text,
                                  thought.Name,
                                  thought.Created
                              };
        }
    }
}