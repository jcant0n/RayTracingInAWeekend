using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace RayTracingInAWeekend
{
    public class Lambertian : Material
    {
        private Vector3 albedo;

        public Lambertian(Vector3 albedo)
        {
            this.albedo = albedo;
        }

        public override bool Scatter(ref Ray ray, ref HitRecord hit, out Vector3 attenuation, out Ray scattered)
        {
            Vector3 target = hit.Position + hit.Normal + Program.RandomInUnitSphere();
            scattered = new Ray(hit.Position, target - hit.Position);
            attenuation = albedo;

            return true;
        }
    }
}
