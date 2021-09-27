using System.Collections.Generic;

namespace LSDView.Models
{
    public class Dream
    {
        public const int CHUNK_DIMENSION = 20;

        public List<LBDDocument> Chunks { get; }
        public int LevelWidth { get; protected set; }
        public List<TIXDocument> TextureSets { get; }

        public Dream(List<LBDDocument> chunks, int levelWidth, List<TIXDocument> textureSets)
        {
            Chunks = chunks;
            TextureSets = textureSets;
            LevelWidth = levelWidth;
        }
    }
}
