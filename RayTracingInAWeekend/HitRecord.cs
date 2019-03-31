using System;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Text;

namespace RayTracingInAWeekend
{
    [StructLayout(LayoutKind.Sequential)]
    public struct HitRecord
    {
        public Vector3 Position;
        public float T;
        public Vector3 Normal;
        public Material Material;

        public static HitRecord Empty
        {
            get
            {
                HitRecord rec;
                rec.T = 0;
                rec.Position = Vector3.Zero;
                rec.Normal = Vector3.Zero;
                rec.Material = null;
                return rec;
            }
        }
    }
}
