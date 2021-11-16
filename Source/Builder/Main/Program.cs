
using System;
using System.IO;
using System.IO.Compression;
using System.Diagnostics;
using System.Collections.Generic;

namespace BundleCat {

    public static class Program {

        private static List<string> compCache = new List<string>();

        public static void Main (string[] args) {

            Console.WriteLine("Initializing BundleCat...");


            Console.WriteLine("Reading Arguments...");

            string projectName = "", projectVersion = "", resourceDirectory = "", outputName = "", icon = "", mexe = "";
            foreach (string arg in args) {

                if (!arg.Contains(":")) continue;

                string[] values = arg.Split(':');

                switch (values[0]) {

                    case "-projectName": case "-name": projectName = values[1]; break;
                    case "-projectVersion": case "-ver": projectVersion = values[1]; break;
                    case "-resourceDirectory": case "-res": resourceDirectory = values[1]; break;
                    case "-out": case "-outputName": outputName = values[1]; break;
                    case "-icon": icon = values[1]; break;
                    case "-mainExecutable": case "-mex": mexe = values[1]; break;
                }
            }
            if (projectName == "") Fail("Please enter a projectName with -projectName");
            if (projectVersion == "") Fail("Please enter a projectVersion with -projectVersion");
            if (resourceDirectory == "") Fail("Please enter a resourceDirectory with -resourceDirectory");
            if (mexe == "") Fail("Please enter a mainExecutable with -mainExecutable \n(dont include the path to the resources, \neg: folder is (myresfolder and in it is myexe.exe) -res:myresfolder/ -mex:myexe.exe), \n but then like this: \neg: folder is (myresfolder and in it is bin/myexe.exe, so like myresfolder/bin/myexe.exe) -res:myresfolder/ -mex:bin/myexe.exe),"); //yes ik this is long, i got no excuse
            if (outputName == "") outputName = projectName + "_" + projectVersion + ".exe";
            if (icon != "") File.Copy(icon, "icontemp.ico"); else {
                LoadResource("defaulticon.ico"); File.Move("defaulticon.ico", "icontemp.ico");
            }

            Console.WriteLine("Building Asset index from" + resourceDirectory + "...");

            string responseData = "-out:" + outputName + " -win32icon:icontemp.ico";
            string assetScript = "namespace BundleCat { public static class Assets { public static string[] Values = new string[] {";
            foreach (string file in Directory.GetFiles(resourceDirectory, "*", SearchOption.AllDirectories)) {

                string hash = GenHash();

                Console.WriteLine("Include: " + hash + ": " + file);

                byte[] fdata = File.ReadAllBytes(file);
                File.WriteAllBytes(file + ".compressed", Compress(fdata));
                compCache.Add(file + ".compressed");

                responseData += "\n-res:'" + file + ".compressed'," + hash;
                assetScript += "\n\"" + file + "@" + hash + "\",";
            }
            assetScript += "}; } }";

            Console.WriteLine("SettingUp for Compilation...");

            File.WriteAllText("ResponseFile", responseData);
            File.WriteAllText("Assets.cs", assetScript);
            LoadResource("AssetLoader.cs");
            LoadResource("Display.cs");
            LoadResource("Program.cs");
            LoadResource("Values.cs");
            string valueScript = File.ReadAllText("Values.cs");
            valueScript = valueScript.Replace("PROJECT_NAME", projectName);
            valueScript = valueScript.Replace("PROJECT_VERSION", projectVersion);
            valueScript = valueScript.Replace("MAIN_EXEC_PATH", mexe);
            File.WriteAllText("Values.cs", valueScript);

            Console.WriteLine("Compiling...");

            ExecuteCommand("mcs AssetLoader.cs Display.cs Program.cs Values.cs Assets.cs @ResponseFile");

            Console.WriteLine("Cleaning up...");

            File.Delete("AssetLoader.cs");
            File.Delete("Display.cs");
            File.Delete("Program.cs");
            File.Delete("Values.cs");
            File.Delete("ResponseFile");
            File.Delete("Assets.cs");
            File.Delete("icontemp.ico");
            foreach (string cfile in compCache)
                File.Delete(cfile);

            Console.WriteLine("Done.");

            Environment.Exit(0);
        }

        private static void LoadResource (string resource) {

        	var assembly = System.Reflection.Assembly.GetExecutingAssembly();
        	var stream = assembly.GetManifestResourceStream(resource);

    		FileStream fileStream = File.Create(resource, (int)stream.Length);
    		byte[] bytesInStream = new byte[4096];
    		int byteReadCount = 0;
    		while (true) {

    			byteReadCount = stream.Read(bytesInStream, 0, 4096);
    			if (byteReadCount == 0) break;
    			fileStream.Write(bytesInStream, 0, byteReadCount);
    		}
    		fileStream.Close();
        }

        private static void Fail (string message) {

            Console.WriteLine("FATAL: " + message);

            Environment.Exit(-1);
        }

        private static void ExecuteCommand (string command) {

            Process bashProcess = new Process();
            bashProcess.StartInfo.RedirectStandardOutput = true;
            bashProcess.StartInfo.UseShellExecute = false;
            bashProcess.StartInfo.CreateNoWindow = true;
            bashProcess.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            bashProcess.StartInfo.FileName = "/bin/bash";
            bashProcess.StartInfo.Arguments = "-c \"" + command + "\"";
            bashProcess.Start();
            while (!bashProcess.StandardOutput.EndOfStream) {

                Console.WriteLine(bashProcess.StandardOutput.ReadLine());
            }
            bashProcess.WaitForExit();
        }

        private static List<string> existingHashes = new List<string>();
        private static string symbols = "QWERTYUIOPASDFGHJKLZXCVBNM1234567890qwertyuiopasdfghjklzxcvbnm";
        private static string GenHash () {

            Random random = new Random();

            string hash = "";

            do {

                hash = "";

                for (int i = 0; i < 42; ++i)
                    hash += symbols[random.Next(0, symbols.Length - 1)];

            } while (existingHashes.Contains(hash));

            existingHashes.Add(hash);

            return hash;
        }

        private static byte[] Compress (byte[] input) {

            using (MemoryStream memory = new MemoryStream()) {

                using (GZipStream zipStream = new GZipStream(memory, CompressionMode.Compress, true)) {

                    zipStream.Write(input, 0, input.Length);
                }

                return memory.ToArray();
            }
        }
    }
}
