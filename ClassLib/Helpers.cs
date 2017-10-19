using System;
using System.Collections.Generic;
using System.IO;
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

        public static string makeOutputPath(string inputFC, double angle, string outWksp = null)
        {
            inputFC = inputFC.Trim();
            if (outWksp != null)
            {
                outWksp = outWksp.Trim();
            }
            
            string outputFileName = Path.GetFileName(inputFC).Replace(".shp", "");
            string outputWorkspace = Path.GetDirectoryName(inputFC);

            if (outWksp != null)
            {
                outputWorkspace = outWksp;
            }

            string angText = Math.Round(angle, 2).ToString();

            if (angText.IndexOf('.') == -1){
                angText += ".0";
            }

            angText = "_" + angText.Replace(".", "p");

            outputFileName += angText;


            if (outputWorkspace.ToLower().IndexOf(".gdb") > -1)
            {

            }
            else
            {
                if (!Directory.Exists(outputWorkspace))
                {
                    Directory.CreateDirectory(outputWorkspace);
                }

                outputFileName += ".shp";
            }
            
            return Path.Combine(outputWorkspace, outputFileName);
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
