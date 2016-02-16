using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.IO;
using System.Diagnostics;
using System.Xml;


using DamirM.CommonLibrary;

namespace DamirM.AutoUpdate
{
    public partial class Main : Form
    {
        //public const string constDefaultServerAddress = "http://www.mobilis.hr/_program/update/";
        public const string constDefaultServerAddress = "http://update.ubertools.net/";
        //public const string constDefaultServerAddress = "http://update.local/";
        public const string constUpdateXMLServer = "update.xml";
        public const string constUpdateXMLLocal = "upd.xml";
        public const string constAutoUpdateExeName = "AutoUpdate.exe";  // TODO: ovo ne bi trebala biti const
        public const string constAutoUpdateProductName = "autoupdate";
        public const string constAuthorization_username = "admin";
        public const string constAuthorization_password = "admin";
        // parametars links
        public const string constURLParams_get_update_version = "update.php?action=get_update_version&productname={0}&productversion={1}";
        public const string constURLParams_get_update_xml = "update.php?action=get_update_xml&productid={0}";
        public const string constURLParams_disable_application = "index.php?action=disabled_application&productid={0}&version={1}";
        public const string constURLParams_get_product_list = "update.php?action=get_product_list";
        public const string constURLParams_get_product_from_product_name = "update.php?action=get_product_from_product_name&productname={0}";
        public const string constURLParams_get_product_from_product_id = "update.php?action=get_product_from_product_id&productid={0}";

        public const string constURLParams_authorization = "update.php?action=authorization";
        public const string constURLParams_download_file = "update.php?action=download_file&fileid={0}";

        Update update;
        

        public enum LogType
        {
            Normal, Append, RewriteLast
        }

        public Main(string serverAddress, Product product)
        {
            InitializeComponent();

            this.Text = "Damir Marijanovic - AutoUpdate application (" + Application.ProductVersion + ")";
            ShowInfo("AutoUpdate application", LogType.Normal);
            ShowInfo("Update for " + product.name, LogType.Normal);
            ShowInfo("Copyright ©  2008 - 2011", LogType.Normal);
            // process
            //applicationPath = Common.SetSlashOnEndOfDirectory(Application.StartupPath);
            //webCalls = WebCalls.Create();
            //webCalls.DownloadProgress += new WebCalls.delWebCallsDownloadProgress(webCalls_DownloadProgress);
            update = new Update(this, serverAddress, product);
            Log.NewMessage += new Log.delGenericLogMessage(Log_NewMessage);
        }

        public void Log_NewMessage(LogEntery logEntery)
        {
            if (logEntery.showExternal)
            {
                foreach (string line in logEntery.GetTextAsArray())
                {
                    ShowInfo(line, LogType.Normal);
                }
            }
        }

        public void webCalls_DownloadProgress(long current, long max)
        {
            try
            {
                int percentage;
                if (max > 0)
                {
                    percentage = (int)(((decimal)current / (decimal)max) * 100);
                }
                else
                {
                    percentage = 0;
                }
                pbDownloadFile.Value = percentage;
            }
            catch (Exception ex)
            {
                Log.Write(ex, this, "webCalls_DownloadProgress", Log.LogType.ERROR);
            }
        }

        private void Main_Load(object sender, EventArgs e)
        {
            this.Visible = true;
            Application.DoEvents();
            // Start main update procedure
            update.MainUpdateProcedure();
        }
        /// <summary>
        /// Dali je novi update dostupan
        /// </summary>
        /// <param name="updateURL"></param>
        /// <returns>Ako vrati true, novi update je dostupan</returns>
        //public static bool TimeToUpdate(string productUpdateURL, string productID, out string message)
        //{
        //    bool result = false;
        //    string requestURL;
        //    string buffer;
        //    Product product = null;

        //    //http://update.local/index.php?action=get_update_version&productid={0}
        //    // Get server version of this product
        //    requestURL = productUpdateURL + string.Format(constURLParams_get_update_version, productID);
        //    if (GetHTTPBody(requestURL, out buffer))
        //    {
        //        // All ok, in buffer is server update version
        //        buffer = buffer.Replace("\r\n", "");
        //        Main.ShowInfo("TimeToUpdate: " + buffer);
        //        // Load local old update xml file and get update version
        //        LoadUpdateXML_local(null, out product);
        //        product.versionServer = buffer;
        //        if (product.versionServer != product.versionLocal)
        //        {
        //            result = true;
        //        }
        //        message = "UpdateVersion: " + product.versionServer + " \r\nLocalVersion: " + product.versionLocal;
        //    }
        //    else
        //    {
        //        Main.ShowInfo("TimeToUpdate: " + buffer);
        //        message = "Error connecting to update server";
        //    }
            
