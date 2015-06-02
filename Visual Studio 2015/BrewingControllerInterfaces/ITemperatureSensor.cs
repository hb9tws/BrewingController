using System;
using System.Threading.Tasks;

namespace BrewingController.Interfaces
{
    public interface ITemperatureSensor
    {
        void Initialize();

        Task<double> Measure();
    }
}
