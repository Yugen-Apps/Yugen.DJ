using System;
using Yugen.Toolkit.Standard.Mvvm.ComponentModel;

namespace Yugen.DJ.ViewModels
{
    public class DeckViewModel : ViewModelBase
    {
        public VinylViewModel VinylLeft = new VinylViewModel();
        public VinylViewModel VinylRight = new VinylViewModel();
    }

    public class VinylViewModel : ViewModelBase
    {
        public int[] TargetElapsedTimeList = {
            10000,
            25000,
            50000,
            75000,
            100000,
            250000,
            500000,
            750000
        };

        private bool _isPaused;
        private TimeSpan _targetElapsedTime = new TimeSpan(10000);

        public bool IsPaused
        {
            get { return _isPaused; }
            set { Set(ref _isPaused, value); }
        }

        public TimeSpan TargetElapsedTime
        {
            get { return _targetElapsedTime; }
            set { Set(ref _targetElapsedTime, value); }
        }

        public int SelectedTargetElapsedTime
        {
            get => (int)TargetElapsedTime.Ticks;
            set => TargetElapsedTime = new TimeSpan(value);
        }
    }
 }