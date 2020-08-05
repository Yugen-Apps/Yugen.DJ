using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Windows.Storage;

namespace Yugen.DJ.WaveForm
{
    /// <summary>
    /// https://github.com/matixmatix/bpmdetector/blob/master/BPMDetector.cs
    /// </summary>
    public class BPMDetector
    {
        //private string filename = null;
        //private float[] leftChn;
        //private float[] rightChn;
        private double BPM;
        //private double sampleRate = 44100;
        private double trackLength = 0;
        private IStorageFile file;

        public double getBPM()
        {
            return BPM;
        }

        //public BPMDetector(string filename)
        //{
        //    this.filename = filename;
        //    Detect();
        //}

        //public BPMDetector(short[] leftChn, short[] rightChn)
        //{
        //    this.leftChn = leftChn;
        //    this.rightChn = rightChn;
        //    Detect();
        //}


        public async Task<double> Detect(IStorageFile file)
        {
            var stream = await file.OpenStreamForReadAsync();
            MyMediaFoundationReader reader = new MyMediaFoundationReader(stream);
                
            var isp = reader.ToSampleProvider();
            var buffer = new float[reader.Length / 2];
            isp.Read(buffer, 0, buffer.Length);
            
            //List<float> chan1 = new List<float>();
            //List<float> chan2 = new List<float>();

            //for (int i = 0; i < buffer.Length; i += 2)
            //{
            //    chan1.Add(buffer[i]);
            //    chan2.Add(buffer[i + 1]);
            //}

            //leftChn = chan1.ToArray();
            //rightChn = chan2.ToArray();            

            //trackLength = (float)leftChn.Length / sampleRate;

            // 0.1s window ... 0.1*44100 = 4410 samples, lets adjust this to 3600 
            //int sampleStep = 3600;
            var sampleStep = (int)(0.1 * isp.WaveFormat.SampleRate);

            // calculate energy over windows of size sampleSetep
            List<double> energies = new List<double>();
            for (int i = 0; i < buffer.Length - sampleStep - 1; i += sampleStep)
            {
                energies.Add(rangeQuadSum(buffer, i, i + sampleStep));
            }

            int beats = 0;
            double average = 0;
            double sumOfSquaresOfDifferences = 0;
            double variance = 0;
            double newC = 0;
            List<double> variances = new List<double>();

            // how many energies before and after index for local energy average
            int offset = 10;

            for (int i = offset; i <= energies.Count - offset - 1; i++)
            {
                // calculate local energy average
                double currentEnergy = energies[i];
                double qwe = rangeSum(energies.ToArray(), i - offset, i - 1) + currentEnergy + rangeSum(energies.ToArray(), i + 1, i + offset);
                qwe /= offset * 2 + 1;

                // calculate energy variance of nearby energies
                List<double> nearbyEnergies = energies.Skip(i - 5).Take(5).Concat(energies.Skip(i + 1).Take(5)).ToList<double>();
                average = nearbyEnergies.Average();
                sumOfSquaresOfDifferences = nearbyEnergies.Select(val => (val - average) * (val - average)).Sum();
                variance = (sumOfSquaresOfDifferences / nearbyEnergies.Count) / Math.Pow(10, 22);

                // experimental linear regression - constant calculated according to local energy variance
                newC = variance * 0.009 + 1.385;
                if (currentEnergy > newC * qwe)
                    beats++;
            }

            BPM = (beats / reader.TotalTime.TotalMinutes) / 2;
            return BPM;
        }

        private static double rangeQuadSum(float[] samples, int start, int stop)
        {
            double tmp = 0;
            for (int i = start; i <= stop; i++)
            {
                tmp += Math.Pow(samples[i], 2);
            }

            return tmp;
        }

        private static double rangeSum(double[] data, int start, int stop)
        {
            double tmp = 0;
            for (int i = start; i <= stop; i++)
            {
                tmp += data[i];
            }

            return tmp;
        }
    }

    public static class BpmExtensions
    {
        const long SecondsPerMinute = TimeSpan.TicksPerMinute / TimeSpan.TicksPerSecond;

        public static int ToBpm(this TimeSpan timeSpan)
        {
            var seconds = 1 / timeSpan.TotalSeconds;
            return (int)Math.Round(seconds * SecondsPerMinute);
        }

        public static TimeSpan ToInterval(this int bpm)
        {
            var bps = (double)bpm / SecondsPerMinute;
            var interval = 1 / bps;
            return TimeSpan.FromSeconds(interval);
        }
    }
}