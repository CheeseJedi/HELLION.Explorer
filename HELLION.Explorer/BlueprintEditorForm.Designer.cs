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
            this.treeView1 = new System.Windows.Forms.TreeView();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.blueprintToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.newToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.closeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.structureToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.addToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripComboBox1 = new System.Windows.Forms.ToolStripComboBox();
            this.dockToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.undockToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.removeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.viewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.expandAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.collapseAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
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
            this.buttonUndock = new System.Windows.Forms.Button();
            this.groupBoxDockPort = new System.Windows.Forms.GroupBox();
            this.button1 = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.comboBoxDockingDestinationPort = new System.Windows.Forms.ComboBox();
            this.comboBoxDockingDestinationStructure = new System.Windows.Forms.ComboBox();
            this.comboBoxDockingSourcePort = new System.Windows.Forms.ComboBox();
            this.menuStrip1.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.groupBoxAddStructure.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxSelectedStructure)).BeginInit();
            this.groupBoxRemoveStructure.SuspendLayout();
            this.groupBoxUndockPort.SuspendLayout();
            this.groupBoxDockPort.SuspendLayout();
            this.SuspendLayout();
            // 
            // treeView1
            // 
            this.treeView1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.treeView1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.treeView1.HideSelection = false;
            this.treeView1.Location = new System.Drawing.Point(0, 24);
            this.treeView1.Name = "treeView1";
            this.treeView1.Size = new System.Drawing.Size(634, 585);
            this.treeView1.TabIndex = 0;
            this.treeView1.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeView1_AfterSelect);
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.blueprintToolStripMenuItem,
            this.structureToolStripMenuItem,
            this.viewToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(834, 24);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // blueprintToolStripMenuItem
            // 
            this.blueprintToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newToolStripMenuItem,
            this.saveToolStripMenuItem,
            this.closeToolStripMenuItem});
            this.blueprintToolStripMenuItem.Name = "blueprintToolStripMenuItem";
            this.blueprintToolStripMenuItem.Size = new System.Drawing.Size(67, 20);
            this.blueprintToolStripMenuItem.Text = "Blueprint";
            // 
            // newToolStripMenuItem
            // 
            this.newToolStripMenuItem.Name = "newToolStripMenuItem";
            this.newToolStripMenuItem.Size = new System.Drawing.Size(103, 22);
            this.newToolStripMenuItem.Text = "New";
            // 
            // saveToolStripMenuItem
            // 
            this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            this.saveToolStripMenuItem.Size = new System.Drawing.Size(103, 22);
            this.saveToolStripMenuItem.Text = "Save";
            // 
            // closeToolStripMenuItem
            // 
            this.closeToolStripMenuItem.Name = "closeToolStripMenuItem";
            this.closeToolStripMenuItem.Size = new System.Drawing.Size(103, 22);
            this.closeToolStripMenuItem.Text = "Close";
            // 
            // structureToolStripMenuItem
            // 
            this.structureToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.addToolStripMenuItem,
            this.dockToolStripMenuItem,
            this.undockToolStripMenuItem,
            this.removeToolStripMenuItem});
            this.structureToolStripMenuItem.Name = "structureToolStripMenuItem";
            this.structureToolStripMenuItem.Size = new System.Drawing.Size(67, 20);
            this.structureToolStripMenuItem.Text = "Structure";
            // 
            // addToolStripMenuItem
            // 
            this.addToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripComboBox1});
            this.addToolStripMenuItem.Name = "addToolStripMenuItem";
            this.addToolStripMenuItem.Size = new System.Drawing.Size(117, 22);
            this.addToolStripMenuItem.Text = "Add...";
            // 
            // toolStripComboBox1
            // 
            this.toolStripComboBox1.Name = "toolStripComboBox1";
            this.toolStripComboBox1.Size = new System.Drawing.Size(121, 23);
            this.toolStripComboBox1.SelectedIndexChanged += new System.EventHandler(this.toolStripComboBox1_SelectedIndexChanged);
            // 
            // dockToolStripMenuItem
            // 
            this.dockToolStripMenuItem.Name = "dockToolStripMenuItem";
            this.dockToolStripMenuItem.Size = new System.Drawing.Size(117, 22);
            this.dockToolStripMenuItem.Text = "Dock...";
            // 
            // undockToolStripMenuItem
            // 
            this.undockToolStripMenuItem.Name = "undockToolStripMenuItem";
            this.undockToolStripMenuItem.Size = new System.Drawing.Size(117, 22);
            this.undockToolStripMenuItem.Text = "Undock";
            // 
            // removeToolStripMenuItem
            // 
            this.removeToolStripMenuItem.Name = "removeToolStripMenuItem";
            this.removeToolStripMenuItem.Size = new System.Drawing.Size(117, 22);
            this.removeToolStripMenuItem.Text = "Remove";
            this.removeToolStripMenuItem.Click += new System.EventHandler(this.removeToolStripMenuItem_Click);
            // 
            // viewToolStripMenuItem
            // 
            this.viewToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.expandAllToolStripMenuItem,
            this.collapseAllToolStripMenuItem});
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
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel1});
            this.statusStrip1.Location = new System.Drawing.Point(0, 609);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(834, 22);
            this.statusStrip1.TabIndex = 1;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(118, 17);
            this.toolStripStatusLabel1.Text = "toolStripStatusLabel1";
            // 
            // groupBoxAddStructure
            // 
            this.groupBoxAddStructure.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBoxAddStructure.Controls.Add(this.buttonAddStructure);
            this.groupBoxAddStructure.Controls.Add(this.comboBoxStructureList);
            this.groupBoxAddStructure.Location = new System.Drawing.Point(640, 31);
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
            this.buttonAddStructure.Location = new System.Drawing.Point(7, 48);
            this.buttonAddStructure.Name = "buttonAddStructure";
            this.buttonAddStructure.Size = new System.Drawing.Size(169, 24);
            this.buttonAddStructure.TabIndex = 2;
            this.buttonAddStructure.Text = "Add";
            this.buttonAddStructure.UseVisualStyleBackColor = true;
            this.buttonAddStructure.Click += new System.EventHandler(this.button1_Click);
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
            this.comboBoxStructureList.SelectedIndexChanged += new System.EventHandler(this.comboBox1_SelectedIndexChanged);
            // 
            // pictureBoxSelectedStructure
            // 
            this.pictureBoxSelectedStructure.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.pictureBoxSelectedStructure.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.pictureBoxSelectedStructure.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pictureBoxSelectedStructure.Location = new System.Drawing.Point(641, 135);
            this.pictureBoxSelectedStructure.Name = "pictureBoxSelectedStructure";
            this.pictureBoxSelectedStructure.Size = new System.Drawing.Size(180, 180);
            this.pictureBoxSelectedStructure.TabIndex = 4;
            this.pictureBoxSelectedStructure.TabStop = false;
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.Location = new System.Drawing.Point(640, 116);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(181, 16);
            this.label1.TabIndex = 5;
            this.label1.Text = "Selected Structure:";
            // 
            // labelSelectedStructureType
            // 
            this.labelSelectedStructureType.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.labelSelectedStructureType.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelSelectedStructureType.Location = new System.Drawing.Point(640, 318);
            this.labelSelectedStructureType.Name = "labelSelectedStructureType";
            this.labelSelectedStructureType.Size = new System.Drawing.Size(181, 20);
            this.labelSelectedStructureType.TabIndex = 5;
            this.labelSelectedStructureType.Text = "Unspecified";
            this.labelSelectedStructureType.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // groupBoxRemoveStructure
            // 
            this.groupBoxRemoveStructure.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBoxRemoveStructure.Controls.Add(this.buttonRemoveStructure);
            this.groupBoxRemoveStructure.Location = new System.Drawing.Point(639, 555);
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
            this.buttonRemoveStructure.Location = new System.Drawing.Point(6, 19);
            this.buttonRemoveStructure.Name = "buttonRemoveStructure";
            this.buttonRemoveStructure.Size = new System.Drawing.Size(169, 24);
            this.buttonRemoveStructure.TabIndex = 1;
            this.buttonRemoveStructure.Text = "Remove";
            this.buttonRemoveStructure.UseVisualStyleBackColor = true;
            this.buttonRemoveStructure.Click += new System.EventHandler(this.button1_Click);
            // 
            // groupBoxUndockPort
            // 
            this.groupBoxUndockPort.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBoxUndockPort.Controls.Add(this.buttonUndock);
            this.groupBoxUndockPort.Location = new System.Drawing.Point(639, 498);
            this.groupBoxUndockPort.Name = "groupBoxUndockPort";
            this.groupBoxUndockPort.Size = new System.Drawing.Size(182, 51);
            this.groupBoxUndockPort.TabIndex = 4;
            this.groupBoxUndockPort.TabStop = false;
            this.groupBoxUndockPort.Text = "Undock Port";
            // 
            // buttonUndock
            // 
            this.buttonUndock.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonUndock.Enabled = false;
            this.buttonUndock.Location = new System.Drawing.Point(6, 19);
            this.buttonUndock.Name = "buttonUndock";
            this.buttonUndock.Size = new System.Drawing.Size(169, 24);
            this.buttonUndock.TabIndex = 1;
            this.buttonUndock.Text = "Undock";
            this.buttonUndock.UseVisualStyleBackColor = true;
            this.buttonUndock.Click += new System.EventHandler(this.button1_Click);
            // 
            // groupBoxDockPort
            // 
            this.groupBoxDockPort.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBoxDockPort.Controls.Add(this.button1);
            this.groupBoxDockPort.Controls.Add(this.label2);
            this.groupBoxDockPort.Controls.Add(this.comboBoxDockingDestinationPort);
            this.groupBoxDockPort.Controls.Add(this.comboBoxDockingDestinationStructure);
            this.groupBoxDockPort.Controls.Add(this.comboBoxDockingSourcePort);
            this.groupBoxDockPort.Location = new System.Drawing.Point(639, 341);
            this.groupBoxDockPort.Name = "groupBoxDockPort";
            this.groupBoxDockPort.Size = new System.Drawing.Size(182, 151);
            this.groupBoxDockPort.TabIndex = 3;
            this.groupBoxDockPort.TabStop = false;
            this.groupBoxDockPort.Text = "Dock Port";
            // 
            // button1
            // 
            this.button1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.button1.Enabled = false;
            this.button1.Location = new System.Drawing.Point(7, 121);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(169, 24);
            this.button1.TabIndex = 1;
            this.button1.Text = "Dock";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label2.Location = new System.Drawing.Point(6, 45);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(169, 16);
            this.label2.TabIndex = 5;
            this.label2.Text = "To Structure and Docking Port";
            // 
            // comboBoxDockingDestinationPort
            // 
            this.comboBoxDockingDestinationPort.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.comboBoxDockingDestinationPort.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxDockingDestinationPort.FormattingEnabled = true;
            this.comboBoxDockingDestinationPort.Location = new System.Drawing.Point(7, 89);
            this.comboBoxDockingDestinationPort.Name = "comboBoxDockingDestinationPort";
            this.comboBoxDockingDestinationPort.Size = new System.Drawing.Size(169, 21);
            this.comboBoxDockingDestinationPort.TabIndex = 0;
            this.comboBoxDockingDestinationPort.SelectedIndexChanged += new System.EventHandler(this.comboBox1_SelectedIndexChanged);
            // 
            // comboBoxDockingDestinationStructure
            // 
            this.comboBoxDockingDestinationStructure.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.comboBoxDockingDestinationStructure.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxDockingDestinationStructure.FormattingEnabled = true;
            this.comboBoxDockingDestinationStructure.Location = new System.Drawing.Point(7, 62);
            this.comboBoxDockingDestinationStructure.Name = "comboBoxDockingDestinationStructure";
            this.comboBoxDockingDestinationStructure.Size = new System.Drawing.Size(169, 21);
            this.comboBoxDockingDestinationStructure.TabIndex = 0;
            this.comboBoxDockingDestinationStructure.SelectedIndexChanged += new System.EventHandler(this.comboBox1_SelectedIndexChanged);
            // 
            // comboBoxDockingSourcePort
            // 
            this.comboBoxDockingSourcePort.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.comboBoxDockingSourcePort.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxDockingSourcePort.FormattingEnabled = true;
            this.comboBoxDockingSourcePort.Location = new System.Drawing.Point(7, 20);
            this.comboBoxDockingSourcePort.Name = "comboBoxDockingSourcePort";
            this.comboBoxDockingSourcePort.Size = new System.Drawing.Size(169, 21);
            this.comboBoxDockingSourcePort.TabIndex = 0;
            this.comboBoxDockingSourcePort.SelectedIndexChanged += new System.EventHandler(this.comboBox1_SelectedIndexChanged);
            // 
            // BlueprintEditorForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(834, 631);
            this.Controls.Add(this.labelSelectedStructureType);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.pictureBoxSelectedStructure);
            this.Controls.Add(this.groupBoxUndockPort);
            this.Controls.Add(this.groupBoxRemoveStructure);
            this.Controls.Add(this.groupBoxDockPort);
            this.Controls.Add(this.groupBoxAddStructure);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.treeView1);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.MinimumSize = new System.Drawing.Size(700, 670);
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
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TreeView treeView1;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem structureToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem addToolStripMenuItem;
        private System.Windows.Forms.ToolStripComboBox toolStripComboBox1;
        private System.Windows.Forms.ToolStripMenuItem dockToolStripMenuItem;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private System.Windows.Forms.ToolStripMenuItem blueprintToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem removeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem newToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem closeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem undockToolStripMenuItem;
        private System.Windows.Forms.GroupBox groupBoxAddStructure;
        private System.Windows.Forms.Button buttonAddStructure;
        private System.Windows.Forms.ComboBox comboBoxStructureList;
        private System.Windows.Forms.PictureBox pictureBoxSelectedStructure;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label labelSelectedStructureType;
        private System.Windows.Forms.GroupBox groupBoxRemoveStructure;
        private System.Windows.Forms.Button buttonRemoveStructure;
        private System.Windows.Forms.GroupBox groupBoxUndockPort;
        private System.Windows.Forms.Button buttonUndock;
        private System.Windows.Forms.GroupBox groupBoxDockPort;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.ComboBox comboBoxDockingSourcePort;
        private System.Windows.Forms.ComboBox comboBoxDockingDestinationPort;
        private System.Windows.Forms.ComboBox comboBoxDockingDestinationStructure;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ToolStripMenuItem viewToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem expandAllToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem collapseAllToolStripMenuItem;
    }
}