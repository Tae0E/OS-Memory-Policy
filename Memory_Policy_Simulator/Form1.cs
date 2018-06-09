using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace Memory_Policy_Simulator
{
    public partial class Form1 : Form
    {
        Graphics g;
        PictureBox pbPlaceHolder;
        Bitmap bResultImage;

        public Form1()
        {
            InitializeComponent();
            this.pbPlaceHolder = new PictureBox();
            this.bResultImage = new Bitmap(2048, 2048);
            this.pbPlaceHolder.Size = new Size(2048, 2048);
            g = Graphics.FromImage(this.bResultImage);
            pbPlaceHolder.Image = this.bResultImage;
            this.pImage.Controls.Add(this.pbPlaceHolder);
        }

        private void DrawBase(LRU core, int windowSize, int dataLength, int[] numb)
        {
            /* parse window */
            var psudoQueue = new List<char>();

            g.Clear(Color.Black);

            for ( int i = 0; i < dataLength; i++ ) // length
            {
                int psudoCursor = core.pageHistory[i].loc;
                char data = core.pageHistory[i].data;
                Page.STATUS status = core.pageHistory[i].status;

                switch ( status )
                {
                    case Page.STATUS.PAGEFAULT:
                        psudoQueue.Add(data);
                        break;
                    case Page.STATUS.MIGRATION:
                        psudoQueue.RemoveAt(numb[i]);
                        psudoQueue.Insert(numb[i], data);
                        break;
                }

                for ( int j = 0; j <= windowSize; j++) // height - STEP
                {
                    if (j == 0)
                    {
                        DrawGridText(i, j, data);
                    }
                    else
                    {
                        DrawGrid(i, j);
                    }
                }

                DrawGridHighlight(i, psudoCursor, status);
                int depth = 1;

                foreach ( char t in psudoQueue )
                {
                    DrawGridText(i, depth++, t);
                }
            }
        }
        private void DrawBase(LFU core, int windowSize, int dataLength, int[] numb)
        {
            /* parse window */
            var psudoQueue = new List<char>();

            g.Clear(Color.Black);

            for (int i = 0; i < dataLength; i++) // length
            {
                int psudoCursor = core.pageHistory[i].loc;
                char data = core.pageHistory[i].data;
                Page.STATUS status = core.pageHistory[i].status;

                switch (status)
                {
                    case Page.STATUS.PAGEFAULT:
                        psudoQueue.Add(data);
                        break;
                    case Page.STATUS.MIGRATION:
                        psudoQueue.RemoveAt(numb[i]);
                        psudoQueue.Insert(numb[i], data);
                        break;
                }

                for (int j = 0; j <= windowSize; j++) // height - STEP
                {
                    if (j == 0)
                    {
                        DrawGridText(i, j, data);
                    }
                    else
                    {
                        DrawGrid(i, j);
                    }
                }

                DrawGridHighlight(i, psudoCursor, status);
                int depth = 1;

                foreach (char t in psudoQueue)
                {
                    DrawGridText(i, depth++, t);
                }
            }
        }
        private void DrawBase(MFU core, int windowSize, int dataLength, int[] numb)
        {
            /* parse window */
            var psudoQueue = new List<char>();

            g.Clear(Color.Black);

            for (int i = 0; i < dataLength; i++) // length
            {
                int psudoCursor = core.pageHistory[i].loc;
                char data = core.pageHistory[i].data;
                Page.STATUS status = core.pageHistory[i].status;

                switch (status)
                {
                    case Page.STATUS.PAGEFAULT:
                        psudoQueue.Add(data);
                        break;
                    case Page.STATUS.MIGRATION:
                        psudoQueue.RemoveAt(numb[i]);
                        psudoQueue.Insert(numb[i], data);
                        break;
                }

                for (int j = 0; j <= windowSize; j++) // height - STEP
                {
                    if (j == 0)
                    {
                        DrawGridText(i, j, data);
                    }
                    else
                    {
                        DrawGrid(i, j);
                    }
                }

                DrawGridHighlight(i, psudoCursor, status);
                int depth = 1;

                foreach (char t in psudoQueue)
                {
                    DrawGridText(i, depth++, t);
                }
            }
        }

        private void DrawGrid(int x, int y)
        {
            int gridSize = 30;
            int gridSpace = 5;
            int gridBaseX = x * gridSize;
            int gridBaseY = y * gridSize;

            g.DrawRectangle(new Pen(Color.White), new Rectangle(
                gridBaseX + (x * gridSpace),
                gridBaseY,
                gridSize,
                gridSize
                ));
        }

        private void DrawGridHighlight(int x, int y, Page.STATUS status)
        {
            int gridSize = 30;
            int gridSpace = 5;
            int gridBaseX = x * gridSize;
            int gridBaseY = y * gridSize;

            SolidBrush highlighter = new SolidBrush(Color.LimeGreen);

                switch (status)
                {
                    case Page.STATUS.HIT:
                        break;
                    case Page.STATUS.MIGRATION:
                        highlighter.Color = Color.Purple;
                        break;
                    case Page.STATUS.PAGEFAULT:
                        highlighter.Color = Color.Red;
                        break;
                }
           
            g.FillRectangle(highlighter, new Rectangle(
                gridBaseX + (x * gridSpace),
                gridBaseY,
                gridSize,
                gridSize
                ));
        }

        private void DrawGridText(int x, int y, char value)
        {
            int gridSize = 30;
            int gridSpace = 5;
            int gridBaseX = x * gridSize;
            int gridBaseY = y * gridSize;

            g.DrawString(
                value.ToString(), 
                new Font(FontFamily.GenericMonospace, 8), 
                new SolidBrush(Color.White), 
                new PointF(
                    gridBaseX + (x * gridSpace) + gridSize / 3,
                    gridBaseY + gridSize / 4));
        }

        private void btnOperate_Click(object sender, EventArgs e)
        {
            this.tbConsole.Clear();

            if (this.tbQueryString.Text != "" || this.tbWindowSize.Text != "")
            {
                string data = this.tbQueryString.Text;
                int windowSize = int.Parse(this.tbWindowSize.Text);

                //카운트 배열 선언
                int[] count = new int[windowSize];
                for (int n = 0; n < windowSize; n++)
                    count[n] = 0;

                int[] fltcnt = new int[windowSize];
                for (int n = 0; n < windowSize; n++)
                    fltcnt[n] = 0;

                int cnt = 0;
                //마이그레이션 카운트
                int[] numb = new int[data.Length];
                for (int n = 0; n < data.Length; n++)
                    numb[n] = 0;
                int nu = 0;

                /* initalize */
                var window = new LRU(windowSize);
                var window1 = new LFU(windowSize);
                var window2 = new MFU(windowSize);
                if (comboBox1.Text == "LRU")
                {
                    window = new LRU(windowSize);
                }
                else if(comboBox1.Text == "LFU")
                {
                    window1 = new LFU(windowSize);
                }
                else if (comboBox1.Text == "MFU")
                {
                    window2 = new MFU(windowSize);
                }
                    



                foreach ( char element in data )
                {


                    if (comboBox1.Text == "LRU")
                    {
                        var status = window.Operate(element, ref count, ref cnt, ref numb, ref nu);
                        this.tbConsole.Text += "DATA " + element + " is " +
                        ((status == Page.STATUS.PAGEFAULT) ? "Page Fault" : status == Page.STATUS.MIGRATION ? "Migrated" : "Hit")
                        + "\r\n";

                    }
                    else if (comboBox1.Text == "LFU")
                    {
                        var status = window1.Operate(element, ref count, ref cnt, ref numb, ref nu, ref fltcnt);
                        this.tbConsole.Text += "DATA " + element + " is " +
                        ((status == Page.STATUS.PAGEFAULT) ? "Page Fault" : status == Page.STATUS.MIGRATION ? "Migrated" : "Hit")
                        + "\r\n";
                    }
                    else if (comboBox1.Text == "MFU")
                    {
                        var status = window2.Operate(element, ref count, ref cnt, ref numb, ref nu, ref fltcnt);
                        this.tbConsole.Text += "DATA " + element + " is " +
                        ((status == Page.STATUS.PAGEFAULT) ? "Page Fault" : status == Page.STATUS.MIGRATION ? "Migrated" : "Hit")
                        + "\r\n";
                    }
                    
                }
                if (comboBox1.Text == "LRU")
                    DrawBase(window, windowSize, data.Length, numb);
                else if (comboBox1.Text == "LFU")
                    DrawBase(window1, windowSize, data.Length, numb);
                else if (comboBox1.Text == "MFU")
                    DrawBase(window2, windowSize, data.Length, numb);
                this.pbPlaceHolder.Refresh();

                /* 차트 생성 */
                chart1.Series.Clear();
                Series resultChartContent = chart1.Series.Add("Statics");
                resultChartContent.ChartType = SeriesChartType.Pie;
                resultChartContent.IsVisibleInLegend = true;
                if (comboBox1.Text == "LRU")
                {
                    resultChartContent.Points.AddXY("Hit", window.hit);
                    resultChartContent.Points.AddXY("Page Fault", window.fault - window.migration);
                    resultChartContent.Points.AddXY("Migrated", window.migration);
                }
                else if (comboBox1.Text == "LFU")
                {
                    resultChartContent.Points.AddXY("Hit", window1.hit);
                    resultChartContent.Points.AddXY("Page Fault", window1.fault - window1.migration);
                    resultChartContent.Points.AddXY("Migrated", window1.migration);
                }
                else if (comboBox1.Text == "MFU")
                {
                    resultChartContent.Points.AddXY("Hit", window2.hit);
                    resultChartContent.Points.AddXY("Page Fault", window2.fault - window2.migration);
                    resultChartContent.Points.AddXY("Migrated", window2.migration);
                }
                resultChartContent.Points[0].IsValueShownAsLabel = true;
                resultChartContent.Points[1].IsValueShownAsLabel = true;
                resultChartContent.Points[2].IsValueShownAsLabel = true;
                if (comboBox1.Text == "LRU")
                    this.lbPageFaultRatio.Text = Math.Round(((float)window.fault / (window.fault + window.hit)), 2) * 100 + "%";
                else if (comboBox1.Text == "LFU")
                    this.lbPageFaultRatio.Text = Math.Round(((float)window1.fault / (window1.fault + window1.hit)), 2) * 100 + "%";
                else if (comboBox1.Text == "MFU")
                    this.lbPageFaultRatio.Text = Math.Round(((float)window2.fault / (window2.fault + window2.hit)), 2) * 100 + "%";
            }
            else
            {
            }

        }

        private void pbPlaceHolder_Paint(object sender, PaintEventArgs e)
        {
        }

        private void chart1_Click(object sender, EventArgs e)
        {

        }

        private void tbWindowSize_KeyDown(object sender, KeyEventArgs e)
        {

        }

        private void tbWindowSize_KeyPress(object sender, KeyPressEventArgs e)
        {
                if (!(Char.IsDigit(e.KeyChar)) && e.KeyChar != 8)
                {
                    e.Handled = true;
                }
        }

        private void btnRand_Click(object sender, EventArgs e)
        {
            Random rd = new Random();

            int count = rd.Next(5, 50);
            StringBuilder sb = new StringBuilder();


            for ( int i = 0; i < count; i++ )
            {
                sb.Append((char)rd.Next(65, 90));
            }

            this.tbQueryString.Text = sb.ToString();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            bResultImage.Save("./result.jpg");
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}
