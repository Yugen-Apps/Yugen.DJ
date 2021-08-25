using AudioVisualizer;
using System;
using System.ComponentModel;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Yugen.Audio.Samples.Views
{
    public sealed partial class VuBarPage : Page
    {
        public FakeDataSource MainSource;
        private SourceConverter _source;
        private float rms = -100.0f;
        private float peak = -100.0f;

        private float rmsFake = -100.0f;
        private float peakFake = -100.0f;

        public VuBarPage()
        {
            this.InitializeComponent();

            MainSource = new FakeDataSource();
            _source = new SourceConverter();
            _source.Source = MainSource;
            GenerateDataFrame();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public float RmsFake
        {
            get => rmsFake;
            set
            {
                rmsFake = value;
                VUBar.Rms = rmsFake;
                DiscreteVUBar.RmsFake = rmsFake;
            }
        }

        public float PeakFake
        {
            get => peakFake;
            set
            {
                peakFake = value;
                DiscreteVUBar.PeakFake = peakFake;
            }
        }

        public float Rms
        {
            get => rms;
            set
            {
                rms = value;
                GenerateDataFrame();
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Rms"));
            }
        }

        public float Peak
        {
            get => peak;
            set
            {
                peak = value;
                GenerateDataFrame();
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Peak"));
            }
        }

        public IVisualizationSource Source => _source;

        private void GenerateDataFrame()
        {
            var rmsValue = ScalarData.Create(new float[] { (float)(Math.Pow(10, rms / 20)) });
            var peakValue = ScalarData.Create(new float[] { (float)(Math.Pow(10, peak / 20)) });
            MainSource.Frame = new VisualizationDataFrame(TimeSpan.Zero, TimeSpan.FromMilliseconds(16.7), rmsValue, peakValue, null);
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            //BarControl.Source = Source;
        }
    }
}