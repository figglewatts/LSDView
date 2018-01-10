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

        private Shader basic;

        private Vertex[] vertices = 
        {
            new Vertex(new Vector3(0, 0, 0), Vector3.Zero, Vector2.Zero, new Vector4(1, 0, 0, 1)),
            new Vertex(new Vector3(0, 1, 0), Vector3.Zero, Vector2.Zero, new Vector4(0, 1, 0, 1)),
            new Vertex(new Vector3(1, 1, 0), Vector3.Zero, Vector2.Zero, new Vector4(0, 0, 1, 1))
        };

        private int[] indices =
        {
            0, 1, 2
        };

        private Mesh test;

        private Matrix4 projection;
        private Matrix4 view;

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

            basic = new Shader("basic", "shaders/basic");
            test = new Mesh(vertices, indices, basic);

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
            projection = Matrix4.CreatePerspectiveFieldOfView(MathHelper.PiOver4, aspect_ratio, 1, 64);
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
            view = Matrix4.LookAt(0, 0, -5, 0, 0, 0, 0, 1, 0); // TODO: CHANGEME

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            DrawCube();

            viewingWindow.SwapBuffers();
        }

        private void DrawCube()
        {
            ErrorCode err = GL.GetError();
            test.Render(view, projection);
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
                    
                    /*
                    List<Vertex> verts = new List<Vertex>();
                    foreach (var obj in tmd.ObjectTable)
                    {
                        foreach (var prim in obj.Primitives)
                        {
                            if (!prim.Options.HasFlag(TMDPrimitivePacket.OptionsFlags.Quad))
                            {
                                Vec3 p0, p1, p2;
                                p0 = obj.Vertices[prim.PacketData.p0] / 500f;
                                p1 = obj.Vertices[prim.PacketData.p1] / 500f;
                                p2 = obj.Vertices[prim.PacketData.p2] / 500f;

                                Vertex v1


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
                                p0 = obj.Vertices[prim.PacketData.p0] / 500f;
                                p1 = obj.Vertices[prim.PacketData.p1] / 500f;
                                p2 = obj.Vertices[prim.PacketData.p2] / 500f;
                                p3 = obj.Vertices[prim.PacketData.p3] / 500f;

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
                    */
                }
            }  
        }
    }
}