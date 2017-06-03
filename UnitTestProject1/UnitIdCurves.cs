using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTestProject1
{
    [TestClass]
    public class UnitIdCurves
    {

        [TestInitialize]
        public void TestInitialize()
        {
            ClassLib.LicenseInit.InitializeLicence();
        }


        [TestMethod]
        public void AnalyzeShp()
        {

            ClassLib.IdentifyCurves curv = new ClassLib.IdentifyCurves(@"C:\Users\glenn\Desktop\RockMM\Segments.shp", 1, true);
            curv.RunCurves("FID");
        }

        [TestMethod]
        public void CheckFeet()
        {

            ESRI.ArcGIS.Geodatabase.IFeatureClass fcMeters = ClassLib.Workspace.getFeatureClass(@"C:\Users\glenn\Desktop\RockMM\SegmentsFt.shp");

            ClassLib.Helpers.isFeetFromFc(fcMeters);

            fcMeters = ClassLib.Workspace.getFeatureClass(@"C:\Users\glenn\Documents\TOPS\CurveFinder\Curves_2.gdb\duval\roads");
            ClassLib.Helpers.isFeetFromFc(fcMeters);
            //ClassLib.IdentifyCurves curv = new ClassLib.IdentifyCurves(1, true);
            //curv.RunCurves(fc, "FID");
        }

        [TestMethod]
        public void TestGeneral()
        {

            
            string inputFC = @"C:\Users\glenn\Documents\TOPS\CurveFinder\Curves_2.gdb\duval\roads";
            double ang = 0.7;
            string outPath = ClassLib.Helpers.makeOutputPath(inputFC, ang);

            ClassLib.IdentifyCurves curv = new ClassLib.IdentifyCurves(inputFC, ang, true);
            curv.RunCurves("OBJECTID");
            curv.MakeOutputFeatureClass(ClassLib.Helpers.makeOutputPath(inputFC, ang) + "_compare");

        }

        [TestMethod]
        public void TestGeneral2()
        {
            string inputFC = @"C:\Users\glenn\Documents\TOPS\CurveFinder\Curves_2.gdb\duval\roads_smooth";
            double ang = 0.65;
            ClassLib.Helpers.makeOutputPath(inputFC, ang);
           
            ClassLib.IdentifyCurves curv = new ClassLib.IdentifyCurves(inputFC, ang, true);
            curv.RunCurves("OBJECTID");
            curv.MakeOutputFeatureClass(ClassLib.Helpers.makeOutputPath(inputFC, ang) + "_compare");
        }


        [TestMethod]
        public void TestWrongField()
        {
            string inputFC = @"C:\Users\glenn\Documents\TOPS\CurveFinder\Curves_2.gdb\duval\roads_smooth";
            double ang = 0.65;
            ClassLib.Helpers.makeOutputPath(inputFC, ang);

            ClassLib.IdentifyCurves curv = new ClassLib.IdentifyCurves(inputFC, ang, true);
            curv.RunCurves("CATS");
            curv.MakeOutputFeatureClass(ClassLib.Helpers.makeOutputPath(inputFC, ang) + "_compare_wrong");
        }

        [TestMethod]
        public void TestWrongInput()
        {
            string inputFC = @"C:\Users\glenn\Documents\TOPS\CurveFinder\Curves_2.gdb\duval\roads_smooth_____";
            double ang = 0.65;
            ClassLib.Helpers.makeOutputPath(inputFC, ang);

            ClassLib.IdentifyCurves curv = new ClassLib.IdentifyCurves(inputFC, ang, true);
            //curv.RunCurves("CATS");
            //curv.MakeOutputFeatureClass(ClassLib.IdentifyCurves.makeOutputPath(inputFC, ang) + "_compare_wrong");
        }

        [TestMethod]
        public void TestWrongOutput()
        {
            string inputFC = @"C:\Users\glenn\Documents\TOPS\CurveFinder\Curves_2.gdb\duval\roads_smooth";
            double ang = 0.65;
            ClassLib.Helpers.makeOutputPath(inputFC, ang);

            ClassLib.IdentifyCurves curv = new ClassLib.IdentifyCurves(inputFC, ang, true);
            curv.RunCurves("OBJECTID");
            curv.MakeOutputFeatureClass("Bad output");
        }

        [TestMethod]
        public void TestMkShp()
        {
            string inputFC = @"C:\Users\glenn\Desktop\RockMM\duval.shp";
            double ang = 0.65;
            ClassLib.Helpers.makeOutputPath(inputFC, ang);

            ClassLib.IdentifyCurves curv = new ClassLib.IdentifyCurves(inputFC, ang, true);
            curv.RunCurves("FID");
            curv.MakeOutputFeatureClass(@"C:\Users\glenn\Desktop\RockMM\duval_mk_shp.shp");
        }

        [TestMethod]
        public void TestMkShpUndis()
        {
            string inputFC = @"C:\Users\glenn\Desktop\RockMM\duval.shp";
            double ang = 0.65;
            ClassLib.Helpers.makeOutputPath(inputFC, ang);

            ClassLib.IdentifyCurves curv = new ClassLib.IdentifyCurves(inputFC, ang, false);
            curv.RunCurves("FID");
            curv.MakeOutputFeatureClass(@"C:\Users\glenn\Desktop\RockMM\duval_mk_shp_undis.shp");
        }
    }
}



