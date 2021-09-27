using System;
using System.Collections.Generic;
using System.IO;
using CommandLine;
using LSDView.Controllers;
using LSDView.Controllers.Headless;
using LSDView.Models;

namespace LSDView.Headless
{
    public class ExportLevelCommand : AbstractHeadlessCommand
    {
        public override Type OptionsType => typeof(ExportLevelOptions);

        [Verb("exportdream", HelpText = "Export a directory of LBD files to a single mesh and textures.")]
        public class ExportLevelOptions : GlobalOptions
        {
            [Option('i', "input", Required = true, HelpText = "The folder containing LBD files to export.")]
            public string Input { get; set; }

            [Option('o', "output", Required = true, HelpText = "The folder to export the complete dream to.")]
            public string Output { get; set; }

            [Option('w', "width", Default = -1, HelpText = "The width of the dream in LBD chunks. " +
                                                           "If not specified, will attempt to automatically " +
                                                           "figure it out.")]
            public int Width { get; set; }

            [Option('f', "format", Default = "ply", HelpText = "The format to export to. Valid values: 'obj', 'ply'. ")]
            public string Format { get; set; }

            [Option('s', "separate-objects", Default = false, HelpText = "Whether or not to separate LBD files in the" +
                                                                         " exported mesh, or combine them into one mesh.")]
            public bool SeparateObjects { get; set; }
        }

        protected readonly HeadlessLBDController _lbdController;
        protected readonly HeadlessTIXController _tixController;
        protected readonly HeadlessExportController _exportController;

        public ExportLevelCommand(HeadlessLBDController lbdController,
            HeadlessTIXController tixController,
            HeadlessExportController exportController)
        {
            _lbdController = lbdController;
            _tixController = tixController;
            _exportController = exportController;
        }

        public override void Register(ref ParserResult<object> parserResult)
        {
            parserResult.WithParsed<ExportLevelOptions>(Handle);
        }

        public void Handle(ExportLevelOptions options)
        {
            handleGlobalOptions(options);

            if (!Directory.Exists(options.Input))
            {
                string errorMessage = $"Input directory '{options.Input}' did not exist";
                throw new HeadlessException(errorMessage);
            }

            // determine export format
            var exportFormat = determineExportFormat(options.Format);

            if (options.Width == -1)
            {
                // auto detect level width
                var detectedWidth = detectDreamWidth(options.Input);
                if (detectedWidth == -1)
                {
                    throw new HeadlessException("Unable to auto detect dream width: directory must be named " +
                                                "in STGXX format, like the original game: you can also " +
                                                "supply a custom dream width with the --width flag");
                }
            }

            // gather LBD files
            var dreamChunkFiles = Directory.GetFiles(options.Input, "*.lbd", SearchOption.AllDirectories);
            if (dreamChunkFiles.Length <= 0)
            {
                string errorMessage = $"Unable to export dream: no LBD files found in '{options.Input}'";
                throw new HeadlessException(errorMessage);
            }

            // load LBD files
            List<LBDDocument> dreamChunks = new List<LBDDocument>(dreamChunkFiles.Length);
            foreach (var dreamChunkFile in dreamChunkFiles)
            {
                var lbd = _lbdController.Load(dreamChunkFile);
                dreamChunks.Add(_lbdController.CreateDocument(lbd));
            }

            // gather TIX files
            var textureSetFiles = Directory.GetFiles(options.Input, "*.tix");
            if (textureSetFiles.Length != 4)
            {
                string errorMessage =
                    $"Unable to export dream: there should be exactly 4 TIX files in '{options.Input}'" +
                    $", found: {textureSetFiles.Length} instead";
                throw new HeadlessException(errorMessage);
            }

            // load TIX files
            List<TIXDocument> textureSets = new List<TIXDocument>(textureSetFiles.Length);
            foreach (var textureSetFile in textureSetFiles)
            {
                var tix = _tixController.Load(textureSetFile);
                textureSets.Add(_tixController.CreateDocument(tix));
            }

            // export the dream
            Dream toExport = new Dream(dreamChunks, options.Width, textureSets);
            _exportController.ExportDream(toExport, !options.SeparateObjects, exportFormat, options.Output);
        }

        protected MeshExportFormat determineExportFormat(string format)
        {
            format = format.ToLowerInvariant();
            switch (format.ToLowerInvariant())
            {
                case "obj":
                    return MeshExportFormat.OBJ;
                case "ply":
                    return MeshExportFormat.PLY;
                default:
                    throw new HeadlessException($"Unrecognized mesh export format '{format}': not in (obj, ply)");
            }
        }

        /// <summary>
        /// Dream chunk widths by directory name, to support in auto-detecting dream widths in detectDreamWidth().
        /// Source is: https://docs.lsdrevamped.net/lsd-de-research/static-analysis/file-formats#lbd
        /// </summary>
        protected readonly Dictionary<string, int> _dreamWidthsByName = new Dictionary<string, int>()
        {
            { "stg00", 0 },  // bright moon cottage
            { "stg01", 3 },  // pit & temple
            { "stg02", 6 },  // kyoto
            { "stg03", 16 }, // the natural world
            { "stg04", 6 },  // happytown
            { "stg05", 5 },  // violence district
            { "stg06", 0 },  // moonlight tower
            { "stg07", 5 },  // temple dojo
            { "stg08", 1 },  // flesh tunnels
            { "stg09", 1 },  // clockwork machines
            { "stg10", 3 },  // long hallway
            { "stg11", 4 },  // sun faces heave
            { "stg12", 4 },  // black space
            { "stg13", 2 }   // monument park
        };

        /// <summary>
        /// Attempt to auto detect the width of a dream based on the name of the directory.
        /// If it cannot auto-detect the width, it will return 0.
        /// </summary>
        /// <param name="inputPath">The directory containing the dream's LBD files.</param>
        /// <returns>The auto-detected width, or -1 if unable to detect.</returns>
        protected int detectDreamWidth(string inputPath)
        {
            if (!inputPath.EndsWith(Path.DirectorySeparatorChar.ToString()))
            {
                inputPath += Path.DirectorySeparatorChar;
            }

            var dirName = Path.GetFileName(Path.GetDirectoryName(inputPath))?.ToLowerInvariant();
            if (dirName == null) return -1;

            if (_dreamWidthsByName.ContainsKey(dirName))
            {
                return _dreamWidthsByName[dirName];
            }

            return -1;
        }
    }
}
