using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace RayTracingInAWeekend
{
    public class Metal : Material
    {
        private Vector3 albedo;
        private float fuzz;

        public Metal(Vector3 albedo, float f)
        {
            this.albedo = albedo;
            this.fuzz = f < 1 ? f : 1;
        }

        public override bool Scatter(ref Ray ray, ref HitRecord rec, out Vector3 attenuation, out Ray scattered)
        {
            var direction = Vector3.Normalize(ray.Direction);
            Vector3 reflected = Vector3.Reflect(direction, rec.Normal);
            scattered = new Ray(rec.Position, reflected + fuzz * Program.RandomInUnitSphere());
            attenuation = albedo;

            return (Vector3.Dot(scattered.Direction, rec.Normal) > 0);
        }
    }
}
