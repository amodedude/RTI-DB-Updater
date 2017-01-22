using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using RTI.DataBase.Model;

namespace RTI.DataBase.Objects
{
    /// <summary>
    /// Contains a collection of 
    /// USGS water data sources.
    /// </summary>
    public class SourceCollection : IList<source>
    {
        private List<source> _sourceList;

        public SourceCollection(IEnumerable<source> sources = null)
        {
            if(sources != null && sources.Any())
                _sourceList = sources.ToList();
            else
                _sourceList = new List<source>();
        }

        /// <summary>
        /// Determines if the 
        /// Source collection contains
        /// a source with the same 
        /// agency id.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool Contains(source item)
        {
            if (item?.agency_id != null)
                return _sourceList.Any(s => s.agency_id == item.agency_id);
            else
                return false;
        }

        public source this[int index]
        {
            get { return _sourceList[index]; }
            set { _sourceList[index] = value; }
        }

        public IEnumerator<source> GetEnumerator() => _sourceList.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        public int IndexOf(source item) => _sourceList.IndexOf(item);
        public void Insert(int index, source item) => _sourceList.Insert(index, item);
        public void RemoveAt(int index) => _sourceList.RemoveAt(index);
        public void Add(source item) => _sourceList.Add(item);
        public void Clear() => _sourceList.Clear();
        public void CopyTo(source[] array, int arrayIndex) => _sourceList.CopyTo(array, arrayIndex);
        public bool Remove(source item) => _sourceList.Remove(item);
        public int Count => _sourceList.Count();
        public bool IsReadOnly { get; } = false;
    }
}
