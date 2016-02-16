using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

using DamirM.CommonLibrary;

namespace DamirM.AutoUpdate.External
{
    public class External
    {
        Product product;
        WebCalls webCalls;

        private string serverAddress;
        private string productName;
        private string username;
        private string password;

        /// <summary>
        /// Ne koristi ovu metodu
        /// </summary>
        /// <param name="serverAddress"></param>
        /// <param name="productName"></param>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <param name="logCallBack"></param>
        public External(string serverAddress, string productName, string username, string password, Log.delGenericLogMessage logCallBack)
        {
            this.productName = productName;
            this.serverAddress = serverAddress;
            this.username = username;
            this.password = password;
            //Log.NewMessage += new Log.delGenericLogMessage(logCallBack);
            webCalls = WebCalls.Create();
            GetProduct();
            GetProductInfo();
        }
        public External(string serverAddress, string productName, string username, string password)
        {
            this.productName = productName;
            this.serverAddress = serverAddress;
            this.username = username;
            this.password = password;
            //Log.NewMessage += new Log.delGenericLogMessage(logCallBack);
            webCalls = WebCalls.Create();
            GetProduct();
            GetProductInfo();
        }

        private void GetProduct()
        {
            Update.LoadUpdateXML_local(null, out product);
        }

        public bool GetProductInfo()
        {
            bool result = false;
            string requestURL;
            Stream stream;

            Log.Write("Requesting product update version", this, "GetProductInfo", Log.LogType.DEBUG);

            // Authorization
            requestURL = serverAddress + AutoUpdate.Main.constURLParams_authorization;
            result = webCalls.WebAuthorization(requestURL, this.username, this.password);



            //http://update.local/index.php?action=get_update_version&productid={0}
            // Get server version of this product
            requestURL = serverAddress + string.Format(Main.constURLParams_get_update_version, productName, product.versionLocal);
            stream = webCalls.WebGETMethod(requestURL);
            product = Product.Create(stream, product);

            Log.Write(Product.Debug(product), this, "GetProductInfo", Log.LogType.DEBUG);
            return result;
        }
        public Product Product
        {
            get
            {
                return product;
            }
        }
    }
}
