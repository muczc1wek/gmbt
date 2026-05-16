using System;
using System.Collections.Generic;
using System.IO;

using Szmyk.Utils.Directory;

namespace GMBT
{
    internal class Cleanup
    {
        private readonly Gothic gothic;
        private readonly string preset;
        private readonly Dictionary<string, List<string>> cleanupDirectories;
        private readonly List<string> toClean;


        public Cleanup(Gothic gothic, string preset)
        {
            this.gothic = gothic;
            this.preset = preset;
            this.cleanupDirectories = Program.Config.CleanupDirectories;
            this.toClean = new List<string>();
            // add 'default' directories to clean
            if (cleanupDirectories.TryGetValue("default", out var defaultDirectories))
            {
                toClean.AddRange(defaultDirectories);
            }
            // add preset directories to clean
            if (!string.IsNullOrEmpty(preset) && cleanupDirectories.TryGetValue(preset, out var presetDirectories))
            {
                toClean.AddRange(presetDirectories);
            }
        }

        public void CleanAssets()
        {
            foreach (var dir in toClean)
            {
                string directory = gothic.GetGameDirectory(Gothic.GameDirectory.WorkData) + "\\" + dir;
                if (new DirectoryInfo(directory).Exists)
                {
                    new DirectoryHelper(directory).Delete();
                }
            }
        }
    }

}