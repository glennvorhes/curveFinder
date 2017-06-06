using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ESRI.ArcGIS.Geodatabase;
using System.Diagnostics;

namespace UnitTestProject1
{
    [TestClass]
    public class TestParseCommands
    {
        [ClassInitialize]
        public static void setup(TestContext ctx)
        {
            ClassLib.LicenseInit.InitializeLicence();
        }

        [TestMethod]
        public void IsHelp()
        {
            Assert.IsFalse(ClassLib.ParseCommandLine.isHelp(new string[] { samplePaths.shpBuffaloFeet }));
            Assert.IsTrue(ClassLib.ParseCommandLine.isHelp(new string[] { "-h", samplePaths.shpBuffaloFeet }));
            Assert.IsTrue(ClassLib.ParseCommandLine.isHelp(new string[] { "--help", samplePaths.shpBuffaloFeet }));
            Assert.IsTrue(ClassLib.ParseCommandLine.isHelp(new string[] { "-H", samplePaths.shpBuffaloFeet }));
            Assert.IsTrue(ClassLib.ParseCommandLine.isHelp(new string[] { "--HeLp", samplePaths.shpBuffaloFeet }));

        }

        [TestMethod]
        public void ParseCom()
        {
            IFeatureClass fc;
            string roadField;
            double angle;
            bool isDissolved;
            bool multi;
            string[] args;

            args = new string[] { samplePaths.shpBuffaloFeet };
            ClassLib.ParseCommandLine.parseCommand(args, out fc, out roadField, out angle, out isDissolved, out multi);
            Assert.IsTrue(isDissolved);
            Assert.IsNull(roadField);
            Assert.AreEqual(1.0, angle);
            Assert.IsFalse(multi);

            args = new string[] { "-u", samplePaths.shpBuffaloFeet };
            ClassLib.ParseCommandLine.parseCommand(args, out fc, out roadField, out angle, out isDissolved, out multi);
            Assert.IsFalse(isDissolved);
   
            args = new string[] { "--undissolved", samplePaths.shpBuffaloFeet };
            ClassLib.ParseCommandLine.parseCommand(args, out fc, out roadField, out angle, out isDissolved, out multi);
            Assert.IsFalse(isDissolved);

            args = new string[] { "-m", samplePaths.shpBuffaloFeet };
            ClassLib.ParseCommandLine.parseCommand(args, out fc, out roadField, out angle, out isDissolved, out multi);
            Assert.IsTrue(multi);

            args = new string[] { "--multi", samplePaths.shpBuffaloFeet };
            ClassLib.ParseCommandLine.parseCommand(args, out fc, out roadField, out angle, out isDissolved, out multi);
            Assert.IsTrue(multi);

            args = new string[] { "-a", "2.01", samplePaths.shpBuffaloFeet };
            ClassLib.ParseCommandLine.parseCommand(args, out fc, out roadField, out angle, out isDissolved, out multi);
            Assert.AreEqual(2.0, angle);

            args = new string[] { "--angle", "2.01", samplePaths.shpBuffaloFeet };
            ClassLib.ParseCommandLine.parseCommand(args, out fc, out roadField, out angle, out isDissolved, out multi);
            Assert.AreEqual(2.0, angle);

            args = new string[] { "--angle", "2", samplePaths.shpBuffaloFeet };
            ClassLib.ParseCommandLine.parseCommand(args, out fc, out roadField, out angle, out isDissolved, out multi);
            Assert.AreEqual(2.0, angle);

            args = new string[] { "-f", "FULLNAME", samplePaths.shpBuffaloFeet };
            ClassLib.ParseCommandLine.parseCommand(args, out fc, out roadField, out angle, out isDissolved, out multi);
            Assert.AreEqual("FULLNAME".ToLower(), roadField.ToLower());

            args = new string[] { "--field", "fullname", samplePaths.shpBuffaloFeet };
            ClassLib.ParseCommandLine.parseCommand(args, out fc, out roadField, out angle, out isDissolved, out multi);
            Assert.AreEqual("FULLNAME".ToLower(), roadField.ToLower());

            bool raised = false;

            try
            {
                raised = false;
                args = new string[] { "--field", "fullnam", samplePaths.shpBuffaloFeet };
                ClassLib.ParseCommandLine.parseCommand(args, out fc, out roadField, out angle, out isDissolved, out multi);
            }
            catch (ArgumentException)
            {
                raised = true;
            }
            Assert.IsTrue(raised);

            try
            {
                raised = false;
                args = new string[] { "-a", "-10.0", samplePaths.shpBuffaloFeet };
                ClassLib.ParseCommandLine.parseCommand(args, out fc, out roadField, out angle, out isDissolved, out multi);
            }
            catch (ArgumentOutOfRangeException)
            {
                raised = true;
            }
            Assert.IsTrue(raised);

            try
            {
                raised = false;
                args = new string[] { "-a", "100.0", samplePaths.shpBuffaloFeet };
                ClassLib.ParseCommandLine.parseCommand(args, out fc, out roadField, out angle, out isDissolved, out multi);
            }
            catch (ArgumentOutOfRangeException)
            {
                raised = true;
            }
            Assert.IsTrue(raised);

            try
            {
                raised = false;
                args = new string[] { "-a", "adfadf", samplePaths.shpBuffaloFeet };
                ClassLib.ParseCommandLine.parseCommand(args, out fc, out roadField, out angle, out isDissolved, out multi);
            }
            catch (ArgumentException)
            {
                raised = true;
            }
            Assert.IsTrue(raised);

            try
            {
                raised = false;
                args = new string[] { samplePaths.shpNone };
                ClassLib.ParseCommandLine.parseCommand(args, out fc, out roadField, out angle, out isDissolved, out multi);
            }
            catch (System.IO.FileNotFoundException)
            {
                raised = true;
            }
            Assert.IsTrue(raised);

            try
            {
                raised = false;
                args = new string[] { samplePaths.gdbBadWorkspace };
                ClassLib.ParseCommandLine.parseCommand(args, out fc, out roadField, out angle, out isDissolved, out multi);
            }
            catch (System.IO.FileNotFoundException)
            {
                raised = true;
            }
            Assert.IsTrue(raised);



            try
            {
                raised = false;
                args = new string[] { "-a", "2.2", "-f", "fullname", @"C:\samples\BuffaloRoads_feet.shp" };
                ClassLib.ParseCommandLine.parseCommand(args, out fc, out roadField, out angle, out isDissolved, out multi);

                ClassLib.Run run = new ClassLib.Run(fc, angle, roadField, isDissolved);
                run.go();
            }
            catch (System.IO.IOException)
            {
                raised = true;
            }
            Assert.IsTrue(raised);

        }


    }
}
