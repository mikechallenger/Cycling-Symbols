static string text;
static int count;
static int size;
static string[,] matrix;

static void Main(string[] args)
{
    //text = Console.ReadLine();
    text = "1234567890123456789abcdefghijklmnopqrstuvwxyz";
    count = text.Length;

    // Определение размеров матрицы
    int temp = count * 2;
    for (int g = 1; true; g += 2)
    {
        if (temp <= g * g)
        {
            size = g;
            if (g == 1 || g == 3 )
                size = 5;
            break;
        }
    }

    // Заполнение пустой матрицы
    matrix = new string[size, size];
    for (int x = 0; x < size; x++)
        for (int y = 0; y < size; y++)
            matrix[x, y] = " ";

    // Координаты центра матрицы, т.е. ячейки с которой начинается алгоритм
    int xc = (size - 1) / 2;
    int yc = xc;

    //направление в которое происходит отрисовка
    int side = 0; // 0 - вниз, 1 - вправо, 2 - вверх, 3 - влево

    int offset = 1; // Максимальное смещение для стороны, на сколько клеток происходит смешение
    int i = 0; // На сколько клеток произошло смещение
    bool firstChar = true;

    // Посимвольно
    foreach (char a in text)
    {
        // Перевод из char -> string
        string b = a.ToString();

        // Если символ первый, тогда помещаем его в центр, по текучим координатам
        // меняем сторону side и производим изменение текущих координат xc, yc
        if (firstChar)
        {
            firstChar = false;
            matrix[xc, yc] = "*";//b;
            switch (side)
            {
                case 0: { yc--; break; }
                case 1: { xc++; break; }
                case 2: { yc++; break; }
                case 3: { xc--; break; }
            }
            side++;
            continue;
        }

        // Для остальных символов в зависимости от side происходит отрисовка
        // в нужную сторону
        switch (side)
        {
            case 0: { matrix[xc, yc--] = b; break; }
            case 1: { matrix[xc++, yc] = b; break; }
            case 2: { matrix[xc, yc++] = b; break; }
            case 3: { matrix[xc--, yc] = b; break; }
        }
        // Увеличение числа уже отрисованных в сторону символов
        i++;

        // Когда side 3 или 1 проиходит увеличение макс. смещения offset
        // обнуление колич-ва уже отрисованных символов
        // так как side = от 0 до 3, то значение делем по модулю 4
        if (i >= offset)
        {
            if(side == 3 || side == 1)
                offset++;
            i = 0;
            side = (side + 1) % 4;
        }
    }

    // Вывод конечного результата
    for (int x = 0; x < size; x++)
    {
        for (int y = 0; y < size; y++)
        {
            Console.Write(matrix[x, y]);
        }
        Console.WriteLine();
    }

    Console.Read();
}
