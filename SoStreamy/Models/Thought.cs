using System;

namespace SoStreamy.Models
{
    public class Thought
    {
        public Thought()
        {
            Created = DateTime.UtcNow;
        }

        public string Id { get; set; }
        public string Name { get; set; }
        public string Text { get; set; }
        public DateTime Created { get; set; }
        public string CallerId { get; set; }
    }
}