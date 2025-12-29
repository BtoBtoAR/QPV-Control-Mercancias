using QPVControlMercancias.Services;
using ZXing.Net.Maui;
using ZXing.Net.Maui.Controls;

namespace QPVControlMercancias.Pages
{
    public partial class ScannerPage : ContentPage
    {
        private readonly DatabaseService _databaseService;
        private bool _isDetecting = true;

        public ScannerPage(DatabaseService databaseService)
        {
            _databaseService = databaseService;
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            Title = "Escanear Código de Barras";
            BackgroundColor = Colors.Black;

            var cameraBarcodeReaderView = new CameraBarcodeReaderView
            {
                Options = new BarcodeReaderOptions
                {
                    Formats = BarcodeFormats.OneDimensional | BarcodeFormats.TwoDimensional,
                    AutoRotate = true,
                    Multiple = false
                },
                IsDetecting = true,
                VerticalOptions = LayoutOptions.Fill,
                HorizontalOptions = LayoutOptions.Fill
            };

            cameraBarcodeReaderView.BarcodesDetected += OnBarcodesDetected;

            var overlay = new Grid
            {
                VerticalOptions = LayoutOptions.Fill,
                HorizontalOptions = LayoutOptions.Fill
            };

            var instructionsLabel = new Label
            {
                Text = "Apunte la cámara al código de barras",
                TextColor = Colors.White,
                FontSize = 18,
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Start,
                Margin = new Thickness(0, 50, 0, 0),
                BackgroundColor = Color.FromRgba(0, 0, 0, 180),
                Padding = new Thickness(20, 10)
            };

            var targetFrame = new Frame
            {
                BorderColor = Colors.Red,
                BackgroundColor = Colors.Transparent,
                WidthRequest = 280,
                HeightRequest = 140,
                CornerRadius = 10,
                Padding = 2,
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center
            };

            var cancelButton = new Button
            {
                Text = "Cancelar",
                BackgroundColor = Color.FromRgba(255, 255, 255, 180),
                TextColor = Colors.Black,
                WidthRequest = 120,
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.End,
                Margin = new Thickness(0, 0, 0, 50)
            };

            cancelButton.Clicked += async (s, e) => await Navigation.PopAsync();

            overlay.Children.Add(instructionsLabel);
            overlay.Children.Add(targetFrame);
            overlay.Children.Add(cancelButton);

            var mainGrid = new Grid
            {
                VerticalOptions = LayoutOptions.Fill,
                HorizontalOptions = LayoutOptions.Fill
            };

            mainGrid.Children.Add(cameraBarcodeReaderView);
            mainGrid.Children.Add(overlay);

            Content = mainGrid;
        }

        private async void OnBarcodesDetected(object? sender, BarcodeDetectionEventArgs e)
        {
            if (!_isDetecting)
                return;

            var barcode = e.Results.FirstOrDefault();
            if (barcode == null)
                return;

            _isDetecting = false;

            await MainThread.InvokeOnMainThreadAsync(async () =>
            {
                var barcodeValue = barcode.Value;
                
                // Vibrate to indicate successful scan
                try
                {
                    Vibration.Default.Vibrate(TimeSpan.FromMilliseconds(200));
                }
                catch { }

                // Search for product in database
                var product = await _databaseService.GetProductByBarcodeAsync(barcodeValue);

                if (product != null)
                {
                    // Product exists - navigate to details page
                    await Navigation.PushAsync(new ProductDetailsPage(_databaseService, product));
                }
                else
                {
                    // Product doesn't exist - navigate to create page
                    await Navigation.PushAsync(new ProductDetailsPage(_databaseService, barcodeValue));
                }
            });
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            _isDetecting = true;
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            _isDetecting = false;
        }
    }
}
