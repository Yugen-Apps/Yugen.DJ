using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using Microsoft.Toolkit.Uwp.Helpers;
using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Yugen.Audio.Samples.ViewModels.Controls
{
    public class VinylViewModel : ObservableObject
    {
        private bool _isPaused = true;
        private double _pitch = 0;

        private TimeSpan _position = new TimeSpan();

        public bool IsPaused
        {
            get { return _isPaused; }
            set { SetProperty(ref _isPaused, value); }
        }

        public double Pitch
        {
            get { return _pitch; }
            set { SetProperty(ref _pitch, value); }
        }

        public TimeSpan Position
        {
            get { return _position; }
            set { SetProperty(ref _position, value); }
        }
    }
}