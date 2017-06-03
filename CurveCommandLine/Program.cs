using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CurveCommandLine
{
    class Program
    {
        static void Main(string[] args)
        {

            if (args.Length == 0)
            {
                Environment.Exit(0);
            }

            string helpMessage =
                "\nCurveFinder command line tool\n\n" +
                "Syntax: CurveCommandLine <input [options] <shapefile,feature class> <id field>\n\n" +
                "Examples:\n" +
                @"\tCurveCommandLine -a=0.7 -u C:\FileGeoDb.gdb\roads OBJECTID" + "\n" +
                @"\tCurveCommandLine --undisolved C:\myInput.shp FID" + "\n" +
                @"\tCurveCommandLine --angle=0.8 C:\FileGeoDb.gdb\featuredataset\roads OBJECTID" + "\n\n" +
                "-------------------------------------------------------------------------\n" +
                "Minimum inputs are the path to the shapefile and the feature identifier column\n" +
                "which will typically be OBJECTID for geodatabase feature classes and FID\n" +
                "for shapefiles\n\n" +
                "-h, --help                     Show the help and exit\n" +
                "-u, --undissolved              Assumes geometries are dissolved by roadway\n" +
                "                               Use this flag if this is not the case\n" +
                "-a=number, --angle=number      Uses 1 degree by default, values of \n" +
                "                               0.5 to 0.8 have yielded good results";


            string arg1 = args[0].Trim();

            if (arg1 == "-h" || arg1 == "--help")
            {
                Console.WriteLine(helpMessage);
            }
            else
            {
                double angle = 1.0;
                bool dissolved = true;

                for (int i = 0; i < args.Length; i++)
                {
                    string g = args[i].Trim();

                    if (g == "-a=" || g == "--angle")
                    {
                        string[] p = g.Split('=');
                        bool worked = Double.TryParse(p[1], out angle);
                    }

                    if (g == "-u" || g == "--undissolved")
                    {
                        dissolved = false;
                    }
                }

                if (args.Length < 2)
                {
                    Console.WriteLine("Not enough parameters specified");
                    Environment.Exit(0);
                }

                string inputPath = args[args.Length - 2].Trim();
                string routeIdField = args[args.Length - 2].Trim();
                string outputPath = ClassLib.Helpers.makeOutputPath(inputPath, angle) + "_compare3";

                Console.WriteLine("Running CurveFinder with");
                Console.WriteLine(String.Format("\tInput: {0}", inputPath));
                Console.WriteLine(String.Format("\tOutput: {0}", outputPath));
                Console.WriteLine(String.Format("\tThreshold angle: {0}", angle));
                Console.WriteLine(String.Format("\tDissolved: {0}", dissolved));

                ClassLib.LicenseInit.InitializeLicence();

                //string inputFC = @"C:\Users\glenn\Documents\TOPS\CurveFinder\Curves_2.gdb\duval\roads";
                ClassLib.Helpers.makeOutputPath(inputPath, angle);

                ClassLib.IdentifyCurves curv = new ClassLib.IdentifyCurves(inputPath, angle, dissolved);
                curv.RunCurves("OBJECTID");
                curv.MakeOutputFeatureClass(ClassLib.Helpers.makeOutputPath(inputPath, angle) + "_compare7");

                //ClassLib.LicenseInit.ShutDownLicense();

            }
        }
    }
}
