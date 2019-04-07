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

        public static Camera Create(Vector3 lookFrom, Vector3 lookat, Vector3 vup, float vfov, float aspect)
        {
            Camera cam = new Camera();
            float theta = vfov * MathF.PI / 180f;
            float half_height = MathF.Tan(theta / 2f);
            float half_width = aspect * half_height;

            cam.Origin = lookFrom;
            Vector3 w = Vector3.Normalize(lookFrom - lookat);
            Vector3 u = Vector3.Normalize(Vector3.Cross(vup, w));
            Vector3 v = Vector3.Cross(w, u);

            cam.LowerLeftCorner = cam.Origin - half_width * u - half_height * v - w;
            cam.Horizontal = 2f * half_width * u;
            cam.Vertical = 2f * half_height * v;

            return cam;
        }

        public Ray GetRay(float u, float v)
        {
            return new Ray(Origin, LowerLeftCorner + u * Horizontal + v * Vertical - Origin);
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
