namespace RayTracingInAWeekend
{
    using System;
    using System.Diagnostics;
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.IO;
    using System.Reflection;
    using System.Runtime.Versioning;
    using System.Numerics;

    public unsafe class Program
    {
        public static bool HitSphere(Vector3 center, float radius, Ray r)
        {
            Vector3 oc = r.Origin - center;
            float a = Vector3.Dot(r.Direction, r.Direction);
            float b = 2f * Vector3.Dot(oc, r.Direction);
            float c = Vector3.Dot(oc, oc) - radius * radius;
            float discriminant = (b * b) - (4 * a * c);
            return discriminant > 0;
        }

        public static Vector3 Color(Ray ray)
        {
            if (HitSphere(new Vector3(0, 0, -1f), 0.5f, ray))
            {
                return new Vector3(1f, 0, 0);
            }

            Vector3 unitDirection = Vector3.Normalize(ray.Direction);
            float t = (0.5f * unitDirection.Y) + 1.0f;
            return ((1.0f - t) * new Vector3(1f, 1f, 1f)) + (t * new Vector3(0.5f, 0.7f, 1f));
        }

        public static void Main(string[] args)
        {
            int width = 200;
            int height = 100;
            string filename = "Render.png";
            byte[] pixels = new byte[width * height * 3];

            var framework = Assembly
                .GetEntryAssembly()?
                .GetCustomAttribute<TargetFrameworkAttribute>()
                ?.FrameworkName;
            Console.WriteLine($"C# Code - {framework ?? "Unknown"}");

            Stopwatch sw = new Stopwatch();
            sw.Start();

            Vector3 lowerLeftCorner = new Vector3(-2.0f, -1.0f, -1.0f);
            Vector3 horizontal = new Vector3(4f, 0f, 0f);
            Vector3 vertical = new Vector3(0f, 2f, 0f);
            Vector3 origin = Vector3.Zero;

            ////Parallel.For(0, height, j =>
            for (int j = 0; j < height; j++)
            {
                for (int i = 0; i < width; i++)
                {
                    float u = (float)i / (float)width;
                    float v = (float)(height - j) / (float)height;

                    Ray r = new Ray(origin, lowerLeftCorner + u * horizontal + v * vertical);
                    Vector3 col = Color(r);

                    int index = (i + (j * width)) * 3;
                    pixels[index++] = (byte)(255.99 * col.Z);
                    pixels[index++] = (byte)(255.99 * col.Y);
                    pixels[index] = (byte)(255.99 * col.X);
                }
            }
            ////});

            sw.Stop();
            Console.WriteLine($"Elapsed Milliseconds: {sw.ElapsedMilliseconds}");

            fixed (byte* dataPointer = pixels)
            {
                Bitmap bmp = new Bitmap(width, height, width * 3, PixelFormat.Format24bppRgb, (IntPtr)dataPointer);
                bmp.Save(filename, ImageFormat.Png);
            }

            // Show render image
            var path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), filename);
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.FileName = path;
            startInfo.UseShellExecute = true;
            startInfo.CreateNoWindow = true;
            startInfo.Verb = string.Empty;
            Process.Start(startInfo);
        }
    }
}