        //    return result;
        //}
        /// <summary>
        /// This will disable application, server application will deside this
        /// </summary>
        /// <param name="updateURL">URL</param>
        /// <param name="messageText">Message text</param>
        /// <returns></returns>
        //public static bool DisableMainApplication(string productUpdateURL, string productID, out string message)
        //{
        //    string requestURL;
        //    string buffer;
        //    Product product = null;
        //    bool disableApplication = false;

        //    LoadUpdateXML_local(null, out product);
        //    //http://update.local/index.php?action=disabled_application&productid={0}&version={1}
        //    requestURL = productUpdateURL + string.Format(constURLParams_disable_application, productID, product.versionLocal);

        //    if (GetHTTPBody(requestURL, out buffer))
        //    {
        //        buffer = buffer.Replace("\r\n", "");
        //        Main.ShowInfo("TimeToUpdate: " + buffer);

        //        // If response text is yes, then disable application
        //        if (buffer.ToLower().Equals("yes"))
        //        {
        //            disableApplication = true;
        //            message = "Application disabled, pleas update";
        //        }
        //        else
        //        {
        //            message = "OK";
        //        }
        //    }
        //    else
        //    {
        //        // Error geting response from server, application will not be disabled
        //        Main.ShowInfo("TimeToUpdate: " + buffer);
        //        message = "Error connecting to update server";
        //    }
        //    return disableApplication;
        //}
        /// <summary>
        /// HTTP request na URL
        /// </summary>
        /// <param name="netUrl">URL destinacije</param>
        /// <returns>Vraca body HTTP requesta</returns>
        //public static bool GetHTTPBody(string netUrl, out string buffer)
        //{
        //    bool result = false;
        //    HttpWebRequest webreq;
        //    HttpWebResponse webresp;
        //    StreamReader sr;
        //    Main.ShowInfo("GetHTTPBody: " + netUrl);
        //    try
        //    {
        //        webreq = (HttpWebRequest)WebRequest.Create(netUrl);
        //        webresp = (HttpWebResponse)webreq.GetResponse();
        //        if (webresp.ResponseUri.AbsoluteUri == netUrl)
        //        {
        //            sr = new StreamReader(webresp.GetResponseStream());
        //            buffer = sr.ReadToEnd();
        //            result = true;
        //        }
        //        else
        //        {
        //            buffer = "Error";
        //        }
        //        Main.ShowInfo("GetHTTPBody: " + buffer);
        //    }
        //    catch (Exception e)
        //    {
        //        Main.ShowInfo("GetHTTPBody: " + e.Message);
        //        result = false;
        //        buffer = e.Message;
        //    }
        //    Main.ShowInfo("GetHTTPBody   Exit    " + result);
        //    return result;
        //}

        //private bool SelfUpdateProcedure()
        //{
        //    Product autoUpdateProduct;
        //    Stream stream;
        //    string requestURL;
        //    bool result;
        //    int fileDownloadCount;

        //    // Get product info, create product object
        //    requestURL = serverAddress + AutoUpdate.Main.constURLParams_get_product_from_product_name;
        //    requestURL = string.Format(requestURL, constAutoUpdateProductName);
        //    stream = webCalls.WebGETMethod(requestURL);
        //    autoUpdateProduct = Product.Create(stream);

        //    // Get File list for autoupdate
        //    requestURL = serverAddress + string.Format(constURLParams_get_update_xml, autoUpdateProduct.productID);
        //    stream = webCalls.WebGETMethod(requestURL);
        //    result = LoadUpdateXML_server(stream, autoUpdateProduct);

        //    // Find autoupdate.exe filesruct and modified it
        //    FileStruct fileStruct = autoUpdateProduct.GetFile(constAutoUpdateExeName);
        //    fileStruct.name = "AutoUpdateUPD.exe";
        //    fileStruct.autoStartArgs = string.Format("server={0} pid={1}", serverAddress, product.productID);

        //    Log.Write(autoUpdateProduct.Files, this, "SelfUpdateProcedure", Log.LogType.DEBUG);

