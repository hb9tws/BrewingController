using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Windows.Devices.I2c;
using BrewingController.Interfaces;

namespace BrewingController.Sensor
{
    public class DS18B20 : ITemperatureSensor
    {
        private const byte I2C_ADDRESS = 0x24; // 7-bit I2C address of the I2C to 1Wire bridge for the DS18B20 sensor

        private I2cDevice _i2CBridge ;
        private I2cConnectionSettings _i2CConnectionSettings;

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

        public async Task<double> Measure()
        {
            double result = Double.NaN;

            byte[] commandBuffer = new byte[1];
            byte[] resultBuffer = new byte[4];

            /* Step 1: Trigger measurement
             */
            commandBuffer[0] = 1; 
            _i2CBridge.Write(commandBuffer); 

            /* Step 2: Wait 800ms to allow sufficient time for the 12-bit ADC converion on DS18B20
             */
            await Task.Delay(800);

            /* Step 3: Read result
             */
            commandBuffer[0] = 4;
            _i2CBridge.Write(commandBuffer);
            _i2CBridge.Read(resultBuffer);

            /* Step 4: Convert result to double 
             */
            if (resultBuffer[0] == 1)
            {
                double temperature = resultBuffer[1];
                temperature = 16 * (0x0F & resultBuffer[2]) + temperature / 16;
                if ((resultBuffer[2] & 0x10) == 0x10) temperature = -1 * temperature;
                result = temperature;
            }

            Debug.WriteLine("Temperature measured from DS18B20 is {0} °C", result);
            return result;
        }
    }
}
