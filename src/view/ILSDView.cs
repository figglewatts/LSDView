using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using LSDView.anim;
using LSDView.graphics;
using OpenTK;

namespace LSDView.view
{
    public interface ILSDView
    {
        TreeView ViewOutline { get; }
        GLControl ViewingWindow { get; }
        Mesh CreateTextureQuad();
		AnimationPlayer AnimPlayer { get; }
        MenuStrip MenuStrip { get; }
        SaveFileDialog SaveDialog { get; }
        OpenFileDialog OpenDialog { get; }
        
        event EventHandler OnGLLoad;
    }
}
