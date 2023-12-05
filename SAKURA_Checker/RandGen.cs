using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SAKURA
{
    public class RandGen
    {
        private int seedKey;
        private int seedPlaintext;
        private Random randKey;
        private Random randPlaintext;

        private Random timerandPlaintext;//2018.10.20

        public RandGen(int seedKey, int seedPlaintext)
        {
            this.seedKey = seedKey;
            this.seedPlaintext = seedPlaintext;
            randKey = new Random(seedKey);
            randPlaintext = new Random(seedPlaintext);
            timerandPlaintext = new Random();//2018.10.20
        }

        public void restartKeyPrng()
        {
            randKey = new Random(seedKey);
        }

        public void restartPlaintextPrng()
        {
            randPlaintext = new Random(seedPlaintext);
            timerandPlaintext = new Random();
        }

        public byte[] generatePlaintext()
        {
            byte[] plaintext = new byte[16];
            for (int i = 0; i < 16; i++)
            {
                //plaintext[i] = (byte)randPlaintext.Next(256);
                //Console.Out.WriteLine("randPlaintext {0} " + randPlaintext.Next(256).ToString());
                //2018.10.20
                plaintext[i] = (byte)timerandPlaintext.Next(256);
                //Console.Out.WriteLine("timerandPlaintext {0} " + plaintext[i].ToString());
            }
            return plaintext;
        }

        public byte[] generateKey(bool generate56bit)
        {
            byte[] key = new byte[16];
            for (int i = 0; i < 16; i++)
            {
                key[i] = (byte)randKey.Next(256);
                if (generate56bit && i <= 8)
                {
                    key[i] = (byte)i;
                }
            }
            return key;
        }
    }
}
