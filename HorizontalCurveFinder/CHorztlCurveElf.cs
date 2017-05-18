using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Framework;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.ArcMapUI;
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Diagnostics;

namespace HorizontalCurveFinder
{
    public class CHorztlCurveElf : ESRI.ArcGIS.Desktop.AddIns.Button
    {
        private IApplication m_pApp = null;
        private IMap m_pMap = null;
        private IActiveView m_pActiveView = null;
        private IMxDocument m_pMxDocument = null;

        public CHorztlCurveElf()
        {
        }

        protected override void OnClick()
        {
            //get app and map
            m_pApp = ArcMap.Application;
            if (m_pApp == null)
                return;

            m_pMxDocument = (IMxDocument) m_pApp.Document;
            m_pActiveView = m_pMxDocument.ActiveView;
            m_pMap = GetMapFromArcMap(m_pApp);
            if (m_pMap == null)
                return;
              
            FormUI.DlgCurveFinderQuery dlg = new FormUI.DlgCurveFinderQuery();
            dlg.theMap = m_pMap;
            dlg.theApp = m_pApp;
            dlg.theMxDoc = m_pMxDocument;
            dlg.theActiveView = m_pActiveView;
            dlg.Show();
        }

        protected override void OnUpdate()
        {
            this.Enabled = ArcMap.Application != null;
        }


        #region "Get Map from ArcMap"

        ///<summary>Get Map from ArcMap</summary>
        ///  
        ///<param name="application">An IApplication interface that is the ArcMap application.</param>
        ///   
        ///<returns>An IMap interface.</returns>
        ///   
        ///<remarks></remarks>
        public IMap GetMapFromArcMap(IApplication application)
        {
            if (application == null)
            {
                return null;
            }
            IMxDocument mxDocument = (IMxDocument)application.Document; // Explicit Cast
            IActiveView activeView = mxDocument.ActiveView;
            IMap map = activeView.FocusMap;

            return map;

        }
        #endregion
    }

}
