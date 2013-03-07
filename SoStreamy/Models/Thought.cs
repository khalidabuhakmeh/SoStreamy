using System;
using Newtonsoft.Json;

namespace SoStreamy.Models
{
    public class Thought
    {
        public Thought()
        {
            Created = DateTime.UtcNow;
        }

        public string Id { get; set; }
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }
        [JsonProperty(PropertyName = "thought")]
        public string Text { get; set; }
        [JsonProperty(PropertyName = "date")]
        public DateTime Created { get; set; }
        [JsonIgnore]
        public string CallerId { get; set; }
    }



}