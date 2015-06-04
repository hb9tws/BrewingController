using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Windows.Devices.I2c;
using BrewingController.Interfaces;

namespace BrewingController.Sensor
{
    public class DS1821 : II2CBridge, ITemperatureSensor, IRelais
    {
        private const byte I2C_ADDRESS = 0x25; // 7-bit I2C address of the I2C to 1Wire bridge for the DS18B20 sensor

        private I2cDevice _i2CBridge;
        private I2cConnectionSettings _i2CConnectionSettings = null;

        private readonly object _lock = new object();

        public bool Ready()
        {
            return _i2CConnectionSettings != null;
        }

        public async void Initialize()
        {
            if (Ready())
            {
                var msg = $"I2C device with address {_i2CConnectionSettings.SlaveAddress} is already initialized.";
                Debug.WriteLine(msg);
                return;
            }

            string deviceSelector = I2cDevice.GetDeviceSelector();
            var i2CDeviceControllers = await DeviceInformation.FindAllAsync(deviceSelector);

            if (i2CDeviceControllers.Count == 0)
            {
                Debug.WriteLine("No I2C controllers were found on this system.");
            }
            else
            {
                _i2CConnectionSettings = new I2cConnectionSettings(I2C_ADDRESS);
                _i2CConnectionSettings.BusSpeed = I2cBusSpeed.FastMode;
                var bridge = await I2cDevice.FromIdAsync(i2CDeviceControllers[0].Id, _i2CConnectionSettings);

                lock (_lock)
                {
                    /* Do not try to setup _i2CBridge twice, so double check first
                     */
                    if (_i2CBridge == null)
                    {
                        /* Check if I2C address is already in use
                         */
                        if (bridge == null)
                        {
                            var msg = $"Slave address {_i2CConnectionSettings.SlaveAddress} is currently in use on {i2CDeviceControllers[0].Id}. " +
                                    "Please ensure that no other applications are using I2C.";
                            Debug.WriteLine(msg);
                        }
                        else
                        {
                            /* Remember the 
                             */
                            _i2CBridge = bridge;
                            var msg = $"Using I2C device with address {_i2CConnectionSettings.SlaveAddress} on {i2CDeviceControllers[0].Id}";
                            Debug.WriteLine(msg);
                        }
                    }
                }
            }
        }

        public void On()
        {
            if (Ready())
            {
                byte[] commandBuffer = new byte[2];
                commandBuffer[0] = 8;
                commandBuffer[1] = 1;
                _i2CBridge.Write(commandBuffer);
                Debug.WriteLine("Relais switched ON");
            }
        }

        public void Off()
        {
            if (Ready())
            {
                byte[] commandBuffer = new byte[2];
                commandBuffer[0] = 8;
                commandBuffer[1] = 0;
                _i2CBridge.Write(commandBuffer);
                Debug.WriteLine("Relais switched OFF");
            }
        }

        public async Task<double> Measure()
        {
            double result = Double.NaN;

            byte[] commandBuffer = new byte[1];
            byte[] stateBuffer = new byte[1];
            byte[] resultBuffer = new byte[4];

            /* Step 1: Trigger measurement
             */
            commandBuffer[0] = 1;
            _i2CBridge.Write(commandBuffer);

            /* Step 2: Wait some time and check if measurement is ready
             */
            do
            {
                await Task.Delay(200);
                commandBuffer[0] = 2;
                _i2CBridge.Write(commandBuffer);
                _i2CBridge.Read(stateBuffer);

            } while (stateBuffer[0] != 1);

            /* Step 3: Read result
             */
            commandBuffer[0] = 4;
            _i2CBridge.Write(commandBuffer);
            _i2CBridge.Read(resultBuffer);

            /* Step 4: Convert result to double 
             */
            if (resultBuffer[0] == 1)
            {
                // Calculate temperature :-)
                double remainder = resultBuffer[3];
                double count_per_c = resultBuffer[2];
                double temperature = resultBuffer[1];

                temperature = temperature - 0.5 + (count_per_c - remainder) / count_per_c;
                result = temperature;
            }

            Debug.WriteLine("Temperature measured from DS1821 is {0:F1} °C", result);
            return result;
        }
    }
}
