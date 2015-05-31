using System;
using System.Collections.Generic;
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
            throw new NotImplementedException();
        }

        public void Deactivate(object parameter)
        {
            throw new NotImplementedException();
        }
    }
}
