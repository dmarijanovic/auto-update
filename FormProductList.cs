using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Xml;
using System.Windows.Forms;
using System.IO;

using DamirM.CommonLibrary;

namespace DamirM.AutoUpdate
{
    public partial class FormProductList : Form
    {
        private string serverAddress;
        private ArrayList productList = new ArrayList();
        WebCalls webCalls;
        Product product;
        bool isAuthorized = false;
        bool isExternal;

        public FormProductList(string serverAddress)
        {
            InitializeComponent();
            this.serverAddress = serverAddress;

            webCalls = new WebCalls();
        }
        public FormProductList(string serverAddress, bool isExternal)
        {
            InitializeComponent();
            this.isExternal = isExternal;
            this.serverAddress = serverAddress;

            webCalls = new WebCalls();
        }

        private void FillComboBox()
        {
            foreach (Product product in productList)
            {
                cbProductList.Items.Add(product);
            }
        }

        private void bCancel_Click(object sender, EventArgs e)
        {
            Application.Exit(); 
        }

        private void bOK_Click(object sender, EventArgs e)
        {
            if (isAuthorized)
            {
                product = this.SelectedProduct;
                this.Close();
            }
            else
            {
                RequestAuthorization(tbUsername.Text, tbPassword.Text);
            }
        }
        public void RequestAuthorization(string username, string password)
        {
            bool result;
            if (!tbUsername.Text.Trim().Equals(""))
            {
                bOK.Enabled = false;
                string requestURL = serverAddress + Main.constURLParams_authorization;
                result = webCalls.WebAuthorization(requestURL, username, password);
                //Log.Write(new string[] { "URL: " + requestURL, "Result: " + result.ToString() }, this, "RequestAuthorization", Log.LogType.DEBUG);
                if (result)
                {
                    isAuthorized = true;
                    gbAuthorization.Visible = false;
                    gbProducts.Visible = true;
                    RequestProductsList();
                }
                else
                {
                    MessageBox.Show("Authorization Failed", "Authorization Error", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    Log.Write("Authorization failed", this, "RequestAuthorization", Log.LogType.WARNING);
                }
                bOK.Enabled = true;
            }
        }
        private void RequestProductsList()
        {
            Stream stream;
            bOK.Enabled = false;
            string requestURL = serverAddress + Main.constURLParams_get_product_list;
            stream = webCalls.WebGETMethod(requestURL);
            //Log.Write(new string[] { "URL: " + requestURL, "Result: " + (stream != null).ToString() }, this, "RequestProductsList", Log.LogType.DEBUG);
            webCalls.GetProductListFromXML(stream, productList);
            Log.Write(productList, this, "RequestProductsList", Log.LogType.DEBUG);
            if (isExternal == false)
            {
                FillComboBox();
            }
            bOK.Enabled = true;
        }
        public Product Product
        {
            get
            {
                if (cbProductList.SelectedIndex != -1)
                {
                    return this.product;
                }
                else
                {
                    return null;
                }
            }
        }
        private Product SelectedProduct
        {
            get
            {
                if (cbProductList.SelectedIndex != -1)
                {
                    return (Product)cbProductList.Items[cbProductList.SelectedIndex];
                }
                else
                {
                    return null;
                }
            }
        }
        public ArrayList GetProductList
        {
            get
            {
                return productList;
            }
        }
        private void cbProductList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.SelectedProduct != null)
            {
                lDescription.Text = this.SelectedProduct.description;
            }
        }
    }
}
