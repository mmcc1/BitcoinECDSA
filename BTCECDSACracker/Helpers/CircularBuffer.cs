using System;
using System.Collections.Generic;
using System.Text;

namespace BTCECDSACracker.Helpers
{
    public class CircleBufferEntry
    {
        public byte[] PrivateKey { get; set; }
        public double[] PrivateKeyAttempt { get; set; }
    }

    public class CircularBuffer
    {
        private int currentReadIndex;
        private int currentWriteIndex;
        private CircleBufferEntry[] cbes;

        public CircularBuffer(int size)
        {
            cbes = new CircleBufferEntry[size];
        }

        public void Create(CircleBufferEntry cbe)
        {
            currentReadIndex = currentWriteIndex;  //Sync

            cbes[currentWriteIndex++] = cbe;

            if (currentWriteIndex == cbes.Length)
                currentWriteIndex = 0; 
        }

        public CircleBufferEntry[] Read()
        {
            CircleBufferEntry[] cbuf = new CircleBufferEntry[cbes.Length];

            for (int i = 0; i < cbes.Length; i++)
            {
                if(cbes[currentReadIndex] != null)
                    cbuf[i] = cbes[currentReadIndex++];

                if (currentReadIndex == cbes.Length)
                    currentReadIndex = 0;
            }

            return cbuf;
        }

        public void Clear()
        {
            cbes = new CircleBufferEntry[cbes.Length];
        }
    }
}
