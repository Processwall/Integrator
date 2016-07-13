using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Integrator.Connection;
using System.IO;
using IOM = Aras.IOM;
using System.Net;

namespace Integrator.Connection.Aras
{
    public class File : Item, IFile
    {
        public String Filename
        {
            get
            {
                return ((Properties.String)this.Property("filename")).Value;
            }
            internal set
            {
                ((Properties.String)this.Property("filename")).Value = value;
            }
        }

        private FileInfo _cacheFilename;
        internal FileInfo CacheFilename
        {
            get
            {
                if (this._cacheFilename == null)
                {
                    this._cacheFilename = new FileInfo(this.Session.WorkSpace.FullName + "\\File\\" + this.ID + ".dat");

                    if (!this._cacheFilename.Directory.Exists)
                    {
                        this._cacheFilename.Directory.Create();
                    }
                }

                return this._cacheFilename;
            }
        }

        public Stream Read()
        {
            String fileurl = this.Session.Innovator.getFileUrl(this.ID, IOM.UrlType.SecurityToken);
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(fileurl);
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            return response.GetResponseStream();
        }

        public Stream Write()
        {
            return new FileStream(this.CacheFilename.FullName, FileMode.Create);
        }

        public new IFile Save(Boolean Unlock = true)
        {
            this.CacheFilename.Refresh();

            if (this.CacheFilename.Exists)
            {
                if (this.Status == State.Created)
                {
                    IOM.Item iomitem = this.Session.Innovator.newItem(((ItemType)this.ItemType).DBName, "add");
                    iomitem.setID(this.ID);
                    iomitem.setProperty("filename", this.Filename);
                    iomitem.attachPhysicalFile(this.CacheFilename.FullName);
                    iomitem = iomitem.apply();

                    // Delete Cache
                    this.CacheFilename.Delete();

                    if (!iomitem.isError())
                    {
                        if (iomitem.getID().Equals(this.ID))
                        {
                            this.Status = State.Stored;
                            return this;
                        }
                        else
                        {
                            File newitem = (File)this.Session.Create((FileType)this.ItemType, iomitem.getID(), State.Stored);
                            newitem.Filename = iomitem.getProperty("filename");
                            return newitem;
                        }
                    }
                    else
                    {
                        throw new Exceptions.UpdateException(iomitem.getErrorString());
                    }

                }
                else
                {
                    throw new Exceptions.UpdateException("Not possible to update Files");
                }
            }
            else
            {
                throw new Exceptions.UpdateException("File does not exist in Cache");
            }
        }

        internal File(FileType FileType, String ID, State Status)
            : base(FileType, ID, Status)
        {
      
        }
    }
}
