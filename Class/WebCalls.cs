using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Collections;

using System.Net;
using System.IO;

using DamirM.CommonLibrary;

namespace DamirM.AutoUpdate
{
    public class WebCalls
    {
        private static WebCalls webCalls;
        CookieContainer cookieContainer;    // save cookies for sessions

        public delegate void delWebCallsDownloadProgress(long current, long max);
        public event delWebCallsDownloadProgress DownloadProgress;

        public WebCalls()
        {
            cookieContainer = new CookieContainer();
        }
        /// <summary>
        /// Make new instance if not exists or return old one
        /// </summary>
        /// <returns></returns>
        public static WebCalls Create()
        {
            if (WebCalls.webCalls == null)
            {
                WebCalls.webCalls = new WebCalls();
            }
            return WebCalls.webCalls;
        }

        public bool GetProductListFromXML(Stream stream, ArrayList productList)
        {
            Product product = null;
            XmlTextReader reader = new XmlTextReader(stream);
           
            //XmlReader reader = XmlReader.Create(stream);
            try
            {
                while (reader.Read())
                {
                    if (reader.NodeType == XmlNodeType.Element && reader.Name == "Product")
                    {
                        // New product class
                        product = new Product(int.Parse(reader.GetAttribute("id")));
                        // Add product to list
                        productList.Add(product);
                    }
                    if (reader.NodeType == XmlNodeType.Element && reader.Name == "Name")
                    {
                        product.name = reader.ReadInnerXml();
                    }
                    if (reader.NodeType == XmlNodeType.Element && reader.Name == "Version")
                    {
                        product.versionServer = reader.ReadInnerXml();
                    }
                    //if (reader.NodeType == XmlNodeType.Element && reader.Name == "")
                    //{

                    //}
                    if (reader.NodeType == XmlNodeType.Element && reader.Name == "Description")
                    {
                        product.description = reader.ReadInnerXml();
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Write(ex, this, "GetProductListFromXML", Log.LogType.ERROR);
            }
            finally
            {
                reader.Close();
            }
            return true;
        }

        //public Product GetProductFromProductName(Stream stream)
        //{
        //    // 
        //    Product product = null;
        //    try
        //    {
        //        XmlTextReader reader = new XmlTextReader(stream);
        //        while (reader.Read())
        //        {
        //            if (reader.NodeType == XmlNodeType.Element && reader.Name == "Product")
        //            {
        //                // New product class
        //                int productID;
        //                productID = int.Parse(reader.GetAttribute("id"));
        //                product = new Product(productID);
        //            }
        //            if (reader.NodeType == XmlNodeType.Element && reader.Name == "Name")
        //            {
        //                product.name = reader.ReadInnerXml();
        //            }
        //            if (reader.NodeType == XmlNodeType.Element && reader.Name == "Version")
        //            {
        //                product.versionServer = reader.ReadInnerXml();
        //            }
        //            if (reader.NodeType == XmlNodeType.Element && reader.Name == "Description")
        //            {
        //                product.description = reader.ReadInnerXml();
        //            }
        //        }
        //        reader.Close();
        //    }
        //    catch (Exception ex)
        //    {
        //        Log.Write(ex, this, "GetProductFromProductName", Log.LogType.ERROR);
        //  }
        //    return product;
        //}

        public bool WebAuthorization(string requestURL, string username, string password)
        {
            HttpWebRequest request;
            HttpWebResponse response;
            bool result = false;
            requestURL = System.Web.HttpUtility.UrlPathEncode(requestURL);
            Log.Write(new string[] { "URL: " + requestURL, "Username:" + username }, this, "WebAuthorization", Log.LogType.DEBUG);
            try
            {
                request = (HttpWebRequest)WebRequest.Create(requestURL);
                request.CookieContainer = cookieContainer;
                request.ContentType = "application/x-www-form-urlencoded";
                request.Method = "POST";
                string postData = string.Format("username={0}&password={1}", username, password);
                byte[] byteArray = Encoding.UTF8.GetBytes(postData);
                Stream stream = request.GetRequestStream();
                stream.Write(byteArray, 0, byteArray.Length);
                stream.Close();

                response = (HttpWebResponse)request.GetResponse();
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    result = true;
                }
                else
                {
                    Log.Write("StatusCode: " + response.StatusCode.ToString(), this, "WebAuthorization", Log.LogType.WARNING);
                }
            }
            catch (WebException ex)
            {
                Log.Write(ex, this, "WebAuthorization", Log.LogType.ERROR);
            }
            catch (Exception ex)
            {
                Log.Write(ex, this, "WebAuthorization", Log.LogType.ERROR);
            }
            return result;
        }
        public Stream WebGETMethod(string requestURL)
        {
            HttpWebRequest request;
            HttpWebResponse response;
            Stream stream = null;

            requestURL = System.Web.HttpUtility.UrlPathEncode(requestURL);
            Log.Write("URL: " + requestURL, this, "WebGETMethod", Log.LogType.DEBUG);
            try
            {
                request = (HttpWebRequest)WebRequest.Create(requestURL);
                request.CookieContainer = cookieContainer;
                request.Method = "GET";

                response = (HttpWebResponse)request.GetResponse();
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    stream = response.GetResponseStream();
                }
                else
                {
                    Log.Write("StatusCode: " + response.StatusCode.ToString(), this, "WebGETMethod", Log.LogType.WARNING);
                }
            }
            catch (WebException ex)
            {
                Log.Write(ex, this, "WebGETMethod", Log.LogType.ERROR);
            }
            catch (Exception ex)
            {
                Log.Write(ex, this, "WebGETMethod", Log.LogType.ERROR);
            }
            return stream;
        }
        public bool DownloadFile(string url, FileStruct fileStruct)
        {
            bool result = false;
            long downloadProgres = 0;
            HttpWebRequest request;
            HttpWebResponse response;

            FileStream fs;
            BinaryReader r;
            BinaryWriter w = null;

            // URLEncode
            url = System.Web.HttpUtility.UrlPathEncode(url);

            Log.Write(new string[] { "URL: " + url, "SavePath:" + fileStruct.SavePath }, this, "DownloadFile", Log.LogType.DEBUG, true);
            //save this in a file with the same name
            try
            {
                request = (HttpWebRequest)WebRequest.Create(url);
                request.CookieContainer = cookieContainer;
                response = (HttpWebResponse)request.GetResponse();
                if (response.ResponseUri.AbsoluteUri == url)
                {
                    // try open file for writing
                    fs = TryCreateFileStream(fileStruct.SavePath);

                    if (fs == null)
                    {
                        // Orginal location is in use, save it in _tmp folder
                        fileStruct.SaveSuccessfully = false; 
                        Common.MakeAllSubFolders(Common.BuildPath(fileStruct.BaseFolder, "_tmp"));

                        fs = TryCreateFileStream(Common.BuildPath(fileStruct.BaseFolder, "_tmp") + fileStruct.md5Server);
                        if (fs == null)
                        {
                            throw new Exception("Error creating file stream in tmp folder");
                        }
                    }
                    else
                    {
                        // File will be saved on orginal locatin
                        fileStruct.SaveSuccessfully = true; 
                    }

                    result = true;
                    r = new BinaryReader(response.GetResponseStream());
                    w = new BinaryWriter(fs);
                    while (true)
                    {
                        if ((downloadProgres % 1000) == 0)
                        {
                            if (DownloadProgress != null)
                            {
                                DownloadProgress(downloadProgres, response.ContentLength);
                            }
                        }
                        w.Write(r.ReadByte());
                        downloadProgres++;
                    }


                }
            }
            catch (System.IO.EndOfStreamException ex)
            {
                //Log.Write(ex, this, "DownloadFile", Log.LogType.ERROR);
            }
            catch (Exception ex)
            {
                result = false;
                Log.Write(ex, this, "DownloadFile", Log.LogType.ERROR, true);
            }
            finally
            {
                if (w != null)
                {
                    w.Flush();
                    w.Close();
                }
            }
            return result;
        }

