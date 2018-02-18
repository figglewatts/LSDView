using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using libLSD.Formats;

namespace LSDView.controller
{
    public class TODController
    {
        private TOD _tod;

        public void RegisterTOD(TOD tod)
        {
            _tod = tod;
        }

        public void WriteTOD(string path, TOD? tod = null)
        {
            using (BinaryWriter bw = new BinaryWriter(File.Open(path, FileMode.Create)))
            {
                if (tod == null)
                {
                    _tod.Write(bw);
                }
                else
                {
                    tod.Value.Write(bw);
                }
            }
        }
    }
}
