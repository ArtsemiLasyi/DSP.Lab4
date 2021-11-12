using DSP.Lab4.Api;
using System;
using System.Diagnostics;
using System.Numerics;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace DSP.Lab4.Presentation
{
    public partial class Form1 : Form
    {
        Chart[] targetCharts;
        int N = 1024;

        public Form1()
        {
            InitializeComponent();

            targetCharts = new Chart[2];
            targetCharts[0] = chart1;
            targetCharts[1] = chart2;
        }

        private void ClearCharts()
        {
            for (int i = 0; i < 2; i++)
            {
                foreach (var j in targetCharts[i].Series)
                {
                    j.Points.Clear();
                }
            }
        }

        private void Calculate(СorrelationType correlationType)
        {
            HarmonicSignal firstHarmonicSignal = new HarmonicSignal(100, 1, 0, N);
            HarmonicSignal secondHarmonicSignal = new HarmonicSignal(70, 3, 30, N);

            Complex[] cs = null;
            Complex[] fcs = null;
            Complex[] fmcs = null;

            Stopwatch firstStopWatch = new Stopwatch();
            Stopwatch secondStopWatch = new Stopwatch();
            Stopwatch thirdStopWatch = new Stopwatch();

            long time = 1;
            long fastTime = 1;
            long minimazeFastTime = 1;

            switch (correlationType)
            {
                case СorrelationType.Cross:
                    firstStopWatch.Start();
                    cs = Сorrelation.CrossCorrelation(firstHarmonicSignal.signVal, secondHarmonicSignal.signVal);
                    firstStopWatch.Stop();
                    time = firstStopWatch.ElapsedTicks;

                    secondStopWatch.Start();
                    fcs = Сorrelation.FastCrossCorrelation(firstHarmonicSignal.signVal, secondHarmonicSignal.signVal);
                    secondStopWatch.Stop();
                    fastTime = secondStopWatch.ElapsedTicks;

                    thirdStopWatch.Start();
                    fmcs = Сorrelation.FastMinimazeCrossCorrelation(firstHarmonicSignal.signVal, secondHarmonicSignal.signVal);
                    thirdStopWatch.Stop();
                    minimazeFastTime = thirdStopWatch.ElapsedTicks;
                    break;
                case СorrelationType.Auto:
                    firstStopWatch.Start();
                    cs = Сorrelation.AutoCorrelation(firstHarmonicSignal.signVal);
                    firstStopWatch.Stop();
                    time = firstStopWatch.ElapsedTicks;

                    secondStopWatch.Start();
                    fcs = Сorrelation.FastAutoCorrelation(firstHarmonicSignal.signVal);
                    secondStopWatch.Stop();
                    fastTime = secondStopWatch.ElapsedTicks;

                    thirdStopWatch.Start();
                    fmcs = Сorrelation.FastMinimazeAutoCorrelation(firstHarmonicSignal.signVal);
                    thirdStopWatch.Stop();
                    minimazeFastTime = secondStopWatch.ElapsedTicks;
                    break;
            }

            ClearCharts();

            if (correlationType == СorrelationType.Cross)
            {
                for (int i = 0; i < N; i++)
                {
                    targetCharts[0].Series[0].Points.AddXY(
                        2 * Math.PI * i / N, 
                        firstHarmonicSignal.signVal[i]
                    );
                    targetCharts[0].Series[1].Points.AddXY(
                        2 * Math.PI * i / N,
                        secondHarmonicSignal.signVal[i]
                    );
                }
            }
            else if (correlationType == СorrelationType.Auto)
            {
                for (int i = 0; i < N; i++)
                {
                    targetCharts[0].Series[0].Points.AddXY(
                        2 * Math.PI * i / N,
                        firstHarmonicSignal.signVal[i]
                    );
                }
            }

            for (int i = 0; i < cs.Length; i++)
            {
                targetCharts[1].Series[0].Points.AddXY(2 * Math.PI * i / N, cs[i].Real);
            }

            for (int i = 0; i < fcs.Length; i++)
            {
                targetCharts[1].Series[1].Points.AddXY(2 * Math.PI * i / N, fcs[i].Real);
            }

            for (int i = 0; i < fmcs.Length; i++)
            {
                targetCharts[1].Series[2].Points.AddXY(2 * Math.PI * i / N, fmcs[i].Real);
            }

            string percents = string.Format("{0:00}%", fastTime * 100 / time);

            label1.Text = 
                $"Прямая корреляция заняла:\n {time} тиков\n\n" +
                $"Быстрая корреляция заняла:\n {fastTime} тиков\n\n" +
                $"Разница в процентах:\n {percents}";
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (comboBox1.SelectedIndex)
            {
                case 0:
                    Calculate(СorrelationType.Cross);
                    break;
                case 1:
                    Calculate(СorrelationType.Auto);
                    break;
                default:
                    return;
            }
        }
    }
}
