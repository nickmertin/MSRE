using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Mertin.SignatureRecognition.WPF
{
    /// <summary>
    /// Provides utility methods for operations such as common calcusations
    /// </summary>
    static internal class Util
    {
        static public double GetAngle(Point p1, Point p2)
        {
            return GetAngle(p1.X, p1.Y, p2.X, p2.Y);
        }
        static public double GetAngle(double x1, double y1, double x2, double y2)
        {
            return Math.Atan2(y2 - y1, x2 - x1);
        }
        static public double GetDistance(Point p1, Point p2)
        {
            return GetDistance(p1.X, p1.Y, p2.X, p2.Y);
        }
        static public double GetDistance(double x1, double y1, double x2, double y2)
        {
            return Math.Sqrt(Math.Pow(x1 - x2, 2) + Math.Pow(y1 - y2, 2));
        }
    }
}
