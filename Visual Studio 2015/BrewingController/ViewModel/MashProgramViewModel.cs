using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Views;

namespace BrewingController.ViewModel
{
    public class MashProgramViewModel : ViewModelBase, INavigable
    {
        private INavigationService navigationService;
        public RelayCommand BackCommand { get; set; }


        public MashProgramViewModel(INavigationService navi)
        {
            navigationService = navi;
            BackCommand = new RelayCommand(() =>
            {
                navigationService.GoBack();
            });
        }


        public void Activate(object parameter)
        {
            Debug.WriteLine("MashProgram activated");
        }

        public void Deactivate(object parameter)
        {
            Debug.WriteLine("MashProgram deactivated");
        }
    }
}
