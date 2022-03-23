
using System;
using System.IO;

using System.Windows.Forms;

namespace BundleCat {

    public static class Program {

        public static void Main (string[] args) {

            // Display.Render(true);

            DialogResult dialogResult = MessageBox.Show(
                "Welcome to the " + Values.ProjectName + " installer!\nClicking 'YES' will start the install, click 'NO' to cancel.",
                Values.ProjectName + ", " + Values.ProjectVersion + " Installer",
                MessageBoxButtons.YesNo
            );
            if (dialogResult == DialogResult.No) {

                MessageBox.Show(
                    "Installation cancelled..",
                    Values.ProjectName + ", " + Values.ProjectVersion + " Installer"
                );

                Environment.Exit(-1);
            }

            MessageBox.Show(
                "The installation has started for '" + Values.ProjectName + ", version " + Values.ProjectVersion + "' has begun..\n You will be notified when it has completed.",
                Values.ProjectName + ", " + Values.ProjectVersion + " Installer"
            );

            AssetLoader.PerformFullUnload();

            // Values.LoadingMessage = "creating shortcut...";
            // Display.Render(true);

            appShortcutToDesktop(Values.ProjectName + " " + Values.ProjectVersion,
                "C:/Users/" + Environment.UserName + "/MyGames/"
                + Values.ProjectName + "_" + Values.ProjectVersion + "/" + Values.ProjectExePath
            );

            // Values.LoadingMessage = "Done! installation succesfull";
            // Display.Render(true);

            MessageBox.Show(
                "Successfully installed '" + Values.ProjectName + ", version " + Values.ProjectVersion + "'!!",
                Values.ProjectName + ", " + Values.ProjectVersion + " Installer"
            );

            Environment.Exit(0);
        }

        private static void appShortcutToDesktop(string linkName, string linkPlace) {

            string deskDir = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);

            string contents = "[InternetShortcut]";
            string app = linkPlace;
            contents += "\nURL=file:///" + app;
            contents += "\nIconIndex=0";
            string icon = app.Replace('\\', '/');
            contents += "\nIconFile=" + icon;

            File.WriteAllText(deskDir + "\\" + linkName + ".url", contents);

            // using (StreamWriter swriter = new StreamWriter(deskDir + "\\" + linkName + ".url")) {
            //
            //     string app = linkPlace
            //     swriter.WriteLine("[InternetShortcut]");
            //     swriter.WriteLine("URL=file:///" + app);
            //     swriter.WriteLine("IconIndex=0");
            //     string icon = app.Replace('\\', '/');
            //     swriter.WriteLine("IconFile=" + icon);
            // }
        }
    }
}
