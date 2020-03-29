using Base58Check;
using System;
using System.Collections.Generic;
using System.Text;

namespace BTCLib
{
    public static class BTCPrep
    {
        public static byte[] PrepareAddress(string publicAddress)
        {
            //For now, only work with addresses of prefix '1'.
            if (BTCInfo.DetermineAddressType(publicAddress) != AddressType.PubKeyHashP2PKH)
                return null;

            return PreparePubKeyHashP2PKH(publicAddress);
        }

        public static byte[] PreparePubKeyHashP2PKH(string publicAddress)
        {
            byte[] result = Base58CheckEncoding.Decode(publicAddress);
            byte[] removedPrefix = new byte[result.Length - 1];
            Array.Copy(result, 1, removedPrefix, 0, removedPrefix.Length);
            return removedPrefix;
        }
    }
}
