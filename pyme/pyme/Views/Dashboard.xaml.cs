using Microcharts.Forms;
using Microcharts;
using pyme.Models;
using pyme.ViewModels;
using SkiaSharp;
using System;
using Microcharts;
using Microcharts.Forms;
using SkiaSharp;
using System.Collections.Generic;
using Xamarin.Forms;

using Xamarin.Forms.Xaml;
using System.Linq;

namespace pyme.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Dashboard : ContentPage
    {
        private readonly ResumenComprobantesViewModel _viewModel;

        public Dashboard()
        {
            InitializeComponent();
            _viewModel = new ResumenComprobantesViewModel();
            BindingContext = _viewModel;
        }

        protected override void OnSizeAllocated(double width, double height)
        {
            base.OnSizeAllocated(width, height);

            // Asegúrate de que chartView y _viewModel no sean nulos antes de asignar el gráfico
            if (chartView != null && _viewModel.ChartEntries != null && _viewModel.ChartEntries.Any())
            {
                chartView.Chart = new BarChart
                {
                    Entries = _viewModel.ChartEntries,
                    LabelTextSize = 40,
                    BackgroundColor = SKColors.White
                };
            }
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            // Suscribirse a mensajes
            MessagingCenter.Subscribe<ResumenComprobantesViewModel, string>(this, "MostrarAlerta", async (sender, mensaje) =>
            {
                await DisplayAlert("Advertencia", mensaje, "Aceptar");
            });

            // Asegúrate de que los datos estén listos antes de actualizar el gráfico
            if (_viewModel.ChartEntries == null || !_viewModel.ChartEntries.Any())
            {
                _viewModel.CargarDatos(); // Cargar los datos si no están disponibles
            }
            else
            {
                ActualizarGrafico();
            }
        }

        private void ActualizarGrafico()
        {
            // Solo actualizar el gráfico si los datos están disponibles
            if (chartView != null && _viewModel.ChartEntries != null && _viewModel.ChartEntries.Any())
            {
                chartView.Chart = new BarChart
                {
                    Entries = _viewModel.ChartEntries,
                    LabelTextSize = 40,
                    BackgroundColor = SKColors.White
                };
            }
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            MessagingCenter.Unsubscribe<ResumenComprobantesViewModel, string>(this, "MostrarAlerta");
        }

    }
}