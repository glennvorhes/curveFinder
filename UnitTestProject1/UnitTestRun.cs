using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;
using ClassLib;

namespace UnitTestProject1
{


    [TestClass]
    public class UnitTestRun
    {

        [ClassInitialize]
        public static void setup(TestContext ctx)
        {
            ClassLib.LicenseInit.InitializeLicence();
        }

        [TestMethod]
        public void TestIsShape()
        {

            Run r;
            r = new Run(samplePaths.shpBuffaloFeet, angle: 1.0);
            Assert.IsTrue(r.isShapefile);

            r = new Run(samplePaths.gdbBuffaloFeet, angle: 1.0);
            Assert.IsFalse(r.isShapefile);

            r = new Run(samplePaths.fdsBuffaloFeet, angle: 1.0);
            Assert.IsFalse(r.isShapefile);

            r = new Run(samplePaths.shpBuffalo84, angle: 1.0);
            Assert.IsTrue(r.isShapefile);
        }

        [TestMethod]
        public void TestInputUnits()
        {

            Run r;
            Debug.WriteLine(samplePaths.shpBuffaloFeet);
            r = new Run(samplePaths.shpBuffaloFeet, angle: 1.0);
            Assert.IsTrue(r.isFeet);
            r = new Run(samplePaths.shpBuffaloMeters, angle: 1.0);
            Assert.IsFalse(r.isFeet);

            r = new Run(samplePaths.shpBuffalo84, angle: 1.0);
            Assert.IsFalse(r.isFeetOrMeters);
        }

        [TestMethod]
        public void TestGoInputTypes()
        {
            return;
            double ang = 0.9;
            string theField = "fullname";
            Run run;
            run = new Run(samplePaths.shpBuffaloFeet, ang, theField);
            run.go();

            run = new Run(samplePaths.fdsBuffaloFeet, ang, theField);
            run.go();

            run = new Run(samplePaths.gdbBuffaloFeet, ang, theField);
            run.go();

            run = new Run(samplePaths.shpBuffaloMeters, ang, theField);
            run.go();

            run = new Run(samplePaths.fdsBuffaloMeters, ang, theField);
            run.go();

            run = new Run(samplePaths.gdbBuffaloMeters, ang, theField);
            run.go();
        }

        [TestMethod]
        public void TestRelPath()
        {
            double ang = 0.9;
            string theField = "fullname";
            Run run;
            run = new Run(samplePaths.shpBuffaloMeters, ang, theField, outWksp: samplePaths.testOutputGdb);
            run.go();
            Debug.WriteLine(run.outputPath);

        }

        [TestMethod]
        public void TestBadInputs()
        {
            double ang = 0.9;
            string theField = "fullname";
            bool excThrown = false;

           
            try
            {
                excThrown = false;
                new Run(samplePaths.shpNone, ang, theField);
            }
            catch (System.IO.FileNotFoundException ex)
            {
                Debug.WriteLine(ex.Message);
                excThrown = true;
            }

            Assert.IsTrue(excThrown);

            try
            {
                excThrown = false;
                new Run(samplePaths.gdbNone, ang, theField);
            }
            catch (System.IO.FileNotFoundException ex)
            {
                Debug.WriteLine(ex.Message);
                excThrown = true;
            }

            Assert.IsTrue(excThrown);

            try
            {
                excThrown = false;
                new Run(samplePaths.fdsNone, ang, theField);
            }
            catch (System.IO.FileNotFoundException ex)
            {
                Debug.WriteLine(ex.Message);
                excThrown = true;
            }

            Assert.IsTrue(excThrown);

            try
            {
                excThrown = false;
                new Run(samplePaths.shpBadWorkspace, ang, theField);
            }
            catch (System.IO.FileNotFoundException ex)
            {
                Debug.WriteLine(ex.Message);
                excThrown = true;
            }

            Assert.IsTrue(excThrown);

            try
            {
                excThrown = false;
                new Run(samplePaths.gdbBadWorkspace, ang, theField);
            }
            catch (System.IO.FileNotFoundException ex)
            {
                Debug.WriteLine(ex.Message);
                excThrown = true;
            }

            Assert.IsTrue(excThrown);

            try
            {
                excThrown = false;
                new Run(samplePaths.fdsBadWorkspace, ang, theField);
            }
            catch (System.IO.FileNotFoundException ex)
            {
                Debug.WriteLine(ex.Message);
                excThrown = true;
            }

            Assert.IsTrue(excThrown);
        }

        [TestMethod]
        public void TestAngleOutOfRange()
        {
            //double ang = 0.9;
            string theField = "fullname";
            Run run;
            bool exThrown = false;

            try
            {
                run = new Run(samplePaths.shpBuffaloFeet, 20.0, theField);
            }
            catch (ArgumentOutOfRangeException)
            {
                exThrown = true;
            }

            Assert.IsTrue(exThrown);

            try
            {
                run = new Run(samplePaths.shpBuffaloFeet, 0.0, theField);
            }
            catch (ArgumentOutOfRangeException)
            {
                exThrown = true;
            }

            Assert.IsTrue(exThrown);
        }

        [TestMethod]
        public void TestOutputProvided()
        {
            System.Console.WriteLine(samplePaths.shpBuffaloFeet);
            Run r;
            r = new Run(
                samplePaths.shpBuffaloFeet, 
                System.IO.Path.Combine(samplePaths.samplesDir, "tmp", "test_ " + Guid.NewGuid().ToString() + ".shp"), 
                angle: 0.4);
            r.go();
        }
    }
}

