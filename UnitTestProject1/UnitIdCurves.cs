using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;

namespace UnitTestProject1
{
    [TestClass]
    public class UnitIdCurves
    {
        public static string curves2Gdb = Path.Combine(samplePaths.samplesDir, "Curves_2.gdb");
        public static string rockMM = Path.Combine(samplePaths.samplesDir, "RockMM");


        

        

        [TestInitialize]
        public void TestInitialize()
        {
            ClassLib.LicenseInit.InitializeLicence();
        }


        [TestMethod]
        public void AnalyzeShp()
        {

            ClassLib.IdentifyCurves curv = new ClassLib.IdentifyCurves(Path.Combine(rockMM, "SegmentsFt.shp"), 1, true);
            curv.RunCurves();
        }

        [TestMethod]
        public void CheckFeet()
        {
            
            ESRI.ArcGIS.Geodatabase.IFeatureClass fcMeters = ClassLib.Workspace.getFeatureClass(Path.Combine(rockMM, "SegmentsFt.shp"));

            ClassLib.Helpers.isFeetFromFc(fcMeters);

            fcMeters = ClassLib.Workspace.getFeatureClass(Path.Combine(curves2Gdb, @"duval\roads"));
            ClassLib.Helpers.isFeetFromFc(fcMeters);
            //ClassLib.IdentifyCurves curv = new ClassLib.IdentifyCurves(1, true);
            //curv.RunCurves(fc, "FID");
        }

        [TestMethod]
        public void TestGeneral()
        {
            //string inputFC = @"C:\Users\glenn\Documents\TOPS\CurveFinder\Curves_2.gdb\duval\roads";
            string inputFC = Path.Combine(curves2Gdb, @"duval\roads");
            double ang = 0.7;
            string outPath = ClassLib.Helpers.makeOutputPath(inputFC, ang);

            ClassLib.IdentifyCurves curv = new ClassLib.IdentifyCurves(inputFC, ang, true);
            curv.RunCurves();
            curv.MakeOutputFeatureClass(ClassLib.Helpers.makeOutputPath(inputFC, ang) + Guid.NewGuid().ToString().Replace("-", ""));
        }

        [TestMethod]
        public void TestGeneral2()
        {
            string inputFC = Path.Combine(curves2Gdb, @"duval\roads_smooth");
            double ang = 0.65;
            ClassLib.Helpers.makeOutputPath(inputFC, ang);
           
            ClassLib.IdentifyCurves curv = new ClassLib.IdentifyCurves(inputFC, ang, true);
            curv.RunCurves();
            curv.MakeOutputFeatureClass(ClassLib.Helpers.makeOutputPath(inputFC, ang) + "_compare");
        }


        [TestMethod]
        public void TestWrongField()
        {
            string inputFC = Path.Combine(curves2Gdb, @"duval\roads_smooth");
            double ang = 0.65;
            ClassLib.Helpers.makeOutputPath(inputFC, ang);

            ClassLib.IdentifyCurves curv = new ClassLib.IdentifyCurves(inputFC, ang, true);
            curv.RunCurves("CATS");
            curv.MakeOutputFeatureClass(ClassLib.Helpers.makeOutputPath(inputFC, ang) + "_compare_wrong");
            //throw new Exception();
        }

        [TestMethod]
        public void TestWrongInput()
        {
            string inputFC = Path.Combine(curves2Gdb, @"duval\roads_smooth_____");
            double ang = 0.65;
            ClassLib.Helpers.makeOutputPath(inputFC, ang);

            bool raised = false;

            try
            {

                ClassLib.IdentifyCurves curv = new ClassLib.IdentifyCurves(inputFC, ang, true);
            }
            catch (FileNotFoundException ex)
            {
                raised = true;

            }

            Assert.IsTrue(raised);
        }

        [TestMethod]
        public void TestWrongOutput()
        {

            string inputFC = System.IO.Path.Combine(curves2Gdb, @"duval\roads_smooth");
            double ang = 0.65;
            ClassLib.Helpers.makeOutputPath(inputFC, ang);

            ClassLib.IdentifyCurves curv = new ClassLib.IdentifyCurves(inputFC, ang, true);
            curv.RunCurves();
            bool raised = false;
            try
            {
                curv.MakeOutputFeatureClass("Bad output");
            }
            catch (System.IO.FileNotFoundException)
            {
                raised = true;
            }

            Assert.IsTrue(raised);
        }

        [TestMethod]
        public void TestMkShp()
        {
            //string inputFC = @"C:\Users\glenn\Desktop\RockMM\duval.shp";
            string inputFC = Path.Combine(rockMM, "duval.shp");

            double ang = 0.65;
            ClassLib.Helpers.makeOutputPath(inputFC, ang);

            ClassLib.IdentifyCurves curv = new ClassLib.IdentifyCurves(inputFC, ang, true);
            curv.RunCurves();
            //curv.MakeOutputFeatureClass(@"C:\Users\glenn\Desktop\RockMM\duval_mk_shp.shp");
            curv.MakeOutputFeatureClass(Path.Combine(rockMM, "duval_mk_shp.shp"));
        }

        [TestMethod]
        public void TestMkShpUndis()
        {
            //string inputFC = @"C:\Users\glenn\Desktop\RockMM\duval.shp";
            string inputFC = Path.Combine(rockMM, "duval.shp");
            double ang = 0.65;
            ClassLib.Helpers.makeOutputPath(inputFC, ang);

            ClassLib.IdentifyCurves curv = new ClassLib.IdentifyCurves(inputFC, ang, false);
            curv.RunCurves();
            //curv.MakeOutputFeatureClass(@"C:\Users\glenn\Desktop\RockMM\duval_mk_shp_undis.shp");
            curv.MakeOutputFeatureClass(Path.Combine(rockMM, "duval_mk_shp_undis.shp"));
        }
    }
}



