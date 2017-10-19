using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLib.segment
{
    public class IowaRoadSegCollection : CollectionBase
    {
        public void Add(CIOWARoadSeg item)
        {
            InnerList.Add(item);
        }

        public void Remove(CIOWARoadSeg item)
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
