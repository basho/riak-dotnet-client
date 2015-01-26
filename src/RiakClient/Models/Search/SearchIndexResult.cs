using System;
using System.Collections.ObjectModel;
using System.Linq;
using RiakClient.Extensions;
using RiakClient.Messages;
using RiakClient.Util;

namespace RiakClient.Models.Search
{
    public class SearchIndexResult
    {
        public ReadOnlyCollection<SearchIndex> Indices { get; private set; }

        internal SearchIndexResult(RpbYokozunaIndexGetResp getResponse)
        {
            var searchIndices = getResponse.index.Select(i => new SearchIndex(i));
            Indices = new ReadOnlyCollection<SearchIndex>(searchIndices.ToList());
        }
    }

    public class SearchIndex
    {
        public String Name { get; private set; }
        public String SchemaName { get; private set; }
        public uint NVal { get; private set; }

        public SearchIndex(string name) : this(name, RiakConstants.Defaults.YokozunaIndex.IndexName, RiakConstants.Defaults.YokozunaIndex.NVal) { }

        public SearchIndex(string name, string schemaName, uint nVal)
        {
            if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Index Name cannot be null, zero length, or whitespace");
            if (string.IsNullOrWhiteSpace(schemaName)) throw new ArgumentException("Schema Name cannot be null, zero length, or whitespace");
            if (nVal == 0) throw new ArgumentException("NVal must be greater than 0");

            Name = name;
            NVal = nVal;
            SchemaName = schemaName;
        }

        internal SearchIndex(RpbYokozunaIndex index)
        {
            Name = index.name.FromRiakString();
            SchemaName = index.schema.FromRiakString();
            NVal = index.n_val;
        }

        internal RpbYokozunaIndex ToMessage()
        {
            return new RpbYokozunaIndex
            {
                name = Name.ToRiakString(),
                schema = SchemaName.ToRiakString(),
                n_val = NVal
            };
        }
    }

    public class SearchSchema
    {
        public String Name { get; private set; }
        public string Content { get; private set; }

        public SearchSchema(string name, string content)
        {
            Name = name;
            Content = content;
        }

        internal SearchSchema(RpbYokozunaSchema schema)
        {
            Name = schema.name.FromRiakString();
            Content = schema.content.FromRiakString();
        }

        internal RpbYokozunaSchema ToMessage()
        {
            return new RpbYokozunaSchema
            {
                name = Name.ToRiakString(),
                content = Content.ToRiakString()
            };
        }
    }
}
