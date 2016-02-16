namespace DamirM.AutoUpdate
{
    partial class FormProductList
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.bOK = new System.Windows.Forms.Button();
            this.bCancel = new System.Windows.Forms.Button();
            this.gbProducts = new System.Windows.Forms.GroupBox();
            this.lDescription = new System.Windows.Forms.Label();
            this.gbAuthorization = new System.Windows.Forms.GroupBox();
            this.tbPassword = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.tbUsername = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.cbProductList = new System.Windows.Forms.ComboBox();
            this.gbProducts.SuspendLayout();
            this.gbAuthorization.SuspendLayout();
            this.SuspendLayout();
            // 
            // bOK
            // 
            this.bOK.Location = new System.Drawing.Point(269, 176);
            this.bOK.Name = "bOK";
            this.bOK.Size = new System.Drawing.Size(75, 23);
            this.bOK.TabIndex = 0;
            this.bOK.Text = "&OK";
            this.bOK.UseVisualStyleBackColor = true;
            this.bOK.Click += new System.EventHandler(this.bOK_Click);
            // 
            // bCancel
            // 
            this.bCancel.Location = new System.Drawing.Point(350, 176);
            this.bCancel.Name = "bCancel";
            this.bCancel.Size = new System.Drawing.Size(75, 23);
            this.bCancel.TabIndex = 1;
            this.bCancel.Text = "&Cancel";
            this.bCancel.UseVisualStyleBackColor = true;
            this.bCancel.Click += new System.EventHandler(this.bCancel_Click);
            // 
            // gbProducts
            // 
            this.gbProducts.Controls.Add(this.lDescription);
            this.gbProducts.Controls.Add(this.label1);
            this.gbProducts.Controls.Add(this.cbProductList);
            this.gbProducts.Location = new System.Drawing.Point(13, 12);
            this.gbProducts.Name = "gbProducts";
            this.gbProducts.Size = new System.Drawing.Size(412, 158);
            this.gbProducts.TabIndex = 2;
            this.gbProducts.TabStop = false;
            this.gbProducts.Visible = false;
            // 
            // lDescription
            // 
            this.lDescription.Location = new System.Drawing.Point(20, 81);
            this.lDescription.Name = "lDescription";
            this.lDescription.Size = new System.Drawing.Size(373, 47);
            this.lDescription.TabIndex = 3;
            this.lDescription.Text = "Select product for update";
            // 
            // gbAuthorization
            // 
            this.gbAuthorization.Controls.Add(this.tbPassword);
            this.gbAuthorization.Controls.Add(this.label3);
            this.gbAuthorization.Controls.Add(this.tbUsername);
            this.gbAuthorization.Controls.Add(this.label2);
            this.gbAuthorization.Location = new System.Drawing.Point(13, 12);
            this.gbAuthorization.Name = "gbAuthorization";
            this.gbAuthorization.Size = new System.Drawing.Size(412, 158);
            this.gbAuthorization.TabIndex = 2;
            this.gbAuthorization.TabStop = false;
            this.gbAuthorization.Text = "Authorization data";
            // 
            // tbPassword
            // 
            this.tbPassword.Location = new System.Drawing.Point(123, 76);
            this.tbPassword.Name = "tbPassword";
            this.tbPassword.PasswordChar = '*';
            this.tbPassword.Size = new System.Drawing.Size(270, 20);
            this.tbPassword.TabIndex = 3;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(28, 76);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(53, 13);
            this.label3.TabIndex = 2;
            this.label3.Text = "&Password";
            // 
            // tbUsername
            // 
            this.tbUsername.Location = new System.Drawing.Point(123, 29);
            this.tbUsername.Name = "tbUsername";
            this.tbUsername.Size = new System.Drawing.Size(270, 20);
            this.tbUsername.TabIndex = 1;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(28, 32);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(55, 13);
            this.label2.TabIndex = 0;
            this.label2.Text = "&Username";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(20, 20);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(49, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "&Products";
            // 
            // cbProductList
            // 
            this.cbProductList.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbProductList.FormattingEnabled = true;
            this.cbProductList.Location = new System.Drawing.Point(23, 42);
            this.cbProductList.Name = "cbProductList";
            this.cbProductList.Size = new System.Drawing.Size(370, 21);
            this.cbProductList.TabIndex = 0;
            this.cbProductList.SelectedIndexChanged += new System.EventHandler(this.cbProductList_SelectedIndexChanged);
            // 
            // FormProductList
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(437, 213);
            this.Controls.Add(this.bCancel);
            this.Controls.Add(this.bOK);
            this.Controls.Add(this.gbAuthorization);
            this.Controls.Add(this.gbProducts);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormProductList";
            this.Text = "Select product";
            this.gbProducts.ResumeLayout(false);
            this.gbProducts.PerformLayout();
            this.gbAuthorization.ResumeLayout(false);
            this.gbAuthorization.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button bOK;
        private System.Windows.Forms.Button bCancel;
        private System.Windows.Forms.GroupBox gbProducts;
        private System.Windows.Forms.GroupBox gbAuthorization;
        private System.Windows.Forms.TextBox tbPassword;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox tbUsername;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cbProductList;
        private System.Windows.Forms.Label lDescription;
    }
}