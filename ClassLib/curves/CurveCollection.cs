using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLib.curves
{


    public class CurveCollection : CollectionBase  
    {
        public void Add(HorizontalCurve item)  
        {  
            InnerList.Add(item);  
        }

        public void Remove(HorizontalCurve item)  
        {  
            InnerList.Remove(item);  
        }  
 
        public new int Count()  
        {  
            return InnerList.Count;  
        }  

        public new void Clear()  
        {  
            InnerList.Clear();  
        }  
    }
}
