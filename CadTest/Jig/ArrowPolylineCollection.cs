using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CadTest.Jig
{
    public class ArrowPolylineCollection : IList<ArrowPolyline>, IDisposable
    {
        public ArrowPolyline this[int index] { get => ((IList<ArrowPolyline>)arrowPolylines)[index]; set => ((IList<ArrowPolyline>)arrowPolylines)[index] = value; }

        private List<ArrowPolyline> arrowPolylines = new List<ArrowPolyline>();

        public int Count => ((ICollection<ArrowPolyline>)arrowPolylines).Count;

        public bool IsReadOnly => ((ICollection<ArrowPolyline>)arrowPolylines).IsReadOnly;

        public void Add(ArrowPolyline item)
        {
            ((ICollection<ArrowPolyline>)arrowPolylines).Add(item);
        }

        public void Clear()
        {
            ((ICollection<ArrowPolyline>)arrowPolylines).Clear();
        }

        public bool Contains(ArrowPolyline item)
        {
            return ((ICollection<ArrowPolyline>)arrowPolylines).Contains(item);
        }

        public void CopyTo(ArrowPolyline[] array, int arrayIndex)
        {
            ((ICollection<ArrowPolyline>)arrowPolylines).CopyTo(array, arrayIndex);
        }

        public void Dispose()
        {
            foreach (var item in arrowPolylines)
            {
                item.Dispose();
            }
            arrowPolylines = null;
        }

        public IEnumerator<ArrowPolyline> GetEnumerator()
        {
            return ((IEnumerable<ArrowPolyline>)arrowPolylines).GetEnumerator();
        }

        public int IndexOf(ArrowPolyline item)
        {
            return ((IList<ArrowPolyline>)arrowPolylines).IndexOf(item);
        }

        public void Insert(int index, ArrowPolyline item)
        {
            ((IList<ArrowPolyline>)arrowPolylines).Insert(index, item);
        }

        public bool Remove(ArrowPolyline item)
        {
            return ((ICollection<ArrowPolyline>)arrowPolylines).Remove(item);
        }

        public void RemoveAt(int index)
        {
            ((IList<ArrowPolyline>)arrowPolylines).RemoveAt(index);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)arrowPolylines).GetEnumerator();
        }
    }
}
