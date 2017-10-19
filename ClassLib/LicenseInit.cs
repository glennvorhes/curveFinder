using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLib
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;


    public static class LicenseInit
    {
        private static ESRI.ArcGIS.esriSystem.IAoInitialize init = null;

        /// <summary>
        /// call this to confirm the license has been checked out before using any other arcobjects stuff
        /// </summary>
        /// <returns>boolean success</returns>
        public static bool InitializeLicence()
        {
            //Bind to runtime
            ESRI.ArcGIS.RuntimeManager.Bind(ESRI.ArcGIS.ProductCode.Desktop);
            LicenseInit.init = new ESRI.ArcGIS.esriSystem.AoInitializeClass();

            //try to get the initialized class, will throw exception if not available
            try
            {
                Debug.WriteLine("first try, check initialization");
                string initializedProduct = LicenseInit.init.InitializedProduct().ToString();
                Debug.WriteLine(initializedProduct);
                return true;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);

                //make list of product codes
                List<ESRI.ArcGIS.esriSystem.esriLicenseProductCode> prodCodeList = new List<ESRI.ArcGIS.esriSystem.esriLicenseProductCode>{
                ESRI.ArcGIS.esriSystem.esriLicenseProductCode.esriLicenseProductCodeBasic,
                ESRI.ArcGIS.esriSystem.esriLicenseProductCode.esriLicenseProductCodeStandard,
                ESRI.ArcGIS.esriSystem.esriLicenseProductCode.esriLicenseProductCodeAdvanced
            };

                //variable for licences status enum
                ESRI.ArcGIS.esriSystem.esriLicenseStatus esriStatus = ESRI.ArcGIS.esriSystem.esriLicenseStatus.esriLicenseNotInitialized;

                //loop over the product codes
                for (int i = 0; i < prodCodeList.Count; i++)
                {
                    //try to initialize
                    esriStatus = LicenseInit.init.Initialize(prodCodeList[i]);

                    //break on the one that worked
                    if (esriStatus == ESRI.ArcGIS.esriSystem.esriLicenseStatus.esriLicenseCheckedOut)
                    {
                        Debug.WriteLine("checked out");
                        Debug.WriteLine(LicenseInit.init.InitializedProduct().ToString());
                        return true;
                    }
                }

                return false;
            }
        }

        public static bool ShutDownLicense()
        {
            if (LicenseInit.init == null)
            {
                ESRI.ArcGIS.RuntimeManager.Bind(ESRI.ArcGIS.ProductCode.Desktop);
                LicenseInit.init = new ESRI.ArcGIS.esriSystem.AoInitializeClass();

            }

            init.Shutdown();
            return true;


        }
        
    }

}
