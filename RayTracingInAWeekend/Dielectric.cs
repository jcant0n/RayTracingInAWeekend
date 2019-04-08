using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace RayTracingInAWeekend
{
    public class Dielectric : Material
    {
        private float ri_in;
        private float ri_out;

        public Dielectric(float ri_in, float ri_out)
        {
            this.ri_in = ri_in;
            this.ri_out = ri_out;
        }

        protected bool Refract(ref Vector3 view, ref Vector3 normal, float ni_over_nt, out Vector3 refracted)
        {
            Vector3 uv = Vector3.Normalize(view);
            float dt = Vector3.Dot(uv, normal);
            float discriminant = 1f - ni_over_nt * ni_over_nt * (1f - dt * dt);

            if (discriminant > 0)
            {
                refracted = ni_over_nt * (uv - normal * dt) - normal * MathF.Sqrt(discriminant);
                return true;
            }
            else
            {
                refracted = Vector3.Zero;
                return false;
            }
        }

        public override bool Scatter(ref Ray ray, ref HitRecord hit, out Vector3 attenuation, out Ray scattered)
        {
            Vector3 outward_normal;
            float ni_over_nt;
            attenuation = new Vector3(1f, 1f, 1f);
            float reflect_prob;
            float cosine;

            if (Vector3.Dot(ray.Direction, hit.Normal) > 0)
            {
                outward_normal = -hit.Normal;
                ni_over_nt = this.ri_in / this.ri_out;
                cosine = this.ri_in * Vector3.Dot(ray.Direction, hit.Normal) / ray.Direction.Length();
            }
            else
            {
                outward_normal = hit.Normal;
                ni_over_nt = this.ri_out / this.ri_in;
                cosine = -Vector3.Dot(ray.Direction, hit.Normal) / ray.Direction.Length();
            }

            if (Refract(ref ray.Direction, ref outward_normal, ni_over_nt, out Vector3 refracted))
            {
                reflect_prob = Schlick(cosine, this.ri_in);
            }
            else
            {
                reflect_prob = 1f;
            }

            if (Program.rand.NextFloat() < reflect_prob)
            {
                scattered = new Ray(hit.Position, Vector3.Reflect(ray.Direction, hit.Normal));
            }
            else
            {
                scattered = new Ray(hit.Position, refracted);
            }

            return true;
        }

        private float Schlick(float cosine, float refIndex)
        {
            float r0 = (1f - refIndex) / (1f + refIndex);
            r0 = r0 * r0;
            return r0 + (1f - r0) * MathF.Pow(1f - cosine, 5f);
        }
    }
}
