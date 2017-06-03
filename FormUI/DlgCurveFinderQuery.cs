using ESRI.ArcGIS.NetworkAnalysis;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Framework;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.ArcMapUI;
using ESRI.ArcGIS.Display;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using ClassLib.curves;
using ClassLib.enums;
using ClassLib.segment;



namespace FormUI
{
    public partial class DlgCurveFinderQuery : Form
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
        public DlgCurveFinderQuery()
        {
            InitializeComponent();
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

        private void finish(string msg)
        {
            Cursor = Cursors.Default;
            MessageBox.Show(msg);
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
                FeatureLayer layer = this.run.go();
                if (this.run.success && layer != null)
                {
                    this.m_pMap.AddLayer(layer);
                    finish("Curves Identified!");
                    btIdentifyCurveAreas.Enabled = false;
                    btIdentify.Enabled = true;
                }
                else
                {
                    finish(this.run.errorMsg);
                }
            }
            catch (System.IO.IOException ex)
            {
                finish(ex.Message);
            }

            //Set back the default Cursor
            

            //Conclusion
            //btIdentifyCurveAreas.Enabled = false;
            //btIdentify.Enabled = true;


            

            ////string strSelLayerName = cbLayers.SelectedItem.ToString();
            ////Int32 nSelectedLayerIndex = ClassLib.CurveAlgo.GetIndexNumberFromLayerName(m_pActiveView, strSelLayerName);
            ////IFeatureLayer pSelLayer = (IFeatureLayer)this.m_pMap.get_Layer(nSelectedLayerIndex);


            //ClassLib.IdentifyCurves idCurves = new ClassLib.IdentifyCurves(
            //    pSelLayer.FeatureClass,
            //    (double)angleSpinner.Value,
            //    cbDisslv.Checked
            //    );

            ////List<string> errorMsgs = idCurves.RunCurves(
            ////    rIDField.Text
            ////    );



            //if (errorMsgs.Count > 0)
            //{
            //    string msg = "";
            //    foreach (string s in errorMsgs){
            //        msg += s + "\n";
            //    }
            //    MessageBox.Show(msg);
            //    return;
            //}

            //IFeatureLayer newLayer = idCurves.MakeOutputLayer(this.txtOutput.Text);
            //if (newLayer == null)
            //{
            //    MessageBox.Show("Problem creating the output feature class");
            //}
            //else
            //{
                
            //}



            //MessageBox.Show("Curves Identified!");   
            //this.Hide();
        }

        private void cbLayers_SelectedIndexChanged(object sender, EventArgs e)
        {
            IFeatureClass fClass = this.GetSelectedFClass();

            if (fClass != null)
            {
                this.run = new ClassLib.Run(fClass, this.cbDisslv.Checked, this.angleSpinner.Value);

                if (this.run.feetOrMeters)
                {
                    this.cbFeet.Checked = this.run.isFeet;
                    this.txtOutput.Text = this.run.outputPath;
                    this.rIDField.Items.AddRange(this.run.fieldNames.ToArray());

                    if (this.rIDField.Items.Count > 0)
                    {
                        this.rIDField.SelectedIndex = 0;
                        this.btIdentifyCurveAreas.Enabled = true;
                        this.run.routeIdField = this.rIDField.SelectedItem as string;
                    }
                }
                else
                {
                    MessageBox.Show("The input feature units must be in either meters or feet");
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
     }
}
