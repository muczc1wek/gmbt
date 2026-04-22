using System.IO;
using System.Diagnostics;
using System.Collections.Generic;

namespace GMBT
{
    /// <summary> 
    /// Performs operations related to building VDF volume.
    /// </summary>
    internal class VDF
    {
        private readonly Gothic gothic;

        private readonly List<string> directoriesToPack = new List<string>();
        private readonly List<string> directoriesToInclude = new List<string>();

        public VDF (Gothic gothic)
        {
            this.gothic = gothic;        
        }

        public void RunBuilder()
        {
            BuildMain();
            BuildUnion();
        }



        public void BuildMain()
        {
            Logger.Normal("VDF.Building".Translate());

            if (Directory.Exists(Path.GetDirectoryName(Program.Config.ModVdf.Output)) == false)
            {
                Directory.CreateDirectory(Path.GetDirectoryName(Program.Config.ModVdf.Output));
            }

            ProcessStartInfo builder = new ProcessStartInfo
            {
                FileName = Program.AppData.GetTool("GothicVDFS.exe"),
                UseShellExecute = false,
                CreateNoWindow = true
            };

            directoriesToPack.Add(@"_work\Data\Anims\_compiled");
            directoriesToPack.Add(@"_work\Data\Meshes\_compiled");
            directoriesToPack.Add(@"_work\Data\Textures\_compiled"); 
            directoriesToPack.Add(@"_work\Data\Scripts\_compiled");
            directoriesToPack.Add(@"_work\Data\Scripts\Content\Cutscene");
            directoriesToPack.Add(@"_work\Data\Worlds");

            directoriesToInclude.Add(@"_work\Data\Anims\*.mds -r");
            directoriesToInclude.Add(@"_work\Data\Textures\Desktop\*.tga -r");
           
            if (Program.Options.BuildVerb.NoPackSounds == false && Program.Options.PackVerb.NoPackSounds == false)
            {
                directoriesToPack.Add(@"_work\Data\Sound");
            }
            if (Program.Options.BuildVerb.PackMusic == true || Program.Options.PackVerb.PackMusic == true)
            {
                directoriesToPack.Add(@"_work\Data\Music");
            }

            string vdfOutput = Program.Options.BuildVerb.Output ?? Program.Options.PackVerb.Output ?? Program.Config.ModVdf.Output;

            var include = new List<string>();

            include.AddRange(directoriesToInclude);

            if (Program.Config.ModVdf.Include != null)
            {
                include.AddRange(Program.Config.ModVdf.Include);
            }

            VDFScript script = new VDFScript(gothic.GetGameDirectory(Gothic.GameDirectory.Root), vdfOutput, Program.Options.BuildVerb.Comment ?? Program.Options.PackVerb.Comment ?? Program.Config.ModVdf.Comment ?? Program.Config.ProjectName ?? string.Empty, directoriesToPack, include, Program.Config.ModVdf.Exclude);

            builder.Arguments = "/B " + script.GenerateAndGetPath();

            Process.Start(builder).WaitForExit();
        }

        public void BuildUnion()
        {
            Logger.Normal("VDF.Building".Translate());

            if (Directory.Exists(Path.GetDirectoryName(Program.Config.ModVdf.Output)) == false)
            {
                Directory.CreateDirectory(Path.GetDirectoryName(Program.Config.ModVdf.Output));
            }

            ProcessStartInfo builder = new ProcessStartInfo
            {
                FileName = Program.AppData.GetTool("GothicVDFS.exe"),
                UseShellExecute = false,
                CreateNoWindow = true
            };

            directoriesToPack.Add(@"System");
            directoriesToPack.Add(@"System\Autorun");

            string vdfOutput = Program.Options.BuildVerb.Output ?? Program.Options.PackVerb.Output ?? Program.Config.ModVdf.Output;
            vdfOutput = Path.Combine(Path.GetDirectoryName(vdfOutput), Path.GetFileNameWithoutExtension(vdfOutput) + "_union" + Path.GetExtension(vdfOutput));


            VDFScript script = new VDFScript("union", vdfOutput, Program.Options.BuildVerb.Comment ?? Program.Options.PackVerb.Comment ?? Program.Config.ModVdf.Comment ?? Program.Config.ProjectName ?? string.Empty, directoriesToPack, null, null);

            builder.Arguments = "/B " + script.GenerateAndGetPath();

            Process.Start(builder).WaitForExit();
        }
    }
}
