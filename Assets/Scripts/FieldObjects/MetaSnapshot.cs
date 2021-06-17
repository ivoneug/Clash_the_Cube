using System;
using System.Collections.Generic;
using Databox;

namespace ClashTheCube
{
    public class MetaSnapshot
    {
        public string Table { get; private set; }
        public int Identifier { get; private set; }
        public Dictionary<string, DataboxType> Fields { get; private set; }

        public MetaSnapshot(string table, int identifier, Dictionary<string, DataboxType> fields)
        {
            Table = table;
            Identifier = identifier;
            Fields = fields;
        }
    }
}