using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTestProject1
{
    public static class samplePaths
    {
        public static string samplesDir = System.IO.Path.Combine(System.IO.Directory.GetCurrentDirectory(), @"..\..\samples");

        public static string sampleGdb = "BuffaloCountyRoads.gdb";

        public static string shpBuffaloFeet = makePath("BuffaloRoads_feet");
        public static string shpBuffaloMeters = makePath("BuffaloRoads_meters");
        public static string shpBuffalo84 = makePath("BuffaloRoads_84");
        
        public static string gdbBuffaloFeet = makePath("BuffaloRoads_feet", sampleGdb);
        public static string gdbBuffaloMeters = makePath("BuffaloRoads_meters", sampleGdb);
        public static string gdbBuffalo84 = makePath("BuffaloRoads_84", sampleGdb);

        public static string fdsBuffaloFeet = makePath("fdsBuffaloRoads_feet", sampleGdb, "fds_BuffaloRoads_feet");
        public static string fdsBuffaloMeters = makePath("fdsBuffaloRoads_meters", sampleGdb, "fds_BuffaloRoads_meters");
        public static string fdsBuffalo84 = makePath("fdsBuffaloRoads_84", sampleGdb, "fds_BuffaloRoads_meters");

        public static string shpNone = makePath("none");
        public static string gdbNone = makePath("none", sampleGdb);
        public static string fdsNone = makePath("none", sampleGdb, "fds_BuffaloRoads_feet");

        public static string shpBadWorkspace = @"C:\dummy\fish.shp";
        public static string gdbBadWorkspace = makePath("BuffaloRoads_feet", "silly.gdb");
        public static string fdsBadWorkspace = makePath("fdsBuffaloRoads_feet", sampleGdb, "silly_fds");



        public static string testOutputGdb = System.IO.Path.Combine(System.IO.Directory.GetCurrentDirectory(), @"..\..\samples", "test_out.gdb");


        public static string makePath(string name, string gdb = null, string fds = null)
        {
            name = name.Trim();
            if (gdb != null)
            {
                gdb = gdb.Trim();
            }
            if (fds != null)
            {
                fds = fds.Trim();
            }


            if (gdb == null)
            {
                name = name.Replace(".shp", "") + ".shp";
                return System.IO.Path.Combine(samplesDir, name);
            }
            else
            {
                name = name.Replace(".shp", "");

                if (fds == null)
                {
                    return System.IO.Path.Combine(samplesDir, gdb, name);
                }
                else
                {
                    return System.IO.Path.Combine(samplesDir, gdb, fds, name);
                }
            }
        }
    }
}
