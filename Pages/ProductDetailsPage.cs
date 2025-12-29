using QPVControlMercancias.Models;
using QPVControlMercancias.Services;

namespace QPVControlMercancias.Pages
{
    public partial class ProductDetailsPage : ContentPage
    {
        private readonly DatabaseService _databaseService;
        private Product? _product;
        private readonly string? _scannedBarcode;
        private string? _selectedPhotoPath;

        private Entry _barcodeEntry = null!;
        private Entry _descriptionEntry = null!;
        private Entry _unitOfMeasureEntry = null!;
        private Entry _costEntry = null!;
        private Entry _sellingPriceEntry = null!;
        private Entry _stockEntry = null!;
        private Image _productImage = null!;
        private Button _capturePhotoButton = null!;
        private Button _saveButton = null!;

        public ProductDetailsPage(DatabaseService databaseService, Product product)
        {
            _databaseService = databaseService;
            _product = product;
            _scannedBarcode = null;
            InitializeComponent();
            LoadProductData();
        }

        public ProductDetailsPage(DatabaseService databaseService, string barcode)
        {
            _databaseService = databaseService;
            _product = null;
            _scannedBarcode = barcode;
            InitializeComponent();
            LoadProductData();
        }

        private void InitializeComponent()
        {
            Title = _product != null ? "Detalles del Producto" : "Nuevo Producto";
            BackgroundColor = Colors.White;

            var scrollView = new ScrollView
            {
                Padding = new Thickness(20)
            };

            var stackLayout = new VerticalStackLayout
            {
                Spacing = 15
            };

            // Product Image
            _productImage = new Image
            {
                HeightRequest = 200,
                Aspect = Aspect.AspectFit,
                BackgroundColor = Color.FromRgb(240, 240, 240),
                HorizontalOptions = LayoutOptions.Fill
            };

            _capturePhotoButton = new Button
            {
                Text = "Capturar Foto del Producto",
                BackgroundColor = Color.FromRgb(33, 150, 243),
                TextColor = Colors.White,
                CornerRadius = 5,
                Margin = new Thickness(0, 0, 0, 10)
            };
            _capturePhotoButton.Clicked += OnCapturePhotoClicked;

            // Barcode
            stackLayout.Add(new Label { Text = "Código de Barras:", FontAttributes = FontAttributes.Bold, FontSize = 14 });
            _barcodeEntry = new Entry
            {
                Placeholder = "Ingrese el código de barras",
                IsReadOnly = _product != null,
                BackgroundColor = Color.FromRgb(245, 245, 245),
                TextColor = Colors.Black
            };
            stackLayout.Add(_barcodeEntry);

            // Description
            stackLayout.Add(new Label { Text = "Descripción:", FontAttributes = FontAttributes.Bold, FontSize = 14, Margin = new Thickness(0, 10, 0, 0) });
            _descriptionEntry = new Entry
            {
                Placeholder = "Ingrese la descripción del producto",
                BackgroundColor = Color.FromRgb(245, 245, 245),
                TextColor = Colors.Black
            };
            stackLayout.Add(_descriptionEntry);

            // Unit of Measure
            stackLayout.Add(new Label { Text = "Unidad de Medida:", FontAttributes = FontAttributes.Bold, FontSize = 14, Margin = new Thickness(0, 10, 0, 0) });
            _unitOfMeasureEntry = new Entry
            {
                Placeholder = "Ej: Pieza, Kg, Litro",
                BackgroundColor = Color.FromRgb(245, 245, 245),
                TextColor = Colors.Black
            };
            stackLayout.Add(_unitOfMeasureEntry);

            // Cost
            stackLayout.Add(new Label { Text = "Costo:", FontAttributes = FontAttributes.Bold, FontSize = 14, Margin = new Thickness(0, 10, 0, 0) });
            _costEntry = new Entry
            {
                Placeholder = "0.00",
                Keyboard = Keyboard.Numeric,
                BackgroundColor = Color.FromRgb(245, 245, 245),
                TextColor = Colors.Black
            };
            stackLayout.Add(_costEntry);

            // Selling Price
            stackLayout.Add(new Label { Text = "Precio de Venta:", FontAttributes = FontAttributes.Bold, FontSize = 14, Margin = new Thickness(0, 10, 0, 0) });
            _sellingPriceEntry = new Entry
            {
                Placeholder = "0.00",
                Keyboard = Keyboard.Numeric,
                BackgroundColor = Color.FromRgb(245, 245, 245),
                TextColor = Colors.Black
            };
            stackLayout.Add(_sellingPriceEntry);

            // Stock
            stackLayout.Add(new Label { Text = "Stock Actual:", FontAttributes = FontAttributes.Bold, FontSize = 14, Margin = new Thickness(0, 10, 0, 0) });
            _stockEntry = new Entry
            {
                Placeholder = "0",
                Keyboard = Keyboard.Numeric,
                BackgroundColor = Color.FromRgb(245, 245, 245),
                TextColor = Colors.Black
            };
            stackLayout.Add(_stockEntry);

            // Save Button
            _saveButton = new Button
            {
                Text = "Guardar Producto",
                BackgroundColor = Color.FromRgb(76, 175, 80),
                TextColor = Colors.White,
                CornerRadius = 5,
                Margin = new Thickness(0, 20, 0, 0),
                HeightRequest = 50,
                FontSize = 16,
                FontAttributes = FontAttributes.Bold
            };
            _saveButton.Clicked += OnSaveClicked;

            stackLayout.Children.Insert(0, _productImage);
            stackLayout.Children.Insert(1, _capturePhotoButton);
            stackLayout.Add(_saveButton);

            scrollView.Content = stackLayout;
            Content = scrollView;
        }

