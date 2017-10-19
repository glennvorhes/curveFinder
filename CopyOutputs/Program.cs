using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CopyOutputs
{
    class Program
    {
        static string outputDirectory = @"T:\Projects\CurveFinder\EsriAddIn";
        static string helperDirectory = @"C:\Users\glenn\PycharmProjects\CurveFinderHelper\bin";

        private static List<string> projectAndFiles(string project, string[] files)
        {
            List<string> outList = new List<string>();

            for (int i = 0; i < files.Length; i++)
            {
                string f = System.IO.Path.Combine(
                    System.IO.Directory.GetCurrentDirectory(), 
                    @"..\..\..\", 
                    String.Format(@"{0}\bin\Release\{1}", project, files[i])
                    );

                if (System.IO.File.Exists(f))
                {
                    outList.Add(f);
                }
                else
                {
                    throw new System.IO.FileNotFoundException(String.Format("File {0} in project {1} not found", files[i], project));
                }
            }

            return outList;
        }

        static void Main(string[] args)
        {
            List<string> fileList = new List<string>();

            if (System.IO.Directory.Exists(outputDirectory)){

                try
                {
                    fileList.AddRange(
                        projectAndFiles("HorizontalCurveFinder", new string[] {"HorizontalCurveFinder.esriAddIn"}));

                    fileList.AddRange(projectAndFiles("FormUI", new string[] { "FormUI.exe" }));
                    fileList.AddRange(projectAndFiles("CurveCommandLine", new string[] {"CurveCommandLine.exe", "ClassLib.dll"}));

                    List<string> errorList = new List<string>();

                    foreach (string f in fileList)
                    {
                        string outFile = System.IO.Path.Combine(Program.outputDirectory, System.IO.Path.GetFileName(f));
                        try
                        {
                            System.IO.File.Copy(f, outFile, true);
                        }
                        catch (System.IO.IOException er)
                        {
                            errorList.Add(er.Message);
                        }
                    }

                    foreach (string f in projectAndFiles("CurveCommandLine", new string[] { "CurveCommandLine.exe", "ClassLib.dll" }))
                    {
                        string outFile = System.IO.Path.Combine(Program.helperDirectory, System.IO.Path.GetFileName(f));
                        try
                        {
                            System.IO.File.Copy(f, outFile, true);
                        }
                        catch (System.IO.IOException er)
                        {
                            errorList.Add(er.Message);
                        }
                    }

                    if (errorList.Count == 0)
                    {
                        Console.Write("Finished Successfully.");
                    }
                    else
                    {
                        foreach (string e in errorList)
                        {
                            Console.WriteLine(e);
                        }
                    }
                }
                catch (System.IO.FileNotFoundException ex){
                    Console.Write(ex.Message + ".");
                }
                
            } else {
                Console.Write("Output directory does not exist.");
            }

            Console.WriteLine(" Enter to exit");
            Console.ReadLine();
        }
    }
}
