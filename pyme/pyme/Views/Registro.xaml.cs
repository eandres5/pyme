using pyme.Models;
using pyme.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace pyme.Views
{
    public partial class Registro : ContentPage
    {

        public Registro()
        {
            InitializeComponent();
            BindingContext = new NewItemViewModel();
            BindingContext = new ResumenComprobantesViewModel();

        }

    }
}