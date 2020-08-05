using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Windows.Storage;
using Yugen.DJ.Audio.WaveForm;

namespace Yugen.DJ.Audio.BPM
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
        //private double trackLength = 0;
        //private IStorageFile file;

        public double GetBPM() => BPM;

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
            var reader = new MyMediaFoundationReader(stream);

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
            var energies = new List<double>();
            for (var i = 0; i < buffer.Length - sampleStep - 1; i += sampleStep)
            {
                energies.Add(RangeQuadSum(buffer, i, i + sampleStep));
            }

            var beats = 0;
            double average = 0;
            double sumOfSquaresOfDifferences = 0;
            double variance = 0;
            double newC = 0;
            var variances = new List<double>();

            // how many energies before and after index for local energy average
            var offset = 10;

            for (var i = offset; i <= energies.Count - offset - 1; i++)
            {
                // calculate local energy average
                var currentEnergy = energies[i];
                var qwe = RangeSum(energies.ToArray(), i - offset, i - 1) + currentEnergy + RangeSum(energies.ToArray(), i + 1, i + offset);
                qwe /= offset * 2 + 1;

                // calculate energy variance of nearby energies
                var nearbyEnergies = energies.Skip(i - 5).Take(5).Concat(energies.Skip(i + 1).Take(5)).ToList();
                average = nearbyEnergies.Average();
                sumOfSquaresOfDifferences = nearbyEnergies.Select(val => (val - average) * (val - average)).Sum();
                variance = sumOfSquaresOfDifferences / nearbyEnergies.Count / Math.Pow(10, 22);

                // experimental linear regression - constant calculated according to local energy variance
                newC = variance * 0.009 + 1.385;
                if (currentEnergy > newC * qwe)
                    beats++;
            }

            BPM = beats / reader.TotalTime.TotalMinutes / 2;
            return BPM;
        }

        private static double RangeQuadSum(float[] samples, int start, int stop)
        {
            double tmp = 0;
            for (var i = start; i <= stop; i++)
            {
                tmp += Math.Pow(samples[i], 2);
            }

            return tmp;
        }

        private static double RangeSum(double[] data, int start, int stop)
        {
            double tmp = 0;
            for (var i = start; i <= stop; i++)
            {
                tmp += data[i];
            }

            return tmp;
        }
    }
}