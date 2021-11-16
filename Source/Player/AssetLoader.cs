
using System;
using System.IO;
using System.IO.Compression;
using System.Collections.Generic;

namespace BundleCat {

    public static class AssetLoader {

        public static void PerformFullUnload () {

            int count = 0;

            foreach (string file in Assets.Values) {

                // string filePath = file.Split('@')[0];
                string filePath = SplitString(file, '@')[0];

                // string fileName = filePath.Split('/')[file.Split('/').Length - 1];
                string fileName = SplitString(filePath, '/')[SplitString(file, '/').Length - 1];

                // string rootFolderName = filePath.Split('/')[0];
                string rootFolderName = SplitString(filePath, '/')[0];

                // string hash = file.Split('@')[1];
                string hash = SplitString(file, '@')[1];

                string fullDir = "";

                // foreach (string value in filePath.Split('/')) {
                foreach (string value in SplitString(filePath, '/')) {

                    if (value != fileName && value != rootFolderName)
                        fullDir += fullDir == "" ? value : "/" + value;
                }

                string path = "C:/Users/" + Environment.UserName + "/MyGames/"
                    + Values.ProjectName + "_" + Values.ProjectVersion + "/" + fullDir;

                // string path = Values.ProjectName + "_" + Values.ProjectVersion + "/" + fullDir;

                EnsureDirectory(path);

                Values.LoadingMessage = "Unloading Asset: " + fileName;

                LoadAsset(hash, path + "/" + fileName);

                count++;
                Values.TotalPercent = (float)count / Assets.Values.Length;
                Display.Render();
            }
        }

        private static void EnsureDirectory (string directory) {

            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);
        }

        private static void LoadAsset (string assetName, string destination) {

        	var assembly = System.Reflection.Assembly.GetExecutingAssembly();
        	var stream = assembly.GetManifestResourceStream(assetName);

    		FileStream fileStream = File.Create(destination, (int)stream.Length);
    		byte[] bytesInStream = new byte[stream.Length];
			stream.Read(bytesInStream, 0, bytesInStream.Length);

            bytesInStream = Decompress(bytesInStream);

			fileStream.Write(bytesInStream, 0, bytesInStream.Length);

    		fileStream.Close();
        }

        private static byte[] Decompress (byte[] input) {

            using (GZipStream zipStream = new GZipStream(new MemoryStream(input), CompressionMode.Decompress)) {

                const int size = 4096;
                byte[] buffer = new byte[size];

                using (MemoryStream memory = new MemoryStream()) {

                    int count = 0;

                    do {

                        count = zipStream.Read(buffer, 0, size);

                        if (count > 0) {

                            memory.Write(buffer, 0, count);
                        }

                    } while (count > 0);

                    return memory.ToArray();
                }
            }
        }

        //windows hates string.Split, so i made my own 1
        //and yea ik its unoptimized, it just need to do tha job
        private static string[] SplitString (string input, char marker) {

            List<string> strings = new List<string>();

            string temp = "";

            foreach (char c in input) {

                if (c == marker) {

                    strings.Add(temp);
                    temp = "";
                    continue;
                }

                temp += c.ToString();
            }

            strings.Add(temp);

            return strings.ToArray();
        }
    }
}
