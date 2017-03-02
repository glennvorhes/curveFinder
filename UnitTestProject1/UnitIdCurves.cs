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
            ESRI.ArcGIS.Geometry.ISpatialReference georef;
            ESRI.ArcGIS.Geodatabase.IFeatureWorkspace ws;
            ESRI.ArcGIS.Geodatabase.IFeatureClass fc = ClassLib.Workspace.getFeatureClass(@"C:\Users\glenn\Desktop\RockMM\Segments.shp");

            ClassLib.IdentifyCurves curv = new ClassLib.IdentifyCurves(1, true);
            curv.RunCurves(fc, "FID");
        }

        [TestMethod]
        public void CheckFeet()
        {
            ESRI.ArcGIS.Geometry.ISpatialReference georef;
            ESRI.ArcGIS.Geodatabase.IFeatureWorkspace ws;
            ESRI.ArcGIS.Geodatabase.IFeatureClass fcMeters = ClassLib.Workspace.getFeatureClass(@"C:\Users\glenn\Desktop\RockMM\SegmentsFt.shp");

            ClassLib.IdentifyCurves.isFeetFromFc(fcMeters);

            fcMeters = ClassLib.Workspace.getFeatureClass(@"C:\Users\glenn\Desktop\RockMM\Segments.shp");
            ClassLib.IdentifyCurves.isFeetFromFc(fcMeters);
            //ClassLib.IdentifyCurves curv = new ClassLib.IdentifyCurves(1, true);
            //curv.RunCurves(fc, "FID");
        }
    }
}
