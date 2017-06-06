using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLib
{
    public static class Helpers
    {
        public static void DebugConsoleWrite(string msg){
            Console.WriteLine(msg);
            System.Diagnostics.Debug.WriteLine(msg);
        }

        public static string makeOutputPath(string inputFC, double angle)
        {
            string outputFile = inputFC.Trim();
            string angText = Math.Round(angle, 2).ToString().Replace(".", "p");

            if (outputFile.Length > 0)
            {
                if (outputFile.EndsWith(".shp"))
                {
                    outputFile = outputFile.Replace(".shp", "");
                    outputFile += "_" + angText + ".shp";
                }
                else
                {
                    outputFile += "_" + angText;
                }
            }

            return outputFile;
        }

        public static Boolean? isFeetFromFc(ESRI.ArcGIS.Geodatabase.IFeatureClass pFeatureClass)
        {
            ESRI.ArcGIS.Geodatabase.IGeoDataset geo = (ESRI.ArcGIS.Geodatabase.IGeoDataset)pFeatureClass;
            if (geo.SpatialReference is ESRI.ArcGIS.Geometry.IProjectedCoordinateSystem)
            {

                ESRI.ArcGIS.Geometry.IProjectedCoordinateSystem proj = (ESRI.ArcGIS.Geometry.IProjectedCoordinateSystem)geo.SpatialReference;
                if (proj.CoordinateUnit.Name.StartsWith("Meter"))
                {
                    return false;
                }
                else if (proj.CoordinateUnit.Name.StartsWith("Foot"))
                {
                    return true;
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
        }
    }

    

   
}
