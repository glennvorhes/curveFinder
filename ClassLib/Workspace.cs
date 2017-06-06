using ESRI.ArcGIS.Geodatabase;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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

            List<IField> fieldList = ClassLib.Fields.getFields(isDissolved);
            fieldsEdit.FieldCount_2 = fieldList.Count + 2;

            for (int i = 0; i < fieldList.Count; i++)
            {
                fieldsEdit.set_Field(i + 2, fieldList[i]);
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



            return fields;
        }

        private static ESRI.ArcGIS.Geodatabase.IFeatureWorkspace getWorkspace(string fPath)
        {
            fPath = fPath.Trim();

            string workspacePath = System.IO.Path.GetDirectoryName(fPath);
            string featureClass = System.IO.Path.GetFileName(fPath);

            if ((new Regex(@"\.shp$")).IsMatch(fPath))
            {
                try
                {
                    return shapeWsf.OpenFromFile(workspacePath, 0) as IFeatureWorkspace;
                }
                catch (System.Runtime.InteropServices.COMException)
                {
                    throw new System.IO.FileNotFoundException("Shapefile workspace not found: " + workspacePath);
                }
            }
            else if ((new Regex(@"\\[^\\]*\.(gdb|GDB)\\[^\\]*$")).IsMatch(fPath))
            {
                try
                {
                    return gdbWsf.OpenFromFile(workspacePath, 0) as IFeatureWorkspace;
                }
                catch (System.Runtime.InteropServices.COMException)
                {
                    throw new System.IO.FileNotFoundException("Geodatabase workspace not found: " + workspacePath);
                }
            }
            else if ((new Regex(@"\\[^\\]*\.(gdb|GDB)\\[^\\]*\\[^\\]*$")).IsMatch(fPath))
            {
                string datasetName = System.IO.Path.GetFileName(workspacePath);
                workspacePath = System.IO.Path.GetDirectoryName(workspacePath);
                ESRI.ArcGIS.Geodatabase.IWorkspace ws = gdbWsf.OpenFromFile(workspacePath, 0);

                ESRI.ArcGIS.Geodatabase.IEnumDataset dss = ws.get_Datasets(ESRI.ArcGIS.Geodatabase.esriDatasetType.esriDTFeatureDataset);
                ESRI.ArcGIS.Geodatabase.IDataset ds;

                while ((ds = dss.Next()) != null)
                {
                    if (ds.Name == datasetName)
                    {
                        return ds.Workspace as IFeatureWorkspace;
                    }
                }
                throw new System.IO.FileNotFoundException("feature dataset not found: " + System.IO.Path.Combine(workspacePath, datasetName));
            }
            else
            {
                throw new System.IO.FileNotFoundException("Invalid feature class path: " + fPath);
            }
        }

        public static ESRI.ArcGIS.Geodatabase.IFeatureClass getFeatureClass(string fPath)
        {
            {
                
                string featureClass = System.IO.Path.GetFileName(fPath);
                ESRI.ArcGIS.Geodatabase.IFeatureWorkspace fws = getWorkspace(fPath);
                
                try
                {
                    return fws.OpenFeatureClass(featureClass);
                }
                catch (System.Runtime.InteropServices.COMException)
                {
                    throw new System.IO.FileNotFoundException("Feature Class not found in workspace:\n" + fPath);
                }
            }
        }

        private static void deleteFeatureClass(IDataset ds){
            if (ds.CanDelete()) {
                try
                {
                    ds.Delete();
                }
                catch (System.Runtime.InteropServices.COMException ex)
                {
                    throw new System.IO.IOException(ex.Message + " " + System.IO.Path.Combine(((IWorkspace)ds).PathName, ds.Name));
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="inputFc"></param>
        /// <param name="outName"></param>
        /// <param name="isDissolved"></param>
        /// <returns></returns>
        public static IFeatureClass CreateOutputFc(ESRI.ArcGIS.Geodatabase.IFeatureClass inputFc, string outName, bool isDissolved)
        {
            IDataset iDataset = (IDataset)inputFc;

            IWorkspace2 ws = (IWorkspace2)iDataset.Workspace;
            IFeatureDataset featDs = inputFc.FeatureDataset;

            if (featDs == null)
            {
                IWorkspace2 ws2 = ((IDataset)inputFc).Workspace as IWorkspace2;

                if (ws2.get_NameExists(esriDatasetType.esriDTFeatureClass, outName))
                {
                    IDataset dsExist = (IDataset)((IFeatureWorkspace)ws2).OpenFeatureClass(outName);
                    deleteFeatureClass(dsExist);
                }
            }
            else
            {
                IEnumDataset enumDataset = featDs.Subsets;
                IDataset ids;

                while ((ids = enumDataset.Next()) != null)
                {
                    if (ids.Name == outName)
                    {
                        deleteFeatureClass(ids);
                        break;
                    }
                }
            }
            
            IFeatureClass fc;
            IFields fields = GetCurveAreaFields(outName.IndexOf(".shp") > -1, isDissolved, ((IGeoDataset)inputFc).SpatialReference);

            try
            {
                if (featDs == null)
                {
                    IFeatureWorkspace fws = (IFeatureWorkspace)iDataset.Workspace;
                    fc = fws.CreateFeatureClass(outName, fields, ocDesc.InstanceCLSID, ocDesc.ClassExtensionCLSID, esriFeatureType.esriFTSimple, "SHAPE", "");
                }
                else
                {
                    fc = featDs.CreateFeatureClass(outName, fields, ocDesc.InstanceCLSID, ocDesc.ClassExtensionCLSID, esriFeatureType.esriFTSimple, "SHAPE", "");
                }
            }
            catch (System.Runtime.InteropServices.COMException ex)
            {
                string errorMessage = "Could not create output feature class:\n";

                if (featDs == null){
                    errorMessage += System.IO.Path.Combine(iDataset.Workspace.PathName, outName) + "\n";
                } else {
                    errorMessage += System.IO.Path.Combine(iDataset.Workspace.PathName, featDs.Name, outName) + "\n";
                }
                errorMessage += ex.Message;

                throw new System.IO.IOException(errorMessage);
            }

            return fc;
        }
    }
}
