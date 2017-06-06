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
        string _roadNameField = null;
        int _roadNameFieldIndex = -1;
        double _angle = 1;
        bool _isDissolved;
        bool _isRun = false;
        private bool _isShapefile;

        IFeatureClass _fClass;
        List<string> errorList = new List<string>();

        private Run(bool isDissolved, double angle, string roadNameField)
        {
            this._isDissolved = isDissolved;
            this._angle = angle;
            this._roadNameField = roadNameField;
        }

        public Run(string inputPath, double angle = 1.0, string roadNameField = null, bool isDissolved = true) :
            this(isDissolved, angle, roadNameField)
        {
            this._inputPath = inputPath.Trim();
            this._isShapefile = (new System.Text.RegularExpressions.Regex(".shp$")).IsMatch(this.inputPath);
            this._fClass = Workspace.getFeatureClass(inputPath);
            this._outputPath = Helpers.makeOutputPath(this._inputPath, this._angle);
        }

        public Run(string inputPath, decimal angle = 1.0M, string roadNameField = null, bool isDissolved = true) :
            this(inputPath, (double)angle, roadNameField, isDissolved) { }


        public Run(IFeatureClass fClass, double angle = 1.0, string roadNameField = null, bool isDissolved = true)
            : this(isDissolved, angle, roadNameField)
        {
            if (fClass != null)
            {
                this._fClass = fClass;
                IDataset ds = (IDataset)fClass;

                this._inputPath = System.IO.Path.Combine(ds.Workspace.PathName, ds.Name);

                if (ds.Category.ToLower().IndexOf("shapefile") > -1)
                {
                    this._inputPath += ".shp";
                    this._isShapefile = true;
                }

                this._outputPath = Helpers.makeOutputPath(this._inputPath, this._angle);
            }
        }

        public Run(IFeatureClass fClass, decimal angle = 1.0M, string roadNameField = null,  bool isDissolved = true) :
            this(fClass, (double)angle, roadNameField, isDissolved) { }



        public bool isFeetOrMeters
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
                if (!this.isFeetOrMeters)
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

        public bool isShapefile
        {
            get
            {
                return this._isShapefile;
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

                    if ((new System.Text.RegularExpressions.Regex("^object|fid|shape")).IsMatch(field.Name.ToLower()))
                    {
                        continue;
                    }

                    fieldList.Add(field.Name);

                }

                return fieldList;
            }
        }

        public string roadNameField
        {
            get
            {
                return this._roadNameField;
            }
            set
            {
                if (value == this.roadNameField)
                {
                    return;
                }

                this._isRun = false;
                this._roadNameField = value;
                this._roadNameFieldIndex = -1;

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
                        this._roadNameFieldIndex = i;
                    }
                }
            }
        }



        public ESRI.ArcGIS.Carto.FeatureLayer go()
        {
            if (this._isRun)
            {
                return null;
            }

             //|| this._roadNameFieldIndex == -1

            this.errorList.Clear();

            IdentifyCurves curv = new IdentifyCurves(this._fClass, this.angle, this.isDissolved);

            this.errorList = curv.RunCurves(this.roadNameField);

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
