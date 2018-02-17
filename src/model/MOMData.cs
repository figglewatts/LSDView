using System.Collections.Generic;
using libLSD.Formats;
using LSDView.anim;
using LSDView.graphics;

namespace LSDView.model
{
    public class MOMData : IDisposable
    {
        public List<Mesh> MomTmd { get; }
        public List<TODAnimation> Animations { get; }
        public MOM Mom { get; }

        public MOMData(MOM mom, List<Mesh> tmd, List<TODAnimation> animations)
        {
            MomTmd = tmd;
            Animations = animations;
            Mom = mom;
        }

        public void Dispose()
        {
            foreach (var mesh in MomTmd)
            {
                mesh.Dispose();
            }
            MomTmd.Clear();

            foreach (var anim in Animations)
            {
                anim.Dispose();
            }
            Animations.Clear();
        }
    }
}
