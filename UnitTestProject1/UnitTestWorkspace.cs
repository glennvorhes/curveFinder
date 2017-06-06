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
            fc = ClassLib.Workspace.getFeatureClass(samplePaths.gdbBuffaloFeet);
            fc = ClassLib.Workspace.getFeatureClass(samplePaths.fdsBuffaloFeet);
        }


        [TestMethod]
        public void createOutputFeatureClass()
        {
            ESRI.ArcGIS.Geodatabase.IFeatureClass fc;
            ESRI.ArcGIS.Geodatabase.IFeatureClass outF;

            fc = ClassLib.Workspace.getFeatureClass(samplePaths.shpBuffaloFeet);

            outF = ClassLib.Workspace.CreateOutputFc(fc, testOutput + "_dis.shp", true);
            Assert.IsTrue(System.IO.File.Exists(samplePaths.makePath(testOutput + "_dis.shp")));

            outF = ClassLib.Workspace.CreateOutputFc(fc, testOutput + "_no_dis.shp", false);
            Assert.IsTrue(System.IO.File.Exists(samplePaths.makePath(testOutput + "_no_dis.shp")));

            fc = ClassLib.Workspace.getFeatureClass(samplePaths.gdbBuffaloFeet);
            outF = ClassLib.Workspace.CreateOutputFc(fc, testOutput + "_dis", true);
            outF = ClassLib.Workspace.CreateOutputFc(fc, testOutput + "_no_dis", false);
            
            fc = ClassLib.Workspace.getFeatureClass(samplePaths.fdsBuffaloFeet);
            outF = ClassLib.Workspace.CreateOutputFc(fc, testOutput + "_dis_fds", true);
            outF = ClassLib.Workspace.CreateOutputFc(fc, testOutput + "_no_dis_fds", false);
        }
    }
}
