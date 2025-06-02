using System.Globalization;

namespace MAUIClient;

public partial class App : Application
{
	public App()
	{
		SetLocale();
		InitializeComponent();

	}

	protected override Window CreateWindow(IActivationState activationState)
	{
		return new Window(new AppShell());
	}

    void SetLocale()
    {
        Thread.CurrentThread.CurrentCulture = new CultureInfo("en-ZA");
        Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-ZA");

    }
}