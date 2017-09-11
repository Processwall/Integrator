using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Integrator.Sync
{
    public abstract class Action
    {
        public String Name { get; private set; }

        public Map Map { get; private set; }

        public abstract Parameters CreateParameters();

        public abstract void Execute(Parameters Parameters);

        /*
        protected Connection.IFile SyncFile(Connection.IFile Source, Schema.FileType TargetFileType)
        {
            int bufferlength = 1024;
            byte[] buffer = new byte[bufferlength];
            int length = 0;

            Connection.IFile Target = this.Map.Target.Create(TargetFileType, Source.Filename);

            using (Stream sourcestream = Source.Read())
            {
                using(Stream targetstream = Target.Write())
                {
                    while((length = sourcestream.Read(buffer, 0, bufferlength)) > 0)
                    {
                        targetstream.Write(buffer, 0, length);
                    }
                }
            }

            Target.Save();

            return Target;
        }

        protected Connection.IItem SyncItem(Maps.ItemType ItemTypeMap, Connection.IItem SourceItem)
        {
            Connection.IItem targetitem = null;

            // Search for Target Item
            IEnumerable<Connection.IItem> targetitems = this.Map.Target.Query(ItemTypeMap.Target, Integrator.Conditions.Eq(ItemTypeMap.KeyPropertyType.Target.Name, SourceItem.Property(ItemTypeMap.KeyPropertyType.Source).Object));
          
            if (targetitems.Count() == 0)
            {
                // Target Item does not exist
                targetitem = ItemTypeMap.Map.Target.Create(ItemTypeMap.Target);
                
            }
            else
            {
                // Target Item exists
                targetitem = targetitems.First();
                targetitem.Lock();
            }

            // Update Properties
            foreach(Maps.PropertyType proptypemap in ItemTypeMap.PropertyTypes)
            {
                targetitem.Property(proptypemap.Target).Object = SourceItem.Property(proptypemap.Source).Object;
            }

            targetitem.Save();

            return targetitem;
        }
        */

        public Action(String Name, Map Map)
        {
            this.Name = Name;
            this.Map = Map;
        }
    }
}
