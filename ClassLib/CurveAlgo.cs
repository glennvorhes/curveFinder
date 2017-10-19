using ClassLib.curves;
using ClassLib.enums;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLib
{
    public static class CurveAlgo
    {
        private static IObjectClassDescription ocDesc = (IObjectClassDescription)(new FeatureClassDescriptionClass());

        private static IFields GetCurveAreaFields(bool isShape, bool isDissolved)
        {
            IFields fields = new FieldsClass();
            IFieldsEdit fieldsEdit = (IFieldsEdit)fields;
            if (isDissolved)
            {
                fieldsEdit.FieldCount_2 = 13;
            }
            else
            {
                fieldsEdit.FieldCount_2 = 20;
            }

            // Create Object ID field.
            IField fieldUserDefined = new Field();
            IFieldEdit fieldEdit = (IFieldEdit)fieldUserDefined;
            if (isShape)
            {
                fieldEdit.Name_2 = "FID";
                fieldEdit.AliasName_2 = "FID";
            }
            else
            {
                fieldEdit.Name_2 = "OBJECTID";
                fieldEdit.AliasName_2 = "OBJECTID";
            }
            
            fieldEdit.Type_2 = esriFieldType.esriFieldTypeOID;
            fieldsEdit.set_Field(0, fieldUserDefined);

            // Create Shape field.
            fieldUserDefined = new Field();
            fieldEdit = (IFieldEdit)fieldUserDefined;
            // Set up geometry definition for the Shape field.
            // You do not have to set the spatial reference, as it is inherited from the feature dataset.
            IGeometryDef geometryDef = new GeometryDefClass();
            IGeometryDefEdit geometryDefEdit = (IGeometryDefEdit)geometryDef;
            geometryDefEdit.GeometryType_2 = ESRI.ArcGIS.Geometry.esriGeometryType.esriGeometryPolyline;
            // By setting the grid size to 0, you are allowing ArcGIS to determine the appropriate grid sizes for the feature class. 
            // If in a personal geodatabase, the grid size is 1,000. If in a file or ArcSDE geodatabase, the grid size
            // is based on the initial loading or inserting of features.
            geometryDefEdit.GridCount_2 = 1;
            geometryDefEdit.set_GridSize(0, 0);
            geometryDefEdit.HasM_2 = false;
            geometryDefEdit.HasZ_2 = false;
            // Set standard field properties
            fieldEdit.Name_2 = "SHAPE";
            fieldEdit.Type_2 = esriFieldType.esriFieldTypeGeometry;
            fieldEdit.GeometryDef_2 = geometryDef;
            fieldEdit.IsNullable_2 = true;
            fieldEdit.Required_2 = true;
            fieldsEdit.set_Field(1, fieldUserDefined);

            if (isDissolved)
            {
                // Create Route_Name Field
                fieldUserDefined = new Field();
                fieldEdit = (IFieldEdit)fieldUserDefined;
                fieldEdit.Name_2 = "ROUTE_NAME";
                fieldEdit.Editable_2 = true;
                fieldEdit.IsNullable_2 = false;
                fieldEdit.Precision_2 = 0;
                fieldEdit.Scale_2 = 5;
                fieldEdit.Type_2 = esriFieldType.esriFieldTypeString;
                fieldsEdit.set_Field(2, fieldUserDefined);

                // Create Route_DIRE Field
                fieldUserDefined = new Field();
                fieldEdit = (IFieldEdit)fieldUserDefined;
                fieldEdit.Name_2 = "ROUTE_DIRE";
                fieldEdit.Editable_2 = true;
                fieldEdit.IsNullable_2 = false;
                fieldEdit.Precision_2 = 0;
                fieldEdit.Scale_2 = 5;
                fieldEdit.Type_2 = esriFieldType.esriFieldTypeString;
                fieldsEdit.set_Field(3, fieldUserDefined);

                // Create FULL_NAME Field
                fieldUserDefined = new Field();
                fieldEdit = (IFieldEdit)fieldUserDefined;
                fieldEdit.Name_2 = "FULL_NAME";
                fieldEdit.Editable_2 = true;
                fieldEdit.IsNullable_2 = false;
                fieldEdit.Precision_2 = 0;
                fieldEdit.Scale_2 = 5;
                fieldEdit.Type_2 = esriFieldType.esriFieldTypeString;
                fieldsEdit.set_Field(4, fieldUserDefined);

                // Create CURVE_ID
                fieldUserDefined = new Field();
                fieldEdit = (IFieldEdit)fieldUserDefined;
                fieldEdit.Name_2 = "CURV_ID";
                fieldEdit.Editable_2 = true;
                fieldEdit.IsNullable_2 = false;
                fieldEdit.Precision_2 = 0;
                fieldEdit.Scale_2 = 5;
                fieldEdit.Type_2 = esriFieldType.esriFieldTypeInteger;
                fieldsEdit.set_Field(5, fieldUserDefined);

                // Create CURV_TYPE Field
                fieldUserDefined = new Field();
                fieldEdit = (IFieldEdit)fieldUserDefined;
                fieldEdit.Name_2 = "CURV_TYPE";
                fieldEdit.Editable_2 = true;
                fieldEdit.IsNullable_2 = false;
                fieldEdit.Precision_2 = 0;
                fieldEdit.Scale_2 = 5;
                fieldEdit.Type_2 = esriFieldType.esriFieldTypeString;
                fieldsEdit.set_Field(6, fieldUserDefined);

                // Create CURV_DIRE Field
                fieldUserDefined = new Field();
                fieldEdit = (IFieldEdit)fieldUserDefined;
                fieldEdit.Name_2 = "CURV_DIRE";
                fieldEdit.Editable_2 = true;
                fieldEdit.IsNullable_2 = false;
                fieldEdit.Precision_2 = 0;
                fieldEdit.Scale_2 = 5;
                fieldEdit.Type_2 = esriFieldType.esriFieldTypeString;
                fieldsEdit.set_Field(7, fieldUserDefined);

                // Create CURV_LENG
                fieldUserDefined = new Field();
                fieldEdit = (IFieldEdit)fieldUserDefined;
                fieldEdit.Name_2 = "CURV_LENG";
                //fieldEdit.AliasName_2 = "Length of the curve area";
                fieldEdit.Editable_2 = true;
                fieldEdit.IsNullable_2 = false;
                fieldEdit.Precision_2 = 2;
                fieldEdit.Scale_2 = 5;
                fieldEdit.Type_2 = esriFieldType.esriFieldTypeDouble;
                fieldsEdit.set_Field(8, fieldUserDefined);

                // Create Radius Field
                fieldUserDefined = new Field();
                fieldEdit = (IFieldEdit)fieldUserDefined;
                fieldEdit.Name_2 = "RADIUS";
                //fieldEdit.AliasName_2 = "Radius of the curve area";
                fieldEdit.Editable_2 = true;
                fieldEdit.IsNullable_2 = false;
                fieldEdit.Precision_2 = 2;
                fieldEdit.Scale_2 = 5;
                fieldEdit.Type_2 = esriFieldType.esriFieldTypeDouble;
                fieldsEdit.set_Field(9, fieldUserDefined);

                // Create DEGREE Field
                fieldUserDefined = new Field();
                fieldEdit = (IFieldEdit)fieldUserDefined;
                fieldEdit.Name_2 = "DEGREE";
                //fieldEdit.AliasName_2 = "Central Angle of the curve area";
                fieldEdit.Editable_2 = true;
                fieldEdit.IsNullable_2 = false;
                fieldEdit.Precision_2 = 3;
                fieldEdit.Scale_2 = 5;
                fieldEdit.Type_2 = esriFieldType.esriFieldTypeDouble;
                fieldsEdit.set_Field(10, fieldUserDefined);

                // Create Transition Field
                fieldUserDefined = new Field();
                fieldEdit = (IFieldEdit)fieldUserDefined;
                fieldEdit.Name_2 = "HAS_TRANS";
                fieldEdit.Editable_2 = true;
                fieldEdit.IsNullable_2 = false;
                fieldEdit.Precision_2 = 0;
                fieldEdit.Scale_2 = 5;
                fieldEdit.Type_2 = esriFieldType.esriFieldTypeString;
                fieldsEdit.set_Field(11, fieldUserDefined);

                // Create INTSC_ANGLE Field
                fieldUserDefined = new Field();
                fieldEdit = (IFieldEdit)fieldUserDefined;
                fieldEdit.Name_2 = "INTSC_ANGLE";
                //fieldEdit.AliasName_2 = "Central Angle of the curve area";
                fieldEdit.Editable_2 = true;
                fieldEdit.IsNullable_2 = false;
                fieldEdit.Precision_2 = 3;
                fieldEdit.Scale_2 = 5;
                fieldEdit.Type_2 = esriFieldType.esriFieldTypeDouble;
                fieldsEdit.set_Field(12, fieldUserDefined);
            }
            else
            {
                // Create taslinkid
                fieldUserDefined = new Field();
                fieldEdit = (IFieldEdit)fieldUserDefined;
                fieldEdit.Name_2 = "TASLINKID";
                fieldEdit.Editable_2 = true;
                fieldEdit.IsNullable_2 = false;
                fieldEdit.Precision_2 = 0;
                fieldEdit.Scale_2 = 5;
                fieldEdit.Type_2 = esriFieldType.esriFieldTypeInteger;
                fieldsEdit.set_Field(2, fieldUserDefined);

                // Create trnlinkid
                fieldUserDefined = new Field();
                fieldEdit = (IFieldEdit)fieldUserDefined;
                fieldEdit.Name_2 = "TRNLINKID";
                fieldEdit.Editable_2 = true;
                fieldEdit.IsNullable_2 = false;
                fieldEdit.Precision_2 = 0;
                fieldEdit.Scale_2 = 5;
                fieldEdit.Type_2 = esriFieldType.esriFieldTypeInteger;
                fieldsEdit.set_Field(3, fieldUserDefined);

                // Create trnnode_f
                fieldUserDefined = new Field();
                fieldEdit = (IFieldEdit)fieldUserDefined;
                fieldEdit.Name_2 = "TRNNODE_F";
                fieldEdit.Editable_2 = true;
                fieldEdit.IsNullable_2 = false;
                fieldEdit.Precision_2 = 0;
                fieldEdit.Scale_2 = 5;
                fieldEdit.Type_2 = esriFieldType.esriFieldTypeInteger;
                fieldsEdit.set_Field(4, fieldUserDefined);

                // Create trnnode_t
                fieldUserDefined = new Field();
                fieldEdit = (IFieldEdit)fieldUserDefined;
                fieldEdit.Name_2 = "TRNNODE_T";
                fieldEdit.Editable_2 = true;
                fieldEdit.IsNullable_2 = false;
                fieldEdit.Precision_2 = 0;
                fieldEdit.Scale_2 = 5;
                fieldEdit.Type_2 = esriFieldType.esriFieldTypeInteger;
                fieldsEdit.set_Field(5, fieldUserDefined);

                // Create rtesys
                fieldUserDefined = new Field();
                fieldEdit = (IFieldEdit)fieldUserDefined;
                fieldEdit.Name_2 = "RTESYS";
                fieldEdit.Editable_2 = true;
                fieldEdit.IsNullable_2 = false;
                fieldEdit.Precision_2 = 0;
                fieldEdit.Scale_2 = 5;
                fieldEdit.Type_2 = esriFieldType.esriFieldTypeInteger;
                fieldsEdit.set_Field(6, fieldUserDefined);

                // Create Route_Name Field
                fieldUserDefined = new Field();
                fieldEdit = (IFieldEdit)fieldUserDefined;
                fieldEdit.Name_2 = "ROUTE_NAME";
                fieldEdit.Editable_2 = true;
                fieldEdit.IsNullable_2 = false;
                fieldEdit.Precision_2 = 0;
                fieldEdit.Scale_2 = 5;
                fieldEdit.Type_2 = esriFieldType.esriFieldTypeString;
                fieldsEdit.set_Field(7, fieldUserDefined);

                // Create Route_DIRE Field
                fieldUserDefined = new Field();
                fieldEdit = (IFieldEdit)fieldUserDefined;
                fieldEdit.Name_2 = "ROUTE_DIRE";
                fieldEdit.Editable_2 = true;
                fieldEdit.IsNullable_2 = false;
                fieldEdit.Precision_2 = 0;
                fieldEdit.Scale_2 = 5;
                fieldEdit.Type_2 = esriFieldType.esriFieldTypeString;
                fieldsEdit.set_Field(8, fieldUserDefined);

                // Create FULL_NAME Field
                fieldUserDefined = new Field();
                fieldEdit = (IFieldEdit)fieldUserDefined;
                fieldEdit.Name_2 = "FULL_NAME";
                fieldEdit.Editable_2 = true;
                fieldEdit.IsNullable_2 = false;
                fieldEdit.Precision_2 = 0;
                fieldEdit.Scale_2 = 5;
                fieldEdit.Type_2 = esriFieldType.esriFieldTypeString;
                fieldsEdit.set_Field(9, fieldUserDefined);

                // Create OFFICIAL_N Field
                fieldUserDefined = new Field();
                fieldEdit = (IFieldEdit)fieldUserDefined;
                fieldEdit.Name_2 = "OFFICIAL_N";
                fieldEdit.Editable_2 = true;
                fieldEdit.IsNullable_2 = false;
                fieldEdit.Precision_2 = 0;
                fieldEdit.Scale_2 = 5;
                fieldEdit.Type_2 = esriFieldType.esriFieldTypeString;
                fieldsEdit.set_Field(10, fieldUserDefined);

                // Create VERS_DATE
                fieldUserDefined = new Field();
                fieldEdit = (IFieldEdit)fieldUserDefined;
                fieldEdit.Name_2 = "VERS_DATE";
                fieldEdit.Editable_2 = true;
                fieldEdit.IsNullable_2 = false;
                fieldEdit.Precision_2 = 0;
                fieldEdit.Scale_2 = 5;
                fieldEdit.Type_2 = esriFieldType.esriFieldTypeInteger;
                fieldsEdit.set_Field(11, fieldUserDefined);

                // Create CURVE_ID
                fieldUserDefined = new Field();
                fieldEdit = (IFieldEdit)fieldUserDefined;
                fieldEdit.Name_2 = "CURV_ID";
                fieldEdit.Editable_2 = true;
                fieldEdit.IsNullable_2 = false;
                fieldEdit.Precision_2 = 0;
                fieldEdit.Scale_2 = 5;
                fieldEdit.Type_2 = esriFieldType.esriFieldTypeInteger;
                fieldsEdit.set_Field(12, fieldUserDefined);

                // Create CURV_TYPE Field
                fieldUserDefined = new Field();
                fieldEdit = (IFieldEdit)fieldUserDefined;
                fieldEdit.Name_2 = "CURV_TYPE";
                fieldEdit.Editable_2 = true;
                fieldEdit.IsNullable_2 = false;
                fieldEdit.Precision_2 = 0;
                fieldEdit.Scale_2 = 5;
                fieldEdit.Type_2 = esriFieldType.esriFieldTypeString;
                fieldsEdit.set_Field(13, fieldUserDefined);

                // Create CURV_DIRE Field
                fieldUserDefined = new Field();
                fieldEdit = (IFieldEdit)fieldUserDefined;
                fieldEdit.Name_2 = "CURV_DIRE";
                fieldEdit.Editable_2 = true;
                fieldEdit.IsNullable_2 = false;
                fieldEdit.Precision_2 = 0;
                fieldEdit.Scale_2 = 5;
                fieldEdit.Type_2 = esriFieldType.esriFieldTypeString;
                fieldsEdit.set_Field(14, fieldUserDefined);

                // Create CURV_LENG
                fieldUserDefined = new Field();
                fieldEdit = (IFieldEdit)fieldUserDefined;
                fieldEdit.Name_2 = "CURV_LENG";
                //fieldEdit.AliasName_2 = "Length of the curve area";
                fieldEdit.Editable_2 = true;
                fieldEdit.IsNullable_2 = false;
                fieldEdit.Precision_2 = 2;
                fieldEdit.Scale_2 = 5;
                fieldEdit.Type_2 = esriFieldType.esriFieldTypeDouble;
                fieldsEdit.set_Field(15, fieldUserDefined);

                // Create Radius Field
                fieldUserDefined = new Field();
                fieldEdit = (IFieldEdit)fieldUserDefined;
                fieldEdit.Name_2 = "RADIUS";
                //fieldEdit.AliasName_2 = "Radius of the curve area";
                fieldEdit.Editable_2 = true;
                fieldEdit.IsNullable_2 = false;
                fieldEdit.Precision_2 = 2;
                fieldEdit.Scale_2 = 5;
                fieldEdit.Type_2 = esriFieldType.esriFieldTypeDouble;
                fieldsEdit.set_Field(16, fieldUserDefined);

                // Create DEGREE Field
                fieldUserDefined = new Field();
                fieldEdit = (IFieldEdit)fieldUserDefined;
                fieldEdit.Name_2 = "DEGREE";
                //fieldEdit.AliasName_2 = "Central Angle of the curve area";
                fieldEdit.Editable_2 = true;
                fieldEdit.IsNullable_2 = false;
                fieldEdit.Precision_2 = 3;
                fieldEdit.Scale_2 = 5;
                fieldEdit.Type_2 = esriFieldType.esriFieldTypeDouble;
                fieldsEdit.set_Field(17, fieldUserDefined);

                // Create Transition Field
                fieldUserDefined = new Field();
                fieldEdit = (IFieldEdit)fieldUserDefined;
                fieldEdit.Name_2 = "HAS_TRANS";
                fieldEdit.Editable_2 = true;
                fieldEdit.IsNullable_2 = false;
                fieldEdit.Precision_2 = 0;
                fieldEdit.Scale_2 = 5;
                fieldEdit.Type_2 = esriFieldType.esriFieldTypeString;
                fieldsEdit.set_Field(18, fieldUserDefined);

                // Create INTSC_ANGLE Field
                fieldUserDefined = new Field();
                fieldEdit = (IFieldEdit)fieldUserDefined;
                fieldEdit.Name_2 = "INTSC_ANGLE";
                //fieldEdit.AliasName_2 = "Central Angle of the curve area";
                fieldEdit.Editable_2 = true;
                fieldEdit.IsNullable_2 = false;
                fieldEdit.Precision_2 = 3;
                fieldEdit.Scale_2 = 5;
                fieldEdit.Type_2 = esriFieldType.esriFieldTypeDouble;
                fieldsEdit.set_Field(19, fieldUserDefined);
            }

            return fields;

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="featureDataset"></param>
        /// <param name="strNameOfFeatureClass"></param>
        /// <param name="isDissolved"></param>
        /// <returns></returns>
        public static IFeatureClass CreateCurveAreaFeatureClass(IFeatureDataset featureDataset, String strNameOfFeatureClass, bool isDissolved)
        {

            // This function creates a new feature class in a supplied feature dataset by building all of the
            // fields from scratch. IFeatureClassDescription (or IObjectClassDescription if the table is 
            // created at the workspace level) can be used to get the required fields and are used to 
            // get the InstanceClassID and ExtensionClassID.
            // Create new fields collection with the number of fields you plan to add. Must add at least two fields
            // for a feature class: Object ID and Shape field.
            

            // Create a feature class description object to use for specifying the CLSID and EXTCLSID.
            IFeatureClassDescription fcDesc = new FeatureClassDescriptionClass();
            IObjectClassDescription ocDesc = (IObjectClassDescription)fcDesc;

            return featureDataset.CreateFeatureClass(strNameOfFeatureClass, GetCurveAreaFields(false, isDissolved), ocDesc.InstanceCLSID, ocDesc.ClassExtensionCLSID, esriFeatureType.esriFTSimple, "SHAPE", "");

        }

        public static IFeatureClass CreateCurveAreaFeatureClass(IFeatureWorkspace fws)
        {
            return null;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="featureDataset"></param>
        /// <param name="strNameOfFeatureClass"></param>
        /// <returns></returns>
        public static IFeatureClass CreateCurveFeatureClass(IFeatureDataset featureDataset, String strNameOfFeatureClass)
        {

            // This function creates a new feature class in a supplied feature dataset by building all of the
            // fields from scratch. IFeatureClassDescription (or IObjectClassDescription if the table is 
            // created at the workspace level) can be used to get the required fields and are used to 
            // get the InstanceClassID and ExtensionClassID.
            // Create new fields collection with the number of fields you plan to add. Must add at least two fields
            // for a feature class: Object ID and Shape field.
            IFields fields = new FieldsClass();
            IFieldsEdit fieldsEdit = (IFieldsEdit)fields;
            fieldsEdit.FieldCount_2 = 10;

            // Create Object ID field.
            IField fieldUserDefined = new Field();
            IFieldEdit fieldEdit = (IFieldEdit)fieldUserDefined;
            fieldEdit.Name_2 = "OBJECTID";
            fieldEdit.AliasName_2 = "OBJECTID";
            fieldEdit.Type_2 = esriFieldType.esriFieldTypeOID;
            fieldsEdit.set_Field(0, fieldUserDefined);

            // Create Shape field.
            fieldUserDefined = new Field();
            fieldEdit = (IFieldEdit)fieldUserDefined;
            // Set up geometry definition for the Shape field.
            // You do not have to set the spatial reference, as it is inherited from the feature dataset.
            IGeometryDef geometryDef = new GeometryDefClass();
            IGeometryDefEdit geometryDefEdit = (IGeometryDefEdit)geometryDef;
            geometryDefEdit.GeometryType_2 = ESRI.ArcGIS.Geometry.esriGeometryType.esriGeometryPolyline;
            // By setting the grid size to 0, you are allowing ArcGIS to determine the appropriate grid sizes for the feature class. 
            // If in a personal geodatabase, the grid size is 1,000. If in a file or ArcSDE geodatabase, the grid size
            // is based on the initial loading or inserting of features.
            geometryDefEdit.GridCount_2 = 1;
            geometryDefEdit.set_GridSize(0, 0);
            geometryDefEdit.HasM_2 = false;
            geometryDefEdit.HasZ_2 = false;
            // Set standard field properties
            fieldEdit.Name_2 = "SHAPE";
            fieldEdit.Type_2 = esriFieldType.esriFieldTypeGeometry;
            fieldEdit.GeometryDef_2 = geometryDef;
            fieldEdit.IsNullable_2 = true;
            fieldEdit.Required_2 = true;
            fieldsEdit.set_Field(1, fieldUserDefined);

            // Create FromLayer
            fieldUserDefined = new Field();
            fieldEdit = (IFieldEdit)fieldUserDefined;
            fieldEdit.Name_2 = "FROMLAYER";
            fieldEdit.AliasName_2 = "The layer where these curves were identified from";
            fieldEdit.Editable_2 = true;
            fieldEdit.IsNullable_2 = false;
            fieldEdit.Precision_2 = 0;
            fieldEdit.Scale_2 = 5;
            fieldEdit.Type_2 = esriFieldType.esriFieldTypeString;
            fieldsEdit.set_Field(2, fieldUserDefined);

            // Create StreetID Field
            fieldUserDefined = new Field();
            fieldEdit = (IFieldEdit)fieldUserDefined;
            fieldEdit.Name_2 = "STREETID";
            fieldEdit.AliasName_2 = "Polyline ID of the street where the curve is located";
            fieldEdit.Editable_2 = true;
            fieldEdit.IsNullable_2 = false;
            fieldEdit.Precision_2 = 0;
            fieldEdit.Scale_2 = 5;
            fieldEdit.Type_2 = esriFieldType.esriFieldTypeInteger;
            fieldsEdit.set_Field(3, fieldUserDefined);

            // Create StreetName Field
            fieldUserDefined = new Field();
            fieldEdit = (IFieldEdit)fieldUserDefined;
            fieldEdit.Name_2 = "STREETNAME";
            fieldEdit.AliasName_2 = "Name of the street where the curve is located";
            fieldEdit.Editable_2 = true;
            fieldEdit.IsNullable_2 = false;
            fieldEdit.Precision_2 = 0;
            fieldEdit.Scale_2 = 5;
            fieldEdit.Type_2 = esriFieldType.esriFieldTypeString;
            fieldsEdit.set_Field(4, fieldUserDefined);

            // Create StreetDirc Field
            fieldUserDefined = new Field();
            fieldEdit = (IFieldEdit)fieldUserDefined;
            fieldEdit.Name_2 = "STREETDIRC";
            fieldEdit.AliasName_2 = "Direction of the street";
            fieldEdit.Editable_2 = true;
            fieldEdit.IsNullable_2 = false;
            fieldEdit.Precision_2 = 0;
            fieldEdit.Scale_2 = 5;
            fieldEdit.Type_2 = esriFieldType.esriFieldTypeString;
            fieldsEdit.set_Field(5, fieldUserDefined);

            // Create CurveIndex Field
            fieldUserDefined = new Field();
            fieldEdit = (IFieldEdit)fieldUserDefined;
            fieldEdit.Name_2 = "CURVEINDEX";
            fieldEdit.AliasName_2 = "Index of the curve within the street";
            fieldEdit.Editable_2 = true;
            fieldEdit.IsNullable_2 = false;
            fieldEdit.Precision_2 = 0;
            fieldEdit.Scale_2 = 5;
            fieldEdit.Type_2 = esriFieldType.esriFieldTypeInteger;
            fieldsEdit.set_Field(6, fieldUserDefined);

            // Create Length
            fieldUserDefined = new Field();
            fieldEdit = (IFieldEdit)fieldUserDefined;
            fieldEdit.Name_2 = "LENGTH";
            fieldEdit.AliasName_2 = "Length of the curve";
            fieldEdit.Editable_2 = true;
            fieldEdit.IsNullable_2 = false;
            fieldEdit.Precision_2 = 2;
            fieldEdit.Scale_2 = 5;
            fieldEdit.Type_2 = esriFieldType.esriFieldTypeDouble;
            fieldsEdit.set_Field(7, fieldUserDefined);

            // Create Radius Field
            fieldUserDefined = new Field();
            fieldEdit = (IFieldEdit)fieldUserDefined;
            fieldEdit.Name_2 = "RADIUS";
            fieldEdit.AliasName_2 = "Radius of the curve";
            fieldEdit.Editable_2 = true;
            fieldEdit.IsNullable_2 = false;
            fieldEdit.Precision_2 = 2;
            fieldEdit.Scale_2 = 5;
            fieldEdit.Type_2 = esriFieldType.esriFieldTypeDouble;
            fieldsEdit.set_Field(8, fieldUserDefined);

            // Create Angle Field
            fieldUserDefined = new Field();
            fieldEdit = (IFieldEdit)fieldUserDefined;
            fieldEdit.Name_2 = "ANGLE";
            fieldEdit.AliasName_2 = "Central Angle of the curve";
            fieldEdit.Editable_2 = true;
            fieldEdit.IsNullable_2 = false;
            fieldEdit.Precision_2 = 3;
            fieldEdit.Scale_2 = 5;
            fieldEdit.Type_2 = esriFieldType.esriFieldTypeDouble;
            fieldsEdit.set_Field(9, fieldUserDefined);

            // Create a feature class description object to use for specifying the CLSID and EXTCLSID.
            IFeatureClassDescription fcDesc = new FeatureClassDescriptionClass();
            IObjectClassDescription ocDesc = (IObjectClassDescription)fcDesc;

            return featureDataset.CreateFeatureClass(strNameOfFeatureClass, fields, ocDesc.InstanceCLSID, ocDesc.ClassExtensionCLSID, esriFeatureType.esriFTSimple, "SHAPE", "");

        }
       
        /// <summary>
        /// determine whether the point is above or below a line
        /// </summary>
        /// <param name="line"></param>
        /// <param name="ptPoint"></param>
        /// <returns></returns>
        public static AboveBelowLine PointAboveOrBelowtheLine(StraightLine line, IPoint ptPoint)
        {
            double Y0 = line.k * ptPoint.X + line.b;
            if (ptPoint.Y > Y0)
                return AboveBelowLine.ABOVE;
            else if (ptPoint.Y < Y0)
                return AboveBelowLine.BELOW;
            else
                return AboveBelowLine.ONTHELINE;
        }
        
        /// <summary>
        /// The third point is on side of the line
        /// </summary>
        /// <param name="ptLineFrom"></param>
        /// <param name="ptLineTo"></param>
        /// <param name="ptPoint"></param>
        /// <returns></returns>
        public static SideOfLine PointOnSideOfALine(IPoint ptLineFrom, IPoint ptLineTo, IPoint ptPoint)
        {
            StraightLine theline = new StraightLine(ptLineFrom, ptLineTo);
            AboveBelowLine LocOfPoint = PointAboveOrBelowtheLine(theline, ptPoint);
            SideOfLine theResult;

            if (ptLineTo.X > ptLineFrom.X)//the line goes to right (Xd > Xc)
            {
                if (LocOfPoint == AboveBelowLine.ABOVE)
                    theResult = SideOfLine.LEFT;
                else if (LocOfPoint == AboveBelowLine.BELOW)
                    theResult = SideOfLine.RIGHT;
                else
                    theResult = SideOfLine.ONTHELINE;
            }
            else if (ptLineTo.X < ptLineFrom.X)//the Line goes to left (Xd < Xc)
            {
                if (LocOfPoint == AboveBelowLine.ABOVE)
                    theResult = SideOfLine.RIGHT;
                else if (LocOfPoint == AboveBelowLine.BELOW)
                    theResult = SideOfLine.LEFT;
                else
                    theResult = SideOfLine.ONTHELINE;
            }
            else //Xd = Xc
            {
                if (ptLineTo.Y > ptLineFrom.Y)//the line goes up (Yd > Yc)
                {
                    if (ptPoint.X > ptLineTo.X)
                        theResult = SideOfLine.RIGHT;
                    else if (ptPoint.X < ptLineTo.X)
                        theResult = SideOfLine.LEFT;
                    else
                        theResult = SideOfLine.ONTHELINE;
                }
                else if (ptLineTo.Y < ptLineFrom.Y)//the line goes down (Yd < Yc)
                {
                    if (ptPoint.X > ptLineTo.X)
                        theResult = SideOfLine.LEFT;
                    else if (ptPoint.X < ptLineTo.X)
                        theResult = SideOfLine.RIGHT;
                    else
                        theResult = SideOfLine.ONTHELINE;
                }
                else//on the line
                    theResult = SideOfLine.ONTHELINE;
            }

            return theResult;

        }

        //Get the angle between two lines.
        //Points A, B, and C are directional, i.e. A->B->C.
        //The return value is the angle between the two lines, meausred in degree. The angle falls into the rangle of 0-180 degrees.
        //In this method, u is vector AB, v is vector BC. u(xb-xa, yb-ya) while v(xc-xb, yc-yb).
        //The greater the angle is, the sharper the turn will be. If angle is 90 degree, u is perpendicular to v. 
        public static double GetAngleBetweenTwoLines(IPoint ptA, IPoint ptB, IPoint ptC)
        {
            double Ux, Uy; //the x and y coordinates of vector u
            double Vx, Vy; //the x and y coordinates of vector v
            double absU;//the absolute of vector u
            double absV;//the absolute of vector v
            double cosVal; //the cos of the angle between u and v
            double arcosVal;//the angle in radians
            double angle;//the angle in degrees

            Ux = ptB.X - ptA.X;
            Uy = ptB.Y - ptA.Y;
            Vx = ptC.X - ptB.X;
            Vy = ptC.Y - ptB.Y;
            absU = Math.Sqrt(Math.Pow(Ux, 2) + Math.Pow(Uy, 2));
            absV = Math.Sqrt(Math.Pow(Vx, 2) + Math.Pow(Vy, 2));

            cosVal = (Ux * Vx + Uy * Vy) / (absU * absV);
            arcosVal = Math.Acos(cosVal);
            angle = arcosVal / Math.PI * 180;

            return angle;
        }

        /// <summary>
        /// GetDistanceBetweenTwoPoints
        /// </summary>
        /// <param name="pt1"></param>
        /// <param name="pt2"></param>
        /// <returns></returns>
        public static double GetDistanceBetweenTwoPoints(IPoint pt1, IPoint pt2)
        {
            return Math.Sqrt(Math.Pow(pt1.X - pt2.X, 2) + Math.Pow(pt1.Y - pt2.Y, 2));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="quePoints"></param>
        /// <returns></returns>
        private static double GetQueSegmentLengh(Queue<IPoint> quePoints)
        {
            double dSegleng = 0;
            IPoint[] arrayPoints = new ESRI.ArcGIS.Geometry.Point[quePoints.Count];
            int i = 0;
            foreach (IPoint ptTemp in quePoints)
            {
                arrayPoints[i] = new ESRI.ArcGIS.Geometry.Point();
                arrayPoints[i].X = ptTemp.X;
                arrayPoints[i].Y = ptTemp.Y;
                i++;
            }

            for (i = 0; i < quePoints.Count - 1; i++)
            {
                dSegleng += GetDistanceBetweenTwoPoints(arrayPoints[i], arrayPoints[i + 1]);
            }

            return dSegleng;
        }

        /// <summary>
        /// Get the index number for the specified layer name
        /// </summary>
        /// <param name="activeView">An IActiveView interface</param>
        /// <param name="layerName">A System.String that is the layer name in the active view. Example: "states"</param>
        /// <returns>
        /// A System.Int32 representing a layer number. 
        /// Return values of 0 and greater are valid layers. A return value of -1 means the layer name was not found.
        /// </returns>
        public static Int32 GetIndexNumberFromLayerName(ESRI.ArcGIS.Carto.IActiveView activeView, System.String layerName)
        {
            if (activeView == null || layerName == null)
            {
                return -1;
            }
            ESRI.ArcGIS.Carto.IMap map = activeView.FocusMap;

            // Get the number of layers
            int numberOfLayers = map.LayerCount;

            // Loop through the layers and get the correct layer index
            for (System.Int32 i = 0; i < numberOfLayers; i++)
            {
                if (layerName == map.get_Layer(i).Name)
                {

                    // Layer was found
                    return i;
                }
            }

            // No layer was found
            return -1;
        }
    
    }
}
