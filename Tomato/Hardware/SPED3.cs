using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Threading;

namespace Tomato.Hardware
{
    public enum SPED3State
    {
        STATE_NO_DATA = 0,
        STATE_RUNNING = 1,
        STATE_TURNING = 2
    }

    public enum SPED3Error
    {
        ERROR_NONE = 0,
        ERROR_BROKEN = 1
    }

    public class SPED3 : Device
    {
        public SPED3()
        {
            rotationTimer = new Timer(UpdateRotation, null, 16, Timeout.Infinite); // Updates at 60 Hz
            EnableFlickering = true;
            FlickerMultipler = 1;
        }

        private Timer rotationTimer;
        float speed = 0.02f;
        private void UpdateRotation(object discarded)
        {
            if (TargetRotation != CurrentRotation)
            {
                if (Math.Abs(TargetRotation - CurrentRotation) < speed)
                    CurrentRotation = TargetRotation;
                else
                {
                    if (CompareDegrees(CurrentRotation, TargetRotation) <= 0)
                        CurrentRotation -= speed;
                    else
                        CurrentRotation += speed;
                    if (speed < 0.8f)
                        speed += 0.02f;
                    while (CurrentRotation < 0)
                        CurrentRotation += 360;
                    CurrentRotation %= 360;
                }
            }
            else
            {
                State = SPED3State.STATE_RUNNING;
                if (speed > 0.02f)
                    speed -= 0.02f;
            }
            rotationTimer = new Timer(UpdateRotation, null, 16, Timeout.Infinite); // Updates at 60 Hz
        }

        private static float CompareDegrees(float A, float B)
        {
            if (A > 180 && B < 180)
                return (360 - A) + B;
            if (A < 180 && B > 180)
                return -((360 - B) + A);
            return B - A;
        }

        public event EventHandler VerticiesChanged;

        public SPED3State State { get; private set; }

        public double FlickerMultipler { get; set; }

        [Category("Device Status")]
        public ushort TargetRotation { get; set; }

        [Browsable(false)]
        public float CurrentRotation { get; set; }

        [Category("Device Status")]
        [TypeConverter(typeof(HexTypeEditor))]
        public ushort MemoryMap { get; set; }

        [Category("Device Status")]
        public ushort TotalVerticies { get; set; }

        [Category("Device Information")]
        [TypeConverter(typeof(HexTypeEditor))]
        public override uint DeviceID
        {
            get { return 0x42babf3c; }
        }

        [Category("Device Information")]
        [TypeConverter(typeof(HexTypeEditor))]
        public override uint ManufacturerID
        {
            get { return 0x1eb37e91; }
        }

        [Category("Device Information")]
        [TypeConverter(typeof(HexTypeEditor))]
        public override ushort Version
        {
            get { return 0x0003; }
        }

        [Category("Emulation Settings")]
        public bool EnableFlickering { get; set; }

        [Browsable(false)]
        public override string FriendlyName
        {
            get { return "Suspended Particle Exciter Display"; }
        }

        [Browsable(false)]
        public override bool SelectedByDefault
        {
            get
            {
                return false;
            }
        }

        public SPED3Vertex[] Verticies
        {
            get
            {
                var verticies = new SPED3Vertex[TotalVerticies];
                for (ushort i = 0; i < TotalVerticies * 2; i += 2)
                {
                    verticies[i / 2] = new SPED3Vertex(AttachedCPU.Memory[MemoryMap + i], 
                        AttachedCPU.Memory[MemoryMap + i + 1]);
                }
                return verticies;
            }
        }

        public override int HandleInterrupt()
        {
            switch (AttachedCPU.A)
            {
                case 0:
                    AttachedCPU.B = (ushort)State;
                    AttachedCPU.C = (ushort)SPED3Error.ERROR_NONE;
                    break;
                case 1:
                    MemoryMap = AttachedCPU.X;
                    TotalVerticies = (ushort)(AttachedCPU.Y % 128);
                    if (TotalVerticies != 0)
                        State = SPED3State.STATE_RUNNING;
                    if (VerticiesChanged != null)
                        VerticiesChanged(this, null); // To notify applications when to rebuild the vertex buffer
                    break;
                case 2:
                    TargetRotation = (ushort)(AttachedCPU.X % 360);
                    if (TargetRotation != CurrentRotation)
                        State = SPED3State.STATE_TURNING;
                    break;
            }
            return 0;
        }

        public override void Reset()
        {
            TotalVerticies = MemoryMap = 0;
            State = SPED3State.STATE_NO_DATA;
        }
    }

    public enum SPED3Color
    {
        Black = 0,
        Red = 1,
        Green = 2,
        Blue = 3
    }

    public enum SPED3Intensity
    {
        Dim = 0,
        Bright = 1
    }

    public struct SPED3Vertex
    {
        public byte X;
        public byte Y;
        public byte Z;
        public SPED3Color Color;
        public SPED3Intensity Intensity;

        public SPED3Vertex(ushort word1, ushort word2)
        {
            X = (byte)(word1 & 0xFF);
            Y = (byte)((word1 >> 8) & 0xFF);
            Z = (byte)(word2 & 0xFF);
            Color = (SPED3Color)((word2 >> 8) & 3);
            Intensity = (SPED3Intensity)((word2 >> 10) & 1);
        }

        public string ToString()
        {
            return "<" + X + "," + Y + "," + Z + "> " +
                (Color == 0 ? "Green" : "Red");
        }
    }
}
