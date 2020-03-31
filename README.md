# BitcoinECDSA
Proof of concept GANN-based cracker of Bitcoin ECDSA

The engine validates the hypothesis that the probability of discovering Bitcoin's ECDSA private key, from the Public Address only, is around 1.0309258098174834118790766041465e-70 rather than 8.6361685550944446253863518628004e-78‬ that would be expected.

In one run a single instance of 0.0077 was observed which, if achieved for all bytes, could result in a probability of 2.331863934537509632978568812176e-68‬.

The engine suggests other approaches, or combination of approaches, may be more successful.

Some notes
1. The engine employs Entity Framework to capture weights and statistics.
2. NBitcoin is used for Bitcoin functions
3. Makes use of Base58Check ported to .NET standard.
