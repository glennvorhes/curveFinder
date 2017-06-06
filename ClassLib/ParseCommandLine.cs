using ESRI.ArcGIS.Geodatabase;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLib
{
    public static class ParseCommandLine
    {
        private static double defaultAngle = 1.0;

        static string[] _help = new string[] { "-h", "--help", "Show the help and exit" };
        static string[] _angle = new string[] { "-a", "--angle", "Uses 1 degree by default, values of\n0.5 to 1.5 have yielded good results\ndefault: 1.0", "number" };
        static string[] _dis = new string[] { "-u", "--undissolved", "Assumes geometries are dissolved by roadway\nUse this flag if this is not the case\ndefault: true" };
        static string[] _field = new string[] { "-f", "--field", "Field in the input that identifies the road\nor highway name\ndefault: feature fid or objectid", "text" };
        static string[] _multi = new string[] { "-m", "--multi", "Multiple runs with angles from 0.5 to 1.5\nAny provided angle parmaeter disregared\ndefault: false" };

        public static string helpMessage = makeHelp();

        static string genHelpParam(string[] parts)
        {
            int gapLength = 35;

            string gapString = "";
            for (int i = 0; i < gapLength; i++)
            {
                gapString += ' ';
            }


            string outString = parts[0];

            if (parts.Length > 3)
            {
                outString += " <" + parts[3] + ">";
            }

            outString += ", " + parts[1];

            if (parts.Length > 3)
            {
                outString += " <" + parts[3] + ">";
            }

            while (outString.Length < gapLength)
            {
                outString += ' ';
            }

            string[] msgParts = parts[2].Split('\n');

            outString += msgParts[0];

            if (msgParts.Length > 1)
            {
                for (int j = 1; j < msgParts.Length; j++)
                {
                    outString += "\n" + gapString + msgParts[j];
                }
            }

            return outString + "\n\n";
        }

        public static bool isHelp(string[] args)
        {
            for (int i = 0; i < args.Length; i++)
            {
                if (args[i].ToLower() == "-h" || args[i].ToLower() == "--help")
                {
                    return true;
                }
            }
            return false;
        }

        static string getParam(string[] args, int ind, string paramType)
        {
            try
            {
                string p = args[ind + 1];

                double d;

                if (Double.TryParse(p, out d)){
                    return d.ToString();
                }

                if (p.IndexOf("-") > -1)
                {
                    throw new ArgumentException("Invalid value for " + paramType);
                }

                return p;
            }
            catch (IndexOutOfRangeException)
            {
                throw new IndexOutOfRangeException("Index out of range for " + paramType);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="args"></param>
        /// <param name="featureClass"></param>
        /// <param name="roadField"></param>
        /// <param name="angle"></param>
        /// <param name="isDissolved"></param>
        /// <param name="multi"></param>
        /// <exception cref="System.IndexOutOfRangeException">Thrown when trying to get a non existing parameter after a flag</exception>
        /// <exception cref="System.ArgumentException">Thrown when is help or hwy field not found</exception>
        public static void parseCommand(string[] args, out IFeatureClass featureClass, out string roadField, out double angle, out bool isDissolved, out bool multi)
        {
            if (isHelp(args))
            {
                throw new ArgumentException("Shouldn't get here, check if args is help first");
            }

            roadField = null;
            isDissolved = true;
            angle = defaultAngle;
            multi = false;

            featureClass = Workspace.getFeatureClass(args[args.Length - 1]);

            for (int i = 0; i < args.Length; i++)
            {
                if (args[i].ToLower() == _field[0] || args[i].ToLower() == _field[1])
                {
                    roadField = getParam(args, i, "field name");

                    IFields fields = featureClass.Fields;

                    bool fieldFound = false;

                    for (int j =0; j < fields.FieldCount; j++){
                        if (fields.get_Field(j).Name.ToLower() == roadField.ToLower())
                        {
                            fieldFound = true;
                            break;
                        }
                    }

                    if (!fieldFound)
                    {
                        throw new ArgumentException("Field '" + roadField + "' not found in input feature class");
                    }
                }

                if (args[i].ToLower() == _angle[0] || args[i].ToLower() == _angle[1])
                {
                    string num = getParam(args, i, "angle");

                    if (!Double.TryParse(num, out angle))
                    {
                        throw new ArgumentException("Invalid number provided for angle");
                    }

                    angle = Math.Round(angle, 1);

                    if (angle < 0.1 || angle > 10)
                    {
                        throw new ArgumentOutOfRangeException("angle must be between 0.1 and 10 inclusive");
                    }
                }

                if (args[i].ToLower() == _dis[0] || args[i].ToLower() == _dis[1])
                {
                    isDissolved = false;
                }

                if (args[i].ToLower() == _multi[0] || args[i].ToLower() == _multi[1])
                {
                    multi = true;
                }
            }
        }


        private static string makeHelp(){

            string helpMessage = "" +
            "\nCurveFinder command line tool\n\n" +
            "Syntax: CurveCommandLine.exe [options] <input shapefile or feature class>\n\n" +
            "Examples:\n" +
            @"  CurveCommandLine.exe C:\myInput.shp" + "\n" +
            @"  CurveCommandLine.exe -a 0.7 -u C:\FileGeoDb.gdb\roads" + "\n" +
            @"  CurveCommandLine.exe -f hwyname C:\FileGeoDb.gdb\feature_dataset\roads" + "\n" +
            "-------------------------------------------------------------------------\n";

            helpMessage += genHelpParam(_help);
            helpMessage += genHelpParam(_angle);
            helpMessage += genHelpParam(_field);
            helpMessage += genHelpParam(_dis);
            helpMessage += genHelpParam(_multi);          

            return helpMessage;
        }


    }
}
