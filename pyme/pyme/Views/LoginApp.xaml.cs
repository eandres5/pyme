using pyme.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace pyme.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LoginApp : ContentPage
    {
        LoginViewModel _loginViewModel;

        public LoginApp()
        {
            InitializeComponent();
            BindingContext = _loginViewModel = new LoginViewModel();
        }

    }
}