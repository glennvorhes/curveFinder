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
        private IFeatureClass fClass;

        public Form1()
        {
            this.fClass = null;
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
                this.fClass = null;
            }
            else
            {
                this.txtInput.Text = nextObj.FullName;
                this.txtOutput.Text = ClassLib.Helpers.makeOutputPath(this.txtInput.Text, this.AngleVariation.Value);


                this.fClass = ClassLib.Workspace.getFeatureClass(this.txtInput.Text);
                Boolean? isFeet = ClassLib.Helpers.isFeetFromFc(this.fClass);

                if (isFeet == null)
                {
                    MessageBox.Show("The input coordinate system must be in meters or feet");
                    return;
                }

                this.cbFeet.Checked = (bool)isFeet;

                IFields fields = this.fClass.Fields;
                IField field = null;

                for (int i = 0; i < fields.FieldCount; i++)
                {
                    // Get the field at the given index.
                    field = fields.get_Field(i);
                    rIDField.Items.Add(field.Name);
                }
            }
        }

        private void reset()
        {
            this.AngleVariation.Value = 1.0M;
            this.txtInput.Text = "";
            this.txtInput.Text = "";
            this.cbDisslv.Checked = true;
            this.cbFeet.Checked = true;
            this.btIdentifyCurveAreas.Enabled = false;


        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void btIdentifyCurveAreas_Click(object sender, EventArgs e)
        {
            if (this.fClass == null)
            {
                return;
            }

            Cursor = Cursors.WaitCursor;

            double ang = (double)this.AngleVariation.Value;

            ClassLib.IdentifyCurves curv = new ClassLib.IdentifyCurves(this.fClass, ang, cbDisslv.Checked);
            
            string errors = curv.RunCurves(this.rIDField.SelectedItem.ToString());
            curv.MakeOutputFeatureClass(this.txtOutput.Text);

            Cursor = Cursors.Default;

            MessageBox.Show("Curves Found");
            this.reset();
        }

        private void NUDMaxAngleVariationInATangent_ValueChanged(object sender, EventArgs e)
        {
            this.txtOutput.Text = ClassLib.Helpers.makeOutputPath(this.txtInput.Text, this.AngleVariation.Value);
        }

        private void rIDField_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.btIdentifyCurveAreas.Enabled = true;
        }
    }
}
