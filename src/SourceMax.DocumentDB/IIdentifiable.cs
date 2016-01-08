using System;

namespace SourceMax.DocumentDB {

    public interface IIdentifiable {

        string Id { get; }

        string Type { get; }
    }
}