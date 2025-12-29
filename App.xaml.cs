namespace QPVControlMercancias;

public partial class App : Application
{
	public App(MainPage mainPage)
	{
		InitializeComponent();

		// Store the mainPage for use in CreateWindow
		Resources.Add("MainPage", mainPage);
	}

	protected override Window CreateWindow(IActivationState? activationState)
	{
		var mainPage = Resources["MainPage"] as MainPage;
		return new Window(new NavigationPage(mainPage!));
	}
}