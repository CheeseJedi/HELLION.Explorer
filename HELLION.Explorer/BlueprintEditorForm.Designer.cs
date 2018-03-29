namespace HELLION.Explorer
{
    partial class BlueprintEditorForm
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
            this.treeViewHierarchy = new System.Windows.Forms.TreeView();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.blueprintToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.newToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.newFromClipboardToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveAsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.propertiesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.closeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.viewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.expandAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.collapseAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.toolPaneToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.groupBoxAddStructure = new System.Windows.Forms.GroupBox();
            this.buttonAddStructure = new System.Windows.Forms.Button();
            this.comboBoxStructureList = new System.Windows.Forms.ComboBox();
            this.pictureBoxSelectedStructure = new System.Windows.Forms.PictureBox();
            this.label1 = new System.Windows.Forms.Label();
            this.labelSelectedStructureType = new System.Windows.Forms.Label();
            this.groupBoxRemoveStructure = new System.Windows.Forms.GroupBox();
            this.buttonRemoveStructure = new System.Windows.Forms.Button();
            this.groupBoxUndockPort = new System.Windows.Forms.GroupBox();
            this.buttonUndockPort = new System.Windows.Forms.Button();
            this.groupBoxDockPort = new System.Windows.Forms.GroupBox();
            this.buttonDockPort = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.comboBoxDockingDestinationPort = new System.Windows.Forms.ComboBox();
            this.comboBoxDockingDestinationSource = new System.Windows.Forms.ComboBox();
            this.comboBoxDockingDestinationStructure = new System.Windows.Forms.ComboBox();
            this.comboBoxDockingSourcePort = new System.Windows.Forms.ComboBox();
            this.panelToolPanel = new System.Windows.Forms.Panel();
            this.menuStrip1.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.groupBoxAddStructure.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxSelectedStructure)).BeginInit();
            this.groupBoxRemoveStructure.SuspendLayout();
            this.groupBoxUndockPort.SuspendLayout();
            this.groupBoxDockPort.SuspendLayout();
            this.panelToolPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // treeViewHierarchy
            // 
            this.treeViewHierarchy.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.treeViewHierarchy.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.treeViewHierarchy.HideSelection = false;
            this.treeViewHierarchy.Location = new System.Drawing.Point(0, 24);
            this.treeViewHierarchy.Name = "treeViewHierarchy";
            this.treeViewHierarchy.Size = new System.Drawing.Size(701, 640);
            this.treeViewHierarchy.TabIndex = 0;
            this.treeViewHierarchy.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeViewHierarchy_AfterSelect);
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.blueprintToolStripMenuItem,
            this.viewToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(899, 24);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // blueprintToolStripMenuItem
            // 
            this.blueprintToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newToolStripMenuItem,
            this.newFromClipboardToolStripMenuItem,
            this.toolStripSeparator1,
            this.saveToolStripMenuItem,
            this.saveAsToolStripMenuItem,
            this.toolStripSeparator2,
            this.propertiesToolStripMenuItem,
            this.closeToolStripMenuItem});
            this.blueprintToolStripMenuItem.Name = "blueprintToolStripMenuItem";
            this.blueprintToolStripMenuItem.Size = new System.Drawing.Size(67, 20);
            this.blueprintToolStripMenuItem.Text = "Blueprint";
            // 
            // newToolStripMenuItem
            // 
            this.newToolStripMenuItem.Enabled = false;
            this.newToolStripMenuItem.Name = "newToolStripMenuItem";
            this.newToolStripMenuItem.Size = new System.Drawing.Size(191, 22);
            this.newToolStripMenuItem.Text = "New";
            // 
            // newFromClipboardToolStripMenuItem
            // 
            this.newFromClipboardToolStripMenuItem.Enabled = false;
            this.newFromClipboardToolStripMenuItem.Name = "newFromClipboardToolStripMenuItem";
            this.newFromClipboardToolStripMenuItem.Size = new System.Drawing.Size(191, 22);
            this.newFromClipboardToolStripMenuItem.Text = "New from Clipboard...";
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(188, 6);
            // 
            // saveToolStripMenuItem
            // 
            this.saveToolStripMenuItem.Enabled = false;
            this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            this.saveToolStripMenuItem.Size = new System.Drawing.Size(191, 22);
            this.saveToolStripMenuItem.Text = "Save";
            // 
            // saveAsToolStripMenuItem
            // 
            this.saveAsToolStripMenuItem.Enabled = false;
            this.saveAsToolStripMenuItem.Name = "saveAsToolStripMenuItem";
            this.saveAsToolStripMenuItem.Size = new System.Drawing.Size(191, 22);
            this.saveAsToolStripMenuItem.Text = "Save As...";
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(188, 6);
            // 
            // propertiesToolStripMenuItem
            // 
            this.propertiesToolStripMenuItem.Name = "propertiesToolStripMenuItem";
            this.propertiesToolStripMenuItem.Size = new System.Drawing.Size(191, 22);
            this.propertiesToolStripMenuItem.Text = "Properties";
            this.propertiesToolStripMenuItem.Click += new System.EventHandler(this.propertiesToolStripMenuItem_Click);
            // 
            // closeToolStripMenuItem
            // 
            this.closeToolStripMenuItem.Name = "closeToolStripMenuItem";
            this.closeToolStripMenuItem.Size = new System.Drawing.Size(191, 22);
            this.closeToolStripMenuItem.Text = "Close";
            this.closeToolStripMenuItem.Click += new System.EventHandler(this.closeToolStripMenuItem_Click);
            // 
            // viewToolStripMenuItem
            // 
            this.viewToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.expandAllToolStripMenuItem,
            this.collapseAllToolStripMenuItem,
            this.toolStripSeparator3,
            this.toolPaneToolStripMenuItem});
            this.viewToolStripMenuItem.Name = "viewToolStripMenuItem";
            this.viewToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
            this.viewToolStripMenuItem.Text = "View";
            // 
            // expandAllToolStripMenuItem
            // 
            this.expandAllToolStripMenuItem.Name = "expandAllToolStripMenuItem";
            this.expandAllToolStripMenuItem.Size = new System.Drawing.Size(136, 22);
            this.expandAllToolStripMenuItem.Text = "Expand All";
            this.expandAllToolStripMenuItem.Click += new System.EventHandler(this.expandAllToolStripMenuItem_Click);
            // 
            // collapseAllToolStripMenuItem
            // 
            this.collapseAllToolStripMenuItem.Name = "collapseAllToolStripMenuItem";
            this.collapseAllToolStripMenuItem.Size = new System.Drawing.Size(136, 22);
            this.collapseAllToolStripMenuItem.Text = "Collapse All";
            this.collapseAllToolStripMenuItem.Click += new System.EventHandler(this.collapseAllToolStripMenuItem_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(133, 6);
            // 
            // toolPaneToolStripMenuItem
            // 
            this.toolPaneToolStripMenuItem.Checked = true;
            this.toolPaneToolStripMenuItem.CheckOnClick = true;
            this.toolPaneToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.toolPaneToolStripMenuItem.Name = "toolPaneToolStripMenuItem";
            this.toolPaneToolStripMenuItem.Size = new System.Drawing.Size(136, 22);
            this.toolPaneToolStripMenuItem.Text = "Tool Pane";
            this.toolPaneToolStripMenuItem.Click += new System.EventHandler(this.toolPaneToolStripMenuItem_Click);
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel1});
            this.statusStrip1.Location = new System.Drawing.Point(0, 664);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(899, 22);
            this.statusStrip1.TabIndex = 1;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(0, 17);
            // 
            // groupBoxAddStructure
            // 
            this.groupBoxAddStructure.Controls.Add(this.buttonAddStructure);
            this.groupBoxAddStructure.Controls.Add(this.comboBoxStructureList);
            this.groupBoxAddStructure.Location = new System.Drawing.Point(8, 3);
            this.groupBoxAddStructure.Name = "groupBoxAddStructure";
            this.groupBoxAddStructure.Size = new System.Drawing.Size(182, 82);
            this.groupBoxAddStructure.TabIndex = 3;
            this.groupBoxAddStructure.TabStop = false;
            this.groupBoxAddStructure.Text = "Add Structure";
            // 
            // buttonAddStructure
            // 
            this.buttonAddStructure.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonAddStructure.Enabled = false;
            this.buttonAddStructure.Location = new System.Drawing.Point(6, 48);
            this.buttonAddStructure.Name = "buttonAddStructure";
            this.buttonAddStructure.Size = new System.Drawing.Size(170, 24);
            this.buttonAddStructure.TabIndex = 2;
            this.buttonAddStructure.Text = "Add";
            this.buttonAddStructure.UseVisualStyleBackColor = true;
            this.buttonAddStructure.Click += new System.EventHandler(this.buttonAddStructure_Click);
            // 
            // comboBoxStructureList
            // 
            this.comboBoxStructureList.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.comboBoxStructureList.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxStructureList.FormattingEnabled = true;
            this.comboBoxStructureList.Location = new System.Drawing.Point(7, 20);
            this.comboBoxStructureList.Name = "comboBoxStructureList";
            this.comboBoxStructureList.Size = new System.Drawing.Size(169, 21);
            this.comboBoxStructureList.TabIndex = 1;
            this.comboBoxStructureList.SelectedIndexChanged += new System.EventHandler(this.comboBoxStructureList_SelectedIndexChanged);
            // 
            // pictureBoxSelectedStructure
            // 
            this.pictureBoxSelectedStructure.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.pictureBoxSelectedStructure.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pictureBoxSelectedStructure.Location = new System.Drawing.Point(10, 126);
            this.pictureBoxSelectedStructure.Name = "pictureBoxSelectedStructure";
            this.pictureBoxSelectedStructure.Size = new System.Drawing.Size(180, 180);
            this.pictureBoxSelectedStructure.TabIndex = 4;
            this.pictureBoxSelectedStructure.TabStop = false;
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(9, 88);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(181, 16);
            this.label1.TabIndex = 5;
            this.label1.Text = "Selected Structure:";
            // 
            // labelSelectedStructureType
            // 
            this.labelSelectedStructureType.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelSelectedStructureType.Location = new System.Drawing.Point(9, 103);
            this.labelSelectedStructureType.Name = "labelSelectedStructureType";
            this.labelSelectedStructureType.Size = new System.Drawing.Size(181, 20);
            this.labelSelectedStructureType.TabIndex = 5;
            this.labelSelectedStructureType.Text = "Unspecified";
            this.labelSelectedStructureType.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // groupBoxRemoveStructure
            // 
            this.groupBoxRemoveStructure.Controls.Add(this.buttonRemoveStructure);
            this.groupBoxRemoveStructure.Location = new System.Drawing.Point(10, 312);
            this.groupBoxRemoveStructure.Name = "groupBoxRemoveStructure";
            this.groupBoxRemoveStructure.Size = new System.Drawing.Size(180, 51);
            this.groupBoxRemoveStructure.TabIndex = 5;
            this.groupBoxRemoveStructure.TabStop = false;
            this.groupBoxRemoveStructure.Text = "Remove Structure";
            // 
            // buttonRemoveStructure
            // 
            this.buttonRemoveStructure.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonRemoveStructure.Enabled = false;
            this.buttonRemoveStructure.Location = new System.Drawing.Point(5, 19);
            this.buttonRemoveStructure.Name = "buttonRemoveStructure";
            this.buttonRemoveStructure.Size = new System.Drawing.Size(170, 24);
            this.buttonRemoveStructure.TabIndex = 1;
            this.buttonRemoveStructure.Text = "Remove";
            this.buttonRemoveStructure.UseVisualStyleBackColor = true;
            this.buttonRemoveStructure.Click += new System.EventHandler(this.buttonRemoveStructure_Click);
            // 
            // groupBoxUndockPort
            // 
            this.groupBoxUndockPort.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.groupBoxUndockPort.Controls.Add(this.buttonUndockPort);
            this.groupBoxUndockPort.Location = new System.Drawing.Point(10, 582);
            this.groupBoxUndockPort.Name = "groupBoxUndockPort";
            this.groupBoxUndockPort.Size = new System.Drawing.Size(180, 51);
            this.groupBoxUndockPort.TabIndex = 4;
            this.groupBoxUndockPort.TabStop = false;
            this.groupBoxUndockPort.Text = "Undock Port";
            // 
            // buttonUndockPort
            // 
            this.buttonUndockPort.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonUndockPort.Enabled = false;
            this.buttonUndockPort.Location = new System.Drawing.Point(4, 19);
            this.buttonUndockPort.Name = "buttonUndockPort";
            this.buttonUndockPort.Size = new System.Drawing.Size(169, 24);
            this.buttonUndockPort.TabIndex = 1;
            this.buttonUndockPort.Text = "Undock";
            this.buttonUndockPort.UseVisualStyleBackColor = true;
            this.buttonUndockPort.Click += new System.EventHandler(this.buttonUndockPort_Click);
            // 
            // groupBoxDockPort
            // 
            this.groupBoxDockPort.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.groupBoxDockPort.Controls.Add(this.buttonDockPort);
            this.groupBoxDockPort.Controls.Add(this.label3);
            this.groupBoxDockPort.Controls.Add(this.label2);
            this.groupBoxDockPort.Controls.Add(this.comboBoxDockingDestinationPort);
            this.groupBoxDockPort.Controls.Add(this.comboBoxDockingDestinationSource);
            this.groupBoxDockPort.Controls.Add(this.comboBoxDockingDestinationStructure);
            this.groupBoxDockPort.Controls.Add(this.comboBoxDockingSourcePort);
            this.groupBoxDockPort.Location = new System.Drawing.Point(10, 376);
            this.groupBoxDockPort.Name = "groupBoxDockPort";
            this.groupBoxDockPort.Size = new System.Drawing.Size(180, 200);
            this.groupBoxDockPort.TabIndex = 3;
            this.groupBoxDockPort.TabStop = false;
            this.groupBoxDockPort.Text = "Dock Port";
            // 
            // buttonDockPort
            // 
            this.buttonDockPort.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonDockPort.Enabled = false;
            this.buttonDockPort.Location = new System.Drawing.Point(6, 170);
            this.buttonDockPort.Name = "buttonDockPort";
            this.buttonDockPort.Size = new System.Drawing.Size(169, 24);
            this.buttonDockPort.TabIndex = 1;
            this.buttonDockPort.Text = "Dock";
            this.buttonDockPort.UseVisualStyleBackColor = true;
            this.buttonDockPort.Click += new System.EventHandler(this.buttonDockPort_Click);
            // 
            // label3
            // 
            this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label3.Location = new System.Drawing.Point(6, 115);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(169, 16);
            this.label3.TabIndex = 5;
            this.label3.Text = "Docking Port";
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label2.Location = new System.Drawing.Point(4, 45);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(169, 16);
            this.label2.TabIndex = 5;
            this.label2.Text = "To Structure";
            // 
            // comboBoxDockingDestinationPort
            // 
            this.comboBoxDockingDestinationPort.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.comboBoxDockingDestinationPort.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxDockingDestinationPort.FormattingEnabled = true;
            this.comboBoxDockingDestinationPort.Location = new System.Drawing.Point(6, 134);
            this.comboBoxDockingDestinationPort.Name = "comboBoxDockingDestinationPort";
            this.comboBoxDockingDestinationPort.Size = new System.Drawing.Size(169, 21);
            this.comboBoxDockingDestinationPort.TabIndex = 0;
            this.comboBoxDockingDestinationPort.SelectedIndexChanged += new System.EventHandler(this.comboBoxDockingDestinationPort_SelectedIndexChanged);
            // 
            // comboBoxDockingDestinationSource
            // 
            this.comboBoxDockingDestinationSource.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.comboBoxDockingDestinationSource.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxDockingDestinationSource.FormattingEnabled = true;
            this.comboBoxDockingDestinationSource.Location = new System.Drawing.Point(5, 64);
            this.comboBoxDockingDestinationSource.Name = "comboBoxDockingDestinationSource";
            this.comboBoxDockingDestinationSource.Size = new System.Drawing.Size(169, 21);
            this.comboBoxDockingDestinationSource.TabIndex = 0;
            this.comboBoxDockingDestinationSource.SelectedIndexChanged += new System.EventHandler(this.comboBoxDockingDestinationSource_SelectedIndexChanged);
            // 
            // comboBoxDockingDestinationStructure
            // 
            this.comboBoxDockingDestinationStructure.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.comboBoxDockingDestinationStructure.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxDockingDestinationStructure.FormattingEnabled = true;
            this.comboBoxDockingDestinationStructure.Location = new System.Drawing.Point(5, 91);
            this.comboBoxDockingDestinationStructure.Name = "comboBoxDockingDestinationStructure";
            this.comboBoxDockingDestinationStructure.Size = new System.Drawing.Size(169, 21);
            this.comboBoxDockingDestinationStructure.TabIndex = 0;
            this.comboBoxDockingDestinationStructure.SelectedIndexChanged += new System.EventHandler(this.comboBoxDockingDestinationStructure_SelectedIndexChanged);
            // 
            // comboBoxDockingSourcePort
            // 
            this.comboBoxDockingSourcePort.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.comboBoxDockingSourcePort.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxDockingSourcePort.FormattingEnabled = true;
            this.comboBoxDockingSourcePort.Location = new System.Drawing.Point(4, 19);
            this.comboBoxDockingSourcePort.Name = "comboBoxDockingSourcePort";
            this.comboBoxDockingSourcePort.Size = new System.Drawing.Size(169, 21);
            this.comboBoxDockingSourcePort.TabIndex = 0;
            this.comboBoxDockingSourcePort.SelectedIndexChanged += new System.EventHandler(this.comboBoxDockingSourcePort_SelectedIndexChanged);
            // 
            // panelToolPanel
            // 
            this.panelToolPanel.Controls.Add(this.groupBoxAddStructure);
            this.panelToolPanel.Controls.Add(this.labelSelectedStructureType);
            this.panelToolPanel.Controls.Add(this.groupBoxDockPort);
            this.panelToolPanel.Controls.Add(this.label1);
            this.panelToolPanel.Controls.Add(this.groupBoxRemoveStructure);
            this.panelToolPanel.Controls.Add(this.pictureBoxSelectedStructure);
            this.panelToolPanel.Controls.Add(this.groupBoxUndockPort);
            this.panelToolPanel.Dock = System.Windows.Forms.DockStyle.Right;
            this.panelToolPanel.Location = new System.Drawing.Point(699, 24);
            this.panelToolPanel.Name = "panelToolPanel";
            this.panelToolPanel.Size = new System.Drawing.Size(200, 640);
            this.panelToolPanel.TabIndex = 6;
            // 
            // BlueprintEditorForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(899, 686);
            this.Controls.Add(this.panelToolPanel);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.treeViewHierarchy);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.MinimumSize = new System.Drawing.Size(700, 725);
            this.Name = "BlueprintEditorForm";
            this.Text = "BlueprintEditorForm";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.BlueprintEditorForm_FormClosing);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.groupBoxAddStructure.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxSelectedStructure)).EndInit();
            this.groupBoxRemoveStructure.ResumeLayout(false);
            this.groupBoxUndockPort.ResumeLayout(false);
            this.groupBoxDockPort.ResumeLayout(false);
            this.panelToolPanel.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TreeView treeViewHierarchy;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private System.Windows.Forms.ToolStripMenuItem blueprintToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem newToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem closeToolStripMenuItem;
        private System.Windows.Forms.GroupBox groupBoxAddStructure;
        private System.Windows.Forms.Button buttonAddStructure;
        private System.Windows.Forms.ComboBox comboBoxStructureList;
        private System.Windows.Forms.PictureBox pictureBoxSelectedStructure;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label labelSelectedStructureType;
        private System.Windows.Forms.GroupBox groupBoxRemoveStructure;
        private System.Windows.Forms.Button buttonRemoveStructure;
        private System.Windows.Forms.GroupBox groupBoxUndockPort;
        private System.Windows.Forms.Button buttonUndockPort;
        private System.Windows.Forms.GroupBox groupBoxDockPort;
        private System.Windows.Forms.Button buttonDockPort;
        private System.Windows.Forms.ComboBox comboBoxDockingSourcePort;
        private System.Windows.Forms.ComboBox comboBoxDockingDestinationPort;
        private System.Windows.Forms.ComboBox comboBoxDockingDestinationStructure;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ToolStripMenuItem viewToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem expandAllToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem collapseAllToolStripMenuItem;
        private System.Windows.Forms.ComboBox comboBoxDockingDestinationSource;
        private System.Windows.Forms.Panel panelToolPanel;
        private System.Windows.Forms.ToolStripMenuItem toolPaneToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem saveAsToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem propertiesToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripMenuItem newFromClipboardToolStripMenuItem;
        private System.Windows.Forms.Label label3;
    }
}