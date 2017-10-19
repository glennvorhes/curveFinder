using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLib.segment
{
    public class IowaCurveCollection : System.Collections.CollectionBase
    {
        public void Add(CIOWACurve item)
        {
            InnerList.Add(item);
        }

        public void Remove(CIOWACurve item)
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
