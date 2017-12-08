using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using FastColoredTextBoxNS;

namespace HELLION.Explorer
{
    public partial class JsonDataViewForm : Form
    {
        //Create style for highlighting
        //TextStyle brownStyle = new TextStyle(Brushes.Brown, null, FontStyle.Regular);


        public JsonDataViewForm()
        {
            InitializeComponent();
        }

        private void JsonDataViewForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Remove the current JsonDataViewForm from the jsonDataViews list
            Program.jsonDataViews.Remove(this);
            GC.Collect();

        }

        private void fastColoredTextBox1_TextChanged(object sender, FastColoredTextBoxNS.TextChangedEventArgs e)
        {
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

        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void findToolStripMenuItem_Click(object sender, EventArgs e)
        {
            fastColoredTextBox1.ShowFindDialog();
        }

        private void replaceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            fastColoredTextBox1.ShowReplaceDialog();
        }
    }
}
