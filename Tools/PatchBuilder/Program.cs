﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using PatchBuilder.Properties;
using Stump.Core.Cryptography;
using Stump.Core.Xml;
using Uplauncher.Patcher;

namespace PatchBuilder
{
    class Program
    {
        static void Main(string[] args)
        {
            string patchDir;
            if (args.Length == 0)
            {
                Console.WriteLine(Resources.Program_Main_Give_the_patch_directory_in_argument);
                patchDir = Console.ReadLine();
            }
            else
            {
                patchDir = args[0];
            }

            if (File.Exists(Path.Combine(patchDir ?? throw new InvalidOperationException(), "patch.xml")))
                File.Delete(Path.Combine(patchDir, "patch.xml"));

            var files = Directory.EnumerateFiles(patchDir, "*", SearchOption.AllDirectories).OrderBy(p => p).ToList();

            foreach (var file in files.Where(file => file.StartsWith("patch\\Resources")).ToArray())
            {
                files.Remove(file);
                files.Add(file);
            }

            var tasks = new List<MetaFileEntry>();
            using (var md5Hasher = MD5.Create())
            {
                for (int i = 0; i < files.Count; i++)
                {
                    var file = files[i];

                    if ((File.GetAttributes(file) & FileAttributes.Hidden) != 0)
                        continue;

                    var content = File.ReadAllBytes(file);
                    var md5Hasher2 = MD5.Create();
                    
                    var task = new MetaFileEntry
                    {
                        LocalURL = GetRelativePath(file, patchDir + "\\"),
                        RelativeURL = GetRelativePath(file, patchDir + "\\"),
                        FileMD5 = Convert.ToBase64String(md5Hasher2.ComputeHash(content)),
                        FileSize = content.Length,
                    };

                    md5Hasher2.Dispose();

                    var pathBytes = Encoding.UTF8.GetBytes(task.LocalURL.ToLower());
                    md5Hasher.TransformBlock(pathBytes, 0, pathBytes.Length, pathBytes, 0);
                    if (i == files.Count - 1)
                        md5Hasher.TransformFinalBlock(content, 0, content.Length);
                    else
                        md5Hasher.TransformBlock(content, 0, content.Length, content, 0);

                    tasks.Add(task);
                    Console.WriteLine(@"Add " + task.RelativeURL);
                }

                var patch = new MetaFile
                {
                    Tasks = tasks.ToArray(),
                    FolderChecksum = BitConverter.ToString(md5Hasher.Hash).Replace("-", "").ToLower(),
                };


                XmlUtils.Serialize(Path.Combine(patchDir, "patch.xml"), patch);
                Console.WriteLine(@"Created Patch in {0} !", Path.Combine(patchDir, "patch.xml"));
            }

            Console.Read();
        }

        static string GetRelativePath(string fullPath, string relativeTo)
        {
            var foldersSplitted = fullPath.Split(new[] { relativeTo.Replace("/", "\\").Replace("\\\\", "\\") }, StringSplitOptions.RemoveEmptyEntries); // cut the source path and the "rest" of the path

            return foldersSplitted.Length > 0 ? foldersSplitted.Last() : ""; // return the "rest"
        }
    }
}
