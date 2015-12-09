# Cycling-Symbols

using System;


namespace Cycling_Symbols
{
    class Program
    {
        static string text;
        static int count;
        static int size;
        static string[,] matrix;

        static void Main(string[] args)
        {
            Enter();
            Computing();
            Print();
            Console.Read();
        }

        static void Enter()
        {
            size = 9;
            text = Console.ReadLine();
            count = text.Length;
        }

        static void Computing()
        {



            matrix = new string[size, size];
            for (int x = 0; x < size; x++)
                for (int y = 0; y < size; y++)
                    matrix[x, y] = " ";



            int side = 1;
            int xc = 4;
            int yc = xc;
            int offset = 1;
            int i = 0;

            foreach (char a in text)
            {
                string b = a.ToString();
                if (i >= offset)
                {
                    offset++;
                    i = 0;
                    side = (side + 1) % 4;
                }
                switch (side)
                {
                    case 0:
                        {
                            matrix[xc, yc] = b;
                            yc--;
                            break;
                        }
                    case 1:
                        {
                            matrix[xc, yc] = b;
                            xc++;
                            break; 
                        }
                    case 2:
                        {
                            matrix[xc, yc] = b;
                            yc++;
                            break;
                        }
                    case 3:
                        {
                            matrix[xc, yc] = b;
                            xc--;
                            break;
                        }
                }
                i++;

            }

        }
        static void Print()
        {
            for(int i = 0; i< size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    Console.Write(matrix[j, i]);
                }
                Console.WriteLine();
            }
                
        }
    }
}
