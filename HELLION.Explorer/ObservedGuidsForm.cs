using System;
using System.Windows.Forms;
using HELLION.DataStructures.Document;

namespace HELLION.Explorer
{
    public partial class ObservedGuidsForm : Form
    {
        public ObservedGuidsForm()
        {
            InitializeComponent();
            listBox1.DataSource = GuidManager.ObservedGuids;
            //listBox1.DisplayMember = 

        }

        private void buttonClose_Click(object sender, EventArgs e)
        {
            Hide();
        }
    }
}
