using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gnn.Visual {

    public static class GeomHelper {

        public static bool LineIntersectsRectangle(Point lineStart, Point lineEnd, Rectangle rect) {
            float XMax;
            float XMin;
            if(lineStart.X > lineEnd.X) {
                XMax = lineStart.X;
                XMin = lineEnd.X;
            } else {
                XMin = lineStart.X;
                XMax = lineEnd.X;
            }
            if(XMax < rect.Left || XMin > rect.Right) { // Rect is to the right or left
                return false;
            }

            float YMax;
            float YMin;
            if(lineStart.Y > lineEnd.Y) {
                YMax = lineStart.Y;
                YMin = lineEnd.Y;
            } else {
                YMin = lineStart.Y;
                YMax = lineEnd.Y;
            }
            if(YMax < rect.Bottom || YMin > rect.Top) { // Rect is above or below line
                return false;
            }

            // dy = change in Y = y0 - y1
            // dx = change in X = x0 - x1
            // m (slope) = dy / dy
            //Formula to find 'Y' for some 'X':
            // Y - y0 = m * (X - x0)
            // Y = (m * (X - x0)) + y0
            var dY = lineStart.Y - lineEnd.Y;
            var dX = lineStart.X - lineEnd.X;
            var m = dX != 0 ? dY / dX : 0;

            var yAtRectLeft = (m * (rect.Left - lineStart.X)) + lineStart.Y;
            var yAtRectRight = (m * (rect.Right - lineStart.X)) + lineStart.Y;

            if(rect.Bottom > yAtRectLeft && rect.Bottom > yAtRectRight) { // Rect is above the line
                return false;
            }

            if(rect.Top > yAtRectLeft && rect.Top < yAtRectRight) { // Rect is below line
                return false;
            }

            return true;
        }

        public static bool LineIntersectsCircle(Vector2 lineStart, Vector2 lineEnd, Vector2 circlePos, float circleRadius) {
            // http://stackoverflow.com/a/1079478/1383035

            var lineLength = Vector2.Distance(lineStart, lineEnd);
            var startToCircleLength = Vector2.Distance(lineStart, circlePos);
            var endToCircleLength = Vector2.Distance(circlePos, lineEnd);

            // Calculate triangle angles when all side lenghts are known:
            // cos(A) = (AC^2 + AB^2 - CB^2) / (2 * AC * AB)
            var angle = Math.Acos((Math.Pow(startToCircleLength, 2) + Math.Pow(lineLength, 2) - Math.Pow(endToCircleLength, 2)) / (2 * startToCircleLength * lineLength));

            var prjctionLength = (float)(startToCircleLength * Math.Cos(angle));

            prjctionLength = prjctionLength > lineLength ? lineLength : prjctionLength;
            prjctionLength = prjctionLength < 0 ? 0 : prjctionLength;

            var v = lineEnd - lineStart;
            var u = v / v.Magnitude();
            var prjctionPoint = lineStart + (prjctionLength * u);

            var prjctionCircleDistance = Vector2.Distance(prjctionPoint, circlePos);
            var intersects = prjctionCircleDistance <= circleRadius;
            return intersects;
        }

        public static Vector2 GetRelative(Vector2 org, float angle, float distance) {
            return org + new Vector2((float)Math.Cos(angle) * distance, (float)Math.Sin(angle) * distance);
        }

        public static Point GetRelative(Point org, float angle, float distance) {
            return org + new Point((int)((float)Math.Cos(angle) * distance), (int)((float)Math.Sin(angle) * distance));
        }

        public static float Magnitude(this Vector2 vect) {
            return (float)Math.Sqrt(Math.Pow(vect.X, 2) + Math.Pow(vect.Y, 2));
        }
    }
}