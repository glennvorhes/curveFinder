using ESRI.ArcGIS.CatalogUI;
using ESRI.ArcGIS.Geodatabase;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FormUI
{
    public partial class Form1 : Form
    {
        private ClassLib.Run run;

        public Form1()
        {
            
            InitializeComponent();
            ToolTip tip = new ToolTip();
            tip.SetToolTip(this.cbFeet, "This checkbox is read only");
        }

        private void openFileDialogue(object sender, EventArgs e)
        {

            IGxDialog igxDialog = new GxDialog();
            igxDialog.AllowMultiSelect = false;
            igxDialog.ObjectFilter = new ESRI.ArcGIS.Catalog.GxFilterPolylineFeatureClasses();

            ESRI.ArcGIS.Catalog.IEnumGxObject selection;
            igxDialog.DoModalOpen(this.Handle.ToInt32(), out selection);

            ESRI.ArcGIS.Catalog.IGxObject nextObj = selection.Next();

            this.rIDField.Items.Clear();

            if (nextObj == null)
            {
                this.txtInput.Clear();
                this.txtOutput.Clear();
                this.run = null;
            }
            else
            {
                this.run = new ClassLib.Run(nextObj.FullName, this.AngleVariation.Value, isDissolved: cbDisslv.Checked);
                this.txtInput.Text = this.run.inputPath;
                this.txtOutput.Text = this.run.outputPath;


                if (!run.isFeetOrMeters)
                {
                    MessageBox.Show("The input coordinate system must be in meters or feet");
                    return;
                }

                this.cbFeet.Checked = this.run.isFeet;

                this.rIDField.Items.Clear();
                this.rIDField.Items.AddRange(run.fieldNames.ToArray());

                if (this.rIDField.Items.Count > 0)
                {
                    this.rIDField.SelectedIndex = 0;
                }
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void btIdentifyCurveAreas_Click(object sender, EventArgs e)
        {
            Cursor = Cursors.WaitCursor;
            try
            {
                this.run.go();
            }
            catch (System.IO.IOException ex)
            {
                MessageBox.Show(ex.Message);
            }

            Cursor = Cursors.Default;

            this.btIdentifyCurveAreas.Enabled = false;

            if (this.run.success)
            {
                MessageBox.Show("Curves Found, output to " + this.run.outputPath);
            }
            else
            {
                MessageBox.Show(this.run.errorMsg);
            }

        }

        private void NUDMaxAngleVariationInATangent_ValueChanged(object sender, EventArgs e)
        {
            this.run.decimalAngle = this.AngleVariation.Value;
            this.txtOutput.Text = this.run.outputPath;
            this.btIdentifyCurveAreas.Enabled = true;
        }

        private void rIDField_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.run.roadNameField = this.rIDField.SelectedItem as string;
            this.btIdentifyCurveAreas.Enabled = true;
        }
    }
}
