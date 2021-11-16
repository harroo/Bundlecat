
using System;

namespace BundleCat {

    public static class Program {

        public static void Main (string[] args) {

            Display.Render();

            AssetLoader.PerformFullUnload();

            Values.LoadingMessage = "Done! installation succesfull";
            Display.Render();
        }
    }
}