        //    if (result == true)
        //    {
        //        if (DownloadAllFile(autoUpdateProduct.Files, out fileDownloadCount))
        //        {
        //            if (fileDownloadCount > 0)
        //            {
        //                // New AutoUpdate version available
        //                Log.Write("New AutoUpdate version available", this, "SelfUpdateProcedure", Log.LogType.DEBUG);
        //                ShowInfo("New AutoUpdate version available", LogType.Normal);
        //                ShowInfo("Restarting AutoUpdate in 3...", LogType.Normal);
        //                System.Threading.Thread.Sleep(1000);
        //                ShowInfo("Restarting AutoUpdate in 2...", LogType.RewriteLast);
        //                System.Threading.Thread.Sleep(1000);
        //                ShowInfo("Restarting AutoUpdate in 1...", LogType.RewriteLast);

        //                AutoStartDownloadFiles(autoUpdateProduct.Files);
        //                // Exit application now
        //                Log.Write("Exiting application", this, "SelfUpdateProcedure", Log.LogType.DEBUG);

        //                Application.Exit();
        //            }
        //            else
        //            {
        //                return true;
        //            }
        //        }
        //        else
        //        {
        //            // Error upgrading
        //            ShowInfo("", LogType.Normal);
        //            ShowInfo("An error occurred while trying to update", LogType.Normal);
        //            Log.Write("An error occurred while trying to update", this, "SelfUpdateProcedure", Log.LogType.WARNING);
        //        }
        //    }
        //    else
        //    {
        //        ShowInfo("", LogType.Normal);
        //        ShowInfo("Error connecting to update server", LogType.Normal);
        //        Log.Write("Error connecting to update server", this, "SelfUpdateProcedure", Log.LogType.WARNING);
        //    }
        //    return false;
        //}
        //private void MainUpdateProcedure()
        //{
        //    Stream stream;
        //    bool result = false;
        //    string updateXMLURL;
        //    int fileDownloadCount;


        //    // Get File list for selected product
        //    updateXMLURL = serverAddress + string.Format(constURLParams_get_update_xml, product.productID);
        //    stream = webCalls.WebGETMethod(updateXMLURL);
        //    result = LoadUpdateXML_server(stream, product);

        //    //GetUpdateLog(autoUpdateProduct, txtLog, "autoupdatelog.txt");
        //    //GetUpdateLog(product, txtLog, "log.txt");
        //    Log.Write("Skiping update change log...", this, "MainUpdateProcedure", Log.LogType.INFO);


        //    // List all files for download
        //    Log.Write(product.Files, this, "MainUpdateProcedure", Log.LogType.DEBUG);

        //    // Start main update method
        //    if (result == true)
        //    {
        //        // Load local update xml file,
        //        Product dummyProduct = new Product();
        //        LoadUpdateXML_local(product.Files, out dummyProduct);

        //        // Download all file 
        //        if (DownloadAllFile(product.Files, out fileDownloadCount))
        //        {
        //            // Upgrade successfull
        //            WriteXML(applicationPath + constUpdateXMLLocal);
        //            ShowInfo("", LogType.Normal);
        //            if (fileDownloadCount == 0)
        //            {
        //                Log.Write("Done", this, "MainUpdateProcedure", Log.LogType.DEBUG);
        //                ShowInfo("Done", LogType.Normal);
        //            }
        //            else
        //            {
        //                Log.Write("Update successful", this, "MainUpdateProcedure", Log.LogType.DEBUG);
        //                ShowInfo("Update successful", LogType.Normal);
        //            }
        //            btnAzuriraj.Enabled = true;
        //        }
        //        else
        //        {
        //            // Error upgrading
        //            ShowInfo("", LogType.Normal);
        //            ShowInfo("An error occurred while trying to update", LogType.Normal);
        //            Log.Write("An error occurred while trying to update", this, "MainUpdateProcedure", Log.LogType.WARNING);
        //        }

        //    }
        //    else
        //    {
        //        ShowInfo("", LogType.Normal);
        //        ShowInfo("Error connecting to update server", LogType.Normal);
        //        Log.Write("Error connecting to update server", this, "MainUpdateProcedure", Log.LogType.WARNING);
        //    }
        //}

        //private bool LoadUpdateXML_server(Stream stream, Product product)
        //{
        //    bool result = false;
        //    FileStruct fileStruct;

