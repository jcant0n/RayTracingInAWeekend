using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace RayTracingInAWeekend
{
    public abstract class Material
    {
        public abstract bool Scatter(ref Ray ray, ref HitRecord hit, out Vector3 attenuation, out Ray scattered);
    }
}
