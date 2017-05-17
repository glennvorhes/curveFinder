using ESRI.ArcGIS.Geodatabase;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLib
{
    public static class Workspace
    {
        private static IObjectClassDescription ocDesc = (IObjectClassDescription)(new FeatureClassDescriptionClass());

        private static ESRI.ArcGIS.Geodatabase.IWorkspaceFactory2 shapeWsf = (ESRI.ArcGIS.Geodatabase.IWorkspaceFactory2)new ESRI.ArcGIS.DataSourcesFile.ShapefileWorkspaceFactory();
        private static ESRI.ArcGIS.Geodatabase.IWorkspaceFactory2 gdbWsf = (ESRI.ArcGIS.Geodatabase.IWorkspaceFactory2)new ESRI.ArcGIS.DataSourcesGDB.FileGDBWorkspaceFactory();

        private static IFields GetCurveAreaFields(bool isShape, bool isDissolved, ESRI.ArcGIS.Geometry.ISpatialReference georef = null)
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
            if (!isShape)
            {
                geometryDefEdit.GridCount_2 = 1;
                geometryDefEdit.set_GridSize(0, 0);
            }
            geometryDefEdit.HasM_2 = false;
            geometryDefEdit.HasZ_2 = false;
            if (georef != null)
            {
                geometryDefEdit.SpatialReference_2 = georef;
            }
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
                fieldEdit.Name_2 = "INTSC_ANGL";
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
                fieldEdit.Name_2 = "INTSC_ANGL";
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

        public static ESRI.ArcGIS.Geodatabase.IFeatureClass getFeatureClass(string fPath)
        {
            {
                ESRI.ArcGIS.Geodatabase.IWorkspaceFactory2 wsf;


                string workspacePath = System.IO.Path.GetDirectoryName(fPath);
                string featureClass = System.IO.Path.GetFileName(fPath);
                ESRI.ArcGIS.Geodatabase.IWorkspace ws;

                if (fPath.EndsWith(".shp"))
                {
                    wsf = shapeWsf;
                    ws = shapeWsf.OpenFromFile(workspacePath, 0);
                }
                else
                {
                    wsf = gdbWsf;
                    if (workspacePath.EndsWith(".gdb"))
                    {
                        ws = gdbWsf.OpenFromFile(workspacePath, 0);
                    }
                    else
                    {
                        string datasetName = System.IO.Path.GetFileName(workspacePath);
                        workspacePath = System.IO.Path.GetDirectoryName(workspacePath);
                        ws = gdbWsf.OpenFromFile(workspacePath, 0);

                        ESRI.ArcGIS.Geodatabase.IEnumDataset dss = ws.get_Datasets(ESRI.ArcGIS.Geodatabase.esriDatasetType.esriDTFeatureDataset);
                        ESRI.ArcGIS.Geodatabase.IDataset ds;

                        while ((ds = dss.Next()) != null)
                        {
                            if (ds.Name == datasetName)
                            {
                                ws = ds.Workspace;
                                break;
                            }
                        }
                    }
                }

                ESRI.ArcGIS.Geodatabase.IFeatureWorkspace fws = (ESRI.ArcGIS.Geodatabase.IFeatureWorkspace)ws;

                ESRI.ArcGIS.Geodatabase.IFeatureClass fc = fws.OpenFeatureClass(featureClass);
                return fc;
            }
        }

        public static IFeatureClass CreateOutputFc(ESRI.ArcGIS.Geodatabase.IFeatureClass inputFc, string outName, bool isDissolved)
        {
            IDataset ds = (IDataset)inputFc;
            IFeatureWorkspace fws = (IFeatureWorkspace)ds.Workspace;

            IWorkspace2 ws = (IWorkspace2)fws;

            if (ws.get_NameExists(esriDatasetType.esriDTFeatureClass, outName))
            {
                IDataset dsExist = (IDataset)fws.OpenFeatureClass(outName);
                ISchemaLock lck = (ISchemaLock)dsExist;
                IEnumSchemaLockInfo lockInfoEnum;
                lck.GetCurrentSchemaLocks(out lockInfoEnum);
                ISchemaLockInfo lockInfo;
                
                while((lockInfo = lockInfoEnum.Next()) != null){
                    Debug.WriteLine(lockInfo.SchemaLockType);
                }

                if (dsExist.CanDelete()){
                        dsExist.Delete();
                } else {
                     return null;   
                }
            }

            
            IFields fields;
            IFeatureClass fc;

            IFeatureDataset featDs = inputFc.FeatureDataset;

            IGeoDataset geoDs = (IGeoDataset)inputFc;
            fields = GetCurveAreaFields(outName.IndexOf(".shp") > -1, isDissolved, geoDs.SpatialReference);

            try
            {
                if (featDs == null || featDs.BrowseName.EndsWith(".gdb"))
                {

                    fc = fws.CreateFeatureClass(outName, fields, ocDesc.InstanceCLSID, ocDesc.ClassExtensionCLSID, esriFeatureType.esriFTSimple, "SHAPE", "");
                }
                else
                {
                    fc = featDs.CreateFeatureClass(outName, fields, ocDesc.InstanceCLSID, ocDesc.ClassExtensionCLSID, esriFeatureType.esriFTSimple, "SHAPE", "");
                }
            }
            catch (System.Runtime.InteropServices.COMException)
            {
                return null;
            }
           
            return fc;
        }
    }
}

