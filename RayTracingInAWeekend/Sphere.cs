using System;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Text;

namespace RayTracingInAWeekend
{
    [StructLayout(LayoutKind.Sequential)]
    public struct Sphere
    {
        public Vector3 Center;
        float Radius;

        public Sphere(Vector3 center, float radius)
        {
            this.Center = center;
            this.Radius = radius;
        }

        public bool Hit(Ray r, float tMin, float tMax, out HitRecord rec)
        {
            Vector3 oc = r.Origin - Center;
            float a = Vector3.Dot(r.Direction, r.Direction);
            float b = Vector3.Dot(oc, r.Direction);
            float c = Vector3.Dot(oc, oc) - Radius * Radius;
            float discriminant = (b * b) - (a * c);

            if (discriminant > 0)
            {
                float temp = (-b - MathF.Sqrt(b * b - a * c)) / a;
                if (temp < tMax && temp > tMin)
                {
                    rec.T = temp;
                    rec.Position = r.PointAtParameter(rec.T);
                    rec.Normal = (rec.Position - Center) / Radius;
                    return true;
                }

                temp = (-b + MathF.Sqrt(b * b - a * c)) / a;
                if(temp < tMax && temp > tMin)
                {
                    rec.T = temp;
                    rec.Position = r.PointAtParameter(rec.T);
                    rec.Normal = (rec.Position - Center) / Radius;
                    return true;
                }
            }

            rec = HitRecord.Empty;
            return false;
        }
    }
}
