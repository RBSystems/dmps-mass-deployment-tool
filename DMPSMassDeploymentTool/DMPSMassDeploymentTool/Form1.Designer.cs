﻿namespace DMPSMassDeploymentTool
{
    partial class Form1
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.browseVTZFileButton = new System.Windows.Forms.Button();
            this.label8 = new System.Windows.Forms.Label();
            this.vtzFileLocation = new System.Windows.Forms.TextBox();
            this.applyFilterButton = new System.Windows.Forms.Button();
            this.filterText = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.loadListFromELKButton = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.addDMPSUnit = new System.Windows.Forms.Button();
            this.addIPAddressBox = new System.Windows.Forms.TextBox();
            this.addHostNameBox = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.deployToDMPSButton = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.dmpsList = new System.Windows.Forms.CheckedListBox();
            this.label3 = new System.Windows.Forms.Label();
            this.browseSPZfileButton = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.spzFileLocation = new System.Windows.Forms.TextBox();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.button1 = new System.Windows.Forms.Button();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(961, 637);
            this.tabControl1.TabIndex = 4;
            // 
            // tabPage1
            // 
            this.tabPage1.BackColor = System.Drawing.SystemColors.Control;
            this.tabPage1.Controls.Add(this.browseVTZFileButton);
            this.tabPage1.Controls.Add(this.label8);
            this.tabPage1.Controls.Add(this.vtzFileLocation);
            this.tabPage1.Controls.Add(this.applyFilterButton);
            this.tabPage1.Controls.Add(this.filterText);
            this.tabPage1.Controls.Add(this.label7);
            this.tabPage1.Controls.Add(this.loadListFromELKButton);
            this.tabPage1.Controls.Add(this.groupBox1);
            this.tabPage1.Controls.Add(this.deployToDMPSButton);
            this.tabPage1.Controls.Add(this.label4);
            this.tabPage1.Controls.Add(this.dmpsList);
            this.tabPage1.Controls.Add(this.label3);
            this.tabPage1.Controls.Add(this.browseSPZfileButton);
            this.tabPage1.Controls.Add(this.label1);
            this.tabPage1.Controls.Add(this.label2);
            this.tabPage1.Controls.Add(this.spzFileLocation);
            this.tabPage1.Location = new System.Drawing.Point(4, 25);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(953, 608);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "DMPS Unit";
            // 
            // browseVTZFileButton
            // 
            this.browseVTZFileButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.browseVTZFileButton.Location = new System.Drawing.Point(870, 121);
            this.browseVTZFileButton.Name = "browseVTZFileButton";
            this.browseVTZFileButton.Size = new System.Drawing.Size(75, 27);
            this.browseVTZFileButton.TabIndex = 20;
            this.browseVTZFileButton.Text = "Browse...";
            this.browseVTZFileButton.UseVisualStyleBackColor = true;
            this.browseVTZFileButton.Click += new System.EventHandler(this.browseVTZFileButton_Click);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(8, 105);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(371, 17);
            this.label8.TabIndex = 18;
            this.label8.Text = "Step 3: Choose the VTZ file to push to all of the UI Panels";
            // 
            // vtzFileLocation
            // 
            this.vtzFileLocation.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.vtzFileLocation.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.vtzFileLocation.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.FileSystem;
            this.vtzFileLocation.Location = new System.Drawing.Point(9, 125);
            this.vtzFileLocation.Name = "vtzFileLocation";
            this.vtzFileLocation.Size = new System.Drawing.Size(855, 22);
            this.vtzFileLocation.TabIndex = 19;
            // 
            // applyFilterButton
            // 
            this.applyFilterButton.Location = new System.Drawing.Point(339, 197);
            this.applyFilterButton.Name = "applyFilterButton";
            this.applyFilterButton.Size = new System.Drawing.Size(75, 26);
            this.applyFilterButton.TabIndex = 17;
            this.applyFilterButton.Text = "Apply";
            this.applyFilterButton.UseVisualStyleBackColor = true;
            this.applyFilterButton.Click += new System.EventHandler(this.applyFilterButton_Click);
            // 
            // filterText
            // 
            this.filterText.Location = new System.Drawing.Point(64, 200);
            this.filterText.Name = "filterText";
            this.filterText.Size = new System.Drawing.Size(270, 22);
            this.filterText.TabIndex = 16;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(19, 202);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(39, 17);
            this.label7.TabIndex = 15;
            this.label7.Text = "Filter";
            // 
            // loadListFromELKButton
            // 
            this.loadListFromELKButton.Location = new System.Drawing.Point(805, 166);
            this.loadListFromELKButton.Name = "loadListFromELKButton";
            this.loadListFromELKButton.Size = new System.Drawing.Size(151, 28);
            this.loadListFromELKButton.TabIndex = 14;
            this.loadListFromELKButton.Text = "Load list from ELK";
            this.loadListFromELKButton.UseVisualStyleBackColor = true;
            this.loadListFromELKButton.Click += new System.EventHandler(this.loadListFromELKButton_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.addDMPSUnit);
            this.groupBox1.Controls.Add(this.addIPAddressBox);
            this.groupBox1.Controls.Add(this.addHostNameBox);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Location = new System.Drawing.Point(9, 497);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(936, 63);
            this.groupBox1.TabIndex = 13;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Add Other DMPS Unit";
            // 
            // addDMPSUnit
            // 
            this.addDMPSUnit.Location = new System.Drawing.Point(696, 28);
            this.addDMPSUnit.Name = "addDMPSUnit";
            this.addDMPSUnit.Size = new System.Drawing.Size(75, 23);
            this.addDMPSUnit.TabIndex = 4;
            this.addDMPSUnit.Text = "Add";
            this.addDMPSUnit.UseVisualStyleBackColor = true;
            this.addDMPSUnit.Click += new System.EventHandler(this.addDMPSUnit_Click);
            // 
            // addIPAddressBox
            // 
            this.addIPAddressBox.Location = new System.Drawing.Point(482, 28);
            this.addIPAddressBox.Name = "addIPAddressBox";
            this.addIPAddressBox.Size = new System.Drawing.Size(208, 22);
            this.addIPAddressBox.TabIndex = 3;
            // 
            // addHostNameBox
            // 
            this.addHostNameBox.Location = new System.Drawing.Point(94, 28);
            this.addHostNameBox.Name = "addHostNameBox";
            this.addHostNameBox.Size = new System.Drawing.Size(300, 22);
            this.addHostNameBox.TabIndex = 2;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(400, 31);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(76, 17);
            this.label6.TabIndex = 1;
            this.label6.Text = "IP Address";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(10, 31);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(78, 17);
            this.label5.TabIndex = 0;
            this.label5.Text = "Host Name";
            // 
            // deployToDMPSButton
            // 
            this.deployToDMPSButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.deployToDMPSButton.Location = new System.Drawing.Point(134, 566);
            this.deployToDMPSButton.Name = "deployToDMPSButton";
            this.deployToDMPSButton.Size = new System.Drawing.Size(262, 34);
            this.deployToDMPSButton.TabIndex = 12;
            this.deployToDMPSButton.Text = "Deploy Program to DMPS Units";
            this.deployToDMPSButton.UseVisualStyleBackColor = true;
            this.deployToDMPSButton.Click += new System.EventHandler(this.deployToDMPSButton_Click);
            // 
            // label4
            // 
            this.label4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(8, 575);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(101, 17);
            this.label4.TabIndex = 11;
            this.label4.Text = "Step 5: Deploy";
            // 
            // dmpsList
            // 
            this.dmpsList.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dmpsList.CheckOnClick = true;
            this.dmpsList.ColumnWidth = 350;
            this.dmpsList.FormattingEnabled = true;
            this.dmpsList.Location = new System.Drawing.Point(9, 229);
            this.dmpsList.MultiColumn = true;
            this.dmpsList.Name = "dmpsList";
            this.dmpsList.Size = new System.Drawing.Size(936, 259);
            this.dmpsList.TabIndex = 10;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(8, 172);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(334, 17);
            this.label3.TabIndex = 8;
            this.label3.Text = "Step 4: Select which DMPS Units you wish to deploy\r\n";
            // 
            // browseSPZfileButton
            // 
            this.browseSPZfileButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.browseSPZfileButton.Location = new System.Drawing.Point(870, 69);
            this.browseSPZfileButton.Name = "browseSPZfileButton";
            this.browseSPZfileButton.Size = new System.Drawing.Size(75, 27);
            this.browseSPZfileButton.TabIndex = 7;
            this.browseSPZfileButton.Text = "Browse...";
            this.browseSPZfileButton.UseVisualStyleBackColor = true;
            this.browseSPZfileButton.Click += new System.EventHandler(this.browseSPZfileButton_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(8, 21);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(427, 17);
            this.label1.TabIndex = 4;
            this.label1.Text = "Step 1: Ensure that Crestron Toolbox is installed on your computer";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(8, 53);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(275, 17);
            this.label2.TabIndex = 5;
            this.label2.Text = "Step 2: Choose the location of the SPZ file";
            // 
            // spzFileLocation
            // 
            this.spzFileLocation.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.spzFileLocation.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.spzFileLocation.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.FileSystem;
            this.spzFileLocation.Location = new System.Drawing.Point(9, 73);
            this.spzFileLocation.Name = "spzFileLocation";
            this.spzFileLocation.Size = new System.Drawing.Size(855, 22);
            this.spzFileLocation.TabIndex = 6;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.button1);
            this.tabPage2.Location = new System.Drawing.Point(4, 25);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(953, 608);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "tabPage2";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(22, 23);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 0;
            this.button1.Text = "button1";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(961, 637);
            this.Controls.Add(this.tabControl1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Form1";
            this.Text = "DMPS Mass Deployment Tool";
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.tabPage2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.Button browseSPZfileButton;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox spzFileLocation;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button addDMPSUnit;
        private System.Windows.Forms.TextBox addIPAddressBox;
        private System.Windows.Forms.TextBox addHostNameBox;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button deployToDMPSButton;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.CheckedListBox dmpsList;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button loadListFromELKButton;
        private System.Windows.Forms.TextBox filterText;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Button applyFilterButton;
        private System.Windows.Forms.Button browseVTZFileButton;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox vtzFileLocation;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.Button button1;
    }
}

