using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows;

namespace Mertin.SignatureRecognition.WPF
{
    /// <summary>
    /// Represents the data required to compare a written signature.
    /// </summary>
    public unsafe struct Signature : IDisposable
    {
        SignatureData* _;
        int _c;
        internal struct SignatureData
        {
            public double TimeOffset, Length, AngleChange, StartingAngle, PressureChange, StartingPressure;
            public ulong AngleWobble, PressureWobble;
            public Point Start, End;
            public SignatureData(double to, double l, double ac, ulong aw, double sa, double pc, ulong pw, double sp, Point start, Point end)
            {
                TimeOffset = to;
                Length = l;
                AngleChange = ac;
                AngleWobble = aw;
                StartingAngle = sa;
                PressureChange = pc;
                PressureWobble = pw;
                StartingPressure = sp;
                Start = start;
                End = end;
            }
        }
        static Signature()
        {
            Config.Load();
        }
        internal Signature(SignatureData* data, int count)
        {
            _ = data;
            _c = count;
        }
        /// <summary>
        /// Reloads configuration data from the known configuration file. Use if configuration data or file path has changed.
        /// </summary>
        static public void ReloadConfiguration()
        {
            Config.Load();
        }
        /// <summary>
        /// Writes signature data to a data stream for use at a later time.
        /// </summary>
        /// <param name="stream">The data stream to write data to.</param>
        public void Save(Stream stream)
        {
            BinaryWriter w = new BinaryWriter(stream);
            w.Write(_c);
            for (int i = 0; i < _c; ++i )
            {
                SignatureData d = _[i];
                w.Write(d.TimeOffset);
                w.Write(d.Length);
                w.Write(d.AngleChange);
                w.Write(d.StartingAngle);
                w.Write(d.AngleWobble);
                w.Write(d.StartingAngle);
                w.Write(d.PressureChange);
                w.Write(d.PressureWobble);
                w.Write(d.StartingAngle);
                w.Write(d.Start.X);
                w.Write(d.Start.Y);
                w.Write(d.End.X);
                w.Write(d.End.Y);
            }
        }
        /// <summary>
        /// Loads signature data that was written using the Save method from a data stream.
        /// </summary>
        /// <param name="stream">The data stream to read from</param>
        /// <returns>A Signature object created from the data.</returns>
        static public Signature Load(Stream stream)
        {
            BinaryReader r = new BinaryReader(stream);
            int dc = r.ReadInt32();
            SignatureData* data = stackalloc SignatureData[dc];
            for (int i = 0; i < dc; ++i)
            {
                data[i] = new SignatureData(r.ReadDouble(), r.ReadDouble(), r.ReadDouble(), r.ReadUInt64(), r.ReadDouble(), r.ReadDouble(), r.ReadUInt64(), r.ReadDouble(), new Point(r.ReadDouble(), r.ReadDouble()), new Point(r.ReadDouble(), r.ReadDouble()));
            }
            return new Signature(data, dc);
        }
        /// <summary>
        /// Compares two Signature objects.
        /// </summary>
        /// <param name="original">The original recorded signature.</param>
        /// <param name="attempt">The attempt to compare.</param>
        /// <returns>The result of the comparison.</returns>
        static public SignatureComparisonResult Compare(Signature original, Signature attempt)
        {
            SignatureComparisonFailureCollection result = new SignatureComparisonFailureCollection();
            if (original._c == attempt._c)
                for (int i = 0; i < original._c; ++i)
                {
                    SignatureData o = original._[i], a = attempt._[i];
                    double time = o.TimeOffset / a.TimeOffset;
                    if (time < 1 / Config.tt | time > Config.tt)
                        result.Add(new SignatureComparisonFailure(SignatureComparisonFailureCode.Timing, i));
                    if (Math.Max(o.Length, a.Length) < Config.dst)
                        goto skip;
                    double length = o.Length / a.Length;
                    if (length < 1 / Config.lt | length > Config.lt)
                        result.Add(new SignatureComparisonFailure(SignatureComparisonFailureCode.Length, i));
                    if (Math.Max(o.AngleChange, a.AngleChange) >= Config.mact)
                    {
                        double angleChange = o.AngleChange / a.AngleChange;
                        if (angleChange < 1 / Config.act | angleChange > Config.act)
                            result.Add(new SignatureComparisonFailure(SignatureComparisonFailureCode.AngleChange, i));
                        double angleWobble = o.AngleWobble / a.AngleWobble;
                        if (angleWobble < 1 / Config.adt | angleWobble > Config.adt)
                            result.Add(new SignatureComparisonFailure(SignatureComparisonFailureCode.AngleWobble, i));
                    }
                    double startingAngle = Math.Abs(o.StartingAngle - a.StartingAngle);
                    if (startingAngle > Config.sat)
                        result.Add(new SignatureComparisonFailure(SignatureComparisonFailureCode.StartingAngle, i));
                skip:
                    if (Math.Max(o.PressureChange, a.PressureChange) >= Config.mpct)
                    {
                        double pressureChange = o.PressureChange / a.PressureChange;
                        if (pressureChange < 1 / Config.pct | pressureChange > Config.pct)
                            result.Add(new SignatureComparisonFailure(SignatureComparisonFailureCode.PressureChange, i));
                        double pressureWobble = o.PressureWobble / a.PressureWobble;
                        if (pressureWobble < 1 / Config.pdt | pressureWobble > Config.pdt)
                            result.Add(new SignatureComparisonFailure(SignatureComparisonFailureCode.PressureWobble, i));
                    }
                    double startingPressure = Math.Abs(o.StartingPressure - a.StartingPressure);
                    if (startingPressure > Config.spt)
                        result.Add(new SignatureComparisonFailure(SignatureComparisonFailureCode.StartingPressure, i));
                    double startPoint = Math.Sqrt(Math.Pow(Math.Abs(o.Start.X - a.Start.X), 2) + Math.Pow(Math.Abs(o.Start.Y - a.Start.Y), 2));
                    if (startPoint > Config.set)
                        result.Add(new SignatureComparisonFailure(SignatureComparisonFailureCode.StartPoint, i));
                    double endPoint = Math.Sqrt(Math.Pow(Math.Abs(o.End.X - a.End.X), 2) + Math.Pow(Math.Abs(o.End.Y - a.End.Y), 2));
                    if (endPoint > Config.set)
                        result.Add(new SignatureComparisonFailure(SignatureComparisonFailureCode.EndPoint, i));
                    double directLinearSize = Math.Sqrt(Math.Pow(Math.Abs(o.Start.X - o.End.X), 2) + Math.Pow(Math.Abs(o.Start.Y - o.End.Y), 2)) / Math.Sqrt(Math.Pow(Math.Abs(a.Start.X - a.End.X), 2) + Math.Pow(Math.Abs(a.Start.Y - a.End.Y), 2));
                    if (directLinearSize < 1 / Config.edt | directLinearSize > Config.edt)
                        result.Add(new SignatureComparisonFailure(SignatureComparisonFailureCode.DirectLinearSize, i));
                }
            else
                result.Add(new SignatureComparisonFailure(SignatureComparisonFailureCode.SegmentCount));
            return new SignatureComparisonResult(result);
        }
        /// <summary>
        /// Wipes all signature data from memory. NOTE: does not wipe data stored outside of the signature object, for example in a data stream.
        /// </summary>
        public void Dispose()
        {
            for (int i = 0; i < _c; ++i)
            {
                _[i].AngleChange = 0;
                _[i].AngleWobble = 0;
                _[i].Length = 0;
                _[i].PressureChange = 0;
                _[i].PressureWobble = 0;
                _[i].StartingAngle = 0;
                _[i].StartingPressure = 0;
                _[i].TimeOffset = 0;
                _[i].Start.X = 0;
                _[i].Start.Y = 0;
                _[i].End.X = 0;
                _[i].End.Y = 0;
            }
            _c = 0;
            _ = null;
        }
    }
}
