namespace XOR
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine(Encoding.ASCII.GetString(encode(Console.ReadLine(), Console.ReadLine())) );
            Console.Read();
        }

        //---
        static long rnd;
        static void XORSet(long a)
        {
            rnd = a;
        }
        static long XORRndGet()
        {
            rnd ^= (rnd << 21);
            rnd ^= (rnd >> 35);
            rnd ^= (rnd << 4);
            return rnd;
        }
        static void XORRnd()
        {
            XORSet(11111111);
            while (true)
            {
                Console.WriteLine(XORRndGet());
                Console.ReadKey();
            }
        }
        //---

        static byte[] encode(string pText, string pKey)
        {
            byte[] text = Encoding.ASCII.GetBytes(pText);
            byte[] key = Encoding.ASCII.GetBytes(pKey);
            byte[] res = new byte[text.Length];

            for (int i = 0; i < res.Length; i++)
                res[i] = (byte)(text[i] ^ key[i % key.Length]);

            return res;
        }
        static byte[] decode(byte[] pText, string pKey)
        {
            return new byte[11];
        }
    }
}
