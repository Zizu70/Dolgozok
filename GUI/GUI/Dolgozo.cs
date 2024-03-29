﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GUI
{
    using System;
    using System.Collections.Generic;

    using System.Globalization;
    //using System.Web.Script.Serialization;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    public partial class Dolgozo
    {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("salary")]
        public long Salary { get; set; }

        [JsonProperty("position")]
        public string Position { get; set; }


        override public string ToString()
        {
            return Name;
        }

    }

    public partial class Dolgozo
    {
        public static Dolgozo[] FromJson(string json) => JsonConvert.DeserializeObject<Dolgozo[]>(json, GUI.Converter.Settings);
    }

    public static class Serialize
    {
        public static string ToJson(this Dolgozo[] self) => JsonConvert.SerializeObject(self, GUI.Converter.Settings);
    }

    internal static class Converter
    {
        public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
            DateParseHandling = DateParseHandling.None,
            Converters =
        {
            new IsoDateTimeConverter { DateTimeStyles = DateTimeStyles.AssumeUniversal }
        },
        };
    }
}
