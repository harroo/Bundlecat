
using System;

namespace BundleCat {

    public static class Display {

        public static void Render () {

            Console.Clear();
            Console.WriteLine("Bundlecat " + "indev");

            for (int i = 0; i < Console.WindowHeight / 4; ++i)
                Console.WriteLine();

            string msg = "Installing " + Values.ProjectName;
            for (int i = 0; i < Console.WindowWidth / 2 - (msg.Length / 2); ++i)
                Console.Write(" ");
            Console.WriteLine(msg);

            for (int i = 0; i < Console.WindowHeight / 6; ++i)
                Console.WriteLine();

            DrawBar(Values.TotalPercent);
            DrawBar(Values.TotalPercent);
            DrawBar(Values.TotalPercent);

            Console.WriteLine();
            Console.Write(" ");
            Console.WriteLine(Values.LoadingMessage);
            Console.WriteLine();
        }

        private static void DrawBar (float percentage) {

            Console.Write("   [");
            for (int i = 0; i < Console.WindowWidth - 8; ++i)
                Console.Write((percentage * Console.WindowWidth >= i ? "=" : " "));

            Console.WriteLine("]");
        }
    }
}