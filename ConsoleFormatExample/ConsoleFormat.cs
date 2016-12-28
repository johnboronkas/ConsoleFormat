using System;
using System.Collections.Generic;
using System.Timers;

namespace ConsoleFormatExample
{
    public class ConsoleFormat
    {
        public int Width { get; private set; }
        public int Height { get; private set; }

        private Dictionary<string, Region> Regions;
        private Timer RegionColorChangeTimer;

        private static readonly object PaintKey = new object();

        public ConsoleFormat(int width, int height)
        {
            lock (PaintKey)
            {
                Resize(width, height);
                Regions = new Dictionary<string, Region>();

                RegionColorChangeTimer = new Timer(50);
                RegionColorChangeTimer.Elapsed += RegionColorChangeTimer_Elapsed;
            }
        }

        private void RegionColorChangeTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            foreach (string tag in Regions.Keys)
            {
                Region region = Regions[tag];

                if (region.ColorChange == null)
                {
                    continue;
                }

                if (DateTime.Now.CompareTo(region.ColorChange.Timeout) >= 0)
                {
                    PrintToRegion(tag, region.Text, region.ColorChange.ConsoleColor);

                    lock (PaintKey)
                    {
                        region.ColorChange = null;
                    }
                }
            }
        }

        public void Resize(int width, int height)
        {
            lock (PaintKey)
            {
                Console.SetWindowSize(width, height);

                Width = width;
                Height = height;
            }
        }

        public void DefineRegion(string tag, int x, int y, int width, int height)
        {
            lock (PaintKey)
            {
                if (Regions.ContainsKey(tag))
                {
                    return;
                }

                Regions.Add(tag, new Region(x, y, width, height));
            }
        }

        public void PrintToRegion(string tag, string text, ConsoleColor consoleColor = ConsoleColor.Gray)
        {
            lock (PaintKey)
            {
                Region region;
                if (Regions.TryGetValue(tag, out region))
                {
                    region.Text = text;

                    ConsoleColor original = Console.ForegroundColor;
                    Console.ForegroundColor = consoleColor;
                    PaintRegion(region);
                    Console.ForegroundColor = original;
                }
            }
        }

        public void RegisterRegionColorChange(string tag, ConsoleColor newConsoleColor, int timeoutMs)
        {
            lock (PaintKey)
            {
                Region region;
                if (Regions.TryGetValue(tag, out region) == false)
                {
                    return;
                }

                if (RegionColorChangeTimer.Enabled == false)
                {
                    RegionColorChangeTimer.Start();
                }

                region.ColorChange = new ColorChange(newConsoleColor, DateTime.Now.AddMilliseconds(timeoutMs));
            }
        }

        private void PaintRegion(Region region)
        {
            ClearRegion(region);
            WriteTextToRegion(region, region.Text);
        }

        private void ClearRegion(Region region)
        {
            int regionLength = region.Width * region.Height;

            string clearText = "";
            for (int i = 0; i < regionLength; i++)
            {
                clearText += " ";
            }

            WriteTextToRegion(region, clearText);
        }

        private void WriteTextToRegion(Region region, string text)
        {
            int x = region.Left;
            int y = region.Top;
            Console.SetCursorPosition(x, y);

            for (int i = 0; i < text.Length; i++)
            {
                if (x > region.Width)
                {
                    x = region.Left;
                    y = y + 1;
                    Console.SetCursorPosition(x, y);
                }

                if (y > region.Height + region.Top)
                {
                    return;
                }

                Console.Write(text[i]);
            }
        }
    } // class ConsoleFormat

    class Region
    {
        public int Left { get; private set; }
        public int Top { get; private set; }
        public int Width { get; private set; }
        public int Height { get; private set; }

        public string Text { get; set; }

        public ColorChange ColorChange { get; set; }

        public Region(int left, int top, int width, int height)
        {
            Left = left;
            Top = top;
            Width = width;
            Height = height;

            Text = "";

            ColorChange = null;
        }
    } // class Region

    class ColorChange
    {
        public ConsoleColor ConsoleColor { get; private set; }
        public DateTime Timeout { get; private set; }

        public ColorChange(ConsoleColor consoleColor, DateTime timeout)
        {
            ConsoleColor = consoleColor;
            Timeout = timeout;
        }
    } // class ColorChange
} // namespace
