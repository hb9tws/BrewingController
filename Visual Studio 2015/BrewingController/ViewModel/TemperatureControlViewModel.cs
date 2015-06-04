using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices.ComTypes;
using Windows.UI.Core;
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
                _temperature = value;
                RaisePropertyChanged();
            }
        }


        private double _setTemperature = 40.0;

        public double SetTemperature
        {
            get { return _setTemperature; }
            set
            {
                if (Math.Abs(value - _setTemperature) < 0.1) return;
                _setTemperature = value;
                RaisePropertyChanged();
            }
        }




        private ActuatorEnum _actuator = ActuatorEnum.HeatingDevice;

        public ActuatorEnum Actuator
        {
            get { return _actuator; }
            set
            {
                if (value == _actuator) return;
                _actuator = value;
                RaisePropertyChanged();
            }
        }

        private ControlEnum _control = ControlEnum.Off;

        public ControlEnum Control
        {
            get { return _control; }
            set
            {
                if (value == _control) return;
                _control = value;
                RaisePropertyChanged();
            }
        }

        private ActuatorStateEnum _actuatorState = ActuatorStateEnum.Unknown;

        public ActuatorStateEnum ActuatorState
        {
            get { return _actuatorState; }
            set
            {
                if (value == _actuatorState) return;
                _actuatorState = value;
                RaisePropertyChanged();
            }
        }

        private readonly ITemperatureSensor _sensor;
        private readonly IRelais _relais;
        private readonly II2CBridge _bridge;

        private readonly INavigationService _navigationService;

        public RelayCommand BackCommand { get; set; }

        public RelayCommand TemperatureUpCommand { get; set; }

        public RelayCommand TemperatureDownCommand { get; set; }


        public TemperatureControlViewModel(INavigationService navi, ITemperatureSensor sensor, IRelais relais, II2CBridge bridge)
        {
            _navigationService = navi;
            _sensor = sensor;
            _relais = relais;
            _bridge = bridge;

            BackCommand = new RelayCommand(() =>
            {
                _navigationService.GoBack();
            });

            TemperatureUpCommand = new RelayCommand(IncreaseSetTemperature);
            TemperatureDownCommand = new RelayCommand(DecreaseSetTemperature);
            
            Control = ControlEnum.Off;
            Actuator = ActuatorEnum.HeatingDevice;

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
                RunTemperatureControl();
            }
        }

        private void RunTemperatureControl()
        {
            switch (Control)
            {
                case ControlEnum.Off:
                    ActuatorState = ActuatorStateEnum.Off;
                    _relais.Off();
                    if (Actuator == ActuatorEnum.HeatingDevice)
                    {
                        Debug.WriteLine("Heating device switched off");
                    }
                    else
                    {
                        Debug.WriteLine("Cooling device switched off");
                    }
                    break;

                case ControlEnum.On:
                    ActuatorState = ActuatorStateEnum.On;
                    _relais.On();
                    if (Actuator == ActuatorEnum.HeatingDevice)
                    {
                        Debug.WriteLine("Heating device switched on");
                    }
                    else
                    {
                        Debug.WriteLine("Cooling device switched on");
                    }
                    break;

                case ControlEnum.Automatic:

                    // Later this should go into the model, for now we keep it here

                    double hysteresis = 1.0;

                    if (Actuator == ActuatorEnum.HeatingDevice)
                    {
                        if ( ActuatorState == ActuatorStateEnum.On && 
                             Temperature > SetTemperature + hysteresis)
                        {
                            ActuatorState = ActuatorStateEnum.Off;
                            _relais.Off();
                            Debug.WriteLine("Temperature = {0:F1} / Setpoint {1:F1} -> Heating device switched off",
                                            Temperature, SetTemperature);
                        }
                        else if ( ActuatorState == ActuatorStateEnum.Off && 
                                  Temperature < SetTemperature - hysteresis )
                        {
                            ActuatorState = ActuatorStateEnum.On;
                            _relais.On();
                            Debug.WriteLine("Temperature = {0:F1} / Setpoint {1:F1} -> Heating device switched on",
                                            Temperature, SetTemperature);
                        }
                    }
                    else
                    {
                        if (ActuatorState == ActuatorStateEnum.On &&
                             Temperature < SetTemperature - hysteresis)
                        {
                            ActuatorState = ActuatorStateEnum.Off;
                            _relais.Off();
                            Debug.WriteLine("Temperature = {0:F1} / Setpoint {1:F1} -> Cooling device switched off",
                                            Temperature, SetTemperature);
                        }
                        else if (ActuatorState == ActuatorStateEnum.Off &&
                                  Temperature > SetTemperature + hysteresis)
                        {
                            ActuatorState = ActuatorStateEnum.On;
                            _relais.On();
                            Debug.WriteLine("Temperature = {0:F1} / Setpoint {1:F1} -> Cooling device switched on",
                                             Temperature, SetTemperature);
                        }
                    }
                    break;

                default:
                    ActuatorState = ActuatorStateEnum.Unknown;
                    break;
            }
        }


        public void Activate(object parameter)
        {
            Debug.WriteLine("TemperatureControl activated");

            _bridge.Initialize();
            measurementTimer.Start();

        }

        public void Deactivate(object parameter)
        {
            Debug.WriteLine("TemperatureControl deactivated");
            measurementTimer.Stop();
        }

        private void IncreaseSetTemperature()
        {
            if (SetTemperature < 99 ) SetTemperature += 1.0;
        }

        private void DecreaseSetTemperature()
        {
            if (SetTemperature > 4 ) SetTemperature -= 1.0;
        }
    }
}
