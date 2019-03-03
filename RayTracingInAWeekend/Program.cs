﻿namespace RayTracingInAWeekend
{
    using System;
    using System.Diagnostics;
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.IO;
    using System.Reflection;
    using System.Runtime.Versioning;
    using System.Numerics;
    using System.Threading.Tasks;

    public unsafe class Program
    {
        public static FastRandom rand = new FastRandom(Environment.TickCount);

        public static Vector3 RandomInUnitSphere()
        {
            Vector3 p;
            do
            {
                p = 2f * new Vector3(rand.NextFloat(), rand.NextFloat(), rand.NextFloat()) - Vector3.One;
            } while (p.LengthSquared() >= 1.0f);

            return p;
        }

        public static Vector3 Color(Ray r, Scene world)
        {
            if (world.Hit(r, 0.001f, float.MaxValue, out HitRecord rec))
            {
                Vector3 target = rec.Position + rec.Normal + RandomInUnitSphere();
                return 0.5f * Color(new Ray(rec.Position, target - rec.Position), world);
            }
            else
            {
                Vector3 unitDirection = Vector3.Normalize(r.Direction);
                float t = (0.5f * unitDirection.Y) + 1.0f;
                return ((1.0f - t) * new Vector3(1f, 1f, 1f)) + (t * new Vector3(0.5f, 0.7f, 1f));
            }
        }

        public static void Main(string[] args)
        {
            int width = 800;
            int height = 400;
            int nSamples = 100;
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

            var spheres = new[]
            {
                new Sphere(new Vector3(0, 0, -1f), 0.5f),
                new Sphere(new Vector3(0, -100.5f, -1f), 100f),
            };

            Scene world = new Scene(spheres);
            Camera cam = Camera.Create();

            Parallel.For(0, height, j =>
            {
                for (int i = 0; i < width; i++)
                {
                    Vector3 col = Vector3.Zero;
                    for (int s = 0; s < nSamples; s++)
                    {
                        float u = (float)(i + rand.NextFloat()) / (float)width;
                        float v = (float)(height - (j + rand.NextFloat())) / (float)height;

                        Ray r = cam.GetRay(u, v);
                        col += Color(r, world);
                    }
                    col /= (float)nSamples;

                    // Gamma correction
                    col.X = MathF.Sqrt(col.X);
                    col.Y = MathF.Sqrt(col.Y);
                    col.Z = MathF.Sqrt(col.Z);

                    int index = (i + (j * width)) * 3;
                    pixels[index++] = (byte)(255.99 * col.Z);
                    pixels[index++] = (byte)(255.99 * col.Y);
                    pixels[index] = (byte)(255.99 * col.X);
                }
            });

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
