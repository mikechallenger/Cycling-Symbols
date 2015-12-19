using System;
using System.Drawing;
using System.Threading;


using System.Windows.Forms;

namespace areaOfView
{
    public partial class Form1 : Form
    {
        // Класс прямой, блок или луч явл. объектами этого класса
        class Line
        {
            public PointF pt1, pt2;
            public float A, B, C;
            public PointF ptt;
            public bool isCross;
            public bool existMinLenght = false;
            public float minLength;
            public Line(Point p1, Point p2)
            {
                pt1 = new PointF(p1.X, p1.Y);
                pt2 = new PointF(p2.X, p2.Y);
                isCross = false;
                determineCoefs();
            }
            public Line(PointF p1, PointF p2)
            {
                pt1 = p1;
                pt2 = p2;
                isCross = false;
                determineCoefs();
            }

            // Проверка, пересекаются ли отрезки
            public bool isCrossWithBlock(Line block, int length)
            {
                float a1 = A;
                float b1 = B;
                float c1 = C;
                float a2 = block.A;
                float b2 = block.B;
                float c2 = block.C;

                if ((a1 / a2) == (b1 / b2))
                {
                    return false;
                }


                float x, y;
                float t1 = a2 - a1 * b2 / b1;
                float t2 = c1 * b2 / b1 - c2;
                x = t2 / t1;
                y = (-a1 * x - c1) / b1;

                ptt = new PointF(x, y);

                float dis1 = distanceBetweenPoints(pt1, ptt);
                float dis2 = distanceBetweenPoints(pt2, ptt);
                float disC = (float)length;
                if (dis1 < disC)
                {
                    if (dis2 < disC)
                    {
                        if ((block.pt1.X <= ptt.X && ptt.X <= block.pt2.X) || (block.pt2.X <= ptt.X && ptt.X <= block.pt1.X))
                            if ((block.pt1.Y <= ptt.Y && ptt.Y <= block.pt2.Y) || (block.pt2.Y <= ptt.Y && ptt.Y <= block.pt1.Y))
                            {
                                if (!existMinLenght)
                                {
                                    minLength = dis1;
                                    return true;
                                }
                                else
                                {
                                    if (dis1 < minLength)
                                    {
                                        minLength = dis1;
                                        return true;
                                    }
                                }
                                
                            }
                    }

                }
                return false;
            }
            // Расстояние между точек
            private float distanceBetweenPoints(PointF p1, PointF p2)
            {
                float disX = Math.Abs(p1.X - p2.X);
                float disY = Math.Abs(p1.Y - p2.Y);
                return (float)Math.Sqrt(disX * disX + disY * disY);
            }
            // Определение коэффициентов для уравнения прямой
            private void determineCoefs()
            {
                A = pt1.Y - pt2.Y;
                B = pt2.X - pt1.X;
                C = pt1.X * pt2.Y - pt2.X * pt1.Y;
                if (A < 0)
                {
                    A *= -1;
                    B *= -1;
                    C *= -1;
                }
            }
        }

        // Класс для передачи значений от и до в потоки
        class FromTo
        {
            public int from;
            public int to;
        }

        public Form1()
        {
            InitializeComponent();

            // Цвета элементов
            penOfPoint = new Pen(Color.Black);
            penOfRay = new Pen(Color.Black);
            penOfBlock = new Pen(Color.Red);

            // Длина лучей
            lengthOfRay = 35; 

            // Массив блоков
            sourceOfBlocks = new Point[,]{
                {new Point(50, 30), new Point(120, 40)},
                {new Point(30, 120), new Point(40, 60)},
            };
            countOfBlocks = 2; // Счетчик блоков
            limitOfBlocks = 10; // Ограничение на количество блоков

            angleBetweenRays = 3; // Угол между лучами

            state = STATE_POV; // Начальное состояние POV - точка обзора 

            tp1 = tp2 = false; // Переменные для рисования блоков

            labelNumberOfBlocks.Text = "Blocks: " + countOfBlocks;
            createBlocks();
        }

        Pen penOfPoint;
        Pen penOfRay;
        Pen penOfBlock;
        Point curPos = new Point(50, 50);
        Point[,] sourceOfBlocks;
        int countOfBlocks;
        int countOfRays;
        Line[] rays;
        Line[] blocks;
        Graphics graph;
        int angleBetweenRays;
        int lengthOfRay;
        int limitOfBlocks;
        FromTo d1, d2, d3, d4;

        // Переменные для создания новых блоков
        Point tempPt1, tempPt2;
        bool tp1, tp2;

        // Состояния
        int state;
        const int STATE_BLOCK = 0;
        const int STATE_POV = 1;

        static object locker = new object(); // Необходимо для потоков, защелкаы

        // Переключение состояний
        private void button1_Click(object sender, EventArgs e)
        {
            if (state == STATE_BLOCK)
            {
                state = STATE_POV;
                button1.Text = "POINT";
            }
            else if (state == STATE_POV)
            {
                state = STATE_BLOCK;
                button1.Text = "BLOCKS";
            }
        }

