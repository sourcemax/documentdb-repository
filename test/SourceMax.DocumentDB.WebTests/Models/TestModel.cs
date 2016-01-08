﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Newtonsoft.Json;

namespace SourceMax.DocumentDB.WebTests.Models {

    public class TestModel : IIdentifiable {

        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        public string Type { get; set; }

        public string Description { get; set; }
    }
}