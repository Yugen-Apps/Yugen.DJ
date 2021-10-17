using Yugen.Toolkit.Standard.Mvvm.ComponentModel;
using Yugen.Toolkit.Uwp.Audio.Services.Abstractions.Helpers;

namespace Yugen.DJ.Uwp.ObservableObjects
{
    public class EqualizerBandObservableOject : ObservableObject<EqualizerBand>
    {
        private double _gain = 0;

        public EqualizerBandObservableOject(EqualizerBand model) : base(model)
        {
        }

        public int BandNo
        {
            get => Model.BandNo;
            set => SetProperty(Model.BandNo, value, (v) => Model.BandNo = v);
        }

        public float CenterFrequency
        {
            get => Model.CenterFrequency;
            set => SetProperty(Model.CenterFrequency, value, (v) => Model.CenterFrequency = v);
        }

        public string Label
        {
            get => Model.Label;
            set => SetProperty(Model.Label, value, (v) => Model.Label = v);
        }

        public double Gain
        {
            get => _gain;
            set => SetProperty(ref _gain, value);
        }
    }
}