        //    try
        //    {
        //        XmlTextReader reader = new XmlTextReader(stream);
        //        while (reader.Read())
        //        {
        //            if (reader.NodeType == XmlNodeType.Element && reader.Name == "File")
        //            {
        //                fileStruct = new FileStruct(serverAddress + constURLParams_download_file, applicationPath);
        //                bool autoStart = reader.GetAttribute("autoStart") == "1";
        //                int fileID = int.Parse(reader.GetAttribute("id"));
        //                int versionServer = int.Parse(reader.GetAttribute("version"));
        //                fileStruct.AddFileInfo(reader.GetAttribute("name"), fileID, reader.GetAttribute("savePath"), reader.GetAttribute("url"), versionServer, reader.GetAttribute("md5"), autoStart);
        //                fileStruct.md5Local = MD5Hash(fileStruct.SavePath);
        //                product.AddFile(fileStruct);
        //            }
        //        }
        //        reader.Close();
        //        result = true;
        //    }
        //    catch (Exception ex)
        //    {
        //        ShowInfo("ERROR: " + ex.Message, LogType.Normal);
        //        Log.Write(ex, this, "LoadUpdateXML_server", Log.LogType.ERROR);
        //    }
        //    return result;
        //}
        //public static void LoadUpdateXML_local(ArrayList fileList, out Product product)
        //{
        //    //string autoUpdateVersion_local = "";
        //    string applicationPath = Application.StartupPath;
        //    string xmlPath = "";
        //    string fileName = "";
        //    int localVersion;
        //    FileStruct fileStruct = null;
        //    product = new Product();

        //    if (!applicationPath.EndsWith("\\"))
        //        applicationPath += "\\";

        //    xmlPath = applicationPath + constUpdateXMLLocal;

        //    Main.ShowInfo("Debug LoadUpdateXML_local Application.StartupPath " + Application.StartupPath);
        //    Main.ShowInfo("Debug LoadUpdateXML_local URL " + xmlPath);

        //    // Ako upd.xml ne postoji, predpostavka je da je update potreban
        //    if (!File.Exists(xmlPath))
        //    {
        //        return;
        //    }
        //    // parse xml

        //    XmlTextReader reader = new XmlTextReader(xmlPath);
        //    try
        //    {
        //        while (reader.Read())
        //        {
        //            if (reader.NodeType == XmlNodeType.Element && reader.Name == "Update")
        //            {
        //                product.versionLocal = reader.GetAttribute("verzija");
        //                product.productID = int.Parse(reader.GetAttribute("productid"));
        //                Main.ShowInfo("Debug LoadUpdateXML_local versionLocal " + product.versionLocal);
        //                Main.ShowInfo("Debug LoadUpdateXML_local productID " + product.productID);
        //            }
        //            else if (reader.NodeType == XmlNodeType.Element && reader.Name == "Datoteka")
        //            {
        //                // Ako je fileList null, tada poziv metodi dolati od vanjske aplikacije, aplikacija kojoj treba samo verzija zadnjeg update-a
        //                if (fileList != null)
        //                {
        //                    // get file name
        //                    fileName = reader.GetAttribute("ime").ToLower();
        //                    // search for collection of file from server xml list
        //                    for (int i = 0; i < fileList.Count; i++)
        //                    {
        //                        // cast to file list type, make reference to list ithem
        //                        fileStruct = (FileStruct)fileList[i];
        //                        //
        //                        if (fileStruct.name.ToLower() == fileName)
        //                        {
        //                            // update local version of file
        //                            int.TryParse(reader.GetAttribute("ver"), out localVersion);
        //                            fileStruct.versionLocal = localVersion;
        //                            //fileStruct.md5Local = reader.GetAttribute("md5");

        //                            break;
        //                        }
        //                    }
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Log.Write(ex, typeof(Main), "LoadUpdateXML_local", Log.LogType.ERROR);
        //    }
        //    finally
        //    {
        //        reader.Close();
        //    }
        //}

        //private bool DownloadAllFile(ArrayList files, out int fileDownloadCount)
        //{
        //    int errorCounter = 0;
        //    fileDownloadCount = 0;

        //    foreach (FileStruct fileStruct in files)
        //    {
        //        if (FileStruct.FileHaveNewVersion(fileStruct))
        //        {
        //            // Download new file from server
        //            fileStruct.Preuzeto = webCalls.DownloadFile(fileStruct.DownloadURL, fileStruct.SavePath);
        //            if (fileStruct.Preuzeto == false)
        //            {
        //                errorCounter++;
        //            }
        //            else
        //            {
        //                fileDownloadCount++;
        //            }
        //        }
        //    }

        //    if (errorCounter > 0)
        //    {
        //        // Error exit
        //        return false;
        //    }
        //    else
        //    {
        //        // OK exit
        //        return true;
        //    }
        //}

