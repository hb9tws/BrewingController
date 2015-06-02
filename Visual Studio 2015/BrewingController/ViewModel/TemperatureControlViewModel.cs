using System;
using System.ComponentModel;
using System.Diagnostics;
using Windows.UI.Xaml;
using BrewingController.Interfaces;
using BrewingController.Sensor;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Threading;
using GalaSoft.MvvmLight.Views;

namespace BrewingController.ViewModel
{
    public class TemperatureControlViewModel : ViewModelBase, INavigable
    {
        private DispatcherTimer measurementTimer;

        private double _temperature = 23.1;

        public double Temperature
        {
            get { return _temperature; }
            set
            {
                if (Math.Abs(value - _temperature) < 0.1) return;
                value = _temperature;
                RaisePropertyChanged();
            }
        }

        private readonly ITemperatureSensor _sensor;

        private readonly INavigationService _navigationService;

        public RelayCommand BackCommand { get; set; }

        public TemperatureControlViewModel(INavigationService navi, ITemperatureSensor sensor)
        {
            _navigationService = navi;
            BackCommand = new RelayCommand(() =>
            {
                _navigationService.GoBack();
            });

            if (IsInDesignMode)
            {
                // code generating sample data for ui design
            }
            else
            {
                _sensor = sensor;
            }

            measurementTimer = new DispatcherTimer();
            measurementTimer.Interval = TimeSpan.FromMilliseconds(1000);
            measurementTimer.Tick += MeasurementTrigger;
        }

        private async void MeasurementTrigger(object sender, object e)
        {
            if (IsInDesignMode)
            {
                var r = new Random();
                Temperature = r.Next(20, 32);
            }
            else
            {
                Temperature = await _sensor.Measure();
            }

        }


        public void Activate(object parameter)
        {
            Debug.WriteLine("TemperatureControl activated");

            _sensor.Initialize();
            measurementTimer.Start();

        }

        public void Deactivate(object parameter)
        {
            Debug.WriteLine("TemperatureControl deactivated");
            measurementTimer.Stop();
        }
    }
}
