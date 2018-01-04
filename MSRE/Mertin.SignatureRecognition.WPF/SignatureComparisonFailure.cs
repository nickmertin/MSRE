using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mertin.SignatureRecognition.WPF
{
    /// <summary>
    /// Represents a failure in a comparison of signature objects.
    /// </summary>
    public struct SignatureComparisonFailure
    {
        /// <summary>
        /// The code for the comparison test which the signatures failed.
        /// </summary>
        public readonly SignatureComparisonFailureCode Code;
        /// <summary>
        /// If applicable, the index of the segment in which the failure occured.
        /// </summary>
        public readonly int? SegmentIndex;
        internal SignatureComparisonFailure(SignatureComparisonFailureCode code, int? segment = null)
        {
            Code = code;
            SegmentIndex = segment;
        }
    }
    /// <summary>
    /// Defines the comparison tests which the signatures can fail.
    /// </summary>
    public enum SignatureComparisonFailureCode
    {
        /// <summary>
        /// The signatures have a different number of segments.
        /// </summary>
        SegmentCount,
        /// <summary>
        /// The time offsets are too different.
        /// </summary>
        Timing,
        /// <summary>
        /// The segment lengths are too different.
        /// </summary>
        Length,
        /// <summary>
        /// The angle change sums are too different.
        /// </summary>
        AngleChange,
        /// <summary>
        /// The angle wobble counts are too different.
        /// </summary>
        AngleWobble,
        /// <summary>
        /// The starting angles are too different.
        /// </summary>
        StartingAngle,
        /// <summary>
        /// The pressure change sums are too different.
        /// </summary>
        PressureChange,
        /// <summary>
        /// The pressure wobble counts are too different.
        /// </summary>
        PressureWobble,
        /// <summary>
        /// The starting pressures are too different.
        /// </summary>
        StartingPressure,
        /// <summary>
        /// The starting points are too far apart.
        /// </summary>
        StartPoint,
        /// <summary>
        /// The ending points are too far apart.
        /// </summary>
        EndPoint,
        /// <summary>
        /// The direct linear sizes are too different.
        /// </summary>
        DirectLinearSize
    }
}
