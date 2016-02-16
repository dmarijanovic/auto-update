using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.IO;
using System.Threading;

using DamirM.CommonLibrary;

namespace DamirM.AutoUpdate
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            Log.SaveLocal = true;
            Log log = new Log();
            log.Show();
            Log.Write(new string[] { "Starting " + Application.ProductName, "Version: " + Application.ProductVersion, "Path: " + Application.ExecutablePath }, typeof(Program), "Main", Log.LogType.DEBUG);
            Log.Write(args, typeof(Program), "Main", Log.LogType.DEBUG);
            string putanja = Application.StartupPath;
            if (!putanja.EndsWith("\\"))
                putanja += "\\";

            // If this application ends with upd then this is new update temperaly application 
            if (Application.ExecutablePath.ToLower().EndsWith("upd.exe"))
            {
                try
                {
                    // Weith for old application to exit
                    Thread.Sleep(1000);
                    // Delete old application
                    if (File.Exists(putanja + "AutoUpdate.exe"))
                    {
                        File.Delete(putanja + "AutoUpdate.exe");
                        Log.Write("Delete AutoUpdate.exe", typeof(Program), "Main", Log.LogType.DEBUG);
                    }
                    // Copy self as new application
                    File.Copy(putanja + "autoupdateupd.exe", putanja + "AutoUpdate.exe");
                    Log.Write(new string[] { "Copy from: " + putanja + "autoupdateupd.exe", "Copy to: " + putanja + "AutoUpdate.exe" }, typeof(Program), "Main", Log.LogType.DEBUG);

                    // Start new application
                    string org_args = "";
                    foreach (string arg in args)
                    {
                        org_args += " " + arg;
                    }
                    System.Diagnostics.Process process = System.Diagnostics.Process.Start(putanja + "AutoUpdate.exe", org_args);
                    // If process is not null new application is started, then exit this application
                    if (process != null)
                    {
                        Log.Write("Exit autoupdateupd.exe", typeof(Program), "Main", Log.LogType.DEBUG);
                        Application.Exit();
                    }
                }
                catch (Exception ex)
                {
                    Log.Write(ex, typeof(Program), "Main", Log.LogType.ERROR);
                    MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                // Entering normal application, delete if new update application exists
                try
                {
                    if (File.Exists(putanja + "AutoUpdateUPD.exe"))
                    {
                        File.Delete(putanja + "AutoUpdateUPD.exe");
                        Log.Write("Delete AutoUpdateUPD.exe", typeof(Program), "Main", Log.LogType.DEBUG);
                    }
                }
                catch
                {
                    MessageBox.Show("Error deleting AutoUpdateUPD.exe, " + Application.ExecutablePath);
                }

                // Resolve update data, server address, program id
                string serverAddress = "";
                string productName = "";    // sent by main application that request update
                int productID = 0;          // sent by autoupdate program, self update
                Product product = null;
                bool result;

                // command line arguments, parse
                if (args.Length > 0)
                {
                    string[] commandAndValue;
                    foreach (string arg in args)
                    {
                        commandAndValue = arg.Split(new char[] { '=' });
                        if (commandAndValue.Length > 1)
                        {
                            if (commandAndValue[0] == "server")
                            {
                                serverAddress = commandAndValue[1];
                            }
                            else if (commandAndValue[0] == "productid")
                            {
                                // ProductID je sad productName, jer master app ima samo productName
                                // Obrisati kasnije
                                productName = commandAndValue[1];
                            }
                            else if (commandAndValue[0] == "productname")
                            {
                                productName = commandAndValue[1];
                            }
                            else if (commandAndValue[0] == "pid")
                            {
                                int.TryParse(commandAndValue[1], out productID);
                            }
                        }
                    }
                }


                // server address
                if (serverAddress == "")
                {
                    serverAddress = AutoUpdate.Main.constDefaultServerAddress;
                }

                // try get product info from local file
                if (productName == "" && productID == 0)
                {
                    // Look for local upd.xml to resolve product
                    // 
                    // TODO: local file parse
                    AutoUpdate.Update.LoadUpdateXML_local(null, out product);
                    productID = product.ProductID;
                }

                // Check self update 
                Update update = new Update(serverAddress, productName, productID);
                Result.ResultType resultType = update.SelfUpdateProcedure();

                if (resultType == Result.ResultType.True)
                {



                    if (productName == "" && productID == 0)
                    {
                        // If not then show dialog
                        FormProductList frm = new FormProductList(serverAddress);
                        frm.ShowDialog();
                        product = frm.Product;

                    }
                    else
                    {
                        // create product object from productName
                        Stream stream;
                        WebCalls webCalls = WebCalls.Create();
                        string requestURL = "";

                        if (productName != "")
                        {
                            requestURL = serverAddress + AutoUpdate.Main.constURLParams_get_product_from_product_name;
                            requestURL = string.Format(requestURL, productName);
                        }
                        else if (productID > 0)
                        {
                            requestURL = serverAddress + AutoUpdate.Main.constURLParams_get_product_from_product_id;
                            requestURL = string.Format(requestURL, productID);
                        }
                        stream = webCalls.WebGETMethod(requestURL);
                        product = Product.Create(stream);
                    }
                    Log.Write(Product.Debug(product), typeof(Program), "Main", Log.LogType.DEBUG);
                    if (product != null)
                    {
                        Application.Run(new Main(serverAddress, product));
                    }
                    else
                    {
                        MessageBox.Show("No product name", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else if (resultType == Result.ResultType.False)
                {
                    MessageBox.Show("Fatal error, please send error log on mail dami.marijanovic@gmail.com", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}