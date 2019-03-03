namespace RayTracingInAWeekend
{
    using System;
    using System.Diagnostics;
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.IO;
    using System.Reflection;
    using System.Runtime.Versioning;

    public unsafe class Program
    {
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

            ////Parallel.For(0, height, j =>
            for (int j = 0; j < height; j++)
            {
                for (int i = 0; i < width; i++)
                {
                    float r = (float)i / (float)width;
                    float g = (float)(height - j) / (float)height;
                    float b = 0.2f;

                    int index = (i + (j * width)) * 3;
                    pixels[index++] = (byte)(255.99 * b);
                    pixels[index++] = (byte)(255.99 * g);
                    pixels[index] = (byte)(255.99 * r);
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
