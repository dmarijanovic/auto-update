using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.IO;
using System.Xml;

using DamirM.CommonLibrary;

namespace DamirM.AutoUpdate
{
    public class Product
    {
        public string name;
        public string description;
        private int productID;
        public string versionServer;
        public string versionLocal;
        public bool updateReady;
        public bool allowStart;
        public bool autoStartUpdate;
        private ArrayList fileStructList;

        public Product()
        {
            fileStructList = new ArrayList();
        }
        public Product(int productID): this("", "", productID)
        { }
        public Product(string name, string description, int productID): this()
        {
            this.name = name;
            this.description = description;
            this.productID = productID;
        }

        public void AddFile(FileStruct fileStruct)
        {
            fileStructList.Add(fileStruct);
        }

        public FileStruct GetFile(string fileName)
        {
            FileStruct fileStruct = null;
            fileStructList.Sort();
            int index = -1;

            for (int i = 0; i < fileStructList.Count; i++)
            {
                fileStruct = (FileStruct)fileStructList[i];
                if (fileStruct.name.ToLower() == fileName.ToLower())
                {
                    index = i;
                }
            }

            if (index >= 0)
            {
                return fileStruct;
            }
            else
            {
                Log.Write("File Not Found", this, "GetFile", Log.LogType.ERROR);
                return fileStruct;
            }
        }


        public override string ToString()
        {
            return name;
        }
        public ArrayList Files
        {
            get
            {
                return fileStructList;
            }
        }

        /// <summary>
        /// Update product from stream
        /// </summary>
        /// <param name="stream">XML data in stream</param>
        /// <returns>Return product object</returns>
        public static Product Create(Stream stream, Product product)
        {
            // 
            try
            {
                XmlTextReader reader = new XmlTextReader(stream);
                while (reader.Read())
                {
                    if (reader.NodeType == XmlNodeType.Element && reader.Name == "Product")
                    {
                        // New product class
                        int productID;
                        productID = int.Parse(reader.GetAttribute("id"));
                        if (product == null)
                        {
                            product = new Product(productID);
                        }
                        else
                        {
                            product.productID = productID;
                        }
                    }
                    if (reader.NodeType == XmlNodeType.Element && reader.Name == "Name")
                    {
                        product.name = reader.ReadInnerXml();
                    }
                    if (reader.NodeType == XmlNodeType.Element && reader.Name == "Version")
                    {
                        product.versionServer = reader.ReadInnerXml();
                    }
                    if (reader.NodeType == XmlNodeType.Element && reader.Name == "Description")
                    {
                        product.description = reader.ReadInnerXml();
                    }
                    if (reader.NodeType == XmlNodeType.Element && reader.Name == "UpdateReady")
                    {
                        product.updateReady = reader.ReadInnerXml().Equals("1") ? true : false;
                    }
                    if (reader.NodeType == XmlNodeType.Element && reader.Name == "AllowStart")
                    {
                        product.allowStart = reader.ReadInnerXml().Equals("1") ? true : false;
                    }
                    if (reader.NodeType == XmlNodeType.Element && reader.Name == "AutoStartUpdate")
                    {
                        product.autoStartUpdate = reader.ReadInnerXml().Equals("1") ? true : false;
                    }
                }
                reader.Close();
            }
            catch (Exception ex)
            {
                Log.Write(ex, typeof(Product), "Create", Log.LogType.ERROR);
            }
            return product;
        }
        /// <summary>
        /// Create product from stream
        /// </summary>
        /// <param name="stream">XML data in stream</param>
        /// <returns>Return product object</returns>
        public static Product Create(Stream stream)
        {
            return Create(stream, null);
        }
        public static string Debug(Product product)
        {
            string buff;
            if (product != null)
            {
                buff = "Name: " + product.name + Environment.NewLine;
                buff += "Description: " + product.description + Environment.NewLine;
                buff += "ProductID: " + product.productID + Environment.NewLine;
                buff += "VersionServer: " + product.versionServer + Environment.NewLine;
                buff += "VersionLocal: " + product.versionLocal + Environment.NewLine;
                buff += "UpdateReady: " + product.updateReady + Environment.NewLine;
                buff += "AllowStart: " + product.allowStart + Environment.NewLine;
                buff += "AutoStartUpdate: " + product.autoStartUpdate + Environment.NewLine;
            }
            else
            {
                buff = "Product is empty";
            }
            return buff;
        }

        public int ProductID
        {
            get
            {
                return productID;
            }
            set
            {
                this.productID = value;
            }

        }
    }
}
