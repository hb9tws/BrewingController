/*
  In App.xaml:
  <Application.Resources>
      <vm:ViewModelLocator xmlns:vm="clr-namespace:BrewingController"
                           x:Key="Locator" />
  </Application.Resources>
  
  In the View:
  DataContext="{Binding Source={StaticResource Locator}, Path=ViewModelName}"

  You can also use Blend to do all this with the tool's support.
  See http://www.galasoft.ch/mvvm
*/

using BrewingController.Interfaces;
using BrewingController.Sensor;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Views;
using Microsoft.Practices.ServiceLocation;

namespace BrewingController.ViewModel
{
    /// <summary>
    /// This class contains static references to all the view models in the
    /// application and provides an entry point for the bindings.
    /// </summary>
    public class ViewModelLocator
    {
        /// <summary>
        /// Initializes a new instance of the ViewModelLocator class.
        /// </summary>
        public ViewModelLocator()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);

            ////if (ViewModelBase.IsInDesignModeStatic)
            ////{
            ////    // Create design time view services and models
            ////    SimpleIoc.Default.Register<IDataService, DesignDataService>();
            ////}
            ////else
            ////{
            ////    // Create run time view services and models
            ////    SimpleIoc.Default.Register<IDataService, DataService>();
            ////}

            var navigationService = this.CreateNavigationService();
            SimpleIoc.Default.Register<INavigationService>(() => navigationService);

            // SimpleIoc.Default.Register<IDialogService, DialogService>();

#if DS18B20

            SimpleIoc.Default.Register<DS18B20>(() => new DS18B20(), "brewingchip", true);
            SimpleIoc.Default.Register<II2CBridge>(() => SimpleIoc.Default.GetInstance<DS18B20>("brewingchip"));
            SimpleIoc.Default.Register<ITemperatureSensor>(() => SimpleIoc.Default.GetInstance<DS18B20>("brewingchip"));
            SimpleIoc.Default.Register<IRelais>(() => SimpleIoc.Default.GetInstance<DS18B20>("brewingchip"));
#endif

#if DS1821

            SimpleIoc.Default.Register<DS1821>(() => new DS1821(), "i2cbridge", true);
            SimpleIoc.Default.Register<II2CBridge>(() => SimpleIoc.Default.GetInstance<DS1821>("i2cbridge"));
            SimpleIoc.Default.Register<ITemperatureSensor>(() => SimpleIoc.Default.GetInstance<DS1821>("i2cbridge"));
            SimpleIoc.Default.Register<IRelais>(() => SimpleIoc.Default.GetInstance<DS1821>("i2cbridge"));
#endif

            SimpleIoc.Default.Register<MainViewModel>();
            SimpleIoc.Default.Register<AboutViewModel>();
            SimpleIoc.Default.Register<TemperatureControlViewModel>();
            SimpleIoc.Default.Register<MashProgramViewModel>();
            SimpleIoc.Default.Register<MultiRestMashViewModel>();
        }
  



        private INavigationService CreateNavigationService()
        {
            var navigationService = new NavigationService();

            navigationService.Configure("Main", typeof(MainPage));
            navigationService.Configure("About", typeof(About));
            navigationService.Configure("TemperatureControl", typeof(TemperatureControl));
            navigationService.Configure("MultiRestMash", typeof(MultiRestMash));
            navigationService.Configure("MashProgram", typeof(MashProgram));

            return navigationService;
        }



        public MainViewModel Main
        {
            get
            {
                return ServiceLocator.Current.GetInstance<MainViewModel>();
            }
        }

        public AboutViewModel About
        {
            get
            {
                return ServiceLocator.Current.GetInstance<AboutViewModel>();
            }
        }

        public TemperatureControlViewModel TemperatureControl
        {
            get
            {
                return ServiceLocator.Current.GetInstance<TemperatureControlViewModel>();
            }
        }

        public MultiRestMashViewModel MultiRestMash
        {
            get
            {
                return ServiceLocator.Current.GetInstance<MultiRestMashViewModel>();
            }
        }

        public MashProgramViewModel MashProgram
        {
            get
            {
                return ServiceLocator.Current.GetInstance<MashProgramViewModel>();
            }
        }

        public static void Cleanup()
        {
            // TODO Clear the ViewModels
        }
    }
}