using ESRI.ArcGIS.ArcMapUI;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Framework;
using ESRI.ArcGIS.Geodatabase;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ClassLib.forms
{
    public partial class AddInForm : Form
    {
        //variables
        private IMap m_pMap;
        private IApplication m_pApp = null;
        private IActiveView m_pActiveView = null;
        private IMxDocument m_pMxDocument = null;
        private IFeatureLayer m_pCurrCurveLayer = null;
        private IFeatureLayer m_pCurrCurveAreaLayer = null;

        //constants
        private int MinNumofSegmentsInACurve;//number of segments
        private double MaxAngleBetweenConsecutiveSegmentsInACurve;//degree
        private double MinAngleToStartACurve;//degree
        private double MinAngleBetweenConsecutiveSegmentsInACurve;//degree
        private double MaxAllowableCounterAngleBetweenConsecutiveSegmentsInACurve;//dgree
        private int MaxAllowableNumOfConsecutiveCounterAngleSegmentsInACurve;//number of segments
        private double MaxAllowableAngleDifferenceBetweenConsecutiveSegments;//degree intersection
        //for curving area
        private int MinNumOfSegmentsToFormATangent;//number of segments
        private double MaxAngleVariationInATangent;//degree
        private int MinNumOfSegmentsInACurveArea;//number of segments
        private double MinAngleToStartACurveArea;//degree
        private double MinLengthOfTangent;//meter
        //filter
        private double MinFilterCentralAngle;
        private double MaxFilterRadious;
        private double MaxFilterLengthDiff;

        ClassLib.Run run = null;

        //properties
        public IMap theMap
        {
            get
            {
                return m_pMap;
            }
            set
            {
                m_pMap = value;
            }
        }

        public IApplication theApp
        {
            get
            {
                return m_pApp;
            }
            set
            {
                m_pApp = value;
            }
        }

        public IActiveView theActiveView
        {
            get
            {
                return m_pActiveView;
            }
            set
            {
                m_pActiveView = value;
            }
        }

        public IMxDocument theMxDoc
        {
            get
            {
                return m_pMxDocument;
            }
            set
            {
                m_pMxDocument = value;
            }
        }

        //constructor
        public AddInForm()
        {
            InitializeComponent();
            ToolTip tip = new ToolTip();
            tip.SetToolTip(this.cbFeet, "This checkbox is read only");
        }

        //initialization
        private void DlgCurveFinderQuery_Load(object sender, EventArgs e)
        {
            IEnumLayer pEnumLayer = m_pMap.get_Layers(null, true);


            pEnumLayer.Reset();
            ILayer pLayer;
            while ((pLayer = pEnumLayer.Next()) != null)
            {
                if (pLayer is IFeatureLayer)
                {
                    IFeatureLayer fLayer = (IFeatureLayer)pLayer;
                    if (fLayer.FeatureClass.ShapeType == ESRI.ArcGIS.Geometry.esriGeometryType.esriGeometryPolyline)
                    {
                        cbLayers.Items.Add(pLayer.Name);
                    }
                }
            }

            if (cbLayers.Items.Count > 0)
            {
                cbLayers.SelectedIndex = 0;
            }

            //old spin control
            //NUDMinNumofSegmentsInACurve.Value = 2;//number of segmen
            //NUDMaxAngleBetweenConsecutiveSegmentsInACurve.Value = 70;//degree
            //NUDMinAngleToStartACurve.Value = 5;//degree
            //NUDMinAngleBetweenConsecutiveSegmentsInACurve.Value = 2;//degree
            //NUDMaxAllowableCounterAngleBetweenConsecutiveSegmentsInACurve.Value = 0;//dgree
            //NUDMaxAllowableNumOfConsecutiveCounterAngleSegmentsInACurve.Value = 1;//number of segments

            //new spin control
            angleSpinner.Value = 1;
            btIdentify.Enabled = false;
            //btIdentifyCurveAreas.Enabled = false;

            cbDisslv.Checked = true;


        }

        private IFeatureClass GetSelectedFClass()
        {
            if (cbLayers.Items.Count == 0)
            {
                return null;
            }

            string strSelLayerName = cbLayers.SelectedItem.ToString();
            System.Int32 nSelectedLayerIndex = ClassLib.CurveAlgo.GetIndexNumberFromLayerName(m_pActiveView, strSelLayerName);
            IFeatureLayer pSelLayer = (IFeatureLayer)this.m_pMap.get_Layer(nSelectedLayerIndex);
            //get the featureclass
            IFeatureClass pFeatureClass = pSelLayer.FeatureClass;
            return pFeatureClass;
        }

        private void finish(string msg, string caption)
        {
            Cursor = Cursors.Default;
            MessageBox.Show(msg, caption);
        }


        private void btIdentifyCurveAreas_Click(object sender, EventArgs e)
        {
            if (this.run == null)
            {
                return;
            }

            Cursor = Cursors.WaitCursor;

            try
            {
                FeatureLayer layer = this.run.go((string)this.rIDField.SelectedItem);
                if (this.run.success && layer != null)
                {
                    this.m_pMap.AddLayer(layer);
                    finish("Curves Identified.\nOutput to\n" + this.run.outputPath, "Success");
                    btIdentifyCurveAreas.Enabled = false;
                    btIdentify.Enabled = true;
                }
                else
                {
                    finish(this.run.errorMsg, "Error");
                }
            }
            catch (System.IO.IOException ex)
            {
                finish(ex.Message, "Error");
            }
        }

        private void cbLayers_SelectedIndexChanged(object sender, EventArgs e)
        {
            IFeatureClass fClass = this.GetSelectedFClass();

            if (fClass != null)
            {
                this.run = new ClassLib.Run(fClass, this.angleSpinner.Value, isDissolved: this.cbDisslv.Checked);

                if (this.run.isFeetOrMeters)
                {
                    this.cbFeet.Checked = this.run.isFeet;
                    this.txtOutput.Text = this.run.outputPath;
                    this.rIDField.Items.Clear();
                    this.rIDField.Items.AddRange(this.run.fieldNames.ToArray());

                    if (this.rIDField.Items.Count > 0)
                    {
                        this.rIDField.SelectedIndex = 0;
                        this.btIdentifyCurveAreas.Enabled = true;
                    }
                }
                else
                {
                    MessageBox.Show("The input feature units must be in either meters or feet", "Invalid Coordinate System");
                    this.rIDField.Items.Clear();
                    this.btIdentifyCurveAreas.Enabled = false;
                }
            }
            else
            {
                this.run = null;
                this.txtOutput.Text = "";
                btIdentifyCurveAreas.Enabled = false;
            }
        }

        private void rIDField_SelectedIndexChanged(object sender, EventArgs e)
        {
            btIdentifyCurveAreas.Enabled = true;
        }

        private void angleSpinner_ValueChanged(object sender, EventArgs e)
        {
            if (this.run != null)
            {
                this.run.decimalAngle = this.angleSpinner.Value;
                this.txtOutput.Text = this.run.outputPath;
                this.btIdentifyCurveAreas.Enabled = true;
            }
        }

        private void cbDisslv_CheckedChanged(object sender, EventArgs e)
        {
            if (this.run != null)
            {
                this.run.isDissolved = this.cbDisslv.Checked;
            }
            
            this.btIdentifyCurveAreas.Enabled = true;
        }
    }
}
