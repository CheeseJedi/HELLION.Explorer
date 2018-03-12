namespace HELLION.Explorer
{
    partial class FindForm
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
            this.textBoxQuery = new System.Windows.Forms.TextBox();
            this.labelTextBoxQuery = new System.Windows.Forms.Label();
            this.checkBoxMatchCase = new System.Windows.Forms.CheckBox();
            this.checkBoxPathSearch = new System.Windows.Forms.CheckBox();
            this.buttonFindNext = new System.Windows.Forms.Button();
            this.buttonFindAll = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // textBoxQuery
            // 
            this.textBoxQuery.Location = new System.Drawing.Point(12, 29);
            this.textBoxQuery.Name = "textBoxQuery";
            this.textBoxQuery.Size = new System.Drawing.Size(371, 20);
            this.textBoxQuery.TabIndex = 0;
            this.textBoxQuery.TextChanged += new System.EventHandler(this.textBoxQuery_TextChanged);
            // 
            // labelTextBoxQuery
            // 
            this.labelTextBoxQuery.AutoSize = true;
            this.labelTextBoxQuery.Location = new System.Drawing.Point(12, 9);
            this.labelTextBoxQuery.Name = "labelTextBoxQuery";
            this.labelTextBoxQuery.Size = new System.Drawing.Size(53, 13);
            this.labelTextBoxQuery.TabIndex = 1;
            this.labelTextBoxQuery.Text = "Find what";
            // 
            // checkBoxMatchCase
            // 
            this.checkBoxMatchCase.AutoSize = true;
            this.checkBoxMatchCase.Location = new System.Drawing.Point(12, 59);
            this.checkBoxMatchCase.Name = "checkBoxMatchCase";
            this.checkBoxMatchCase.Size = new System.Drawing.Size(82, 17);
            this.checkBoxMatchCase.TabIndex = 2;
            this.checkBoxMatchCase.Text = "Match case";
            this.checkBoxMatchCase.UseVisualStyleBackColor = true;
            this.checkBoxMatchCase.CheckedChanged += new System.EventHandler(this.checkBoxMatchCase_CheckedChanged);
            // 
            // checkBoxPathSearch
            // 
            this.checkBoxPathSearch.AutoSize = true;
            this.checkBoxPathSearch.Location = new System.Drawing.Point(12, 76);
            this.checkBoxPathSearch.Name = "checkBoxPathSearch";
            this.checkBoxPathSearch.Size = new System.Drawing.Size(83, 17);
            this.checkBoxPathSearch.TabIndex = 3;
            this.checkBoxPathSearch.Text = "Path search";
            this.checkBoxPathSearch.UseVisualStyleBackColor = true;
            this.checkBoxPathSearch.CheckedChanged += new System.EventHandler(this.checkBoxPathSearch_CheckedChanged);
            // 
            // buttonFindNext
            // 
            this.buttonFindNext.Location = new System.Drawing.Point(125, 70);
            this.buttonFindNext.Name = "buttonFindNext";
            this.buttonFindNext.Size = new System.Drawing.Size(82, 23);
            this.buttonFindNext.TabIndex = 4;
            this.buttonFindNext.Text = "Find Next";
            this.buttonFindNext.UseVisualStyleBackColor = true;
            this.buttonFindNext.Click += new System.EventHandler(this.buttonFindNext_Click);
            // 
            // buttonFindAll
            // 
            this.buttonFindAll.Location = new System.Drawing.Point(213, 70);
            this.buttonFindAll.Name = "buttonFindAll";
            this.buttonFindAll.Size = new System.Drawing.Size(82, 23);
            this.buttonFindAll.TabIndex = 4;
            this.buttonFindAll.Text = "Find All";
            this.buttonFindAll.UseVisualStyleBackColor = true;
            this.buttonFindAll.Click += new System.EventHandler(this.buttonFindAll_Click);
            // 
            // buttonCancel
            // 
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(301, 70);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(82, 23);
            this.buttonCancel.TabIndex = 4;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // FindForm
            // 
            this.AcceptButton = this.buttonFindNext;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.buttonCancel;
            this.ClientSize = new System.Drawing.Size(394, 103);
            this.ControlBox = false;
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonFindAll);
            this.Controls.Add(this.buttonFindNext);
            this.Controls.Add(this.checkBoxPathSearch);
            this.Controls.Add(this.checkBoxMatchCase);
            this.Controls.Add(this.labelTextBoxQuery);
            this.Controls.Add(this.textBoxQuery);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FindForm";
            this.ShowIcon = false;
            this.Text = "Find";
            this.TopMost = true;
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox textBoxQuery;
        private System.Windows.Forms.Label labelTextBoxQuery;
        private System.Windows.Forms.CheckBox checkBoxMatchCase;
        private System.Windows.Forms.CheckBox checkBoxPathSearch;
        private System.Windows.Forms.Button buttonFindNext;
        private System.Windows.Forms.Button buttonFindAll;
        private System.Windows.Forms.Button buttonCancel;


    }
}