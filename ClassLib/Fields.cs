using ESRI.ArcGIS.Geodatabase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLib
{
    public static class Fields
    {
        public const string NO_HIGHWAY = "<None>";

        public const string SEGMENT_ID = "SEGMENT_ID";
        public const string CURV_ID = "CURV_ID";
        public const string FULL_NAME = "FULL_NAME";
        public const string TASLINKID = "TASLINKID";
        public const string TRNLINKID = "TRNLINKID";
        public const string TRNNODE_F = "TRNNODE_F";
        public const string TRNNODE_T = "TRNNODE_T";
        public const string RTESYS = "RTESYS";
        public const string OFFICIAL_N = "OFFICIAL_N";
        public const string VERS_DATE = "VERS_DATE";
        public const string CURV_TYPE = "CURV_TYPE";
        public const string CURV_DIRE = "CURV_DIRE";
        public const string CURV_LENG = "CURV_LENG";
        public const string RADIUS = "RADIUS";
        public const string DEGREE = "DEGREE";
        public const string HAS_TRANS = "HAS_TRANS";
        public const string INTSC_ANGL = "INTSC_ANGL";


        private static IField makeFieldBase(string fieldName)
        {
            IField fieldUserDefined = new Field();
            IFieldEdit fieldEdit = (IFieldEdit)fieldUserDefined;
            fieldEdit.Name_2 = fieldName;
            fieldEdit.Editable_2 = true;
            fieldEdit.IsNullable_2 = false;
            return fieldUserDefined;
        }

        private static IField makeFieldText(string fieldName)
        {
            IField fieldUserDefined = makeFieldBase(fieldName);
            IFieldEdit fieldEdit = (IFieldEdit)fieldUserDefined;
            fieldEdit.Type_2 = esriFieldType.esriFieldTypeString;
            fieldEdit.Length_2 = 50;
            return fieldUserDefined;
        }

        private static IField makeFieldInteger(string fieldName)
        {
            IField fieldUserDefined = makeFieldBase(fieldName);
            IFieldEdit fieldEdit = (IFieldEdit)fieldUserDefined;
            fieldEdit.Type_2 = esriFieldType.esriFieldTypeInteger;
            fieldEdit.Precision_2 = 8;
            return fieldUserDefined;
        }

        private static IField makeFieldDouble(string fieldName)
        {
            IField fieldUserDefined = makeFieldBase(fieldName);
            IFieldEdit fieldEdit = (IFieldEdit)fieldUserDefined;
            fieldEdit.Type_2 = esriFieldType.esriFieldTypeDouble;
            fieldEdit.Precision_2 = 8;
            fieldEdit.Scale_2 = 2;
            return fieldUserDefined;
        }

        public static List<IField> getFields(bool isDissolved)
        {
            List<IField> fieldList = new List<IField>();

            fieldList.Add(makeFieldInteger(SEGMENT_ID));
            fieldList.Add(makeFieldInteger(CURV_ID));
            fieldList.Add(makeFieldText(FULL_NAME));

            if (!isDissolved)
            {
                fieldList.Add(makeFieldInteger(TASLINKID));
                fieldList.Add(makeFieldInteger(TRNLINKID));
                fieldList.Add(makeFieldInteger(TRNNODE_F));
                fieldList.Add(makeFieldInteger(TRNNODE_T));
                fieldList.Add(makeFieldInteger(RTESYS));
                fieldList.Add(makeFieldText(OFFICIAL_N));
                fieldList.Add(makeFieldInteger(VERS_DATE));
            }

            fieldList.Add(makeFieldText(CURV_TYPE));
            fieldList.Add(makeFieldText(CURV_DIRE));
            fieldList.Add(makeFieldDouble(CURV_LENG));
            fieldList.Add(makeFieldDouble(RADIUS));
            fieldList.Add(makeFieldDouble(DEGREE));
            fieldList.Add(makeFieldText(HAS_TRANS));
            fieldList.Add(makeFieldDouble(INTSC_ANGL));

            return fieldList;
        }
    }
}
