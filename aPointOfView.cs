using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Threading;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Windows.Forms;

namespace areaOfView
{
    public partial class Form1 : Form
    {
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
            private float distanceBetweenPoints(PointF p1, PointF p2)
            {
                float disX = Math.Abs(p1.X - p2.X);
                float disY = Math.Abs(p1.Y - p2.Y);
                return (float)Math.Sqrt(disX * disX + disY * disY);
            }
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
        class FromTo
        {
            public int from;
            public int to;
        }

        public Form1()
        {
            InitializeComponent();
            penOfPoint = new Pen(Color.Black);
            penOfRay = new Pen(Color.Red);
            penOfBlock = new Pen(Color.Green);
            pointSize = new Size(10, 10);
            rectOfPoint = new Rectangle(curPos, pointSize);
            lengthOfRay = 50;

            sourceOfBlocks = new Point[,]{
                {new Point(30, 30), new Point(80, 40)},
                {new Point(30, 120), new Point(40, 60)},
            };a
            countOfBlocks = 2;
            angleBetweenRays = 10;

            state = STATE_POV;
            limitOfBlocks = 10;
            tp1 = tp2 = false;

            createBlocks();
        }

        Pen penOfPoint;
        Pen penOfRay;
        Pen penOfBlock;
        Rectangle rectOfPoint;
        Point curPos = new Point(50, 50);
        Size pointSize;
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
        bool isDown = false;

        Point tempPt1, tempPt2;
        bool tp1, tp2;

        int state;
        const int STATE_BLOCK = 0;
        const int STATE_POV = 1;

        static object locker = new object();

        private void button1_Click(object sender, EventArgs e)
        {
            if (state == STATE_BLOCK)
            {
                state = STATE_POV;
                button1.Text = "POW";
                label2.Text = STATE_POV.ToString();
            }
            else if (state == STATE_POV)
            {
                state = STATE_BLOCK;
                button1.Text = "blocks";
                label2.Text = STATE_BLOCK.ToString();
            }



        }

        private void Form1_MouseMove(object sender, MouseEventArgs e)
        {


        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            //e.Graphics.DrawEllipse(penOfPoint, rectOfPoint);
            Bitmap bm = new Bitmap(Form1.ActiveForm.Width, Form1.ActiveForm.Height, e.Graphics);
            graph = e.Graphics;
            drawBlocks();
            drawRays();

        }


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
        private void addBlock()
        {

        }
        private void drawBlocks()
        {
            for (int i = 0; i < countOfBlocks; i++)
            {
                graph.DrawLine(penOfBlock, blocks[i].pt1, blocks[i].pt2);
            }
        }

        private void Form1_MouseUp(object sender, MouseEventArgs e)
        {

            isDown = false;
        }

        private void Form1_MouseDown(object sender, MouseEventArgs e)
        {

            if (state == STATE_BLOCK)
            {
                if (!tp1)
                {
                    tempPt1 = new Point(e.X, e.Y);
                    label1.Text = "1";
                    tp1 = true;
                }
                else
                {
                    if (!tp2)
                    {
                        tempPt2 = new Point(e.X, e.Y);
                        label1.Text = "2";
                        tp2 = true;
                    }
                    else
                    {
                        if (countOfBlocks < limitOfBlocks)
                        {
                            tp1 = tp2 = false;
                            blocks[countOfBlocks] = new Line(tempPt1, tempPt2);
                            countOfBlocks++;
                            Invalidate();
                            label1.Text = "0";
                            labelNumberOfBlocks.Text = "Blocks: " + countOfBlocks;
                        }

                    }
                }
            }

            isDown = true;

        }

        private void createRays()
        {
            countOfRays = 360 / angleBetweenRays;
            rays = new Line[countOfRays];
            int degrees = 0;
            for (int i = 0; i < countOfRays; i++)
            {
                degrees += angleBetweenRays;
                double angle = Math.PI * degrees / 180.0;
                PointF pt1 = new PointF((float)curPos.X, (float)curPos.Y);
                PointF pt2 = new PointF(
                    pt1.X + (float)(lengthOfRay * Math.Cos(angle)),
                    pt1.Y - (float)(lengthOfRay * Math.Sin(angle))
                    );
                Line temp = new Line(pt1, pt2);
                for (int j = 0; j < countOfBlocks; j++)
                {
                    if (temp.isCrossWithBlock(blocks[j], lengthOfRay))
                    {
                        temp.pt2 = temp.ptt;
                    }

                }
                rays[i] = temp;



            }
        }
        private void drawRays()
        {
            createRays();

            //for (int i = 0; i < countOfRays; i++)
            //{
            //    graph.DrawLine(penOfRay, rays[i].pt1, rays[i].pt2);
            //}

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

            while (tr1.IsAlive || tr2.IsAlive || tr3.IsAlive || tr4.IsAlive) { }

            /*for (int i = 0; i < countOfRays; i++)
            {
                gr.DrawLine(penOfRay, rays[i].pt1, rays[i].pt2);
            }*/
        }
        private void drawRay(object x)
        {
            int t = ((FromTo)x).from;
            try
            {
                lock (locker)
                {
                    graph.DrawLine(penOfRay, rays[t].pt1, rays[t].pt2);
                }
            }
            catch (Exception e)
            {
            }

        }
        private void drawPartOfRays(object x)
        {
            FromTo a = (FromTo)x;
            for (int i = a.from; i < a.to; i++)
            {
                try
                {
                    lock (locker)
                    {
                        graph.DrawLine(penOfRay, rays[i].pt1, rays[i].pt2);
                    }
                }
                catch (Exception e)
                {
                }
            }
        }


        private void Form1_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (state == STATE_POV)
                {
                    curPos = new Point(e.X, e.Y);
                    Invalidate();
                }

            }

        }
    }
}
