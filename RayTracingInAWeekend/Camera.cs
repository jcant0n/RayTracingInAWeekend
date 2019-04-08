using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace RayTracingInAWeekend
{
    public struct Camera
    {
        public Vector3 Origin;
        public Vector3 LowerLeftCorner;
        public Vector3 Horizontal;
        public Vector3 Vertical;
        public Vector3 u, v, w;
        public float lens_radius;

        public static Camera Create(Vector3 lookFrom, Vector3 lookat, Vector3 vup, float vfov, float aspect, float aperture, float focus_dist)
        {
            Camera cam = new Camera();
            cam.lens_radius = aperture / 2f;
            float theta = vfov * MathF.PI / 180f;
            float half_height = MathF.Tan(theta / 2f);
            float half_width = aspect * half_height;

            cam.Origin = lookFrom;
            cam.w = Vector3.Normalize(lookFrom - lookat);
            cam.u = Vector3.Normalize(Vector3.Cross(vup, cam.w));
            cam.v = Vector3.Cross(cam.w, cam.u);

            cam.LowerLeftCorner = cam.Origin - half_width * focus_dist * cam.u - half_height * focus_dist * cam.v - focus_dist * cam.w;
            cam.Horizontal = 2f * half_width * focus_dist * cam.u;
            cam.Vertical = 2f * half_height * focus_dist * cam.v;

            return cam;
        }

        public Ray GetRay(float u, float v)
        {
            Vector3 rd = this.lens_radius * Program.RandomInUnitSphere();
            Vector3 offset = this.u * rd.X + this.v * rd.Y;
            return new Ray(Origin + offset, LowerLeftCorner + u * Horizontal + v * Vertical - Origin - offset);
        }
    }
    ////public struct Camera
    ////{
    ////    public Vector3 Origin;
    ////    public Vector3 LowerLeftCorner;
    ////    public Vector3 Horizontal;
    ////    public Vector3 Vertical;

    ////    public static Camera Create()
    ////    {
    ////        Camera cam = new Camera();
    ////        cam.LowerLeftCorner = new Vector3(-2f, -1f, -1f);
    ////        cam.Horizontal = new Vector3(4f, 0, 0);
    ////        cam.Vertical = new Vector3(0f, 2f, 0);
    ////        cam.Origin = Vector3.Zero;

    ////        return cam;
    ////    }

    ////    public Ray GetRay(float u, float v)
    ////    {
    ////        return new Ray(Origin, LowerLeftCorner + u * Horizontal + v * Vertical - Origin);
    ////    }
    ////}
}
