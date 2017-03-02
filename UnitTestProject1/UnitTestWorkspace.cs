using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;

namespace UnitTestProject1
{
    [TestClass]
    public class UnitTestWorkspace
    {

        [TestInitialize]
        public void TestInitialize()
        {
            ClassLib.LicenseInit.InitializeLicence();
        }

        [TestMethod]
        public void getFeatureClassShp()
        {
            ESRI.ArcGIS.Geometry.ISpatialReference georef;
            ESRI.ArcGIS.Geodatabase.IFeatureWorkspace ws;
            ESRI.ArcGIS.Geodatabase.IFeatureClass fc = ClassLib.Workspace.getFeatureClass(@"C:\Users\glenn\Desktop\RockMM\Segments.shp");
            Assert.IsNotNull(fc.Fields);
        }


        [TestMethod]
        public void getFeatureClassGdb()
        {
            ESRI.ArcGIS.Geometry.ISpatialReference georef;
            ESRI.ArcGIS.Geodatabase.IFeatureWorkspace ws;
            ESRI.ArcGIS.Geodatabase.IFeatureClass fc = ClassLib.Workspace.getFeatureClass(@"C:\Users\glenn\Documents\TOPS\CurveFinder\Curves.gdb\roads2_1");
            Assert.IsNotNull(fc.Fields);
        }

        [TestMethod]
        public void getFeatureClassGdbFd()
        {
            ESRI.ArcGIS.Geometry.ISpatialReference georef;
            ESRI.ArcGIS.Geodatabase.IFeatureWorkspace ws;
            ESRI.ArcGIS.Geodatabase.IFeatureClass fc = ClassLib.Workspace.getFeatureClass(@"C:\Users\glenn\Documents\TOPS\CurveFinder\Curves.gdb\duval_1\roads2");
            Assert.IsNotNull(fc.Fields);
        }

        [TestMethod]
        public void createFeatureClassShp()
        {
            ESRI.ArcGIS.Geometry.ISpatialReference georef;
            ESRI.ArcGIS.Geodatabase.IFeatureWorkspace ws;
            ESRI.ArcGIS.Geodatabase.IFeatureClass fc = ClassLib.Workspace.getFeatureClass(@"C:\Users\glenn\Desktop\RockMM\Segments.shp");
            ClassLib.Workspace.CreateOutputFc(fc, "cat3.shp", true);
        }

        [TestMethod]
        public void createFeatureClassGdb()
        {
            ESRI.ArcGIS.Geometry.ISpatialReference georef;
            ESRI.ArcGIS.Geodatabase.IFeatureWorkspace ws;
            ESRI.ArcGIS.Geodatabase.IFeatureClass fc = ClassLib.Workspace.getFeatureClass(@"C:\Users\glenn\Documents\TOPS\CurveFinder\Curves.gdb\roads2_1");
            ClassLib.Workspace.CreateOutputFc(fc, "roads_52", true);
        }

        [TestMethod]
        public void createFeatureClassGdbFd()
        {


            ESRI.ArcGIS.Geometry.ISpatialReference georef;
            ESRI.ArcGIS.Geodatabase.IFeatureWorkspace ws;
            ESRI.ArcGIS.Geodatabase.IFeatureClass fc = ClassLib.Workspace.getFeatureClass(@"C:\Users\glenn\Documents\TOPS\CurveFinder\Curves.gdb\duval_1\roads2");

            ClassLib.Workspace.CreateOutputFc(fc, "roads2_13", true);
        }
    }
}