        private void btnIzlaz_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
        private void btnAzuriraj_Click(object sender, EventArgs e)
        {
            update.AutoStartDownloadFiles(update.Product.Files);
            Application.Exit();
        }

        //private void AutoStartDownloadFiles(ArrayList fileUpdateList)
        //{
        //    foreach (FileStruct fileStruct in fileUpdateList)
        //    {
        //        if (fileStruct.AutoStart)
        //        {
        //            ExecuteFileStart(fileStruct.SavePath, fileStruct.autoStartArgs, 1000);
        //        }
        //    }
        //}
        //private bool ExecuteFileStart(string filePath, string args, int sleep)
        //{
        //    bool result = false;
        //    ShowInfo("Starting " + filePath, LogType.Normal);
        //    Log.Write(new string[] { "FilePath: " + filePath, "Args:" + args }, this, "ExecuteFileStart", Log.LogType.DEBUG);
        //    if (File.Exists(filePath))
        //    {
        //        try
        //        {
        //            if (sleep > 0)
        //            {
        //                System.Threading.Thread.Sleep(sleep);
        //            }
        //            Process process = Process.Start(filePath, args);
        //            if (process != null)
        //            {
        //                result = true;
        //            }
        //            else
        //            {
        //                ShowInfo("Error starting file", LogType.Normal);
        //                Log.Write("Error starting file", this, "ExecuteFileStart", Log.LogType.WARNING);
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            Log.Write(ex, this, "ExecuteFileStart", Log.LogType.ERROR);
        //        }
        //    }
        //    else
        //    {
        //        ShowInfo("Error starting file", LogType.Normal);
        //        Log.Write("Error starting file", this, "ExecuteFileStart", Log.LogType.WARNING);
        //    }
        //    return result;
        //}
        //private void WriteXML(string putanja)
        //{
        //    const string format = "<Datoteka ime=\"{0}\" local=\"\" url=\"\" ver=\"{1}\" md5=\"{2}\" />";
        //    StreamWriter writer = new StreamWriter(putanja);
        //    writer.WriteLine("<?xml version=\"1.0\" encoding=\"utf-8\" ?>");
        //    writer.WriteLine("<Update verzija=\"{0}\" productid=\"{1}\">", product.versionServer, product.productID);
        //    foreach (FileStruct file in product.Files)
        //    {
        //        int ver;
        //        string md5;
        //        if (file.Preuzeto)
        //        {
        //            ver = file.versionServer;
        //            md5 = file.md5Server;
        //        }
        //        else
        //        {
        //            ver = file.versionLocal;
        //            md5 = file.md5Local;
        //        }
        //        writer.WriteLine(format, file.name, ver, md5);
        //    }
        //    writer.WriteLine("</Update>");
        //    writer.Flush();
        //    writer.Close();
        //}
        //public string MD5Hash(string sFilePath)
        //{
        //    try
        //    {
        //        if (File.Exists(sFilePath))
        //        {
        //            System.Security.Cryptography.MD5CryptoServiceProvider md5Provider
        //            = new System.Security.Cryptography.MD5CryptoServiceProvider();
        //            FileStream fs
        //            = new FileStream(sFilePath, FileMode.Open, FileAccess.Read);
        //            Byte[] hashCode
        //            = md5Provider.ComputeHash(fs);

        //            string ret = "";

        //            foreach (byte a in hashCode)
        //            {
        //                if (a < 16)
        //                    ret += "0" + a.ToString("x");
        //                else
        //                    ret += a.ToString("x");
        //            }

        //            fs.Close();
        //            return ret;
        //        }
        //        else
        //        {
        //            Log.Write(new string[] { "Text: File not found", "File: " + sFilePath }, this, "MD5Hash", Log.LogType.WARNING);
        //            return "";
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Log.Write(ex, this, "MD5Hash", Log.LogType.ERROR);
        //        return "";
        //    }
        //}
        public void ShowInfo(string text, LogType logType)
        {
            if (LogType.Append == logType)
            {
                lsbLog.Items[lsbLog.Items.Count - 1] = lsbLog.Items[lsbLog.Items.Count - 1] + text;
            }
            else if (LogType.RewriteLast == logType)
            {
                lsbLog.Items[lsbLog.Items.Count - 1] = text;
            }
            else
            {
                lsbLog.Items.Add(text);
            }
            lsbLog.SelectedIndex = lsbLog.Items.Count - 1;
            Application.DoEvents();
            //Log.Write(text, this, "ShowInfo", Log.LogType.INFO);
        }
    }


}