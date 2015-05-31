using System.Diagnostics;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Views;

namespace BrewingController.ViewModel
{
    public class TemperatureControlViewModel : ViewModelBase, INavigable
    {

        private readonly INavigationService _navigationService;
        public RelayCommand BackCommand { get; set; }

        public TemperatureControlViewModel(INavigationService navi)
        {
            _navigationService = navi;
            BackCommand = new RelayCommand(() =>
            {
                _navigationService.GoBack();
            });

            //if (IsInDesignMode)
            //{
            //    // Code runs in Blend --> create design time data.
            //}
            //else
            //{
            //    // Code runs "for real"
            //}
        }


        public void Activate(object parameter)
        {
            Debug.WriteLine("TemperatureControl activated");
        }

        public void Deactivate(object parameter)
        {
            Debug.WriteLine("TemperatureControl deactivated");
        }
    }
}
