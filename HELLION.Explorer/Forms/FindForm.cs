using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HELLION.Explorer
{
    public partial class FindForm : Form
    {
        private Form parentForm = null;

        public string QueryValue => textBoxQuery.Text;
        public bool MatchCaseValue => checkBoxMatchCase.Checked;
        private bool previousMatchCaseValue = false;
        public bool PathSearchValue => checkBoxPathSearch.Checked;

        private bool haveQueryControlsBeenModified = false;

        /// <summary>
        /// Enables or disables the text box and the two check boxes.
        /// </summary>
        /// <remarks>
        /// This is used to prevent the form controls from being adjusted once a
        /// search operation has started. To re-search the window should be closed.
        /// </remarks>
        /// <param name="newValue"></param>


        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="passedParent">Takes a reference to the parent form.</param>
        public FindForm(Form passedParent)
        {
            InitializeComponent();
            parentForm = passedParent ?? throw new NullReferenceException("passedParent was null.");
            ResetForm();
        }

        /// <summary>
        /// Enables or disabled the forms input controls (not buttons).
        /// </summary>
        /// <param name="newValue"></param>
        public void EnableDisableParameters(bool newValue)
        {
            textBoxQuery.Enabled = newValue;
            checkBoxMatchCase.Enabled = newValue;
            checkBoxPathSearch.Enabled = newValue;
        }

        /// <summary>
        /// Resets the form values ready for a new search operation.
        /// </summary>
        public void ResetForm()
        {
            textBoxQuery.Enabled = true;
            textBoxQuery.Text = string.Empty;
            checkBoxMatchCase.Enabled = true;
            checkBoxMatchCase.Checked = false;
            checkBoxPathSearch.Enabled = true;
            checkBoxPathSearch.Checked = false;
        }

        public void MainFormFindNextActivated()
        {
            buttonFindNext_Click(null, null);
        }

        private void buttonFindNext_Click(object sender, EventArgs e)
        {
            if (haveQueryControlsBeenModified)
            {
                haveQueryControlsBeenModified = false;
                HellionExplorerProgram.EditFind(NewQuery: true);
            }
            else
            {
                HellionExplorerProgram.EditFind();
            }
        }

        private void buttonFindAll_Click(object sender, EventArgs e)
        {
            if (haveQueryControlsBeenModified)
            {
                haveQueryControlsBeenModified = false;
                HellionExplorerProgram.EditFind(NewQuery: true, JumpToResultsSet: true);
            }
            else
            {
                HellionExplorerProgram.EditFind(JumpToResultsSet: true);
            }
            Hide();
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            Hide();
        }

        private void checkBoxPathSearch_CheckedChanged(object sender, EventArgs e)
        {
            haveQueryControlsBeenModified = true;
            if (checkBoxPathSearch.Checked)
            {
                //We're setting, save value and disable MatchCase control
                previousMatchCaseValue = checkBoxMatchCase.Checked;
                checkBoxMatchCase.Checked = false;
                checkBoxMatchCase.Enabled = false;
            }
            else
            {
                // We're un-setting, restore value and enable MatchCase control
                checkBoxMatchCase.Checked = previousMatchCaseValue;
                checkBoxMatchCase.Enabled = true;
            }
        }

        private void textBoxQuery_TextChanged(object sender, EventArgs e)
        {
            haveQueryControlsBeenModified = true;
        }

        private void checkBoxMatchCase_CheckedChanged(object sender, EventArgs e)
        {
            haveQueryControlsBeenModified = true;

        }
    }
}
