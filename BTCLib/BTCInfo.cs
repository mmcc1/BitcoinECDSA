using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace BTCLib
{
    public enum AddressType
    {
        PubKeyHashP2PKH,
        ScriptHashP2SH,
        Bech32,
        Unknown
    }

    public static class BTCInfo
    {
        public static AddressType DetermineAddressType(string publicAddress)
        {
            if (publicAddress.StartsWith("1"))
                return AddressType.PubKeyHashP2PKH;
            else if (publicAddress.StartsWith("3"))
                return AddressType.ScriptHashP2SH;
            else if (publicAddress.StartsWith("bc1"))
                return AddressType.Bech32;
            else
                return AddressType.Unknown;
        }

        public static bool ComparePublicAddresses(string pubAdd1, string pubAdd2)
        {
            return pubAdd1 == pubAdd2 ? true : false;
        }
    }
}
