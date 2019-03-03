using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Numerics;

namespace RayTracingInAWeekend
{
    [StructLayout(LayoutKind.Explicit, Size = 32)]
    public struct Ray
    {
        [FieldOffset (0)]
        public Vector3 Origin;

        [FieldOffset(16)]
        public Vector3 Direction;

        public Ray(Vector3 origin, Vector3 direction)
        {
            this.Origin = origin;
            this.Direction = direction;
        }

        public Vector3 PointAtParameter(float t)
        {
            return Origin + t * Direction;
        }
    }
}
