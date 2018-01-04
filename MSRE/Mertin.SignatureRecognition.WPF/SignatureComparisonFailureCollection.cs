using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mertin.SignatureRecognition.WPF
{
    /// <summary>
    /// Contains a read-only collection of SignatureComparisonFailure objects.
    /// </summary>
    public sealed class SignatureComparisonFailureCollection : IReadOnlyCollection<SignatureComparisonFailure>
    {
        List<SignatureComparisonFailure> list;
        internal SignatureComparisonFailureCollection()
        {
            list = new List<SignatureComparisonFailure>();
        }
        /// <summary>
        /// Gets the total number of SignatureComparisonFailure objects.
        /// </summary>
        public int Count
        {
            get { return list.Count; }
        }
        /// <summary>
        /// Gets a specific SignatureComparisonFailure object.
        /// </summary>
        /// <param name="index">The zero-based index of the requested SignatureComparisonFailure object.</param>
        /// <returns>The requested SignatureComparisonFailure object.</returns>
        public SignatureComparisonFailure this[int index]
        {
            get { return list[index]; }
        }
        /// <summary>
        /// Gets an enumerator of SignatureComparisonFailure objects for this collection.
        /// </summary>
        /// <returns></returns>
        public IEnumerator<SignatureComparisonFailure> GetEnumerator()
        {
            return new Enumerator(this);
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
        internal void Add(SignatureComparisonFailure value)
        {
            list.Add(value);
        }
        class Enumerator : IEnumerator<SignatureComparisonFailure>
        {
            SignatureComparisonFailureCollection _;
            int index;
            public Enumerator(SignatureComparisonFailureCollection collection)
            {
                _ = collection;
            }
            public SignatureComparisonFailure Current
            {
                get { return _.list[index]; }
            }
            public void Dispose()
            {
                _ = null;
                index = 0;
            }
            object IEnumerator.Current
            {
                get { return Current; }
            }
            public bool MoveNext()
            {
                if (index >= _.list.Count)
                    return false;
                ++index;
                return true;
            }
            public void Reset()
            {
                index = 0;
            }
        }
    }
}
