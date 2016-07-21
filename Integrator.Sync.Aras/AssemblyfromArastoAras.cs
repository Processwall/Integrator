using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Integrator.Sync.Aras
{
    public class AssemblyfromArastoAras : Action
    {
        private readonly String[] parameternames = new String[1] {"Number"};

        public override Integrator.Parameters CreateParameters()
        {
            return new Integrator.Parameters(parameternames);
        }

        private Connection.IItem SyncPart(Connection.IItem Source)
        {
            Connection.IItem Target = null;
            Boolean updatetarget = false;

            // Search for Target Item
            IEnumerable<Connection.IItem> targetitems = this.Map.Target.Query("Part", Integrator.Conditions.Eq("item_number", Source.Property("item_id").Object.ToString()));

            if (targetitems.Count() == 0)
            {
                // Target Item does not exist
                Target = this.Map.Target.Create("Part");
                updatetarget = true;
            }
            else
            {
                // Target Item exists
                Target = targetitems.First();

                if (((Connection.Properties.IDate)Source.Property("modified_on")).Value > ((Connection.Properties.IDate)Target.Property("modified_on")).Value)
                {
                    // Source modified after Target
                    Target.Lock();
                    updatetarget = true;
                }
            }

            if (updatetarget)
            {
                Target.Property("item_number").Object = Source.Property("item_id").Object;
                Target.Property("major_rev").Object = Source.Property("item_revision_id").Object;
                Target.Property("name").Object = Source.Property("object_name").Object;
                Target.Save();
                Target.Lock();

                // Process Datasets
                foreach (Connection.IRelationship sourceimanspec in Source.Relationships("tc_iman_specification"))
                {
                    Connection.IItem sourcedataset = sourceimanspec.Related;

                    foreach (Connection.IRelationship sourcenamedref in sourcedataset.Relationships("tc_named_reference"))
                    {
                        Connection.IFile sourcefile = (Connection.IFile)sourcenamedref.Related;

                        if (sourcefile.Filename != null)
                        {
                            // Sync File
                            Connection.IFile targetfile = this.SyncFile(sourcefile, this.Map.Target.FileType("File"));

                            String sourceitemnumber = Path.GetFileNameWithoutExtension(sourcefile.Filename);
                            Connection.IItemType targetcaditemtype = null;

                            switch (Path.GetExtension(sourcefile.Filename).ToLower())
                            {
                                case ".catdrawing":
                                    targetcaditemtype = this.Map.Target.ItemType("CAD.Mechanical.Drawing");
                                    break;
                                case ".catproduct":
                                    targetcaditemtype = this.Map.Target.ItemType("CAD.Mechanical.Assembly");
                                    break;
                                default:
                                    targetcaditemtype = this.Map.Target.ItemType("CAD.Mechanical.Part");
                                    break;
                            }

                            // Search for Target CAD
                            Connection.IItem targetcad = null;
                            IEnumerable<Connection.IItem> targetcads = this.Map.Target.Query(targetcaditemtype, Integrator.Conditions.Eq("item_number", sourceitemnumber));

                            if (targetcads.Count() > 0)
                            {
                                // Target CAD Exists
                                targetcad = targetcads.First();
                                targetcad.Lock();
                            }
                            else
                            {
                                // Target CAD does not exist
                                targetcad = this.Map.Target.Create(targetcaditemtype);
                            }

                            targetcad.Property("native_file").Object = targetfile;
                            targetcad.Property("item_number").Object = sourceitemnumber;
                            targetcad.Property("major_rev").Object = Target.Property("major_rev").Object;
                            targetcad.Property("name").Object = Target.Property("name").Object;
                            targetcad.Save();

                            // Check for Target Part CAD
                            Connection.IRelationship targetpartcad = null;

                            foreach(Connection.IRelationship thistargetpartcad in Target.Relationships("Part CAD"))
                            {
                                if (thistargetpartcad.Related.Equals(targetcad))
                                {
                                    targetpartcad = thistargetpartcad;
                                    break;
                                }
                            }

                            if (targetpartcad == null)
                            {
                                targetpartcad = Target.Create("Part CAD", targetcad);
                                targetpartcad.Save();
                            }

                        }
                    }
                }

                // Process BOM Lines
                Dictionary<Connection.IItem, Connection.IRelationship> relatedsource = new Dictionary<Connection.IItem, Connection.IRelationship>();

                foreach (Connection.IRelationship sourcebomline in Source.Relationships("tc_bomline"))
                {
                    Connection.IRelationship targetbomline = null;

                    if (!relatedsource.ContainsKey(sourcebomline.Related))
                    {
                        foreach (Connection.IRelationship thistargetbomline in Target.Relationships("Part BOM"))
                        {
                            if (thistargetbomline.Related.Property("item_number").Object.Equals(sourcebomline.Related.Property("item_id").Object))
                            {
                                targetbomline = thistargetbomline;
                                break;
                            }
                        }

                        if (targetbomline == null)
                        {
                            Connection.IItem TargetRelated = this.SyncPart(sourcebomline.Related);
                            targetbomline = Target.Create("Part BOM", TargetRelated);
                            targetbomline.Save();
                            relatedsource[sourcebomline.Related] = targetbomline;
                        }
                    }
                    else
                    {
                        targetbomline = relatedsource[sourcebomline.Related];
                    }
                }

                Target.UnLock();

            }

            return Target;
        }

        public override void Execute(Integrator.Parameters Parameters)
        {
            IEnumerable<Connection.IItem> sourceitems = this.Map.Source.Query("tc_item", Integrator.Conditions.Eq("item_id", Parameters.Parameter("Number").Value));
            
            if (sourceitems.Count() > 0)
            {
                Connection.IItem sourcepart = sourceitems.First();
                Connection.IItem targetpart = this.SyncPart(sourcepart); 
            }
        }

        public AssemblyfromArastoAras(String Name, Integrator.Sync.Map Map)
            :base(Name, Map)
        {

        }
    }
}
