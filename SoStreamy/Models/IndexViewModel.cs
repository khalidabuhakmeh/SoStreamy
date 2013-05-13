using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Microsoft.Security.Application;
using Newtonsoft.Json;

namespace SoStreamy.Models
{
    public class IndexViewModel
    {
        public IndexViewModel()
        {
            Thoughts = new List<Thought>();
            PageLoaded = DateTimeOffset.UtcNow;
        }

        public IList<Thought> Thoughts { get; set; }
        public int TotalThoughts { get; set; }
        public DateTimeOffset PageLoaded { get; set; }

        public MvcHtmlString ToJson()
        {
            return MvcHtmlString.Create(JsonConvert.SerializeObject(new
            {
                thoughts = Thoughts.Select(i => new
                {
                    name = i.Name,
                    date = i.Created.ToString(),
                    thought = i.Text
                }),
                totalThoughts = TotalThoughts,
                pageLoaded = PageLoaded
            }, new HtmlEncodeStringPropertiesConverter()));
        }
    }

    public class HtmlEncodeStringPropertiesConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(string);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            writer.WriteValue(Encoder.HtmlEncode(value.ToString()));
        }
    }
}