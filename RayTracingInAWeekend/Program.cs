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
    using System.Threading.Tasks;

    public unsafe class Program
    {
        public static FastRandom rand = new FastRandom(Environment.TickCount);

        public static Vector3 InUnitSphere()
        {
            // 3D random vector in a unit sphere
            float r = MathF.Sqrt(rand.NextFloat());
            float u = rand.NextFloat();
            float v = rand.NextFloat();

            var phi = MathF.Acos((2f * v) - 1f);
            var theta = 2 * MathF.PI * u;

            var x = r * MathF.Cos(theta) * MathF.Sin(phi);
            var y = r * MathF.Sin(theta) * MathF.Sin(phi);
            var z = r * MathF.Cos(phi);

            Vector3 res = new Vector3((float)x, (float)y, (float)z);
            return res;

        }

        public static Vector3 RandomInUnitSphere()
        {
            Vector3 p;
            do
            {
                p = 2f * new Vector3(rand.NextFloat(), rand.NextFloat(), rand.NextFloat()) - Vector3.One;
            } while (p.LengthSquared() >= 1.0f);

            return p;
        }

        public static Vector3 Color(ref Ray r, ref Scene world, uint depth)
        {
            if (world.Hit(r, 0.001f, float.MaxValue, out HitRecord rec))
            {
                if (depth < 50 && rec.Material.Scatter(ref r, ref rec, out Vector3 attenuation, out Ray scattered))
                {
                    return attenuation * Color(ref scattered, ref world, depth + 1);
                }
                else
                {
                    return Vector3.Zero;
                }
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
            int nSamples = 1000;
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
                new Sphere(new Vector3(0, 0, -1f), 0.5f, new Lambertian(new Vector3(0.1f, 0.2f, 0.5f))),
                new Sphere(new Vector3(0, -100.5f, -1f), 100f, new Lambertian(new Vector3(0.8f, 0.8f, 0f))),
                new Sphere(new Vector3(1, -0, -1f), 0.5f, new Metal(new Vector3(0.8f, 0.6f, 0.2f), 0.3f)),
                new Sphere(new Vector3(-1, -0, -1f), -0.45f, new Dielectric(1.5f, 1f)),
            };

            Scene world = new Scene(spheres);

            Vector3 lookFrom = new Vector3(3, 3, 2);
            Vector3 lookat = new Vector3(0, 0, -1);
            float dist_to_focus = (lookFrom - lookat).Length();
            float aperture = 0.01f;
            Camera cam = Camera.Create(lookFrom, lookat, Vector3.UnitY, 20, (float)width / height, aperture, dist_to_focus);

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
                        col += Color(ref r, ref world, 0);
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
