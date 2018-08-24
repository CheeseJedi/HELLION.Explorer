namespace HELLION.Explorer
{
    partial class JsonDataViewForm
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(JsonDataViewForm));
            this.fastColoredTextBox1 = new FastColoredTextBoxNS.FastColoredTextBox();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exportToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.applyChangesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.closeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.findToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.replaceToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deserialiseAsYouTypeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripMainStatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStrip_SpringSeperator = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripReadOnlyStatus = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLineCharCount = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripCursorPositionLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabel3 = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabel_SerialisationStatus = new System.Windows.Forms.ToolStripStatusLabel();
            ((System.ComponentModel.ISupportInitialize)(this.fastColoredTextBox1)).BeginInit();
            this.menuStrip1.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // fastColoredTextBox1
            // 
            this.fastColoredTextBox1.AutoCompleteBracketsList = new char[] {
        '(',
        ')',
        '{',
        '}',
        '[',
        ']',
        '\"',
        '\"',
        '\'',
        '\''};
            this.fastColoredTextBox1.AutoScrollMinSize = new System.Drawing.Size(179, 14);
            this.fastColoredTextBox1.BackBrush = null;
            this.fastColoredTextBox1.CharHeight = 14;
            this.fastColoredTextBox1.CharWidth = 8;
            this.fastColoredTextBox1.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.fastColoredTextBox1.DisabledColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(180)))), ((int)(((byte)(180)))), ((int)(((byte)(180)))));
            this.fastColoredTextBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.fastColoredTextBox1.IsReplaceMode = false;
            this.fastColoredTextBox1.Location = new System.Drawing.Point(0, 24);
            this.fastColoredTextBox1.Name = "fastColoredTextBox1";
            this.fastColoredTextBox1.Paddings = new System.Windows.Forms.Padding(0);
            this.fastColoredTextBox1.SelectionColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(255)))));
            this.fastColoredTextBox1.ServiceColors = ((FastColoredTextBoxNS.ServiceColors)(resources.GetObject("fastColoredTextBox1.ServiceColors")));
            this.fastColoredTextBox1.Size = new System.Drawing.Size(815, 493);
            this.fastColoredTextBox1.TabIndex = 2;
            this.fastColoredTextBox1.Text = "fastColoredTextBox1";
            this.fastColoredTextBox1.Zoom = 100;
            this.fastColoredTextBox1.TextChanged += new System.EventHandler<FastColoredTextBoxNS.TextChangedEventArgs>(this.fastColoredTextBox1_TextChanged);
            this.fastColoredTextBox1.SelectionChanged += new System.EventHandler(this.fastColoredTextBox1_SelectionChanged);
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.editToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(815, 24);
            this.menuStrip1.TabIndex = 3;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.exportToolStripMenuItem,
            this.applyChangesToolStripMenuItem,
            this.closeToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // exportToolStripMenuItem
            // 
            this.exportToolStripMenuItem.Enabled = false;
            this.exportToolStripMenuItem.Name = "exportToolStripMenuItem";
            this.exportToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.E)));
            this.exportToolStripMenuItem.Size = new System.Drawing.Size(156, 22);
            this.exportToolStripMenuItem.Text = "Export...";
            // 
            // applyChangesToolStripMenuItem
            // 
            this.applyChangesToolStripMenuItem.Name = "applyChangesToolStripMenuItem";
            this.applyChangesToolStripMenuItem.Size = new System.Drawing.Size(156, 22);
            this.applyChangesToolStripMenuItem.Text = "Apply Changes";
            this.applyChangesToolStripMenuItem.Click += new System.EventHandler(this.applyChangesToolStripMenuItem_Click);
            // 
            // closeToolStripMenuItem
            // 
            this.closeToolStripMenuItem.Name = "closeToolStripMenuItem";
            this.closeToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.F4)));
            this.closeToolStripMenuItem.Size = new System.Drawing.Size(156, 22);
            this.closeToolStripMenuItem.Text = "Close";
            this.closeToolStripMenuItem.Click += new System.EventHandler(this.closeToolStripMenuItem_Click);
            // 
            // editToolStripMenuItem
            // 
            this.editToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.findToolStripMenuItem,
            this.replaceToolStripMenuItem,
            this.deserialiseAsYouTypeToolStripMenuItem});
            this.editToolStripMenuItem.Name = "editToolStripMenuItem";
            this.editToolStripMenuItem.Size = new System.Drawing.Size(39, 20);
            this.editToolStripMenuItem.Text = "Edit";
            // 
            // findToolStripMenuItem
            // 
            this.findToolStripMenuItem.Name = "findToolStripMenuItem";
            this.findToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.F)));
            this.findToolStripMenuItem.Size = new System.Drawing.Size(200, 22);
            this.findToolStripMenuItem.Text = "Find...";
            this.findToolStripMenuItem.Click += new System.EventHandler(this.findToolStripMenuItem_Click);
            // 
            // replaceToolStripMenuItem
            // 
            this.replaceToolStripMenuItem.Name = "replaceToolStripMenuItem";
            this.replaceToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.R)));
            this.replaceToolStripMenuItem.Size = new System.Drawing.Size(200, 22);
            this.replaceToolStripMenuItem.Text = "Replace...";
            this.replaceToolStripMenuItem.Click += new System.EventHandler(this.replaceToolStripMenuItem_Click);
            // 
            // deserialiseAsYouTypeToolStripMenuItem
            // 
            this.deserialiseAsYouTypeToolStripMenuItem.Checked = true;
            this.deserialiseAsYouTypeToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.deserialiseAsYouTypeToolStripMenuItem.Name = "deserialiseAsYouTypeToolStripMenuItem";
            this.deserialiseAsYouTypeToolStripMenuItem.Size = new System.Drawing.Size(200, 22);
            this.deserialiseAsYouTypeToolStripMenuItem.Text = "Deserialise As-You-Type";
            this.deserialiseAsYouTypeToolStripMenuItem.Click += new System.EventHandler(this.deserialiseAsYouTypeToolStripMenuItem_Click);
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMainStatusLabel,
            this.toolStrip_SpringSeperator,
            this.toolStripReadOnlyStatus,
            this.toolStripStatusLineCharCount,
            this.toolStripCursorPositionLabel,
            this.toolStripStatusLabel3,
            this.toolStripStatusLabel_SerialisationStatus});
            this.statusStrip1.Location = new System.Drawing.Point(0, 517);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.ShowItemToolTips = true;
            this.statusStrip1.Size = new System.Drawing.Size(815, 24);
            this.statusStrip1.TabIndex = 4;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStripMainStatusLabel
            // 
            this.toolStripMainStatusLabel.Name = "toolStripMainStatusLabel";
            this.toolStripMainStatusLabel.Size = new System.Drawing.Size(16, 19);
            this.toolStripMainStatusLabel.Text = "   ";
            // 
            // toolStrip_SpringSeperator
            // 
            this.toolStrip_SpringSeperator.Name = "toolStrip_SpringSeperator";
            this.toolStrip_SpringSeperator.Size = new System.Drawing.Size(389, 19);
            this.toolStrip_SpringSeperator.Spring = true;
            // 
            // toolStripReadOnlyStatus
            // 
            this.toolStripReadOnlyStatus.BorderSides = ((System.Windows.Forms.ToolStripStatusLabelBorderSides)((((System.Windows.Forms.ToolStripStatusLabelBorderSides.Left | System.Windows.Forms.ToolStripStatusLabelBorderSides.Top) 
            | System.Windows.Forms.ToolStripStatusLabelBorderSides.Right) 
            | System.Windows.Forms.ToolStripStatusLabelBorderSides.Bottom)));
            this.toolStripReadOnlyStatus.Name = "toolStripReadOnlyStatus";
            this.toolStripReadOnlyStatus.Size = new System.Drawing.Size(73, 19);
            this.toolStripReadOnlyStatus.Text = "READ ONLY";
            this.toolStripReadOnlyStatus.Visible = false;
            // 
            // toolStripStatusLineCharCount
            // 
            this.toolStripStatusLineCharCount.Name = "toolStripStatusLineCharCount";
            this.toolStripStatusLineCharCount.Size = new System.Drawing.Size(93, 19);
            this.toolStripStatusLineCharCount.Text = "Line/Char count";
            // 
            // toolStripCursorPositionLabel
            // 
            this.toolStripCursorPositionLabel.Name = "toolStripCursorPositionLabel";
            this.toolStripCursorPositionLabel.Size = new System.Drawing.Size(77, 19);
            this.toolStripCursorPositionLabel.Text = "Cursor [    ,   ]";
            // 
            // toolStripStatusLabel3
            // 
            this.toolStripStatusLabel3.Name = "toolStripStatusLabel3";
            this.toolStripStatusLabel3.Size = new System.Drawing.Size(105, 19);
            this.toolStripStatusLabel3.Text = "Serialisation Status";
            // 
            // toolStripStatusLabel_SerialisationStatus
            // 
            this.toolStripStatusLabel_SerialisationStatus.AutoToolTip = true;
            this.toolStripStatusLabel_SerialisationStatus.Name = "toolStripStatusLabel_SerialisationStatus";
            this.toolStripStatusLabel_SerialisationStatus.Size = new System.Drawing.Size(16, 19);
            this.toolStripStatusLabel_SerialisationStatus.Text = "   ";
            // 
            // JsonDataViewForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(815, 541);
            this.Controls.Add(this.fastColoredTextBox1);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "JsonDataViewForm";
            this.Text = "Json Data View";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.JsonDataViewForm_FormClosing);
            ((System.ComponentModel.ISupportInitialize)(this.fastColoredTextBox1)).EndInit();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private FastColoredTextBoxNS.FastColoredTextBox fastColoredTextBox1;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exportToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem closeToolStripMenuItem;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripMenuItem editToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem findToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem replaceToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem applyChangesToolStripMenuItem;
        private System.Windows.Forms.ToolStripStatusLabel toolStripMainStatusLabel;
        private System.Windows.Forms.ToolStripStatusLabel toolStrip_SpringSeperator;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel3;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel_SerialisationStatus;
        private System.Windows.Forms.ToolStripStatusLabel toolStripCursorPositionLabel;
        private System.Windows.Forms.ToolStripMenuItem deserialiseAsYouTypeToolStripMenuItem;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLineCharCount;
        private System.Windows.Forms.ToolStripStatusLabel toolStripReadOnlyStatus;
    }
}