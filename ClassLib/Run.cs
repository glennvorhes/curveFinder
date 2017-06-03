using ESRI.ArcGIS.Geodatabase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLib
{
    public class Run
    {
        string _inputPath;
        string _outputPath;
        string _routeIdField;
        int _routeIdIndex = -1;
        double _angle = 1;
        bool _isDissolved;
        bool _isRun = false;

        IFeatureClass _fClass;
        List<string> errorList = new List<string>();

        private Run(bool isDissolved, double angle, string routeIdField)
        {
            this._isDissolved = isDissolved;
            this._angle = angle;
            this.routeIdField = routeIdField;
        }

        public Run(string inputPath, bool isDissolved = true, double angle = 1.0, string routeIdField = null) :
            this(isDissolved, angle, routeIdField)
        {
            this._inputPath = inputPath;
            this._fClass = Workspace.getFeatureClass(inputPath);
            this._outputPath = Helpers.makeOutputPath(this._inputPath, this._angle);
        }

        public Run(string inputPath, bool isDissolved = true, decimal angle = 1.0M, string routeIdField = null) :
            this(inputPath, isDissolved, (double)angle, routeIdField) { }


        public Run(IFeatureClass fClass, bool isDissolved = true, double angle = 1.0, string routeIdField = null)
            : this(isDissolved, angle, routeIdField)
        {
            if (fClass != null)
            {
                this._fClass = fClass;
                IDataset ds = (IDataset)fClass;

                this._inputPath = System.IO.Path.Combine(ds.Workspace.PathName, ds.Name);

                if (ds.Category.ToLower().IndexOf("shapefile") > -1)
                {
                    this._inputPath += ".shp";
                }

                this._outputPath = Helpers.makeOutputPath(this._inputPath, this._angle);

                //string outputPath = ClassLib.Helpers.makeOutputPath(inputPath, (double)this.angleSpinner.Value);
                //this.txtOutput.Text = outputPath;



                //Boolean? isFeet = ClassLib.Helpers.isFeetFromFc(fClass);

                //if (isFeet == null)
                //{
                //    MessageBox.Show("The input feature units must be in either meters or feet");
                //    this.rIDField.Items.Clear();
                //    this.btIdentifyCurveAreas.Enabled = false;
                //}
                //else
                //{
                //    this.cbFeet.Checked = (bool)isFeet;
                //}

                //return System.IO.Path.GetFileName(outputPath);
            }
        }

        public Run(IFeatureClass fClass, bool isDissolved = true, decimal angle = 1.0M, string routeIdField = null) :
            this(fClass, isDissolved, (double)angle, routeIdField) { }



        public bool feetOrMeters
        {
            get
            {
                return Helpers.isFeetFromFc(this._fClass) != null;
            }
        }

        public bool isFeet
        {
            get
            {
                if (!this.feetOrMeters)
                {
                    throw new Exception("should check if feet or meters first");
                }

                return (bool)Helpers.isFeetFromFc(this._fClass);
            }
        }

        public bool isDissolved
        {
            get
            {
                return this._isDissolved;
            }
        }

        public double angle
        {
            get { return this._angle; }
            set
            {
                this._isRun = false;
                this._angle = value;
                this._outputPath = Helpers.makeOutputPath(this._inputPath, this._angle);
            }
        }

        public decimal decimalAngle
        {
            set
            {
                this.angle = (double)value;
            }
        }

        public string inputPath
        {
            get
            {
                return this._inputPath;
            }
        }

        public string outputPath
        {
            get
            {
                return this._outputPath;
            }
        }

        public bool success
        {
            get
            {
                return this._isRun && this.errorList.Count == 0;
            }
        }

        public string errorMsg
        {
            get
            {
                string outMsg = "";

                foreach (string s in this.errorList)
                {
                    outMsg += s + "\n";
                }

                return outMsg;
            }

        }

        public List<string> fieldNames
        {
            get
            {
                List<string> fieldList = new List<string>();
                IFields fields = this._fClass.Fields;

                for (int i = 0; i < fields.FieldCount; i++)
                {
                    IField field = fields.get_Field(i);
                    if (field.Name == "Shape")
                    {
                        continue;
                    }

                    fieldList.Add(field.Name);

                }

                return fieldList;
            }
        }

        public string routeIdField
        {
            get
            {
                return this._routeIdField;
            }
            set
            {
                if (value == this.routeIdField)
                {
                    return;
                }

                this._isRun = false;
                this._routeIdField = value;
                this._routeIdIndex = -1;

                if (value == null)
                {
                    return;
                }

                IFields fields = this._fClass.Fields;

                for (int i = 0; i < fields.FieldCount; i++)
                {
                    IField field = fields.get_Field(i);

                    if (field.Name == value)
                    {
                        this._routeIdIndex = i;
                    }
                }
            }
        }



        public ESRI.ArcGIS.Carto.FeatureLayer go()
        {
            if (this._isRun || this._routeIdIndex == -1)
            {
                return null;
            }

            this.errorList.Clear();

            IdentifyCurves curv = new IdentifyCurves(this._fClass, this.angle, this.isDissolved);

            this.errorList = curv.RunCurves(this.routeIdField);

            this._isRun = true;

            if (this.success)
            {
                IFeatureClass outFeatureClass = curv.MakeOutputFeatureClass(this.outputPath);

                string layerName = System.IO.Path.GetFileName(this.outputPath).Replace(".shp", "");
                ESRI.ArcGIS.Carto.FeatureLayer layer = new ESRI.ArcGIS.Carto.FeatureLayer();
                layer.Name = layerName;
                layer.FeatureClass = outFeatureClass;
                return layer;
            }
            else
            {
                return null;
            }
        }
    }
}
