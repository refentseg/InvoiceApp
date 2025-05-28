namespace MAUIClient;

public partial class MainPage : ContentPage
{

	public MainPage()
	{
		InitializeComponent();
	}

	private async void LogoutButton_Clicked(object sender, EventArgs e)
	{
        SecureStorage.Default.RemoveAll();
		Preferences.Default.Clear();

        await Shell.Current.GoToAsync("//loading");
    }

}