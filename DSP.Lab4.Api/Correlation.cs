using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;

namespace DSP.Lab4.Api
{
    public static class Сorrelation
    {
        public static Complex[] CrossCorrelation(double[] signal1, double[] signal2)
        {
            int length = signal1.Length + signal2.Length - 1;

            Complex[] corr = new Complex[length];

            int index1 = 0;
            int index2 = signal2.Length - 1;

            for (int i = 0; i < length; i++)
            {
                Complex sum = 0;

                int start = (i < signal1.Length) 
                    ? index1 
                    : index2;
                int end = (i < signal2.Length) 
                    ? signal1.Length
                    : signal2.Length;

                for (int j = start; j < end; j++)
                {
                    sum += signal1[index1] * signal2[index2];
                }

                if (i < signal1.Length - 1)
                {
                    index1++;
                    index2--;
                }
                else
                {
                    index1--;
                    index2++;
                }

                corr[i] = sum;
            }

            double max = corr.Max(c => Math.Abs(c.Real));

            for (int i = 0; i < corr.Length; i++)
            {
                corr[i] /= max;
            }

            return corr;
        }

        public static Complex[] AutoCorrelation(double[] signal)
        {
            int length = 2 * signal.Length - 1;

            Complex[] corr = new Complex[length];

            int index1 = 0;
            int index2 = signal.Length - 1;

            for (int i = 0; i < length; i++)
            {
                Complex sum = 0;

                int start = (i < signal.Length)
                    ? index1 
                    : index2;
                int end = signal.Length;

                for (int j = start; j < end; j++)
                {
                    sum += signal[index1] * signal[index2];
                }

                if (i < signal.Length - 1)
                {
                    index1++;
                    index2--;
                }
                else
                {
                    index1--;
                    index2++;
                }

                corr[i] = sum;
            }

            double max = corr.Max(c => Math.Abs(c.Real));

            for (int i = 0; i < corr.Length; i++)
            {
                corr[i] /= max;
            }

            return corr;
        }

        public static Complex[] FastCrossCorrelation(double[] signal1, double[] signal2)
        {
            int L = signal1.Length;
            Complex[] cps1 = new Complex[L];
            Complex[] cps2 = new Complex[L];

            for (int i = 0; i < signal1.Length; i++)
            {
                cps1[i] = signal1[i];
                cps2[i] = signal2[i];
            }

            Complex[] bpf1 = Butterfly.DecimationInTime(cps1, true);
            Complex[] bpf2 = Butterfly.DecimationInTime(cps2, true);

            Complex[] mult = new Complex[L];
            for (int i = 0; i < L; i++)
            {
                bpf1[i] /= bpf1.Length;
                bpf2[i] /= bpf2.Length;
                mult[i] = bpf1[i] * Complex.Conjugate(bpf2[i]);
            }

            Complex[] corr = Butterfly.DecimationInTime(mult, false);

            double max = corr.Max(c => Math.Abs(c.Real));

            for (int i = 0; i < corr.Length; i++)
            {
                corr[i] /= max;
            }

            return corr;
        }

        public static Complex[] FastAutoCorrelation(double[] signal)
        {
            int L = signal.Length;
            Complex[] cps = new Complex[L];

            for (int i = 0; i < signal.Length; i++)
            {
                cps[i] = signal[i];
            }

            Complex[] bpf = Butterfly.DecimationInTime(cps, true);

            Complex[] mult = new Complex[L];
            for (int i = 0; i < L; i++)
            {
                bpf[i] /= bpf.Length;
                mult[i] = bpf[i] * Complex.Conjugate(bpf[i]);
            }

            Complex[] corr = Butterfly.DecimationInTime(mult, false);

            double max = corr.Max(c => Math.Abs(c.Real));

            for (int i = 0; i < corr.Length; i++)
            {
                corr[i] /= max;
            }

            return corr;
        }

        public static Complex[] FastMinimazeCrossCorrelation(double[] signal1, double[] signal2)
        {
            int L = signal1.Length;
            Complex[] cps = new Complex[L];

            for (int i = 0; i < signal1.Length; i++)
            {
                cps[i] = new Complex(signal1[i], signal2[i]);
            }

            Complex[] bpf = Butterfly.DecimationInTime(cps, true);

            Complex[] mult = new Complex[L];
            for (int i = 0; i < L; i++)
            {
                bpf[i] /= bpf.Length;
                mult[i] = bpf[i] * Complex.Conjugate(bpf[i])
                    - bpf[L - i - 1] * Complex.Conjugate(bpf[L - i - 1])
                    - 2 * Complex.ImaginaryOne * (bpf[i] * bpf[L - i - 1]).Imaginary;
            }

            Complex[] corr = Butterfly.DecimationInTime(mult, false);

            for (int i = 0; i < corr.Length; i++)
            {
                corr[i] *= Complex.ImaginaryOne / 4;
            }

            double max = corr.Max(c => Math.Abs(c.Real));

            for (int i = 0; i < corr.Length; i++)
            {
                corr[i] /= max;
            }

            return corr;
        }

        public static Complex[] FastMinimazeAutoCorrelation(double[] signal)
        {
            int L = signal.Length;
            Complex[] cps = new Complex[L];

            for (int i = 0; i < signal.Length; i++)
            {
                cps[i] = new Complex(signal[i], signal[i]);
            }

            Complex[] bpf = Butterfly.DecimationInTime(cps, true);

            Complex[] mult = new Complex[L];
            for (int i = 0; i < L; i++)
            {
                bpf[i] /= bpf.Length;
                mult[i] = bpf[i] * Complex.Conjugate(bpf[i])
                    - bpf[L - i - 1] * Complex.Conjugate(bpf[L - i - 1])
                    - 2 * Complex.ImaginaryOne * (bpf[i] * bpf[L - i - 1]).Imaginary;
            }

            Complex[] corr = Butterfly.DecimationInTime(mult, false);

            for (int i = 0; i < corr.Length; i++)
            {
                corr[i] *= Complex.ImaginaryOne / 4;
            }

            double max = corr.Max(c => Math.Abs(c.Real));

            for (int i = 0; i < corr.Length; i++)
            {
                corr[i] /= max;
            }

            return corr;
        }
    }
}
