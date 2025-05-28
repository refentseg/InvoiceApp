using MAUIClient.Services;

namespace MAUIClient.Views;

public partial class LoginPage : ContentPage
{
    private readonly IRestService _restService;
    public LoginPage(IRestService restService)
	{
        InitializeComponent();
        _restService = restService;
    }

    protected override bool OnBackButtonPressed()
    {
        Application.Current.Quit();
        return true;
    }

    private async void LoginButton_Clicked(object sender, EventArgs e)
    {
        if (IsCredentialCorrect(Username.Text, Password.Text))
        {
            try
            {
                // When valid credentials are entered, get JWT token from API
                var user = await _restService.LoginAsync(Username.Text, Password.Text);

                if (user != null && !string.IsNullOrEmpty(user.Token))
                {
                    // Authentication successful with API
                    await SecureStorage.SetAsync("hasAuth", "true");

                    // Optionally store user info if needed
                    await SecureStorage.SetAsync("user_email", user.Email);
                    await SecureStorage.SetAsync("user_name", $"{user.FirstName} {user.LastName}");

                    // Navigate to home page
                    await Shell.Current.GoToAsync("///MainPage");
                }
                else
                {
                    // API authentication failed
                    await DisplayAlert("Login failed", "API authentication error", "Try again");
                }
            }
            catch (Exception ex)
            {
                // Handle API errors
                await DisplayAlert("Error", $"An error occurred: {ex.Message}", "OK");
            }
        }
        else
        {
            await DisplayAlert("Login failed", "Uusername or password if invalid", "Try again");
        }

        bool IsCredentialCorrect(string username, string password)
        {
            // Validate that credentials aren't empty
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                return false;
            }

            return true;
        }
    }
}