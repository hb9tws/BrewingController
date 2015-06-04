using System.Threading.Tasks;

namespace BrewingController.Interfaces
{
    public interface ITemperatureSensor
    {
        Task<double> Measure();
    }
}
