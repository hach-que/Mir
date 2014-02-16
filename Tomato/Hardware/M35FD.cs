using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.IO;

namespace Tomato.Hardware
{
    public class M35FD : Device
    {
        public M35FD()
        {
            DeviceState = M35FDStateCode.STATE_NO_MEDIA;
            LastError = M35FDErrorCode.ERROR_NONE;
        }

        public ushort[] Disk { get; set; }
        [Category("Disk Information")]
        public bool Writable { get; set; }

        [Category("Device Status")]
        public M35FDErrorCode LastError
        {
            get
            {
                return lastError;
            }
            set
            {
                lastError = value;
                if (InterruptMessage != 0)
                    AttachedCPU.FireInterrupt(InterruptMessage);
            }
        }
        private M35FDErrorCode lastError;
        [Category("Device Status")]
        public ushort InterruptMessage { get; set; }
        [Category("Device Status")]
        public M35FDStateCode DeviceState 
        {
            get
            {
                return deviceState;
            }
            set
            {
                deviceState = value;
                if (InterruptMessage != 0)
                    AttachedCPU.FireInterrupt(InterruptMessage);
            }
        }
        private M35FDStateCode deviceState;

        [Category("Device Information")]
        [TypeConverter(typeof(HexTypeEditor))]
        public override uint DeviceID
        {
            get { return 0x4fd524c5; }
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
            get { return 0x000b; }
        }

        [Browsable(false)]
        public override string FriendlyName
        {
            get { return "3.5\" Floppy Drive (M35FD)"; }
        }

        public override int HandleInterrupt()
        {
            switch (AttachedCPU.A)
            {
                case 0: // Poll device
                    AttachedCPU.B = (ushort)DeviceState;
                    AttachedCPU.C = (ushort)LastError;
                    break;
                case 1: // Set interrupt
                    InterruptMessage = AttachedCPU.X;
                    break;
                case 2: // Read sector
                    if (DeviceState == M35FDStateCode.STATE_NO_MEDIA)
                    {
                        LastError = M35FDErrorCode.ERROR_NO_MEDIA;
                        AttachedCPU.B = 0;
                        break;
                    }
                    if (isReading || isWriting)
                    {
                        LastError = M35FDErrorCode.ERROR_BUSY;
                        AttachedCPU.B = 0;
                        break;
                    }
                    targetTrack = (uint)AttachedCPU.X / wordsPerTrack;
                    seekTicks = (int)(tracksPerTick * Math.Abs(targetTrack - currentTrack));
                    fromAddress = (uint)AttachedCPU.X * wordsPerSector;
                    toAddress = AttachedCPU.Y;
                    AttachedCPU.B = 1;
                    wordsWritten = 0;
                    isReading = true;
                    LastError = M35FDErrorCode.ERROR_NONE;
                    DeviceState = M35FDStateCode.STATE_BUSY;
                    break;
                case 3: // Write sector
                    if (DeviceState == M35FDStateCode.STATE_NO_MEDIA)
                    {
                        LastError = M35FDErrorCode.ERROR_NO_MEDIA;
                        AttachedCPU.B = 0;
                        break;
                    }
                    if (isReading || isWriting)
                    {
                        LastError = M35FDErrorCode.ERROR_BUSY;
                        AttachedCPU.B = 0;
                        break;
                    }
                    if (!Writable)
                    {
                        LastError = M35FDErrorCode.ERROR_PROTECTED;
                        AttachedCPU.B = 0;
                        break;
                    }
                    targetTrack = (uint)AttachedCPU.X / wordsPerTrack;
                    seekTicks = (int)(tracksPerTick * Math.Abs(targetTrack - currentTrack));
                    toAddress = (uint)AttachedCPU.X * wordsPerSector;
                    fromAddress = AttachedCPU.Y;
                    AttachedCPU.B = 1;
                    wordsWritten = 0;
                    isWriting = true;
                    LastError = M35FDErrorCode.ERROR_NONE;
                    DeviceState = M35FDStateCode.STATE_BUSY;
                    break;
            }
            return 0;
        }

