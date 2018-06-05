namespace RevitLookup.Snoop.Forms
{
    partial class SelectBy
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
            this.label1 = new System.Windows.Forms.Label();
            this.tbFilePath = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.bntBrowseFolder = new System.Windows.Forms.Button();
            this.tbFolderPath = new System.Windows.Forms.TextBox();
            this.btnBrowseFile = new System.Windows.Forms.Button();
            this.btnDumpInfo = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.cbxBuildingType1 = new System.Windows.Forms.ComboBox();
            this.cbxBuildingType2 = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.cbxBuildingLevel = new System.Windows.Forms.ComboBox();
            this.label5 = new System.Windows.Forms.Label();
            this.tbBuildingName = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 92);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(89, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Select rvt folders:";
            // 
            // tbFilePath
            // 
            this.tbFilePath.Location = new System.Drawing.Point(117, 118);
            this.tbFilePath.Name = "tbFilePath";
            this.tbFilePath.ReadOnly = true;
            this.tbFilePath.Size = new System.Drawing.Size(186, 20);
            this.tbFilePath.TabIndex = 2;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 121);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(79, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Select rvt files: ";
            // 
            // bntBrowseFolder
            // 
            this.bntBrowseFolder.Location = new System.Drawing.Point(309, 87);
            this.bntBrowseFolder.Name = "bntBrowseFolder";
            this.bntBrowseFolder.Size = new System.Drawing.Size(82, 23);
            this.bntBrowseFolder.TabIndex = 4;
            this.bntBrowseFolder.Text = "Browse Folder";
            this.bntBrowseFolder.UseVisualStyleBackColor = true;
            this.bntBrowseFolder.Click += new System.EventHandler(this.btnSelectFolder_Click);
            // 
            // tbFolderPath
            // 
            this.tbFolderPath.Location = new System.Drawing.Point(117, 89);
            this.tbFolderPath.Name = "tbFolderPath";
            this.tbFolderPath.ReadOnly = true;
            this.tbFolderPath.Size = new System.Drawing.Size(186, 20);
            this.tbFolderPath.TabIndex = 5;
            // 
            // btnBrowseFile
            // 
            this.btnBrowseFile.Location = new System.Drawing.Point(309, 116);
            this.btnBrowseFile.Name = "btnBrowseFile";
            this.btnBrowseFile.Size = new System.Drawing.Size(82, 23);
            this.btnBrowseFile.TabIndex = 6;
            this.btnBrowseFile.Text = "Browse File";
            this.btnBrowseFile.UseVisualStyleBackColor = true;
            this.btnBrowseFile.Click += new System.EventHandler(this.btnSelectFile_Click);
            // 
            // btnDumpInfo
            // 
            this.btnDumpInfo.Location = new System.Drawing.Point(117, 151);
            this.btnDumpInfo.Name = "btnDumpInfo";
            this.btnDumpInfo.Size = new System.Drawing.Size(82, 23);
            this.btnDumpInfo.TabIndex = 7;
            this.btnDumpInfo.Text = "Dump Info";
            this.btnDumpInfo.UseVisualStyleBackColor = true;
            this.btnDumpInfo.Click += new System.EventHandler(this.btnDumpInfo_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 35);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(102, 13);
            this.label3.TabIndex = 8;
            this.label3.Text = "Select building type:";
            // 
            // cbxBuildingType1
            // 
            this.cbxBuildingType1.FormattingEnabled = true;
            this.cbxBuildingType1.Location = new System.Drawing.Point(117, 32);
            this.cbxBuildingType1.Name = "cbxBuildingType1";
            this.cbxBuildingType1.Size = new System.Drawing.Size(82, 21);
            this.cbxBuildingType1.TabIndex = 9;
            this.cbxBuildingType1.SelectedIndexChanged += new System.EventHandler(this.cbxBuildingType1_SelectedIndexChanged);
            // 
            // cbxBuildingType2
            // 
            this.cbxBuildingType2.FormattingEnabled = true;
            this.cbxBuildingType2.Location = new System.Drawing.Point(209, 32);
            this.cbxBuildingType2.Name = "cbxBuildingType2";
            this.cbxBuildingType2.Size = new System.Drawing.Size(94, 21);
            this.cbxBuildingType2.TabIndex = 10;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(12, 62);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(104, 13);
            this.label4.TabIndex = 11;
            this.label4.Text = "Select building level:";
            // 
            // cbxBuildingLevel
            // 
            this.cbxBuildingLevel.FormattingEnabled = true;
            this.cbxBuildingLevel.Location = new System.Drawing.Point(117, 59);
            this.cbxBuildingLevel.Name = "cbxBuildingLevel";
            this.cbxBuildingLevel.Size = new System.Drawing.Size(186, 21);
            this.cbxBuildingLevel.TabIndex = 12;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(12, 7);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(78, 13);
            this.label5.TabIndex = 13;
            this.label5.Text = "Building Name:";
            // 
            // tbBuildingName
            // 
            this.tbBuildingName.Location = new System.Drawing.Point(117, 4);
            this.tbBuildingName.Name = "tbBuildingName";
            this.tbBuildingName.Size = new System.Drawing.Size(186, 20);
            this.tbBuildingName.TabIndex = 14;
            // 
            // SelectBy
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(408, 193);
            this.Controls.Add(this.tbBuildingName);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.cbxBuildingLevel);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.cbxBuildingType2);
            this.Controls.Add(this.cbxBuildingType1);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.btnDumpInfo);
            this.Controls.Add(this.btnBrowseFile);
            this.Controls.Add(this.tbFolderPath);
            this.Controls.Add(this.bntBrowseFolder);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.tbFilePath);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "SelectBy";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "SelectBy";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox tbFilePath;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button bntBrowseFolder;
        private System.Windows.Forms.TextBox tbFolderPath;
        private System.Windows.Forms.Button btnBrowseFile;
        private System.Windows.Forms.Button btnDumpInfo;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox cbxBuildingType1;
        private System.Windows.Forms.ComboBox cbxBuildingType2;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox cbxBuildingLevel;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox tbBuildingName;
    }
}