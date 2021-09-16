using Microsoft.Toolkit.Mvvm.Input;
using System;
using System.Threading.Tasks;
using Yugen.Toolkit.Standard.Mvvm;

namespace Yugen.Audio.Samples.ViewModels
{
    public class VinylViewModel : ViewModelBase
    {
        private bool _isStep;

        public VinylViewModel()
        {
            UpdateCommand = new RelayCommand(UpdateCommandBehavior);
            StepCommand = new RelayCommand(StepCommandBehavior);
            TaskCommand = new AsyncRelayCommand(TaskCommandBehavior);
            BackgroundTaskCommand = new RelayCommand(BackgroundTaskCommandBehavior);
        }
        public IRelayCommand UpdateCommand { get; }

        public IRelayCommand StepCommand { get; }

        public IAsyncRelayCommand TaskCommand { get; }

        public IRelayCommand BackgroundTaskCommand { get; }

        public bool IsStep
        {
            get => _isStep;
            set => SetProperty(ref _isStep, value);
        }

        private void UpdateCommandBehavior()
        {
        }

        private void StepCommandBehavior()
        {
            IsStep = !IsStep;
        }

        private async Task TaskCommandBehavior()
        {
            await Task.Delay(1000);
            System.Diagnostics.Debug.WriteLine("Bello!");
        }

        private void BackgroundTaskCommandBehavior()
        {
            _ = Task.Run(async () =>
              {
                  await Task.Delay(1000);
                  System.Diagnostics.Debug.WriteLine("Bello!");
              });
        }
    }
}