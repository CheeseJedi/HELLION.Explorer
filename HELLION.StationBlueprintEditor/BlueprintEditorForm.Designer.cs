namespace HELLION.StationBlueprintEditor
{
    partial class StationBlueprintEditorForm
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
            this.secondaryStructuresPaneToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.groupBoxAddStructure = new System.Windows.Forms.GroupBox();
            this.buttonAddStructure = new System.Windows.Forms.Button();
            this.comboBoxStructureList = new System.Windows.Forms.ComboBox();
            this.pictureBoxSelectedPrimaryStructure = new System.Windows.Forms.PictureBox();
            this.label1 = new System.Windows.Forms.Label();
            this.labelSelectedPrimaryStructure = new System.Windows.Forms.Label();
            this.groupBoxRemoveStructure = new System.Windows.Forms.GroupBox();
            this.buttonRemoveStructure = new System.Windows.Forms.Button();
            this.buttonUndockPort = new System.Windows.Forms.Button();
            this.buttonDockPort = new System.Windows.Forms.Button();
            this.panelToolPanel = new System.Windows.Forms.Panel();
            this.labelSelectedSecondaryDockingPort = new System.Windows.Forms.Label();
            this.labelSelectedPrimaryDockingPort = new System.Windows.Forms.Label();
            this.labelSelectedSecondaryStructure = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.pictureBoxSelectedSecondaryStructure = new System.Windows.Forms.PictureBox();
            this.splitContainerTreeViews = new System.Windows.Forms.SplitContainer();
            this.treeViewPrimaryStructure = new System.Windows.Forms.TreeView();
            this.labelSecondaryStructuresPane = new System.Windows.Forms.Label();
            this.treeViewSecondaryStructures = new System.Windows.Forms.TreeView();
            this.menuStrip1.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.groupBoxAddStructure.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxSelectedPrimaryStructure)).BeginInit();
            this.groupBoxRemoveStructure.SuspendLayout();
            this.panelToolPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxSelectedSecondaryStructure)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerTreeViews)).BeginInit();
            this.splitContainerTreeViews.Panel1.SuspendLayout();
            this.splitContainerTreeViews.Panel2.SuspendLayout();
            this.splitContainerTreeViews.SuspendLayout();
            this.SuspendLayout();
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
            this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            this.saveToolStripMenuItem.Size = new System.Drawing.Size(191, 22);
            this.saveToolStripMenuItem.Text = "Save";
            this.saveToolStripMenuItem.Click += new System.EventHandler(this.saveToolStripMenuItem_Click);
            // 
            // saveAsToolStripMenuItem
            // 
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
            this.propertiesToolStripMenuItem.Visible = false;
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
            this.toolPaneToolStripMenuItem,
            this.secondaryStructuresPaneToolStripMenuItem});
            this.viewToolStripMenuItem.Name = "viewToolStripMenuItem";
            this.viewToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
            this.viewToolStripMenuItem.Text = "View";
            // 
            // expandAllToolStripMenuItem
            // 
            this.expandAllToolStripMenuItem.Name = "expandAllToolStripMenuItem";
            this.expandAllToolStripMenuItem.Size = new System.Drawing.Size(214, 22);
            this.expandAllToolStripMenuItem.Text = "Expand All";
            this.expandAllToolStripMenuItem.Click += new System.EventHandler(this.expandAllToolStripMenuItem_Click);
            // 
            // collapseAllToolStripMenuItem
            // 
            this.collapseAllToolStripMenuItem.Name = "collapseAllToolStripMenuItem";
            this.collapseAllToolStripMenuItem.Size = new System.Drawing.Size(214, 22);
            this.collapseAllToolStripMenuItem.Text = "Collapse All";
            this.collapseAllToolStripMenuItem.Click += new System.EventHandler(this.collapseAllToolStripMenuItem_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(211, 6);
            // 
            // toolPaneToolStripMenuItem
            // 
            this.toolPaneToolStripMenuItem.Checked = true;
            this.toolPaneToolStripMenuItem.CheckOnClick = true;
            this.toolPaneToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.toolPaneToolStripMenuItem.Name = "toolPaneToolStripMenuItem";
            this.toolPaneToolStripMenuItem.Size = new System.Drawing.Size(214, 22);
            this.toolPaneToolStripMenuItem.Text = "Tool Pane";
            this.toolPaneToolStripMenuItem.Click += new System.EventHandler(this.toolPaneToolStripMenuItem_Click);
            // 
            // secondaryStructuresPaneToolStripMenuItem
            // 
            this.secondaryStructuresPaneToolStripMenuItem.Checked = true;
            this.secondaryStructuresPaneToolStripMenuItem.CheckOnClick = true;
            this.secondaryStructuresPaneToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.secondaryStructuresPaneToolStripMenuItem.Name = "secondaryStructuresPaneToolStripMenuItem";
            this.secondaryStructuresPaneToolStripMenuItem.Size = new System.Drawing.Size(214, 22);
            this.secondaryStructuresPaneToolStripMenuItem.Text = "Secondary Structures Pane";
            this.secondaryStructuresPaneToolStripMenuItem.Click += new System.EventHandler(this.secondaryStructuresPaneToolStripMenuItem_Click);
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel1});
            this.statusStrip1.Location = new System.Drawing.Point(0, 729);
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
            this.groupBoxAddStructure.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.groupBoxAddStructure.Controls.Add(this.buttonAddStructure);
            this.groupBoxAddStructure.Controls.Add(this.comboBoxStructureList);
            this.groupBoxAddStructure.Location = new System.Drawing.Point(8, 560);
            this.groupBoxAddStructure.Name = "groupBoxAddStructure";
            this.groupBoxAddStructure.Size = new System.Drawing.Size(184, 82);
            this.groupBoxAddStructure.TabIndex = 3;
            this.groupBoxAddStructure.TabStop = false;
            this.groupBoxAddStructure.Text = "Add Structure";
            // 
            // buttonAddStructure
            // 
            this.buttonAddStructure.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonAddStructure.Enabled = false;
            this.buttonAddStructure.Location = new System.Drawing.Point(7, 48);
            this.buttonAddStructure.Name = "buttonAddStructure";
            this.buttonAddStructure.Size = new System.Drawing.Size(172, 24);
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
            this.comboBoxStructureList.Size = new System.Drawing.Size(172, 21);
            this.comboBoxStructureList.TabIndex = 1;
            this.comboBoxStructureList.SelectedIndexChanged += new System.EventHandler(this.comboBoxStructureList_SelectedIndexChanged);
            // 
            // pictureBoxSelectedPrimaryStructure
            // 
            this.pictureBoxSelectedPrimaryStructure.BackColor = System.Drawing.SystemColors.ActiveBorder;
            this.pictureBoxSelectedPrimaryStructure.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.pictureBoxSelectedPrimaryStructure.Location = new System.Drawing.Point(8, 38);
            this.pictureBoxSelectedPrimaryStructure.Name = "pictureBoxSelectedPrimaryStructure";
            this.pictureBoxSelectedPrimaryStructure.Padding = new System.Windows.Forms.Padding(1);
            this.pictureBoxSelectedPrimaryStructure.Size = new System.Drawing.Size(182, 182);
            this.pictureBoxSelectedPrimaryStructure.TabIndex = 4;
            this.pictureBoxSelectedPrimaryStructure.TabStop = false;
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(9, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(181, 16);
            this.label1.TabIndex = 5;
            this.label1.Text = "Selected Primary Structure / Port";
            // 
            // labelSelectedPrimaryStructure
            // 
            this.labelSelectedPrimaryStructure.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelSelectedPrimaryStructure.Location = new System.Drawing.Point(9, 15);
            this.labelSelectedPrimaryStructure.Name = "labelSelectedPrimaryStructure";
            this.labelSelectedPrimaryStructure.Size = new System.Drawing.Size(181, 20);
            this.labelSelectedPrimaryStructure.TabIndex = 5;
            this.labelSelectedPrimaryStructure.Text = "Unspecified";
            this.labelSelectedPrimaryStructure.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // groupBoxRemoveStructure
            // 
            this.groupBoxRemoveStructure.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.groupBoxRemoveStructure.Controls.Add(this.buttonRemoveStructure);
            this.groupBoxRemoveStructure.Location = new System.Drawing.Point(8, 648);
            this.groupBoxRemoveStructure.Name = "groupBoxRemoveStructure";
            this.groupBoxRemoveStructure.Size = new System.Drawing.Size(182, 51);
            this.groupBoxRemoveStructure.TabIndex = 5;
            this.groupBoxRemoveStructure.TabStop = false;
            this.groupBoxRemoveStructure.Text = "Remove Structure";
            // 
            // buttonRemoveStructure
            // 
            this.buttonRemoveStructure.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonRemoveStructure.Enabled = false;
            this.buttonRemoveStructure.Location = new System.Drawing.Point(7, 19);
            this.buttonRemoveStructure.Name = "buttonRemoveStructure";
            this.buttonRemoveStructure.Size = new System.Drawing.Size(170, 24);
            this.buttonRemoveStructure.TabIndex = 1;
            this.buttonRemoveStructure.Text = "Remove";
            this.buttonRemoveStructure.UseVisualStyleBackColor = true;
            this.buttonRemoveStructure.Click += new System.EventHandler(this.buttonRemoveStructure_Click);
            // 
            // buttonUndockPort
            // 
            this.buttonUndockPort.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonUndockPort.Enabled = false;
            this.buttonUndockPort.Location = new System.Drawing.Point(8, 246);
            this.buttonUndockPort.Name = "buttonUndockPort";
            this.buttonUndockPort.Size = new System.Drawing.Size(182, 24);
            this.buttonUndockPort.TabIndex = 1;
            this.buttonUndockPort.Text = "Undock";
            this.buttonUndockPort.UseVisualStyleBackColor = true;
            this.buttonUndockPort.Click += new System.EventHandler(this.buttonUndockPort_Click);
            // 
            // buttonDockPort
            // 
            this.buttonDockPort.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonDockPort.Enabled = false;
            this.buttonDockPort.Location = new System.Drawing.Point(8, 525);
            this.buttonDockPort.Name = "buttonDockPort";
            this.buttonDockPort.Size = new System.Drawing.Size(183, 24);
            this.buttonDockPort.TabIndex = 1;
            this.buttonDockPort.Text = "Dock";
            this.buttonDockPort.UseVisualStyleBackColor = true;
            this.buttonDockPort.Click += new System.EventHandler(this.buttonDockPort_Click);
            // 
            // panelToolPanel
            // 
            this.panelToolPanel.Controls.Add(this.buttonUndockPort);
            this.panelToolPanel.Controls.Add(this.buttonDockPort);
            this.panelToolPanel.Controls.Add(this.groupBoxAddStructure);
            this.panelToolPanel.Controls.Add(this.labelSelectedSecondaryDockingPort);
            this.panelToolPanel.Controls.Add(this.labelSelectedPrimaryDockingPort);
            this.panelToolPanel.Controls.Add(this.labelSelectedSecondaryStructure);
            this.panelToolPanel.Controls.Add(this.labelSelectedPrimaryStructure);
            this.panelToolPanel.Controls.Add(this.label2);
            this.panelToolPanel.Controls.Add(this.label1);
            this.panelToolPanel.Controls.Add(this.pictureBoxSelectedSecondaryStructure);
            this.panelToolPanel.Controls.Add(this.groupBoxRemoveStructure);
            this.panelToolPanel.Controls.Add(this.pictureBoxSelectedPrimaryStructure);
            this.panelToolPanel.Dock = System.Windows.Forms.DockStyle.Right;
            this.panelToolPanel.Location = new System.Drawing.Point(699, 24);
            this.panelToolPanel.Name = "panelToolPanel";
            this.panelToolPanel.Size = new System.Drawing.Size(200, 705);
            this.panelToolPanel.TabIndex = 6;
            // 
            // labelSelectedSecondaryDockingPort
            // 
            this.labelSelectedSecondaryDockingPort.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.labelSelectedSecondaryDockingPort.AutoEllipsis = true;
            this.labelSelectedSecondaryDockingPort.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelSelectedSecondaryDockingPort.Location = new System.Drawing.Point(8, 502);
            this.labelSelectedSecondaryDockingPort.Name = "labelSelectedSecondaryDockingPort";
            this.labelSelectedSecondaryDockingPort.Size = new System.Drawing.Size(182, 20);
            this.labelSelectedSecondaryDockingPort.TabIndex = 5;
            this.labelSelectedSecondaryDockingPort.Text = "Unspecified";
            this.labelSelectedSecondaryDockingPort.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // labelSelectedPrimaryDockingPort
            // 
            this.labelSelectedPrimaryDockingPort.AutoEllipsis = true;
            this.labelSelectedPrimaryDockingPort.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelSelectedPrimaryDockingPort.Location = new System.Drawing.Point(6, 223);
            this.labelSelectedPrimaryDockingPort.Name = "labelSelectedPrimaryDockingPort";
            this.labelSelectedPrimaryDockingPort.Size = new System.Drawing.Size(184, 20);
            this.labelSelectedPrimaryDockingPort.TabIndex = 5;
            this.labelSelectedPrimaryDockingPort.Text = "Unspecified";
            this.labelSelectedPrimaryDockingPort.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // labelSelectedSecondaryStructure
            // 
            this.labelSelectedSecondaryStructure.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.labelSelectedSecondaryStructure.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelSelectedSecondaryStructure.Location = new System.Drawing.Point(9, 294);
            this.labelSelectedSecondaryStructure.Name = "labelSelectedSecondaryStructure";
            this.labelSelectedSecondaryStructure.Size = new System.Drawing.Size(181, 20);
            this.labelSelectedSecondaryStructure.TabIndex = 5;
            this.labelSelectedSecondaryStructure.Text = "Unspecified";
            this.labelSelectedSecondaryStructure.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.label2.Location = new System.Drawing.Point(9, 278);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(181, 16);
            this.label2.TabIndex = 5;
            this.label2.Text = "Selected Secondary Structure / Port";
            // 
            // pictureBoxSelectedSecondaryStructure
            // 
            this.pictureBoxSelectedSecondaryStructure.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.pictureBoxSelectedSecondaryStructure.BackColor = System.Drawing.SystemColors.ActiveBorder;
            this.pictureBoxSelectedSecondaryStructure.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.pictureBoxSelectedSecondaryStructure.Location = new System.Drawing.Point(8, 317);
            this.pictureBoxSelectedSecondaryStructure.Name = "pictureBoxSelectedSecondaryStructure";
            this.pictureBoxSelectedSecondaryStructure.Padding = new System.Windows.Forms.Padding(1);
            this.pictureBoxSelectedSecondaryStructure.Size = new System.Drawing.Size(182, 182);
            this.pictureBoxSelectedSecondaryStructure.TabIndex = 4;
            this.pictureBoxSelectedSecondaryStructure.TabStop = false;
            // 
            // splitContainerTreeViews
            // 
            this.splitContainerTreeViews.BackColor = System.Drawing.SystemColors.ControlLight;
            this.splitContainerTreeViews.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainerTreeViews.Location = new System.Drawing.Point(0, 24);
            this.splitContainerTreeViews.Name = "splitContainerTreeViews";
            this.splitContainerTreeViews.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainerTreeViews.Panel1
            // 
            this.splitContainerTreeViews.Panel1.Controls.Add(this.treeViewPrimaryStructure);
            // 
            // splitContainerTreeViews.Panel2
            // 
            this.splitContainerTreeViews.Panel2.Controls.Add(this.labelSecondaryStructuresPane);
            this.splitContainerTreeViews.Panel2.Controls.Add(this.treeViewSecondaryStructures);
            this.splitContainerTreeViews.Size = new System.Drawing.Size(699, 705);
            this.splitContainerTreeViews.SplitterDistance = 605;
            this.splitContainerTreeViews.TabIndex = 7;
            // 
            // treeViewPrimaryStructure
            // 
            this.treeViewPrimaryStructure.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.treeViewPrimaryStructure.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.treeViewPrimaryStructure.HideSelection = false;
            this.treeViewPrimaryStructure.Location = new System.Drawing.Point(0, 0);
            this.treeViewPrimaryStructure.Name = "treeViewPrimaryStructure";
            this.treeViewPrimaryStructure.Size = new System.Drawing.Size(699, 605);
            this.treeViewPrimaryStructure.TabIndex = 1;
            this.treeViewPrimaryStructure.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeViewPrimaryStructure_AfterSelect);
            // 
            // labelSecondaryStructuresPane
            // 
            this.labelSecondaryStructuresPane.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.labelSecondaryStructuresPane.BackColor = System.Drawing.SystemColors.Control;
            this.labelSecondaryStructuresPane.Location = new System.Drawing.Point(0, 0);
            this.labelSecondaryStructuresPane.Name = "labelSecondaryStructuresPane";
            this.labelSecondaryStructuresPane.Size = new System.Drawing.Size(699, 13);
            this.labelSecondaryStructuresPane.TabIndex = 1;
            this.labelSecondaryStructuresPane.Text = "Secondary Structures";
            // 
            // treeViewSecondaryStructures
            // 
            this.treeViewSecondaryStructures.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.treeViewSecondaryStructures.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.treeViewSecondaryStructures.HideSelection = false;
            this.treeViewSecondaryStructures.Location = new System.Drawing.Point(0, 13);
            this.treeViewSecondaryStructures.Name = "treeViewSecondaryStructures";
            this.treeViewSecondaryStructures.Size = new System.Drawing.Size(699, 83);
            this.treeViewSecondaryStructures.TabIndex = 0;
            this.treeViewSecondaryStructures.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeViewSecondaryStructures_AfterSelect);
            // 
            // StationBlueprintEditorForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(899, 751);
            this.Controls.Add(this.splitContainerTreeViews);
            this.Controls.Add(this.panelToolPanel);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.MinimumSize = new System.Drawing.Size(700, 790);
            this.Name = "StationBlueprintEditorForm";
            this.Text = "Blueprint Editor";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.BlueprintEditorForm_FormClosing);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.groupBoxAddStructure.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxSelectedPrimaryStructure)).EndInit();
            this.groupBoxRemoveStructure.ResumeLayout(false);
            this.panelToolPanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxSelectedSecondaryStructure)).EndInit();
            this.splitContainerTreeViews.Panel1.ResumeLayout(false);
            this.splitContainerTreeViews.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerTreeViews)).EndInit();
            this.splitContainerTreeViews.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
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
        private System.Windows.Forms.PictureBox pictureBoxSelectedPrimaryStructure;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label labelSelectedPrimaryStructure;
        private System.Windows.Forms.GroupBox groupBoxRemoveStructure;
        private System.Windows.Forms.Button buttonRemoveStructure;
        private System.Windows.Forms.Button buttonUndockPort;
        private System.Windows.Forms.Button buttonDockPort;
        private System.Windows.Forms.ToolStripMenuItem viewToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem expandAllToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem collapseAllToolStripMenuItem;
        private System.Windows.Forms.Panel panelToolPanel;
        private System.Windows.Forms.ToolStripMenuItem toolPaneToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem saveAsToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem propertiesToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripMenuItem newFromClipboardToolStripMenuItem;
        private System.Windows.Forms.SplitContainer splitContainerTreeViews;
        private System.Windows.Forms.TreeView treeViewPrimaryStructure;
        private System.Windows.Forms.TreeView treeViewSecondaryStructures;
        private System.Windows.Forms.Label labelSecondaryStructuresPane;
        private System.Windows.Forms.ToolStripMenuItem secondaryStructuresPaneToolStripMenuItem;
        private System.Windows.Forms.Label labelSelectedPrimaryDockingPort;
        private System.Windows.Forms.Label labelSelectedSecondaryDockingPort;
        private System.Windows.Forms.Label labelSelectedSecondaryStructure;
        private System.Windows.Forms.PictureBox pictureBoxSelectedSecondaryStructure;
        private System.Windows.Forms.Label label2;
    }
}