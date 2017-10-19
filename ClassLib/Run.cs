using ESRI.ArcGIS.Geodatabase;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        double _angle = 1;
        bool _isDissolved;
        bool _isRun = false;
        bool _isShapefile;
        ESRI.ArcGIS.Carto.FeatureLayer outputLayer = null;
        IFeatureClass _fClass;
        List<string> errorList = new List<string>();
        string NO_HIGHWAY = " <<none>>";

        /// <summary>
        /// private helper constructor
        /// </summary>
        /// <param name="isDissolved">if the features are dissolved on highway name</param>
        /// <param name="angle">threshold angle for curve identification</param>
        /// <param name="roadNameField">field name used to identify the road</param>
        private Run(bool isDissolved, double angle, string roadNameField)
        {
            this.isDissolved = isDissolved;
            this.angle = angle;
            this._roadNameField = roadNameField == null ? Fields.NO_HIGHWAY : roadNameField;
        }

        private Run(string inputPath, double angle = 1.0, string roadNameField = Fields.NO_HIGHWAY, bool isDissolved = true)
            : this(isDissolved, angle, roadNameField)
        {
            this._inputPath = inputPath.Trim();
            this._isShapefile = (new System.Text.RegularExpressions.Regex(".shp$")).IsMatch(this.inputPath);
            this._fClass = Workspace.getFeatureClass(inputPath);
        }


        /// <summary>
        /// Constructor taking input path as a string and other parameters
        /// </summary>
        /// <param name="inputPath">path to input feature class</param>
        /// <param name="angle">threshold angle for curve identification, as double</param>
        /// <param name="roadNameField">field name used to identify the road</param>
        /// <param name="isDissolved">if the features are dissolved on highway name</param>
        /// <param name="outWksp">output workspace</param>
        /// <exception cref="System.IO.FileNotFoundException">Thrown when input not found</exception>
        public Run(string inputPath, double angle = 1.0, string roadNameField = Fields.NO_HIGHWAY, bool isDissolved = true, string outWksp = null) :
            this(inputPath, angle, roadNameField, isDissolved)
        {
            this._outputPath = Helpers.makeOutputPath(this._inputPath, this._angle, outWksp);
        }

        /// <summary>
        /// Constructor taking input path as a string and other parameters, angle as decimal
        /// </summary>
        /// <param name="inputPath">path to input feature class</param>
        /// <param name="angle">threshold angle for curve identification, as decimal</param>
        /// <param name="roadNameField">field name used to identify the road</param>
        /// <param name="isDissolved">if the features are dissolved on highway name</param>
        /// <param name="outWksp">output workspace</param>
        /// <exception cref="System.IO.FileNotFoundException">Thrown when input not found</exception>
        public Run(string inputPath, decimal angle = 1.0M, string roadNameField = Fields.NO_HIGHWAY, bool isDissolved = true, string outWksp = null) :
            this(inputPath, (double)angle, roadNameField, isDissolved, outWksp) { }


        /// <summary>
        /// Constructor taking input and ouput path as a string and other parameters
        /// </summary>
        /// <param name="inputPath">path to input feature class</param>
        /// <param name="outFeatureClass">output workspace</param>
        /// <param name="angle">threshold angle for curve identification, as double</param>
        /// <param name="roadNameField">field name used to identify the road</param>
        /// <param name="isDissolved">if the features are dissolved on highway name</param>
        /// <exception cref="System.IO.FileNotFoundException">Thrown when input not found</exception>
        public Run(string inputPath, string outFeatureClass, double angle = 1.0, string roadNameField = Fields.NO_HIGHWAY, bool isDissolved = true) :
            this(inputPath, angle, roadNameField, isDissolved)
        {
            this._outputPath = outFeatureClass;
        }

        /// <summary>
        /// Constructor taking input and ouput path as a string and other parameters, angle as a decimal
        /// </summary>
        /// <param name="inputPath">path to input feature class</param>
        /// <param name="outFeatureClass">output workspace</param>
        /// <param name="angle">threshold angle for curve identification, as double</param>
        /// <param name="roadNameField">field name used to identify the road</param>
        /// <param name="isDissolved">if the features are dissolved on highway name</param>
        /// <exception cref="System.IO.FileNotFoundException">Thrown when input not found</exception>
        public Run(string inputPath, string outFeatureClass, decimal angle = 1.0M, string roadNameField = Fields.NO_HIGHWAY, bool isDissolved = true) :
            this(inputPath, outFeatureClass, (double)angle, roadNameField, isDissolved) { }


        /// <summary>
        /// Constructor taking input as a feature class and other parameters
        /// </summary>
        /// <param name="fClass">input feature class</param>
        /// <param name="angle">threshold angle for curve identification, as double</param>
        /// <param name="roadNameField">field name used to identify the road</param>
        /// <param name="isDissolved">if the features are dissolved on highway name</param>
        /// <exception cref="System.IO.FileNotFoundException">Thrown when input not found</exception>
        public Run(IFeatureClass fClass, double angle = 1.0, string roadNameField = Fields.NO_HIGHWAY, bool isDissolved = true, string outWksp=null)
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

                this._outputPath = Helpers.makeOutputPath(this._inputPath, this._angle, outWksp);
            }
        }

        /// <summary>
        /// Constructor taking input as a feature class and other parameters
        /// </summary>
        /// <param name="fClass">input feature class</param>
        /// <param name="angle">threshold angle for curve identification, as decimal</param>
        /// <param name="roadNameField">field name used to identify the road</param>
        /// <param name="isDissolved">if the features are dissolved on highway name</param>
        /// <exception cref="System.IO.FileNotFoundException">Thrown when input not found</exception>
        public Run(IFeatureClass fClass, decimal angle = 1.0M, string roadNameField = Fields.NO_HIGHWAY, bool isDissolved = true, string outWksp = null) :
            this(fClass, (double)angle, roadNameField, isDissolved, outWksp) { }

        /// <summary>
        /// run the finder alogrithm
        /// </summary>
        /// <returns></returns>
        public ESRI.ArcGIS.Carto.FeatureLayer go()
        {
            if (this._isRun && this.outputLayer != null)
            {
                return this.outputLayer;
            }

            this.errorList.Clear();

            IdentifyCurves curv = new IdentifyCurves(this._fClass, this.angle, this.isDissolved);

            this.errorList = curv.RunCurves(this.roadNameField == NO_HIGHWAY ? null : this.roadNameField);

            this._isRun = true;

            if (this.success)
            {
                IFeatureClass outFeatureClass = curv.MakeOutputFeatureClass(this.outputPath);

                string layerName = System.IO.Path.GetFileName(this.outputPath).Replace(".shp", "");
                this.outputLayer = new ESRI.ArcGIS.Carto.FeatureLayer();
                this.outputLayer.Name = layerName;
                this.outputLayer.FeatureClass = outFeatureClass;
                return this.outputLayer;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// run the algorithm, helper to set road name field first
        /// </summary>
        /// <param name="roadNameField">field identifiying the road name</param>
        /// <returns></returns>
        public ESRI.ArcGIS.Carto.FeatureLayer go(string roadNameField)
        {
            this.roadNameField = roadNameField;
            return this.go();
        }

        /// <summary>
        /// get if input units is feet or meters, if not the algorithm can't/shouln't be run
        /// </summary>
        public bool isFeetOrMeters
        {
            get
            {
                return Helpers.isFeetFromFc(this._fClass) != null;
            }
        }

        /// <summary>
        /// if input units is feet
        /// </summary>
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

        /// <summary>
        /// if the input is a shapefile
        /// </summary>
        public bool isShapefile
        {
            get
            {
                return this._isShapefile;
            }
        }

        /// <summary>
        /// if the input features are dissolved on highway name
        /// </summary>
        public bool isDissolved
        {
            get
            {
                return this._isDissolved;
            }
            set
            {
                this._isDissolved = value;
                this.isRun = false;
            }
        }



        /// <summary>
        /// curve angle threshold
        /// </summary>
        public double angle
        {
            get { return this._angle; }
            set
            {
                if (value > 0 && value <= 10)
                {
                    this.isRun = false;
                    this._angle = value;
                    if (this._inputPath != null)
                    {
                        this._outputPath = Helpers.makeOutputPath(this._inputPath, this._angle);
                    }
                }
                else
                {
                    throw new ArgumentOutOfRangeException("Angle threshold must be between in range 0 < angle <= 10");
                }
            }
        }

        /// <summary>
        /// helper to set the threshold angle as a decimal
        /// </summary>
        public decimal decimalAngle
        {
            set
            {
                this.angle = (double)value;
            }
        }

        /// <summary>
        /// full path to the input feature class
        /// </summary>
        public string inputPath
        {
            get
            {
                return this._inputPath;
            }
        }

        /// <summary>
        /// full path to output feature class
        /// </summary>
        public string outputPath
        {
            get
            {
                return this._outputPath;
            }
        }

        /// <summary>
        /// if the run was successful
        /// </summary>
        public bool success
        {
            get
            {
                return this._isRun && this.errorList.Count == 0;
            }
        }

        /// <summary>
        /// concatentated error message
        /// </summary>
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

        /// <summary>
        /// list of field names
        /// </summary>
        public List<string> fieldNames
        {
            get
            {
                List<string> fieldList = new List<string>(){this.NO_HIGHWAY};
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

        /// <summary>
        /// the selected road name field
        /// </summary>
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
            }
        }

        private bool isRun
        {
            set
            {
                this._isRun = value;
                if (!this._isRun)
                {
                    this.outputLayer = null;
                }
            }
        }
    }
}
