using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using HELLION.DataStructures.Blueprints;
using HELLION.DataStructures.StaticData;
using static HELLION.DataStructures.Blueprints.StationBlueprint;

namespace HELLION.StationBlueprintEditor
{
    public partial class BlueprintPropertiesForm : Form
    {
        public BlueprintPropertiesForm()
        {
            InitializeComponent();

            PopulateFields();
            PopulateListView();
        }

        private void PopulateFields()
        {
            if (StationBlueprintEditorProgram.DocCurrent != null)
            {
                StationBlueprint blueprint = StationBlueprintEditorProgram.DocCurrent.BlueprintObject;

                label_Version.Text = blueprint.Version.ToString();
                textBox_Name.Text = blueprint.Name;
                textBox_LinkURI.Text = blueprint.LinkURI?.ToString() ?? string.Empty;
                label_StructureCount.Text = blueprint.Structures.Count().ToString();



            }


        }

        private void PopulateListView()
        {
            listView1.Clear();

            if (StationBlueprintEditorProgram.DocCurrent != null)
            {
                listView1.View = View.Details;
                listView1.Columns.Add("Type", 180, HorizontalAlignment.Left);
                listView1.Columns.Add("Count", 50, HorizontalAlignment.Left);
                //listView1.Columns.Add("Power Req.", 50, HorizontalAlignment.Left);
                //listView1.Columns.Add("Press. vol.", 50, HorizontalAlignment.Left);

                var results = StationBlueprintEditorProgram.DocCurrent.BlueprintObject.Structures
                    .GroupBy(x => x.SceneID)
                    .Select(x => new { SceneID = x.Key, Count = x.Distinct().Count() });


                foreach (var result in results.OrderByDescending(r => r.Count).ThenBy(r => r.SceneID.ToString()))
                {

                    string[] arr = new string[2];
                    arr[0] = result.SceneID.ToString();
                    arr[1] = result.Count.ToString();
                    //arr[2] = "0";
                    //arr[3] = "0";

                    ListViewItem liParentItem = new ListViewItem(arr)
                    {
                        Name = result.SceneID.ToString(),
                        Text = result.SceneID.ToString(),
                        //Tag = selectedNode.Parent,
                        //ImageIndex = EmbeddedImages_ImageList.GetIconImageIndexByNodeType(nodeParent.NodeType)
                    };
                    // Add the item
                    listView1.Items.Add(liParentItem);


                }

                //IEnumerable<BlueprintStructure> results = blueprint.Structures
                //    .GroupBy(r => r.SceneID)

                //    .Select(r => new { StructureType = r.Key, Count = r.Count() })
                //    .AsEnumerable<BlueprintStructure>();



            }




        }




        private void button1_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
