using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using LSDView.graphics;
using OpenTK;

namespace LSDView.view
{
    public interface ILSDView
    {
        TreeView ViewOutline { get; }
        GLControl ViewingWindow { get; }
        Mesh CreateTextureQuad();
        
        event EventHandler OnGLLoad;
    }
}
