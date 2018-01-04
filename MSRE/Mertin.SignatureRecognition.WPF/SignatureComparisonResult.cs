using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mertin.SignatureRecognition.WPF
{
    /// <summary>
    /// Represents a result from a comparison of Signature objects.
    /// </summary>
    public struct SignatureComparisonResult
    {
        SignatureComparisonFailureCollection _;
        internal SignatureComparisonResult(SignatureComparisonFailureCollection failure)
        {
            _ = failure;
        }
        /// <summary>
        /// Gets a boolean value that identifies whether or not the comparison concluded that the signatures match.
        /// </summary>
        public bool IsMatch { get { return _.Count == 0; } }
        /// <summary>
        /// Gets a collection of SignatureComparisonFailure objects that identify why the signatures do not match. An empty collection signifies a match.
        /// </summary>
        public SignatureComparisonFailureCollection Failures { get { return _; } }
    }
}
