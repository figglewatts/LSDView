using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using System.IO;
using System.Threading;
using libLSD.Exceptions;
using libLSD.Formats;
using libLSD.Types;
using LSDView.graphics;
using LSDView.util;
using OpenTK.Graphics.OpenGL;
using OpenTK;

namespace LSDView.view
{
    public partial class LSDViewForm : Form
    {
        static float angle = 0.0f;

        private VertexBuffer buffer;

        public LSDViewForm()
        {
            InitializeComponent();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            viewingWindow.KeyDown += new KeyEventHandler(glControl_KeyDown);
            viewingWindow.KeyUp += new KeyEventHandler(glControl_KeyUp);
            viewingWindow.Resize += new EventHandler(glControl_Resize);
            viewingWindow.Paint += new PaintEventHandler(glControl_Paint);

            GL.ClearColor(Color.Black);
            GL.Enable(EnableCap.DepthTest);
            GL.Enable(EnableCap.CullFace);

            Application.Idle += Application_Idle;

            // Ensure that the viewport and projection matrix are set correctly.
            glControl_Resize(viewingWindow, EventArgs.Empty);
        }

        void glControl_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F12)
            {
                GrabScreenshot().Save("screenshot.png");
            }
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            Application.Idle -= Application_Idle;

            base.OnClosing(e);
        }

        void Application_Idle(object sender, EventArgs e)
        {
            while (viewingWindow.IsIdle)
            {
                Render();
            }
        }

        void glControl_Resize(object sender, EventArgs e)
        {
            OpenTK.GLControl c = sender as OpenTK.GLControl;

            if (c.ClientSize.Height == 0)
                c.ClientSize = new System.Drawing.Size(c.ClientSize.Width, 1);

            GL.Viewport(0, 0, c.Size.Width, c.Size.Height);

            float aspect_ratio = c.Size.Width / (float)c.Size.Height;
            Matrix4 perpective = Matrix4.CreatePerspectiveFieldOfView(MathHelper.PiOver4, aspect_ratio, 1, 64);
            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadMatrix(ref perpective);
        }

        void glControl_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyData)
            {
                case Keys.Escape:
                    this.Close();
                    break;
            }
        }

        void glControl_Paint(object sender, PaintEventArgs e)
        {
            Render();
        }

        private void Render()
        {
            Matrix4 lookat = Matrix4.LookAt(5, 0, -2, 0, 0, -1f, 0, 0, -1); // TODO: CHANGEME
            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadMatrix(ref lookat);

            GL.Rotate(angle, 0.0f, 0.0f, 1.0f);
            angle += 0.02f;

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            DrawCube();

            viewingWindow.SwapBuffers();
        }

        private void DrawCube()
        {
            if (buffer != null)
            {
                buffer.Bind();
                buffer.Draw();
            }

            /*GL.Begin(BeginMode.Points);

            GL.Color3(Color.Red);
            float scale = 500f;
            foreach (var v in pointCloud)
            {
                GL.Vertex3(v.X / scale, v.Y / scale, v.Z / scale);
            }
            GL.End();*/
        }

        Bitmap GrabScreenshot()
        {
            Bitmap bmp = new Bitmap(this.ClientSize.Width, this.ClientSize.Height);
            System.Drawing.Imaging.BitmapData data =
            bmp.LockBits(this.ClientRectangle, System.Drawing.Imaging.ImageLockMode.WriteOnly,
                System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            GL.ReadPixels(0, 0, this.ClientSize.Width, this.ClientSize.Height, PixelFormat.Bgr, PixelType.UnsignedByte,
                data.Scan0);
            bmp.UnlockBits(data);
            bmp.RotateFlip(RotateFlipType.RotateNoneFlipY);
            return bmp;
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if(openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)  
            {
                using (BinaryReader br = new BinaryReader(File.Open(openFileDialog.FileName, FileMode.Open)))
                {
                    Logger.Log()(LogLevel.INFO, "Opening {0}", openFileDialog.FileName);
                    TMD tmd = null;
                    try
                    {
                        tmd = new TMD(br);
                    }
                    catch (BadFormatException ex)
                    {
                        MessageBox.Show(ex.Message, "Error loading file", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    
                    List<Vertex> verts = new List<Vertex>();
                    foreach (var obj in tmd.ObjectTable)
                    {
                        foreach (var prim in obj.Primitives)
                        {
                            if (!prim.Options.HasFlag(TMDPrimitivePacket.OptionsFlags.Quad))
                            {
                                Vec3 p0, p1, p2;
                                p0 = obj.Vertices[prim.PacketData.p0];
                                p1 = obj.Vertices[prim.PacketData.p1];
                                p2 = obj.Vertices[prim.PacketData.p2];

                                verts.Add(new Vertex(p1 / 500f));
                                verts.Add(new Vertex(p0 / 500f));
                                verts.Add(new Vertex(p2 / 500f));

                                if ((prim.Flags & TMDPrimitivePacket.PrimitiveFlags.DoubleSided) != 0)
                                {
                                    verts.Add(new Vertex(p0 / 500f));
                                    verts.Add(new Vertex(p1 / 500f));
                                    verts.Add(new Vertex(p2 / 500f));
                                }
                            }
                            else
                            {
                                Vec3 p0, p1, p2, p3;
                                p0 = obj.Vertices[prim.PacketData.p0];
                                p1 = obj.Vertices[prim.PacketData.p1];
                                p2 = obj.Vertices[prim.PacketData.p2];
                                p3 = obj.Vertices[prim.PacketData.p3];

                                verts.Add(new Vertex(p1 / 500f));
                                verts.Add(new Vertex(p0 / 500f));
                                verts.Add(new Vertex(p2 / 500f));
                                verts.Add(new Vertex(p1 / 500f));
                                verts.Add(new Vertex(p2 / 500f));
                                verts.Add(new Vertex(p3 / 500f));

                                if ((prim.Flags & TMDPrimitivePacket.PrimitiveFlags.DoubleSided) != 0)
                                {
                                    verts.Add(new Vertex(p0 / 500f));
                                    verts.Add(new Vertex(p1 / 500f));
                                    verts.Add(new Vertex(p2 / 500f));
                                    verts.Add(new Vertex(p2 / 500f));
                                    verts.Add(new Vertex(p1 / 500f));
                                    verts.Add(new Vertex(p3 / 500f));
                                }
                            }
                        }
                    }

                    buffer = new VertexBuffer(verts.ToArray());
                }
            }  
        }
    }
}