        // Функция отрисовки
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            graph = e.Graphics;
            drawBlocks();
            drawRays();
        }

        // Создание из первоначального массива точек массив блоков
        private void createBlocks()
        {
            blocks = new Line[limitOfBlocks];
            for (int i = 0; i < countOfBlocks; i++)
            {
                blocks[i] = new Line(
                    new Point(sourceOfBlocks[i, 0].X, sourceOfBlocks[i, 0].Y),
                    new Point(sourceOfBlocks[i, 1].X, sourceOfBlocks[i, 1].Y)
                    );
            }
        }

        // Отрисовка блоков
        private void drawBlocks()
        {
            for (int i = 0; i < countOfBlocks; i++)
            {
                graph.DrawLine(penOfBlock, blocks[i].pt1, blocks[i].pt2);
            }
        }

        // Для того чтобы точка следовала за курсором мыши
        private void Form1_MouseMove(object sender, MouseEventArgs e)
        {
            if (state == STATE_POV)
            {
                curPos = new Point(e.X, e.Y);
                Invalidate();
            }
        }

        // Так как точка постоянно перемещается, необходимо пересодавать лучи
        private void createRays()
        {
            countOfRays = 360 / angleBetweenRays;
            rays = new Line[countOfRays];
            int degrees = 0;
            for (int i = 0; i < countOfRays; i++)
            {
                // Определяем координаты лучей
                degrees += angleBetweenRays;
                double angle = Math.PI * degrees / 180.0;
                PointF pt1 = new PointF((float)curPos.X, (float)curPos.Y);
                PointF pt2 = new PointF(
                    pt1.X + (float)(lengthOfRay * Math.Cos(angle)),
                    pt1.Y - (float)(lengthOfRay * Math.Sin(angle))
                    );
                Line temp = new Line(pt1, pt2);

                // Проверяем не пересекается ли луч с блоком
                for (int j = 0; j < countOfBlocks; j++)
                {
                    if (temp.isCrossWithBlock(blocks[j], lengthOfRay))
                    {
                        // Луч пересекается, значит меняем конечную точку
                        // отрезка на точку пересечения 
                        temp.pt2 = temp.ptt;
                    }

                }
                // Добавление данного луча в массив лучей
                rays[i] = temp;
            }
        }
        private void drawRays()
        {
            // Перед отрисовкой пересоздаем лучиы
            createRays();
            
            // Разбиение массива лучей на 4 потока
            int delta = (countOfRays / 4);
            d1 = new FromTo { from = 1, to = delta - 1 };
            d2 = new FromTo { from = delta, to = 2 * delta - 1 };
            d3 = new FromTo { from = 2 * delta, to = 3 * delta - 1 };
            d4 = new FromTo { from = 3 * delta, to = 4 * delta - 1 };
            Thread tr1 = new Thread(new ParameterizedThreadStart(drawPartOfRays));
            tr1.Start(d1);
            Thread tr2 = new Thread(new ParameterizedThreadStart(drawPartOfRays));
            tr2.Start(d2);
            Thread tr3 = new Thread(new ParameterizedThreadStart(drawPartOfRays));
            tr3.Start(d3);
            Thread tr4 = new Thread(new ParameterizedThreadStart(drawPartOfRays));
            tr4.Start(d4);

            // Чтобы функция не закрывалась пока не отрисуются все лучи
            while (tr1.IsAlive || tr2.IsAlive || tr3.IsAlive || tr4.IsAlive) { }
        }
        // Функция отрисовывающая часть лучей, 1 поток
        private void drawPartOfRays(object x)
        {
            FromTo a = (FromTo)x;
            for (int i = a.from; i < a.to; i++)
            {
                try
                {
                    lock (locker) // Защелка
                    {
                        // Чтобы не было одновременно несколько обращений к graph
                        graph.DrawLine(penOfRay, rays[i].pt1, rays[i].pt2);
                    }
                }
                catch (Exception e)
                {
                }
            }
        }

        // По кликам создаются новые блоки, если включен режим STATE_BLOCK
        private void Form1_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                //if (state == STATE_POV)
                //{
                //    curPos = new Point(e.X, e.Y);
                //    Invalidate();
                //}
                if (state == STATE_BLOCK)
                {
                    if (!tp1)
                    {
                        // Задаем 1-ю точку
                        tempPt1 = new Point(e.X, e.Y);
                        tp1 = true;
                    }
                    else
                    {
                        if (!tp2)
                        {
                            // Задаем 2-ю точку
                            tempPt2 = new Point(e.X, e.Y);
                            tp2 = true;
                        }
                        else
                        {
                            if (countOfBlocks < limitOfBlocks)
                            {
                                // Добавляем новый блок
                                tp1 = tp2 = false;
                                blocks[countOfBlocks] = new Line(tempPt1, tempPt2);
                                countOfBlocks++;
                                Invalidate();
                                labelNumberOfBlocks.Text = "Blocks: " + countOfBlocks;
                            }

                        }
                    }
                }
                
                
            }

        }
    }
}
