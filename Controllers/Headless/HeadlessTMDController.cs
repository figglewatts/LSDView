using System.Collections.Generic;
using libLSD.Formats;
using LSDView.Controllers.Interface;
using LSDView.Graphics;
using LSDView.Models;
using LSDView.Util;

namespace LSDView.Controllers.Headless
{
    public class HeadlessTMDController : IFileFormatController<TMD, TMDDocument>
    {
        public TMD Load(string path) { return LibLSDUtil.LoadTMD(path); }

        public TMDDocument CreateDocument(TMD tmd)
        {
            List<IRenderable> objectMeshes =
                LibLSDUtil.CreateMeshesFromTMD(tmd, null, null, headless: true);
            return new TMDDocument(tmd, objectMeshes);
        }
    }
}
