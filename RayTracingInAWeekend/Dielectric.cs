using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace RayTracingInAWeekend
{
    public class Dielectric : Material
    {
        private float ref_idx;

        public Dielectric(float ri)
        {
            this.ref_idx = ri;
        }

        protected bool Refract(Vector3 view, Vector3 normal, float ni_over_nt, out Vector3 refracted)
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

        public override bool Scatter(ref Ray ray, ref HitRecord rec, out Vector3 attenuation, out Ray scattered)
        {
            Vector3 outward_normal;
            float ni_over_nt;
            attenuation = new Vector3(1f, 1f, 1f);
            float reflect_prob;
            float cosine;

            if (Vector3.Dot(ray.Direction, rec.Normal) > 0)
            {
                outward_normal = -rec.Normal;
                ni_over_nt = this.ref_idx;
                cosine = this.ref_idx * Vector3.Dot(ray.Direction, rec.Normal) / ray.Direction.Length();
            }
            else
            {
                outward_normal = rec.Normal;
                ni_over_nt = 1f / this.ref_idx;
                cosine = -Vector3.Dot(ray.Direction, rec.Normal) / ray.Direction.Length();
            }

            if (Refract(ray.Direction, outward_normal, ni_over_nt, out Vector3 refracted))
            {
                reflect_prob = Schlick(cosine, this.ref_idx);
            }
            else
            {
                reflect_prob = 1f;
            }

            if (Program.rand.NextFloat() < reflect_prob)
            {
                scattered = new Ray(rec.Position, Vector3.Reflect(ray.Direction, rec.Normal));
            }
            else
            {
                scattered = new Ray(rec.Position, refracted);
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
