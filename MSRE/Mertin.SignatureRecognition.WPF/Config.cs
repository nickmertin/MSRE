using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Mertin.SignatureRecognition.WPF
{
    internal struct Config
    {
        /// <summary>
        /// Angle threshold for recognizing point
        /// </summary>
        static public double at = 7;
        /// <summary>
        /// Pressure threshold for recognizing point
        /// </summary>
        static public double pt = .05;
        /// <summary>
        /// Time difference threshold
        /// </summary>
        static public double tt = 2;
        /// <summary>
        /// Length difference threshold
        /// </summary>
        static public double lt = 2;
        /// <summary>
        /// Detail skip threshold
        /// </summary>
        static public double dst = 100;
        /// <summary>
        /// Angle change threshold
        /// </summary>
        static public double act = 2;
        /// <summary>
        /// Minimum angle change threshold
        /// </summary>
        static public double mact = 120;
        /// <summary>
        /// Angle direction toggle quantity threshold
        /// </summary>
        static public double adt = 1.8;
        /// <summary>
        /// Starting angle difference threshold
        /// </summary>
        static public double sat = 60;
        /// <summary>
        /// Pressure change threshold
        /// </summary>
        static public double pct = 2;
        /// <summary>
        /// Minimum pressure change threshold
        /// </summary>
        static public double mpct = 5;
        /// <summary>
        /// Pressure direction toggle quantity threshold
        /// </summary>
        static public double pdt = 2.5;
        /// <summary>
        /// Starting pressure difference threshold
        /// </summary>
        static public double spt = .2;
        /// <summary>
        /// Starting/ending point difference threshold
        /// </summary>
        static public double set = 100;
        /// <summary>
        /// Endpoint distance ratio threshold
        /// </summary>
        static public double edt = 2.5;
#if DEBUG
        /// <summary>
        /// Loads default configuration
        /// </summary>
#else
        /// <summary>
        /// Loads settings from application's custom configuration file, identified by the MSRE-CONFIG-FILE-PATH application resource
        /// </summary>
#endif
        static public void Load()
        {
#if DEBUG
                at = 7;
                pt = .05;
                tt = 2;
                lt = 2;
                dst = 100;
                act = 2;
                mact = 120;
                adt = 1.8;
                sat = 60;
                pct = 2;
                mpct = 2;
                pdt = 2.5;
                spt = .2;
                set = 100;
                edt = 2.5;
#else
            BinaryReader r = new BinaryReader(File.OpenRead(Application.Current.Resources["MSRE-CONFIG-FILE-PATH"] as string));
            at = r.ReadDouble();
            pt = r.ReadDouble();
            tt = r.ReadDouble();
            lt = r.ReadDouble();
            dst = r.ReadDouble();
            act = r.ReadDouble();
            mact = r.ReadDouble();
            adt = r.ReadDouble();
            sat = r.ReadDouble();
            pct = r.ReadDouble();
            mpct = r.ReadDouble();
            pdt = r.ReadDouble();
            spt = r.ReadDouble();
            set = r.ReadDouble();
            edt = r.ReadDouble();
#endif
        }
    }
}
