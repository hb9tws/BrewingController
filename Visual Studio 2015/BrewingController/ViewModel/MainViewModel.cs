using System.Diagnostics;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Views;

namespace BrewingController.ViewModel
{
    /// <summary>
    /// This class contains properties that the main View can data bind to.
    /// <para>
    /// Use the <strong>mvvminpc</strong> snippet to add bindable properties to this ViewModel.
    /// </para>
    /// <para>
    /// You can also use Blend to data bind with the tool's support.
    /// </para>
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class MainViewModel : ViewModelBase, INavigable
    {
        /* Navigation service and related commands
        */
        private INavigationService navigationService;
        public RelayCommand TemperatureControlCommand { get; set; }
        public RelayCommand MashProgamCommand { get; set; }
        public RelayCommand MultiRestMashCommand { get; set; }

        public RelayCommand AboutCommand { get; set; }


        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        //public MainViewModel()
        public MainViewModel(INavigationService navi)
        {
            navigationService = navi;
            WireUpNaviationCommands();

            //if (IsInDesignMode)
            //{
            //    // Code runs in Blend --> create design time data.
            //}
            //else
            //{
            //    // Code runs "for real"
            //}
        }


        private void WireUpNaviationCommands()
        {
            AboutCommand = new RelayCommand(() =>
            {
                navigationService.NavigateTo("About");
            });

            TemperatureControlCommand = new RelayCommand(() =>
            {
                navigationService.NavigateTo("TemperatureControl");
            });

            MashProgamCommand = new RelayCommand(() =>
            {
                navigationService.NavigateTo("MashProgram");
            });

            MultiRestMashCommand = new RelayCommand(() =>
            {
                navigationService.NavigateTo("MultiRestMash");
            });
        }


        public void Activate(object parameter)
        {
            Debug.WriteLine("MainPage activated");
        }

        public void Deactivate(object parameter)
        {
            Debug.WriteLine("MainPage deactivated");
        }
    }
}