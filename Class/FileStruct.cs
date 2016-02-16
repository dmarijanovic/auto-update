using System;
using System.Collections.Generic;
using System.Text;
using DamirM.CommonLibrary;
using System.IO;

namespace DamirM.AutoUpdate
{
    public class FileStruct: IComparable
    {
        public string name;
        public int id;
        private string savePath;
        private string externalDownloadURL;
        public int versionServer;
        public int versionLocal;
        public string md5Server;
        public string md5Local;
        private string downloadURL;
        private string baseFolder;  // Application folder without subfolder from web
        private bool autoStart;
        public string autoStartArgs;
        private bool preuzeto;
        private bool saveSuccessfully;   // If file is saved on orginal location after download, else is saved in _tmp folder

        public FileStruct(string downloadURL, string baseSavePath)
        {
            this.downloadURL = downloadURL;
            this.baseFolder = baseSavePath;
        }
        public void AddFileInfo(string name, int id, string savePath, string externalDownloadURL, int versionServer, string md5Server, bool autoStart)
        {
            this.name = name;
            this.id = id;
            this.savePath = savePath;
            this.externalDownloadURL = externalDownloadURL;
            this.versionServer = versionServer;
            this.md5Server = md5Server;
            this.md5Local = "";
            this.versionLocal = 0;
            this.autoStart = autoStart;
        }
        public override string ToString()
        {
            string buff;
            buff = "Name: " + this.name + Environment.NewLine;
            buff += "Id: " + this.id + Environment.NewLine;
            buff += "downloadURL: " + this.DownloadURL + Environment.NewLine;
            buff += "SavePath: " + this.savePath + Environment.NewLine;
            buff += "versionServer: " + this.versionServer + Environment.NewLine;
            buff += "versionLocal: " + this.versionLocal + Environment.NewLine;
            buff += "md5Server: " + this.md5Server + Environment.NewLine;
            buff += "md5Local: " + this.md5Local + Environment.NewLine;
            buff += "autoStart: " + this.autoStart + Environment.NewLine;
            return buff;
        }
        public int CompareTo(object obj)
        {
            //FileStruct fileStruct = (FileStruct)obj;
            string name = obj.ToString();
            return string.Compare(this.name, name, true);
        }


        public string DownloadURL
        {
            get
            {
                // Extrenal download url
                if (externalDownloadURL.IndexOf("://") > 0)
                {
                    return externalDownloadURL;
                }
                else
                {
                    return string.Format(downloadURL, this.id);
                }
            }
        }
        public string SavePath
        {
            get
            {
                string filePath;
                if (savePath == "")
                {
                    filePath = baseFolder + name;
                }
                else
                {
                    filePath = Common.BuildPath(baseFolder, savePath);
                    if (!Directory.Exists(filePath))
                    {
                        Common.MakeAllSubFolders(filePath);
                    }
                    filePath += name;
                }
                return filePath;
            }
        }
        public string BaseFolder
        {
            get
            {
                return this.baseFolder;
            }
        }
        public bool Preuzeto
        {
            set { preuzeto = value; }
            get { return preuzeto; }
        }
        public bool AutoStart
        {
            get { return this.autoStart; }
        }

        public bool FileHaveNewVersion()
        {
            // New version of file avaible
            if (this.md5Server == "")
            {
                // If server dont have MD5 code then compare versions
                if (this.versionLocal != this.versionServer)
                {
                    return true;
                }
            }
            else
            {
                // Compare MD5 code
                if (this.md5Server != this.md5Local)
                {
                    return true;
                }
            }
            return false;
        }
        public bool SaveSuccessfully
        {
            get
            {
                if (saveSuccessfully)
                {
                    return true;
                }
                else
                {
                    if (FileHaveNewVersion() && saveSuccessfully == false)
                    {
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                }

            }
            set
            {
                this.saveSuccessfully = value;
            }
        }
    }
}