        public void UploadFiles(string url, string[] files, System.Collections.Specialized.NameValueCollection nvc)
        {

            long length = 0;
            string boundary = "----------------------------" +
            DateTime.Now.Ticks.ToString("x");


            HttpWebRequest httpWebRequest2 = (HttpWebRequest)WebRequest.Create(url);
            httpWebRequest2.ContentType = "multipart/form-data; boundary=" +
            boundary;
            httpWebRequest2.Method = "POST";
            httpWebRequest2.KeepAlive = true;
            httpWebRequest2.Credentials =
            System.Net.CredentialCache.DefaultCredentials;



            Stream memStream = new System.IO.MemoryStream();

            byte[] boundarybytes = System.Text.Encoding.ASCII.GetBytes("\r\n--" +
            boundary + "\r\n");


            string formdataTemplate = "\r\n--" + boundary +
            "\r\nContent-Disposition: form-data; name=\"{0}\";\r\n\r\n{1}";

            foreach (string key in nvc.Keys)
            {
                string formitem = string.Format(formdataTemplate, key, nvc[key]);
                byte[] formitembytes = System.Text.Encoding.UTF8.GetBytes(formitem);
                memStream.Write(formitembytes, 0, formitembytes.Length);
            }


            memStream.Write(boundarybytes, 0, boundarybytes.Length);

            string headerTemplate = "Content-Disposition: form-data; name=\"{0}\"; filename=\"{1}\"\r\n Content-Type: application/octet-stream\r\n\r\n";

            for (int i = 0; i < files.Length; i++)
            {

                //string header = string.Format(headerTemplate, "file" + i, files[i]);
                string header = string.Format(headerTemplate, "uplTheFile", files[i]);

                byte[] headerbytes = System.Text.Encoding.UTF8.GetBytes(header);

                memStream.Write(headerbytes, 0, headerbytes.Length);


                FileStream fileStream = new FileStream(files[i], FileMode.Open,
                FileAccess.Read);
                byte[] buffer = new byte[1024];

                int bytesRead = 0;

                while ((bytesRead = fileStream.Read(buffer, 0, buffer.Length)) != 0)
                {
                    memStream.Write(buffer, 0, bytesRead);

                }


                memStream.Write(boundarybytes, 0, boundarybytes.Length);


                fileStream.Close();
            }

            httpWebRequest2.ContentLength = memStream.Length;

            Stream requestStream = httpWebRequest2.GetRequestStream();

            memStream.Position = 0;
            byte[] tempBuffer = new byte[memStream.Length];
            memStream.Read(tempBuffer, 0, tempBuffer.Length);
            memStream.Close();
            requestStream.Write(tempBuffer, 0, tempBuffer.Length);
            requestStream.Close();


            WebResponse webResponse2 = httpWebRequest2.GetResponse();

            Stream stream2 = webResponse2.GetResponseStream();
            StreamReader reader2 = new StreamReader(stream2);

            Log.Write(ReadStream(stream2), this, "UploadFiles", Log.LogType.DEBUG);
            //MessageBox.Show(reader2.ReadToEnd());

            webResponse2.Close();
            httpWebRequest2 = null;
            webResponse2 = null;

        }
        public string ReadStream( Stream stream)
        {
            StreamReader sr = new StreamReader(stream);
            return sr.ReadToEnd();
        }
        private FileStream TryCreateFileStream(string filePath)
        {
            FileStream fs = null;
            try
            {
                fs = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.Write);
            }
            catch (Exception ex)
            {
                Log.Write(ex, this, "TryCreateFileStream", Log.LogType.ERROR);
            }
            return fs;
        }

    }
}
