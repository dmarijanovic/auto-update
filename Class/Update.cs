using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using System.IO;
using System.Xml;

using DamirM.CommonLibrary;
using System.Threading;

using System.Diagnostics;
//using System.Windows.Forms;

namespace DamirM.AutoUpdate
{
    class Update
    {

        Main main;
        Product product;
        WebCalls webCalls;

        private string serverAddress;
        private string applicationPath;

        public Update(string serverAddress, Product product)
        {
            this.serverAddress = serverAddress;
            this.product = product;
            applicationPath = Common.SetSlashOnEndOfDirectory(System.Windows.Forms.Application.StartupPath);
            webCalls = WebCalls.Create();
        }
        public Update(string serverAddress, string productName, int productID)
        {
            this.serverAddress = serverAddress;
            applicationPath = Common.SetSlashOnEndOfDirectory(System.Windows.Forms.Application.StartupPath);
            product = new Product(productName, "", productID);
            webCalls = WebCalls.Create();
        }
        public Update(Main main, string serverAddress, Product product)
            : this(serverAddress, product)
        {
            this.main = main;
            webCalls.DownloadProgress += new WebCalls.delWebCallsDownloadProgress(main.webCalls_DownloadProgress);
        }

        public Result.ResultType SelfUpdateProcedure()
        {
            Product autoUpdateProduct;
            Stream stream;
            string requestURL;
            bool result;
            int fileDownloadCount;

            try
            {
                // Authorization
                requestURL = serverAddress + AutoUpdate.Main.constURLParams_authorization;
                result = webCalls.WebAuthorization(requestURL, AutoUpdate.Main.constAuthorization_username, AutoUpdate.Main.constAuthorization_password);


                // Get product info, create product object
                requestURL = serverAddress + AutoUpdate.Main.constURLParams_get_product_from_product_name;
                requestURL = string.Format(requestURL, Main.constAutoUpdateProductName);
                stream = webCalls.WebGETMethod(requestURL);
                autoUpdateProduct = Product.Create(stream);

                // Get File list for autoupdate
                requestURL = serverAddress + string.Format(Main.constURLParams_get_update_xml, autoUpdateProduct.ProductID);
                stream = webCalls.WebGETMethod(requestURL);
                result = LoadUpdateXML_server(stream, autoUpdateProduct);

                // Find autoupdate.exe filesruct and modified it
                FileStruct fileStruct = autoUpdateProduct.GetFile(Main.constAutoUpdateExeName);
                fileStruct.autoStartArgs = string.Format("server={0} pid={1}", serverAddress, product.ProductID);

                Log.Write(autoUpdateProduct.Files, this, "SelfUpdateProcedure", Log.LogType.DEBUG);

                if (result == true)
                {
                    if (DownloadAllFile(autoUpdateProduct.Files, out fileDownloadCount))
                    {
                        if (fileDownloadCount > 0)
                        {
                            // New AutoUpdate version available
                            Log.Write("New AutoUpdate version available", this, "SelfUpdateProcedure", Log.LogType.DEBUG);

                            KillAllProccess(autoUpdateProduct);

                            //ExecuteFileStart(fileStruct.SavePath, fileStruct.autoStartArgs, 0);

                            BatchScripts bs = new BatchScripts(autoUpdateProduct);
                            bs.GenerateScript();

                            //AutoStartDownloadFiles(autoUpdateProduct.Files);

                            ExecuteFileStart("replace.bat", "", 3);

                            // Exit application now
                            Log.Write("Exiting application, restarting", this, "SelfUpdateProcedure", Log.LogType.DEBUG);
                            System.Windows.Forms.Application.Exit();
                            return Result.ResultType.Pass;
                        }
                        else
                        {
                            return Result.ResultType.True;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Write(ex, this, "SelfUpdateProcedure", Log.LogType.ERROR);
            }

            return Result.ResultType.False;
        }
        public void MainUpdateProcedure()
        {
            Stream stream;
            bool result = false;
            string updateXMLURL;
            int fileDownloadCount;


            // Get File list for selected product
            updateXMLURL = serverAddress + string.Format(Main.constURLParams_get_update_xml, product.ProductID);
            stream = webCalls.WebGETMethod(updateXMLURL);
            result = LoadUpdateXML_server(stream, product);

            //GetUpdateLog(autoUpdateProduct, txtLog, "autoupdatelog.txt");
            //GetUpdateLog(product, txtLog, "log.txt");
            Log.Write("Skiping update change log...", this, "MainUpdateProcedure", Log.LogType.INFO);


            // List all files for download
            Log.Write(product.Files, this, "MainUpdateProcedure", Log.LogType.DEBUG);

            // Start main update method
            if (result == true)
            {
                // Load local update xml file,
                Product dummyProduct = new Product();
                LoadUpdateXML_local(product.Files, out dummyProduct);

                // Download all file 
                if (DownloadAllFile(product.Files, out fileDownloadCount))
                {
                    // Upgrade successfull
                    WriteXML(applicationPath + Main.constUpdateXMLLocal);
                    ShowInfo("", Main.LogType.Normal);
                    if (fileDownloadCount == 0)
                    {
                        Log.Write("Done", this, "MainUpdateProcedure", Log.LogType.DEBUG);
                        ShowInfo("Done", Main.LogType.Normal);
                    }
                    else
                    {
                        if (KillAllProccess(product))
                        {
                            BatchScripts bs = new BatchScripts(product);
                            bs.GenerateScript();
                            ExecuteFileStart("replace.bat", "", 3);
                            // Exit application now
                            Log.Write("Exiting application, restarting", this, "SelfUpdateProcedure", Log.LogType.DEBUG);
                            System.Windows.Forms.Application.Exit();
                        }
                        else
                        {
                            Log.Write("Update successful", this, "MainUpdateProcedure", Log.LogType.DEBUG);
                            ShowInfo("Update successful", Main.LogType.Normal);
                        }
                    }
                    main.btnAzuriraj.Enabled = true;
                }
                else
                {
                    // Error upgrading
                    ShowInfo("", Main.LogType.Normal);
                    ShowInfo("An error occurred while trying to update", Main.LogType.Normal);
                    Log.Write("An error occurred while trying to update", this, "MainUpdateProcedure", Log.LogType.WARNING);
                }

            }
            else
            {
                ShowInfo("", Main.LogType.Normal);
                ShowInfo("Error connecting to update server", Main.LogType.Normal);
                Log.Write("Error connecting to update server", this, "MainUpdateProcedure", Log.LogType.WARNING);
            }
        }


        private bool LoadUpdateXML_server(Stream stream, Product product)
        {
            bool result = false;
            FileStruct fileStruct;

            try
            {
                XmlTextReader reader = new XmlTextReader(stream);
                while (reader.Read())
                {
                    if (reader.NodeType == XmlNodeType.Element && reader.Name == "File")
                    {
                        fileStruct = new FileStruct(serverAddress + Main.constURLParams_download_file, applicationPath);
                        bool autoStart = reader.GetAttribute("autoStart") == "1";
                        int fileID = int.Parse(reader.GetAttribute("id"));
                        int versionServer = int.Parse(reader.GetAttribute("version"));
                        fileStruct.AddFileInfo(reader.GetAttribute("name"), fileID, reader.GetAttribute("savePath"), reader.GetAttribute("url"), versionServer, reader.GetAttribute("md5"), autoStart);
                        fileStruct.md5Local = MD5Hash(fileStruct.SavePath);
                        product.AddFile(fileStruct);
                    }
                }
                reader.Close();
                result = true;
            }
            catch (Exception ex)
            {
                ShowInfo("ERROR: " + ex.Message, Main.LogType.Normal);
                Log.Write(ex, this, "LoadUpdateXML_server", Log.LogType.ERROR);
            }
            return result;
        }

        public static void LoadUpdateXML_local(ArrayList fileList, out Product product)
        {
            //string autoUpdateVersion_local = "";
            string applicationPath = System.Windows.Forms.Application.StartupPath;
            string xmlPath = "";
            string fileName = "";
            int localVersion;
            FileStruct fileStruct = null;
            product = new Product();

            if (!applicationPath.EndsWith("\\"))
                applicationPath += "\\";

            xmlPath = applicationPath + Main.constUpdateXMLLocal;


            // Ako upd.xml ne postoji, predpostavka je da je update potreban
            if (!File.Exists(xmlPath))
            {
                return;
            }
            // parse xml


            XmlTextReader reader = new XmlTextReader(xmlPath);
            try
            {
                while (reader.Read())
                {
                    if (reader.NodeType == XmlNodeType.Element && reader.Name == "Update")
                    {
                        product.versionLocal = reader.GetAttribute("verzija");
                        product.ProductID = int.Parse(reader.GetAttribute("productid"));
                    }
                    else if (reader.NodeType == XmlNodeType.Element && reader.Name == "Datoteka")
                    {
                        // Ako je fileList null, tada poziv metodi dolati od vanjske aplikacije, aplikacija kojoj treba samo verzija zadnjeg update-a
                        if (fileList != null)
                        {
                            // get file name
                            fileName = reader.GetAttribute("ime").ToLower();
                            // search for collection of file from server xml list
                            for (int i = 0; i < fileList.Count; i++)
                            {
                                // cast to file list type, make reference to list ithem
                                fileStruct = (FileStruct)fileList[i];
                                //
                                if (fileStruct.name.ToLower() == fileName)
                                {
                                    // update local version of file
                                    int.TryParse(reader.GetAttribute("ver"), out localVersion);
                                    fileStruct.versionLocal = localVersion;
                                    //fileStruct.md5Local = reader.GetAttribute("md5");

                                    break;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Write(ex, typeof(Main), "LoadUpdateXML_local", Log.LogType.ERROR);
            }
            finally
            {
                reader.Close();
            }
        }

        private bool DownloadAllFile(ArrayList files, out int fileDownloadCount)
        {
            int errorCounter = 0;
            fileDownloadCount = 0;

            foreach (FileStruct fileStruct in files)
            {
                if (fileStruct.FileHaveNewVersion())
                {
                    // Download new file from server
                    fileStruct.Preuzeto = webCalls.DownloadFile(fileStruct.DownloadURL, fileStruct);
                    if (fileStruct.Preuzeto == false)
                    {
                        errorCounter++;
                    }
                    else
                    {
                        fileDownloadCount++;
                    }
                }
            }

            if (errorCounter > 0)
            {
                // Error exit
                return false;
            }
            else
            {
                // OK exit
                return true;
            }
        }

        public void AutoStartDownloadFiles(ArrayList fileUpdateList)
        {
            try
            {
                foreach (FileStruct fileStruct in fileUpdateList)
                {
                    if (fileStruct.AutoStart)
                    {
                        ExecuteFileStart(fileStruct.SavePath, fileStruct.autoStartArgs, 1000);
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Write(ex, this, "AutoStartDownloadFiles", Log.LogType.ERROR);
            }
        }
        private bool ExecuteFileStart(string filePath, string args, int sleep)
        {
            bool result = false;
            ShowInfo("Starting " + filePath, Main.LogType.Normal);
            Log.Write(new string[] { "FilePath: " + filePath, "Args:" + args }, this, "ExecuteFileStart", Log.LogType.DEBUG);
            if (File.Exists(filePath))
            {
                try
                {
                    if (sleep > 0)
                    {
                        System.Threading.Thread.Sleep(sleep);
                    }
                    System.Diagnostics.Process process = System.Diagnostics.Process.Start(filePath, args);
                    if (process != null)
                    {
                        result = true;
                    }
                    else
                    {
                        ShowInfo("Error starting file", Main.LogType.Normal);
                        Log.Write("Error starting file", this, "ExecuteFileStart", Log.LogType.WARNING);
                    }
                }
                catch (Exception ex)
                {
                    Log.Write(ex, this, "ExecuteFileStart", Log.LogType.ERROR);
                }
            }
            else
            {
                ShowInfo("Error starting file", Main.LogType.Normal);
                Log.Write(new string[] { "File don't exists", "File: " + filePath, "Args: " + args }, this, "ExecuteFileStart", Log.LogType.WARNING);
            }
            return result;
        }



        public string MD5Hash(string sFilePath)
        {
            try
            {
                if (File.Exists(sFilePath))
                {
                    System.Security.Cryptography.MD5CryptoServiceProvider md5Provider
                    = new System.Security.Cryptography.MD5CryptoServiceProvider();
                    FileStream fs
                    = new FileStream(sFilePath, FileMode.Open, FileAccess.Read);
                    Byte[] hashCode
                    = md5Provider.ComputeHash(fs);

                    string ret = "";

                    foreach (byte a in hashCode)
                    {
                        if (a < 16)
                            ret += "0" + a.ToString("x");
                        else
                            ret += a.ToString("x");
                    }

                    fs.Close();
                    return ret;
                }
                else
                {
                    Log.Write(new string[] { "Text: File not found", "File: " + sFilePath }, this, "MD5Hash", Log.LogType.WARNING);
                    return "";
                }
            }
            catch (Exception ex)
            {
                Log.Write(ex, this, "MD5Hash", Log.LogType.ERROR);
                return "";
            }
        }
        private void WriteXML(string putanja)
        {
            const string format = "<Datoteka ime=\"{0}\" local=\"\" url=\"\" ver=\"{1}\" md5=\"{2}\" />";
            StreamWriter writer = new StreamWriter(putanja);
            writer.WriteLine("<?xml version=\"1.0\" encoding=\"utf-8\" ?>");
            writer.WriteLine("<Update verzija=\"{0}\" productid=\"{1}\">", product.versionServer, product.ProductID);
            foreach (FileStruct file in product.Files)
            {
                int ver;
                string md5;
                if (file.Preuzeto)
                {
                    ver = file.versionServer;
                    md5 = file.md5Server;
                }
                else
                {
                    ver = file.versionLocal;
                    md5 = file.md5Local;
                }
                writer.WriteLine(format, file.name, ver, md5);
            }
            writer.WriteLine("</Update>");
            writer.Flush();
            writer.Close();
        }
        public void ShowInfo(string text, Main.LogType logType)
        {
            if (main != null)
            {
                main.ShowInfo(text, logType);
            }
        }
        public Product Product
        {
            get
            {
                return this.product;
            }
        }
        private bool KillAllProccess(Product product)
        {
            bool showMessage = false;
            bool restart = false;

            foreach (FileStruct fileStruct in product.Files)
            {
                if (fileStruct.SaveSuccessfully == false && fileStruct.FileHaveNewVersion())
                {
                    // Copy is in _tmp directory
                    if (fileStruct.name.ToLower().EndsWith(".exe"))
                    {
                        // File is application try to kill it
                        if (!fileStruct.name.ToLower().StartsWith("autoupdate"))
                        {
                            if (!FindAndKillProcess(fileStruct.name.Replace(".exe", "")))
                            {
                                showMessage = true;
                            }
                        }
                    }
                    else
                    {
                        // File is not exe, is dll ...

                    }
                    restart = true;
                }
            }
            if (showMessage)
            {
                System.Windows.Forms.MessageBox.Show("Ugasi applikacije sve!!");
            }
            return restart;
        }

        // Code from WEB :(
        // URL: http://www.dreamincode.net/code/snippet1543.htm
        public bool FindAndKillProcess(string name)
        {
            bool result = false;
            try
            {
                //here we're going to get a list of all running processes on
                //the computer
                foreach (Process process in Process.GetProcesses())
                {
                    //now we're going to see if any of the running processes
                    //match the currently running processes by using the StartsWith Method,
                    //this prevents us from incluing the .EXE for the process we're looking for.
                    //. Be sure to not
                    //add the .exe to the name you provide, i.e: NOTEPAD,
                    //not NOTEPAD.EXE or false is always returned even if
                    //notepad is running
                    if (process.ProcessName.StartsWith(name))
                    {
                        //since we found the proccess we now need to use the
                        //Kill Method to kill the process. Remember, if you have
                        //the process running more than once, say IE open 4
                        //times the loop thr way it is now will close all 4,
                        //if you want it to just close the first one it finds
                        //then add a return; after the Kill
                        Log.Write("Killing process " + process.ProcessName, this, "FindAndKillProcess", Log.LogType.DEBUG);
                        process.Kill();
                        //process killed, return true
                        result = true;
                    }
                }
                if (result == false)
                {
                    Log.Write("Unable to kill process " + name, this, "FindAndKillProcess", Log.LogType.WARNING);
                }
            }
            catch (Exception ex)
            {
                Log.Write(ex, this, "FindAndKillProcess", Log.LogType.ERROR);
            }
            //process not found, return false
            return result;
        }

    }
}
