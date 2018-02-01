using System.Collections.Generic;
using libLSD.Formats;
using LSDView.graphics;

namespace LSDView.anim
{
    public class TODAnimation : IDisposable
    {
        public List<Mesh> AnimationMeshes { get; }
        public TOD AnimationData { get; }

        public TODAnimation(List<Mesh> animMeshes, TOD data)
        {
            AnimationData = data;
            AnimationMeshes = animMeshes;
        }

        public void Dispose()
        {
            foreach (var mesh in AnimationMeshes)
            {
                mesh.Dispose();
            }
            AnimationMeshes.Clear();
        }
    }
}
