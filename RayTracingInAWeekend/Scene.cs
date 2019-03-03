using System;
using System.Collections.Generic;
using System.Text;

namespace RayTracingInAWeekend
{
    public struct Scene
    {
        public Sphere[] Spheres;

        public Scene(Sphere[] spheres)
        {
            this.Spheres = spheres;
        }

        public bool Hit(Ray r, float tMin, float tMax, out HitRecord rec)
        {
            HitRecord tempRec;
            rec = HitRecord.Empty;
            bool hitAnything = false;
            float closestSoFar = tMax;
            for (int i = 0; i < this.Spheres.Length; i++)
            {
                var elem = this.Spheres[i];
                if (elem.Hit(r, tMin, closestSoFar, out tempRec))
                {
                    hitAnything = true;
                    closestSoFar = tempRec.T;
                    rec = tempRec;
                }
            }

            return hitAnything;
        }
    }
}