        private void LoadProductData()
        {
            if (_product != null)
            {
                _barcodeEntry.Text = _product.Barcode;
                _descriptionEntry.Text = _product.Description;
                _unitOfMeasureEntry.Text = _product.UnitOfMeasure;
                _costEntry.Text = _product.Cost.ToString("F2");
                _sellingPriceEntry.Text = _product.SellingPrice.ToString("F2");
                _stockEntry.Text = _product.Stock.ToString();

                if (!string.IsNullOrEmpty(_product.PhotoPath) && File.Exists(_product.PhotoPath))
                {
                    _productImage.Source = ImageSource.FromFile(_product.PhotoPath);
                    _selectedPhotoPath = _product.PhotoPath;
                }
            }
            else if (!string.IsNullOrEmpty(_scannedBarcode))
            {
                _barcodeEntry.Text = _scannedBarcode;
            }
        }

        private async void OnCapturePhotoClicked(object? sender, EventArgs e)
        {
            try
            {
                if (MediaPicker.Default.IsCaptureSupported)
                {
                    var photo = await MediaPicker.Default.CapturePhotoAsync();
                    if (photo != null)
                    {
                        // Save the photo to local storage with unique filename
                        var fileName = $"product_{Guid.NewGuid():N}.jpg";
                        var localFilePath = Path.Combine(FileSystem.AppDataDirectory, fileName);
                        
                        using (var sourceStream = await photo.OpenReadAsync())
                        using (var localFileStream = File.OpenWrite(localFilePath))
                        {
                            await sourceStream.CopyToAsync(localFileStream);
                        }

                        _selectedPhotoPath = localFilePath;
                        _productImage.Source = ImageSource.FromFile(localFilePath);
                    }
                }
                else
                {
                    await DisplayAlert("Error", "La captura de fotos no está soportada en este dispositivo", "OK");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Error al capturar foto: {ex.Message}", "OK");
            }
        }

        private async void OnSaveClicked(object? sender, EventArgs e)
        {
            try
            {
                // Validate inputs
                if (string.IsNullOrWhiteSpace(_barcodeEntry.Text))
                {
                    await DisplayAlert("Error", "El código de barras es obligatorio", "OK");
                    return;
                }

                if (string.IsNullOrWhiteSpace(_descriptionEntry.Text))
                {
                    await DisplayAlert("Error", "La descripción es obligatoria", "OK");
                    return;
                }

                if (string.IsNullOrWhiteSpace(_unitOfMeasureEntry.Text))
                {
                    await DisplayAlert("Error", "La unidad de medida es obligatoria", "OK");
                    return;
                }

                if (!decimal.TryParse(_costEntry.Text, out var cost))
                {
                    await DisplayAlert("Error", "El costo debe ser un número válido", "OK");
                    return;
                }

                if (!decimal.TryParse(_sellingPriceEntry.Text, out var sellingPrice))
                {
                    await DisplayAlert("Error", "El precio de venta debe ser un número válido", "OK");
                    return;
                }

                if (!int.TryParse(_stockEntry.Text, out var stock))
                {
                    await DisplayAlert("Error", "El stock debe ser un número entero válido", "OK");
                    return;
                }

                // Create or update product
                var product = _product ?? new Product();
                product.Barcode = _barcodeEntry.Text.Trim();
                product.Description = _descriptionEntry.Text.Trim();
                product.UnitOfMeasure = _unitOfMeasureEntry.Text.Trim();
                product.Cost = cost;
                product.SellingPrice = sellingPrice;
                product.Stock = stock;
                product.PhotoPath = _selectedPhotoPath;

                await _databaseService.SaveProductAsync(product);

                await DisplayAlert("Éxito", "Producto guardado correctamente", "OK");
                await Navigation.PopToRootAsync();
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Error al guardar el producto: {ex.Message}", "OK");
            }
        }
    }
}
