using System.Collections.Generic;
using libLSD.Formats;
using libLSD.Formats.Packets;
using LSDView.Controllers.Interface;
using LSDView.Math;
using LSDView.Models;
using OpenTK;

namespace LSDView.Controllers.GUI
{
    public class AnimationController : IAnimationController
    {
        private TOD _animation;

        public TOD Animation
        {
            get => _animation;
            set
            {
                _animation = value;
                CurrentFrame = -1;
            }
        }

        public MOMDocument Focus { get; private set; }
        public bool Active { get; set; }
        public int CurrentFrame { get; set; }

        public const double TICK = 1d / 60;

        protected double time = 0;

        protected readonly Dictionary<int, AnimationObjectTableEntry> _objectTable;

        protected class AnimationObjectTableEntry
        {
            public int TmdID;
            public readonly Transform Transform;

            public AnimationObjectTableEntry()
            {
                TmdID = -1;
                Transform = new Transform();
            }
        }

        protected bool Ready => Animation.Header.ID != 0 && Focus != null;

        public AnimationController()
        {
            Active = false;
            Focus = null;
            CurrentFrame = -1;
            _objectTable = new Dictionary<int, AnimationObjectTableEntry> {[0] = new AnimationObjectTableEntry()};
        }

        public void SetFocus(MOMDocument mom, int animationIdx = 0)
        {
            Active = true;
            Focus = mom;
            if (mom != null) Animation = mom.Document.MOS.TODs[animationIdx];
            CurrentFrame = -1;
        }

        public void Update(double dt)
        {
            if (!Active || !Ready) return;

            double tickRate = Animation.Header.Resolution * TICK;

            time += dt;
            if (time > tickRate)
            {
                time = 0;

                CurrentFrame++;

                processFrame(CurrentFrame);
            }
        }

        protected void processFrame(int frameNumber)
        {
            if (frameNumber >= Animation.Header.NumberOfFrames - 1)
            {
                frameNumber = 0;
                CurrentFrame = 0;
            }

            if (Animation.Frames.Length <= 0) return;

            TODFrame frame = Animation.Frames[frameNumber];

            if (frame.Packets == null)
                return;

            foreach (var packet in frame.Packets)
            {
                if (packet.Data is TODObjectControlPacketData)
                {
                    handleObjectControlPacket(packet);
                }
                else if (packet.Data is TODObjectIDPacketData)
                {
                    handleObjectIDPacket(packet);
                }
                else if (packet.Data is TODCoordinatePacketData)
                {
                    handleCoordinatePacket(packet);
                }
            }
        }

        protected void handleObjectControlPacket(TODPacket packet)
        {
            TODObjectControlPacketData packetData = packet.Data as TODObjectControlPacketData;
            if (packetData.ObjectControl == TODObjectControlPacketData.ObjectControlType.Create)
            {
                _objectTable[packet.ObjectID] = new AnimationObjectTableEntry();
            }
            else if (packetData.ObjectControl == TODObjectControlPacketData.ObjectControlType.Kill)
            {
                _objectTable.Remove(packet.ObjectID);
            }
        }

        protected void handleObjectIDPacket(TODPacket packet)
        {
            TODObjectIDPacketData packetData = packet.Data as TODObjectIDPacketData;
            if (packet.PacketType == TODPacket.PacketTypes.TMDDataID)
            {
                // create mapping in object table
                _objectTable[packet.ObjectID].TmdID = packetData.ObjectID;
                Focus.Models.ObjectMeshes[packetData.ObjectID - 1].Transform.Parent =
                    _objectTable[packet.ObjectID].Transform;
            }
            else if (packet.PacketType == TODPacket.PacketTypes.ParentObjectID)
            {
                // set object parent
                Transform parentTransform = _objectTable[packetData.ObjectID].Transform;
                _objectTable[packet.ObjectID].Transform.Parent = parentTransform;
            }
        }

        protected void handleCoordinatePacket(TODPacket packet)
        {
            TODCoordinatePacketData packetData = packet.Data as TODCoordinatePacketData;

            Transform objTransform = _objectTable[packet.ObjectID].Transform;
            if (packetData.HasScale)
            {
                if (packetData.MatrixType == TODPacketData.PacketDataType.Absolute)
                {
                    objTransform.Scale = new Vector3(packetData.ScaleX / 4096f, packetData.ScaleY / 4096f,
                        packetData.ScaleZ / 4096f);
                }
                else
                {
                    objTransform.Scale *= new Vector3(packetData.ScaleX / 4096f, packetData.ScaleY / 4096f,
                        packetData.ScaleZ / 4096f);
                }
            }

            if (packetData.HasTranslation)
            {
                if (packetData.MatrixType == TODPacketData.PacketDataType.Absolute)
                {
                    objTransform.Position =
                        new Vector3(packetData.TransX, packetData.TransY, packetData.TransZ) / 2048f;
                }
                else
                {
                    objTransform.Translate(new Vector3(packetData.TransX, packetData.TransY, packetData.TransZ) /
                                           2048f);
                }
            }

            if (packetData.HasRotation)
            {
                float pitch = MathHelper.DegreesToRadians(packetData.RotX / 4096f);
                float yaw = MathHelper.DegreesToRadians(packetData.RotY / 4096f);
                float roll = MathHelper.DegreesToRadians(packetData.RotZ / 4096f);

                if (packetData.MatrixType == TODPacketData.PacketDataType.Absolute)
                {
                    objTransform.Rotation = Quaternion.Identity;
                    objTransform.Rotation *= Quaternion.FromAxisAngle(Vector3.UnitX, pitch);
                    objTransform.Rotation *= Quaternion.FromAxisAngle(Vector3.UnitY, yaw);
                    objTransform.Rotation *= Quaternion.FromAxisAngle(Vector3.UnitZ, roll);
                }
                else
                {
                    objTransform.Rotate(pitch, Vector3.UnitX, true);
                    objTransform.Rotate(yaw, Vector3.UnitY, true);
                    objTransform.Rotate(roll, Vector3.UnitZ, true);
                }
            }
        }
    }
}
