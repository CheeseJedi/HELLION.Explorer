namespace HELLION.StationBlueprintEditor
{
    partial class BlueprintPropertiesForm
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
            this.label_Version = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.textBox_Name = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.textBox_LinkURI = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label_StructureCount = new System.Windows.Forms.Label();
            this.listView1 = new System.Windows.Forms.ListView();
            this.button1 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(165, 16);
            this.label1.TabIndex = 0;
            this.label1.Text = "StationBlueprint Version";
            // 
            // label_Version
            // 
            this.label_Version.Location = new System.Drawing.Point(183, 9);
            this.label_Version.Name = "label_Version";
            this.label_Version.Size = new System.Drawing.Size(309, 16);
            this.label_Version.TabIndex = 0;
            this.label_Version.Text = "label_Version";
            // 
            // label3
            // 
            this.label3.Location = new System.Drawing.Point(12, 41);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(165, 16);
            this.label3.TabIndex = 0;
            this.label3.Text = "Name";
            // 
            // textBox_Name
            // 
            this.textBox_Name.Location = new System.Drawing.Point(186, 41);
            this.textBox_Name.Name = "textBox_Name";
            this.textBox_Name.Size = new System.Drawing.Size(306, 20);
            this.textBox_Name.TabIndex = 1;
            this.textBox_Name.Text = "textBox_Name";
            // 
            // label4
            // 
            this.label4.Location = new System.Drawing.Point(12, 77);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(165, 16);
            this.label4.TabIndex = 0;
            this.label4.Text = "Link URI";
            // 
            // textBox_LinkURI
            // 
            this.textBox_LinkURI.Location = new System.Drawing.Point(186, 77);
            this.textBox_LinkURI.Name = "textBox_LinkURI";
            this.textBox_LinkURI.Size = new System.Drawing.Size(306, 20);
            this.textBox_LinkURI.TabIndex = 1;
            this.textBox_LinkURI.Text = "textBox_LinkURI";
            // 
            // label5
            // 
            this.label5.Location = new System.Drawing.Point(12, 113);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(165, 16);
            this.label5.TabIndex = 0;
            this.label5.Text = "Total Structure Count";
            // 
            // label_StructureCount
            // 
            this.label_StructureCount.Location = new System.Drawing.Point(183, 113);
            this.label_StructureCount.Name = "label_StructureCount";
            this.label_StructureCount.Size = new System.Drawing.Size(309, 16);
            this.label_StructureCount.TabIndex = 0;
            this.label_StructureCount.Text = "label_StructureCount";
            // 
            // listView1
            // 
            this.listView1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listView1.Location = new System.Drawing.Point(12, 143);
            this.listView1.Name = "listView1";
            this.listView1.Size = new System.Drawing.Size(479, 305);
            this.listView1.TabIndex = 2;
            this.listView1.UseCompatibleStateImageBehavior = false;
            // 
            // button1
            // 
            this.button1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.button1.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.button1.Location = new System.Drawing.Point(313, 460);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(179, 28);
            this.button1.TabIndex = 3;
            this.button1.Text = "OK";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // BlueprintPropertiesForm
            // 
            this.AcceptButton = this.button1;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(509, 496);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.listView1);
            this.Controls.Add(this.textBox_LinkURI);
            this.Controls.Add(this.textBox_Name);
            this.Controls.Add(this.label_StructureCount);
            this.Controls.Add(this.label_Version);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "BlueprintPropertiesForm";
            this.Text = "BlueprintPropertiesForm";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label_Version;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox textBox_Name;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox textBox_LinkURI;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label_StructureCount;
        private System.Windows.Forms.ListView listView1;
        private System.Windows.Forms.Button button1;
    }
}