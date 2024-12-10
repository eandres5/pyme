using pyme.Services;
using pyme.Views;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace pyme.ViewModels
{
    public class LoginViewModel : BaseViewModel
    {
        private string _identificacion;
        private string _password;
        private readonly AuthenticationService _authenticationService;

        public LoginViewModel()
        {
            _authenticationService = new AuthenticationService();
        }

        public string Identificacion
        {
            get => _identificacion;
            set
            {
                _identificacion = value;
                OnPropertyChanged();
            }
        }

        public string Password
        {
            get => _password;
            set
            {
                _password = value;
                OnPropertyChanged();
            }
        }

        public Command LoginCommand => new Command(async () => await LoginAsync());

        private async Task LoginAsync()
        {
            //var isSuccess = await _authenticationService.LoginAsync(Identificacion, Password);

            //if (isSuccess)
            //{
            //    // Navegar a la página principal o mostrar un mensaje de éxito
            //    //Application.Current.MainPage.Navigation.PushAsync(new MainPage());
            //    //MainPage = new AppShell();
            //}
            //else
            //{
            //    // Mostrar un mensaje de error
            //    await Application.Current.MainPage.DisplayAlert("Error", "Usuario o contraseña incorrectos", "OK");
            //}
            //await Shell.Current.GoToAsync("//ItemsPage");

        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
