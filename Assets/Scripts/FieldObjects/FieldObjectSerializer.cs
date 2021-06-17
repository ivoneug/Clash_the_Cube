using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Databox;

namespace ClashTheCube
{
    public class FieldObjectSerializer : MonoBehaviour, IMetaSerializable
    {
        [SerializeField] private DataboxObject databox;
        
        public void MetaSave(MetaSnapshot snapshot)
        {
            var table = snapshot.Table;
            var identifier = snapshot.Identifier.ToString();
            
            if (!databox.EntryExists(table, identifier))
            {
                foreach (var pair in snapshot.Fields)
                {
                    databox.AddData(table, identifier, pair.Key, pair.Value);                    
                }
            }
            else
            {
                foreach (var pair in snapshot.Fields)
                {
                    SetData(table, identifier, pair.Key, pair.Value);                    
                }
            }
        }

        public MetaSnapshot MetaLoad(string table, int identifier)
        {
            var key = identifier.ToString();
            
            if (!databox.EntryExists(table, key))
            {
                return null;
            }

            var fields = new Dictionary<string, DataboxType>();
            var values = databox.GetValuesFromEntry(table, key);
            foreach (var pair in values)
            {
                fields[pair.Key] = pair.Value.First().Value;
            }
            
            return new MetaSnapshot(
                table,
                int.Parse(key),
                fields
            );
        }

        public void MetaRemove(string table, int identifier)
        {
            databox.RemoveEntry(table, identifier.ToString());
        }
        
        private bool SetData(string tableID, string entryID, string valueID, DataboxType value)
        {
            DataboxObject.Database tempDB;
            DataboxObject.DatabaseEntry tempEntry;
            DataboxType data;
            Dictionary<System.Type, DataboxType> tempData;
            
            if (!databox.DB.TryGetValue(tableID, out tempDB)) return false;
            if (!databox.DB[tableID].entries.TryGetValue(entryID, out tempEntry)) return false;
            if (!databox.DB[tableID].entries[entryID].data.TryGetValue(valueID, out tempData)) return false;

            var type = value.GetType();

            if (!tempData.TryGetValue(type, out data)) return false;
            tempData[type] = value;
            
            return true;
        }
    }
}