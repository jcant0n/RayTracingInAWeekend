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

        public static Camera Create()
        {
            Camera cam = new Camera();
            cam.LowerLeftCorner = new Vector3(-2f, -1f, -1f);
            cam.Horizontal = new Vector3(4f, 0, 0);
            cam.Vertical = new Vector3(0f, 2f, 0);
            cam.Origin = Vector3.Zero;

            return cam;
        }

        public Ray GetRay(float u, float v)
        {
            return new Ray(Origin, LowerLeftCorner + u * Horizontal + v * Vertical - Origin);
        }
    }
}
