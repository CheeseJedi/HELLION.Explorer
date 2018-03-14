using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using FastColoredTextBoxNS;
using HELLION.DataStructures;

namespace HELLION.Explorer
{
    public partial class JsonDataViewForm : Form
    {
        //Create style for highlighting
        //TextStyle brownStyle = new TextStyle(Brushes.Brown, null, FontStyle.Regular);

        private bool isDirty = false;

        public bool IsDirty { get => isDirty; }

        private HEGameDataTreeNode sourceNode = null;

        public HEGameDataTreeNode SourceNode => sourceNode;

        public JsonDataViewForm()
        {
            InitializeComponent();
            Icon = Program.frmMainForm.Icon;
        }

        public JsonDataViewForm(HEGameDataTreeNode passedSourceNode) : this()
        {
            sourceNode = passedSourceNode ?? throw new NullReferenceException("passedSourceNode was null.");
            Text = passedSourceNode.FullPath;
            fastColoredTextBox1.Language = Language.JS;
            fastColoredTextBox1.Text = passedSourceNode.Tag.ToString();
            isDirty = false;
        }

        private void JsonDataViewForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            
            if (isDirty)
            {
                // Unsaved changes, prompt the user to apply them before closing the window.
                MessageBox.Show("Un-applied changes!");
            }

            
            // Remove the current JsonDataViewForm from the jsonDataViews list
            Program.jsonDataViews.Remove(this);
            GC.Collect();
        }

        private void fastColoredTextBox1_TextChanged(object sender, FastColoredTextBoxNS.TextChangedEventArgs e)
        {
            isDirty = true;
            Debug.Print("Text Changed called");

            //clear previous highlighting
            //e.ChangedRange.ClearStyle(brownStyle);
            //highlight tags
            //e.ChangedRange.SetStyle(brownStyle, "<[^>]+>");

            //clear folding markers of changed range
            e.ChangedRange.ClearFoldingMarkers();
            //set folding markers
            e.ChangedRange.SetFoldingMarkers("{", "}");
            e.ChangedRange.SetFoldingMarkers(Regex.Escape(@"["), Regex.Escape(@"]"));

            //e.ChangedRange.SetFoldingMarkers(@"#region\b", @"#endregion\b");

        }

        #region menuStrip1

        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void findToolStripMenuItem_Click(object sender, EventArgs e)
        {
            fastColoredTextBox1.ShowFindDialog();
        }

        private void replaceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            fastColoredTextBox1.ShowReplaceDialog();
        }

        #endregion

        private void applyChangesToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }
    }
}
