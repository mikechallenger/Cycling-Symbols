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
            text = Console.ReadLine();
            count = text.Length;
        }

        static void Computing()
        {
            int temp = count * 2;
            for(int g = 1; true; g+=2)
            {
                if (temp <= g * g)
                {
                    size = g;
                    if (g == 1 || g == 3)
                        size = 5;
                    break;
                }
            }

            matrix = new string[size, size];
            for (int x = 0; x < size; x++)
                for (int y = 0; y < size; y++)
                    matrix[x, y] = " ";

            int side = 1;
            
            int xc = (size - 1) / 2;
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