        private bool isReading = false;
        private bool isWriting = false;
        private uint fromAddress, toAddress, currentTrack, targetTrack;
        private int seekTicks, wordsWritten;
        private const int wordsPerTick = 512;
        private const int wordsPerSector = 512, wordsPerTrack = 512 * 18;
        private const float tracksPerTick = 0.144f;

        public override void Tick()
        {
            if (isReading)
            {
                // Handle seeking
                if (seekTicks != 0)
                    seekTicks--;
                else
                {
                    currentTrack = targetTrack;
                    int wordsToWrite = wordsPerTick;
                    if (wordsToWrite + wordsWritten > wordsPerSector)
                        wordsToWrite = wordsPerSector - wordsWritten;
                    if ((wordsToWrite + fromAddress > Disk.Length) ||
                        (wordsToWrite + toAddress > AttachedCPU.Memory.Length))
                    {
                        LastError = M35FDErrorCode.ERROR_BROKEN;
                        DeviceState = M35FDStateCode.STATE_READY;
                        isReading = false;
                        return;
                    }
                    Array.Copy(Disk, fromAddress, AttachedCPU.Memory, toAddress, wordsToWrite);
                    toAddress += wordsPerTick;
                    wordsWritten += wordsPerTick;
                    if (wordsWritten >= wordsPerSector)
                    {
                        isReading = false;
                        DeviceState = M35FDStateCode.STATE_READY;
                    }
                }
            }
            else if (isWriting)
            {
                // Handle seeking
                if (seekTicks != 0)
                    seekTicks--;
                else
                {
                    currentTrack = targetTrack;
                    int wordsToWrite = wordsPerTick;
                    if (wordsToWrite + wordsWritten > wordsPerSector)
                        wordsToWrite = wordsPerSector - wordsWritten;
                    if ((wordsToWrite + toAddress > Disk.Length) ||
                        (wordsToWrite + fromAddress > AttachedCPU.Memory.Length))
                    {
                        LastError = M35FDErrorCode.ERROR_BROKEN;
                        DeviceState = M35FDStateCode.STATE_READY;
                        isWriting = false;
                        return;
                    }
                    Array.Copy(AttachedCPU.Memory, fromAddress, Disk, toAddress, wordsToWrite);
                    toAddress += wordsPerTick;
                    wordsWritten += wordsPerTick;
                    if (wordsWritten >= wordsPerSector)
                    {
                        isWriting = false;
                        DeviceState = M35FDStateCode.STATE_READY;
                    }
                }
            }
        }

        public void InsertDisk(ref ushort[] disk, bool writable)
        {
            if (disk.Length != 737280)
                throw new IOException("Invalid disk size.");
            Disk = disk;
            Writable = writable;
            if (writable)
                DeviceState = M35FDStateCode.STATE_READY;
            else
                DeviceState = M35FDStateCode.STATE_READY_WP;
            currentTrack = 0;
        }

        public void Eject()
        {
            if (Disk == null)
                throw new IOException("No disk present.");
            Disk = null;
            DeviceState = M35FDStateCode.STATE_NO_MEDIA;
            if (isReading || isWriting)
            {
                isReading = isWriting = false;
                LastError = M35FDErrorCode.ERROR_EJECT;
            }
        }

        public override void Reset()
        {
            isReading = false;
            isWriting = false;
            if (DeviceState == M35FDStateCode.STATE_BUSY)
            {
                if (Writable)
                    DeviceState = M35FDStateCode.STATE_READY;
                else
                    DeviceState = M35FDStateCode.STATE_READY_WP;
            }
            LastError = M35FDErrorCode.ERROR_NONE;
            InterruptMessage = 0;
        }
    }

    public enum M35FDStateCode
    {
        STATE_NO_MEDIA = 0,
        STATE_READY = 1,
        STATE_READY_WP = 2,
        STATE_BUSY = 3
    }

    public enum M35FDErrorCode
    {
        ERROR_NONE = 0,
        ERROR_BUSY = 1,
        ERROR_NO_MEDIA = 2,
        ERROR_PROTECTED = 3,
        ERROR_EJECT = 4,
        ERROR_BAD_SECTOR = 5,
        ERROR_BROKEN = 6
    }
}
