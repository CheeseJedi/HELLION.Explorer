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
    public partial class ObservedGuidsForm : Form
    {
        public ObservedGuidsForm()
        {
            InitializeComponent();
            listBox1.DataSource = DataStructures.HEGuidManager.ObservedGuids;
            //listBox1.DisplayMember = 

        }

        private void buttonClose_Click(object sender, EventArgs e)
        {
            Hide();
        }
    }
}
