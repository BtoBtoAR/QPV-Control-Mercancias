using QPVControlMercancias.Models;
using QPVControlMercancias.Pages;
using QPVControlMercancias.Services;

namespace QPVControlMercancias;

public partial class MainPage : ContentPage
{
	private readonly DatabaseService _databaseService;

	public MainPage(DatabaseService databaseService)
	{
		_databaseService = databaseService;
		InitializeComponent();
	}

	protected override async void OnAppearing()
	{
		base.OnAppearing();
		await LoadProducts();
	}

	private async Task LoadProducts()
	{
		try
		{
			var products = await _databaseService.GetProductsAsync();
			ProductsCollectionView.ItemsSource = products;
			
			var count = products.Count;
			StatsLabel.Text = $"Total de productos: {count}";
		}
		catch (Exception ex)
		{
			await DisplayAlert("Error", $"Error al cargar productos: {ex.Message}", "OK");
		}
	}

	private async void OnScanButtonClicked(object? sender, EventArgs e)
	{
		try
		{
			await Navigation.PushAsync(new ScannerPage(_databaseService));
		}
		catch (Exception ex)
		{
			await DisplayAlert("Error", $"Error al abrir escáner: {ex.Message}", "OK");
		}
	}

	private async void OnProductSelected(object? sender, SelectionChangedEventArgs e)
	{
		if (e.CurrentSelection.FirstOrDefault() is Product selectedProduct)
		{
			ProductsCollectionView.SelectedItem = null;
			await Navigation.PushAsync(new ProductDetailsPage(_databaseService, selectedProduct));
		}
	}
}
