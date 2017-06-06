using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;
using ESRI.ArcGIS.Geodatabase;

namespace UnitTestProject1
{
    [TestClass]
    public class UnitTestWorkspace
    {
        static string testOutput = "testOutput";

        [ClassInitialize]
        public static void setup(TestContext ctx)
        {
            ClassLib.LicenseInit.InitializeLicence();
        }


        [TestMethod]
        public void getFeatureClass()
        {
            ESRI.ArcGIS.Geodatabase.IFeatureClass fc;

            fc = ClassLib.Workspace.getFeatureClass(samplePaths.shpBuffaloFeet);
            Assert.IsNotNull(fc);

            fc = ClassLib.Workspace.getFeatureClass(samplePaths.gdbBuffaloFeet);
            Assert.IsNotNull(fc);

            fc = ClassLib.Workspace.getFeatureClass(samplePaths.fdsBuffaloFeet);
            Assert.IsNotNull(fc);
        }

        [TestMethod]
        public void getFeatureClassGdbFd()
        {
            ESRI.ArcGIS.Geodatabase.IFeatureClass fc = ClassLib.Workspace.getFeatureClass(@"C:\Users\glenn\Documents\TOPS\CurveFinder\Curves.gdb\duval_1\roads2");
            Assert.IsNotNull(fc.Fields);
        }

        [TestMethod]
        public void createOutputFeatureClass()
        {
            ESRI.ArcGIS.Geodatabase.IFeatureClass fc;
            ESRI.ArcGIS.Geodatabase.IFeatureClass outF;
            fc = ClassLib.Workspace.getFeatureClass(samplePaths.shpBuffaloFeet);
            outF = ClassLib.Workspace.CreateOutputFc(fc, testOutput + "_dis.shp", true);
            Assert.IsTrue(System.IO.File.Exists(samplePaths.makePath(testOutput + "_dis.shp")));
            Assert.IsNotNull(outF);
            outF = ClassLib.Workspace.CreateOutputFc(fc, testOutput + "_no_dis.shp", false);
            Assert.IsTrue(System.IO.File.Exists(samplePaths.makePath(testOutput + "_no_dis.shp")));
            Assert.IsNotNull(outF);
            
            fc = ClassLib.Workspace.getFeatureClass(samplePaths.gdbBuffaloFeet);
            outF = ClassLib.Workspace.CreateOutputFc(fc, testOutput + "_dis", true);
            Assert.IsNotNull(outF);
            outF = ClassLib.Workspace.CreateOutputFc(fc, testOutput + "_no_dis", false);
            Assert.IsNotNull(outF);

            fc = ClassLib.Workspace.getFeatureClass(samplePaths.fdsBuffaloFeet);
            outF = ClassLib.Workspace.CreateOutputFc(fc, testOutput + "_dis_fds", true);
            Assert.IsNotNull(outF);
            outF = ClassLib.Workspace.CreateOutputFc(fc, testOutput + "_no_dis_fds", false);
            Assert.IsNotNull(outF);
        }
    }
}
