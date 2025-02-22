﻿using pyme.ViewModels;
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
    public partial class LoginPage : ContentPage
    {
        public LoginPage()
        {
            InitializeComponent();
            //this.BindingContext = new LoginViewModel();
        }

        private async void Button_Clicked(object sender, EventArgs e)
        {
            MessagingCenter.Send<LoginPage>(this,
                (txtUser.Text == "admin") ? "admin" : "user"
            );

            if (txtUser.Text == "administrador")
            {
                await Shell.Current.GoToAsync("//main");
            }
            else {
                await DisplayAlert("Error", "Crendenciales incorrectas", "Ok");
            }
        }

        private async void TapGestureRecognizer_Tapped(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//login/registration");
        }
    }
}