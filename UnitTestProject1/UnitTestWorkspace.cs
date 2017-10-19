using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;
using ESRI.ArcGIS.Geodatabase;
using System.IO;

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

        private string makeOutputPath(string inp, string extra)
        {
            return Path.GetDirectoryName(inp) + "\\" + extra;

        }


        [TestMethod]
        public void createOutputFeatureClass()
        {
            ESRI.ArcGIS.Geodatabase.IFeatureClass fc;
            ESRI.ArcGIS.Geodatabase.IFeatureClass outF;

            fc = ClassLib.Workspace.getFeatureClass(samplePaths.shpBuffaloFeet);

            string output;

            IFeatureClass fc2;

            output = makeOutputPath(samplePaths.shpBuffaloFeet, testOutput + "_dis.shp");
            fc2 = ClassLib.Workspace.CreateOutputFc(output, fc, true);
            Assert.IsTrue(System.IO.File.Exists(output));

            output = makeOutputPath(samplePaths.shpBuffaloFeet, testOutput + "_no_dis.shp");
            fc2 = ClassLib.Workspace.CreateOutputFc(output, fc, false);
            Assert.IsTrue(System.IO.File.Exists(output));

            output = makeOutputPath(samplePaths.gdbBuffaloFeet, testOutput + "_dis_gdb");
            fc2 = ClassLib.Workspace.CreateOutputFc(output, fc, true);

            output = makeOutputPath(samplePaths.gdbBuffaloFeet, testOutput + "_no_dis_gdb");
            fc2 = ClassLib.Workspace.CreateOutputFc(output, fc, false);

            output = makeOutputPath(samplePaths.fdsBuffaloFeet, testOutput + "_dis_fds_bf");
            fc2 = ClassLib.Workspace.CreateOutputFc(output, fc, true);

            output = makeOutputPath(samplePaths.fdsBuffaloFeet, testOutput + "_no_dis_fds_bf");
            fc2 = ClassLib.Workspace.CreateOutputFc(output, fc, false);

            output = makeOutputPath(samplePaths.fdsBuffaloMeters, testOutput + "_dis_fds_bm");
            fc2 = ClassLib.Workspace.CreateOutputFc(output, fc, true);

            output = makeOutputPath(samplePaths.fdsBuffaloMeters, testOutput + "_no_dis_fds_bm");
            fc2 = ClassLib.Workspace.CreateOutputFc(output, fc, false);


        }
    }
}
