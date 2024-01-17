using devDept.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AESC_Eyeshot_Viewer.Models
{
    public class DistancePoints
    {
        public Point3D PointA { get; private set; }
        public Point3D PointB { get; private set; }

        public DistancePoints(Point3D pointA, Point3D pointB)
        {
            PointA = pointA;
            PointB = pointB;
        }
    }
}
