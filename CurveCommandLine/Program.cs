using ESRI.ArcGIS.Geodatabase;
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

            if (args.Length == 0 || ClassLib.ParseCommandLine.isHelp(args))
            {
                Console.WriteLine(ClassLib.ParseCommandLine.helpMessage);
                Environment.Exit(0);
            }

            ClassLib.LicenseInit.InitializeLicence();

            IFeatureClass fc;
            string roadField;
            double angle;
            bool isDissolved;
            bool multi;
            string outWksp;

            try
            {
                ClassLib.ParseCommandLine.parseCommand(args, out fc, out roadField, out angle, out isDissolved, out multi, out outWksp);

                if (multi)
                {
                    for (double a = 0.5; a <= 1.6; a += 0.1)
                    {
                        Console.WriteLine("Running angle {0}", a);
                        ClassLib.Run run = new ClassLib.Run(fc, a, roadField, isDissolved, outWksp);
                        run.go();
                        Console.WriteLine("Output to: {0}", run.outputPath);
                    }
                }
                else
                {
                    ClassLib.Run run = new ClassLib.Run(fc, angle, roadField, isDissolved, outWksp);
                    run.go();
                    Console.WriteLine("Output to: {0}", run.outputPath);
                }
                Console.WriteLine("Finished Sucessfully");
            }
            catch (IndexOutOfRangeException ex)
            {
                Console.WriteLine(ex.Message);
            }
            catch (System.IO.FileNotFoundException ex)
            {
                Console.WriteLine(ex.Message);
            }
            catch (ArgumentOutOfRangeException ex)
            {
                Console.WriteLine(ex.Message);
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine(ex.Message);
            }
            catch (System.IO.IOException ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
