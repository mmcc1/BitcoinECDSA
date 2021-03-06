﻿/*
 * PRNGSHA512
 * 
 * A SHA-512-based random number generator.
 * 
 * -Loops through the entire Double range in values of Double.Epsilon.
 * -Limited to 64 bytes per call.
 */
using System;
using System.Security.Cryptography;

namespace MarxMLL2
{
    public class PRNGSHA512
    {
        private double seed;

        public PRNGSHA512(double seed)
        {
            this.seed = seed;
        }

        public byte[] GetNextBytes()
        {
            if (seed + double.Epsilon >= int.MaxValue)
                seed = 0;

            seed += double.Epsilon;
            return SHA512_Hash();
        }

        private byte[] SHA512_Hash()
        {
            using (var hash = SHA512.Create())
            {
                return hash.ComputeHash(BitConverter.GetBytes(seed));
            }
        }
    }
}
