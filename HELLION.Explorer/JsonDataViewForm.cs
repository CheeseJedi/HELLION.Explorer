using System;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using FastColoredTextBoxNS;
using HELLION.DataStructures;
using HELLION.DataStructures.EmbeddedImages;
using HELLION.DataStructures.UI;
using HELLION.DataStructures.Utilities;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using static HELLION.DataStructures.EmbeddedImages.EmbeddedImages_ImageList;

namespace HELLION.Explorer
{
    public partial class JsonDataViewForm : Form
    {
        #region Constructors

        /// <summary>
        /// Private constructor.
        /// </summary>
        private JsonDataViewForm()
        {
            InitializeComponent();
            Icon = HellionExplorerProgram.MainForm.Icon;

            // We're not using a custom language for the FastColouredTextBox, instead
            // the JavaScript built-in language is utilised.
            fastColoredTextBox1.Language = Language.JS;
            // Set the line numbering to start from zero.
            //fastColoredTextBox1.LineNumberStartValue = 0;
            applyChangesToolStripMenuItem.Enabled = false;
        }

        /// <summary>
        /// Normal Constructor.
        /// </summary>
        /// <param name="passedSourceNode"></param>
        /// <param name="imageList"></param>
        public JsonDataViewForm(Json_TN passedSourceNode, EmbeddedImages_ImageList imageList) : this()
        {
            // Store a reference to the ImageList
            _imageList = imageList ?? throw new NullReferenceException("passed ImageList was null.");

            SourceNode = passedSourceNode ?? throw new NullReferenceException("passedSourceNode was null.");
            _formTitleText = passedSourceNode.FullPath;
            Text = _formTitleText;
            _appliedText = SourceNode.JData.ToString();

            // Character length limit -  this is a guessed figure!
            if (_appliedText.Length > 25000)
                deserialiseAsYouTypeToolStripMenuItem.Checked = false;

            // Apply the text.
            fastColoredTextBox1.Text = _appliedText;

            // Required as setting the FastColouredTextBox triggers the _isDirty
            IsDirty = false;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Tracks un-applied changes to this editor window.
        /// </summary>
        public bool IsDirty
        {
            get => _isDirty;
            private set
            {
                _isDirty = value;
                // Enable or disable the Apply Changes menu option.
                applyChangesToolStripMenuItem.Enabled = value;
                // Update the form name
                RefreshJsonDataViewFormTitleText();
            }
        }

        public Json_TN SourceNode { get; private set; } = null;

        #endregion

        #region Methods

        private void RefreshJsonDataViewFormTitleText()
        {
            if (IsDirty) Text = _formTitleText + "*";
            else Text = _formTitleText;
        }

        /// <summary>
        /// De-serialises the text from the FastColoredTextBox and replaces the
        /// source JToken with the results if the serialisation was successful.
        /// </summary>
        /// <returns></returns>
        private bool ApplyChanges()
        {

            if (fastColoredTextBox1.Text.
                TryParseJson(out JToken newToken, out JsonReaderException jrex))
            {
                // Successful parse.

                // Make a note of the starting time
                DateTime startingTime = DateTime.Now;

                //Application.UseWaitCursor = true;
                Cursor = Cursors.WaitCursor;

                // Suppress repainting the TreeView until all the objects have been created.
                HellionExplorerProgram.MainForm.treeView1.BeginUpdate();

                // Update the status bar
                HellionExplorerProgram.MainForm.toolStripStatusLabel1.Text =
                    String.Format("Applying changes...");



                // Find the parent Json_File.
                Json_File parentFile = HellionExplorerProgram.docCurrent?.GameData.FindOwningFile(SourceNode);
                Debug.Print("ApplyChanges: parentFile = " + parentFile.File.FullName);

                if (parentFile == null) throw new NullReferenceException("ApplyChanges: parentFile was null.");

                // Check to make sure it's a .save file, as that's all that's supported currently.
                if (parentFile != HellionExplorerProgram.docCurrent.GameData.SaveFile)
                {
                    // Begin repainting the TreeView.
                    HellionExplorerProgram.MainForm.treeView1.EndUpdate();

                    //Application.UseWaitCursor = false;
                    Cursor = Cursors.Default;

                    MessageBox.Show("Only the main .save file is currently supported for saving edited changes"
                        + "- other Json files are view only.", "Save Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                    return false;
                }

                if (SourceNode.JData.Parent == null) throw new Exception();

                // Get a reference to the old (current) token.
                JToken oldToken = SourceNode.JData;

                // Add the new token to the parent token directly after this one.
                oldToken.AddAfterSelf(newToken);

                // Remove the old token.
                oldToken.Remove();

                // Set the new token for the node, triggering updates.
                SourceNode.JData = newToken;

                Debug.Print("Marking parent file " + parentFile.File.FullName + " as dirty.");
                // Mark the parent file as dirty.
                parentFile.IsDirty = true;

                // Set the AppliedText.
                _appliedText = fastColoredTextBox1.Text;

                // Mark this editor as not dirty.
                IsDirty = false;

                // Trigger the Solar System to rebuild - SLOW!
                HellionExplorerProgram.docCurrent.SolarSystem.RebuildSolarSystem();

                HellionExplorerProgram.RefreshMainFormTitleText();

                // Begin repainting the TreeView.
                HellionExplorerProgram.MainForm.treeView1.EndUpdate();

                //Application.UseWaitCursor = false;
                Cursor = Cursors.Default;

                // Update the status bar
                HellionExplorerProgram.MainForm.toolStripStatusLabel1.Text =
                    String.Format("Changes applied and Node regeneration completed in {0:mm}m{0:ss}s",
                    DateTime.Now - startingTime);


                MessageBox.Show("Changes applied. Remember to save the main document!", "SUCCESS");


                return true;

            }
            else
            {
                string msg = string.Format("An exception occurred: {0}{1}{0}{0}Full Detail{0}{2}",
                    Environment.NewLine, jrex != null ? jrex.Message : "Non-JsonTextReader error.", jrex);
                MessageBox.Show(msg, "De-serialisation error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;

            }
        }

        /// <summary>
        /// Refreshes the Serialise-As-You-Type status indicator (text and icon).
        /// </summary>
        private void RefreshToolStripDeserialisatonStatus(bool bypass = false)
        {
            if (bypass)
            {
                toolStripStatusLabel3.Text = "Deserialisation OFF";
                toolStripMainStatusLabel.Text = "Deserialise As-You-Type is tuned off.";
            }
            else
            {
                if (fastColoredTextBox1.Text.
                    TryParseJson(out JToken tmp, out JsonReaderException jrex))
                {
                    toolStripStatusLabel3.Text = "Deserialisation PASSED";

                    if (_imageList != null)
                    {
                        toolStripStatusLabel_SerialisationStatus.Image =
                            _imageList.IconImageList.Images[(int)HEIconsImageNames.FileOK_16x];
                    }

                    toolStripStatusLabel_SerialisationStatus.ToolTipText =
                        "The text is syntactically correct and deserialised OK.";
                    toolStripMainStatusLabel.Text = toolStripStatusLabel_SerialisationStatus.ToolTipText;

                }
                else
                {
                    toolStripStatusLabel3.Text = "Deserialisation ERROR";

                    if (_imageList != null)
                    {
                        toolStripStatusLabel_SerialisationStatus.Image =
                            _imageList.IconImageList.Images[(int)HEIconsImageNames.FileError_16x];
                    }

                    toolStripStatusLabel_SerialisationStatus.ToolTipText = jrex != null ? jrex.Message : "Non-JsonTextReader error.";
                    toolStripMainStatusLabel.Text = toolStripStatusLabel_SerialisationStatus.ToolTipText;
                }
            }
        }

        /// <summary>
        /// Refreshes the cursor position indicator.
        /// </summary>
        private void RefreshToolStripLineAndCharCount()
        {
            toolStripStatusLineCharCount.Text = string.Format("[Lines {0:n0} Chars {1:n0}]",
                fastColoredTextBox1.LinesCount,
                fastColoredTextBox1.Text.Length);
        }

        /// <summary>
        /// Refreshes the cursor position indicator.
        /// </summary>
        private void RefreshToolStripCursorPositionLabel()
        {
            if (fastColoredTextBox1.Selection.Start != null)
            {
                toolStripCursorPositionLabel.Text = string.Format("[Ln {0:n0} Col {1:n0}]",
                // The line number position for the cursor is returned by the FastColouredTextBox
                // counting from zero, seemingly regardless of the configured starting line number.
                fastColoredTextBox1.Selection.Start.iLine + 1,
                fastColoredTextBox1.Selection.Start.iChar); 
            }
            else toolStripCursorPositionLabel.Text = "[Ln ? Col ?]";
        }

        #endregion

        #region Fields

        private bool _isDirty = false;
        /// <summary>
        /// Stores a copy of the unmodified text - updated after the apply changes operation.
        /// </summary>
        private string _appliedText = null;
        private string _formTitleText = null;

        private EmbeddedImages_ImageList _imageList = null;

        #endregion


        //Create style for highlighting
        //TextStyle brownStyle = new TextStyle(Brushes.Brown, null, FontStyle.Regular);

        /// <summary>
        /// This code really needs to happen _before_ form closing
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void JsonDataViewForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (IsDirty)
            {
                // Unsaved changes, prompt the user to apply them before closing the window.

                switch (MessageBox.Show("Do you want to apply changes to the main file?",
                    "Un-Applied Changes Warning", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Exclamation))
                {
                    case DialogResult.Cancel:
                        e.Cancel = true;
                        return;

                    case DialogResult.Yes:

                        ApplyChanges();
                        break;

                    default:
                        break;
                }
            }

            // Clear the lock on the node
            SourceNode.Unlock();

            // Remove the current JsonDataViewForm from the jsonDataViews list
            HellionExplorerProgram.jsonDataViews.Remove(this);
            GC.Collect();
        }

        /// <summary>
        /// Handles setting folding markers on the FastColoredTextBox and setting the _isDirty bool.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void fastColoredTextBox1_TextChanged(object sender, TextChangedEventArgs e)
        {
            IsDirty = true;
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

            RefreshToolStripDeserialisatonStatus(!deserialiseAsYouTypeToolStripMenuItem.Checked);
            RefreshToolStripLineAndCharCount();

        }

        /// <summary>
        /// Handles updating the cursor position indicator when the selection changes.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void fastColoredTextBox1_SelectionChanged(object sender, EventArgs e)
        {
            RefreshToolStripCursorPositionLabel();
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

        private void applyChangesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ApplyChanges()) IsDirty = false;
        }




        #endregion

        private void deserialiseAsYouTypeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            deserialiseAsYouTypeToolStripMenuItem.Checked = !deserialiseAsYouTypeToolStripMenuItem.Checked;
            RefreshToolStripDeserialisatonStatus(!deserialiseAsYouTypeToolStripMenuItem.Checked);

        }
    }
}
