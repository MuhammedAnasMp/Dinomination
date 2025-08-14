using Denomination.Services;
using System.Threading.Tasks;

namespace Denomination.Pages;

public partial class LoginPage : ContentPage
{
	private readonly AuthService _authService;
	public LoginPage(AuthService authService)
	{
		InitializeComponent();
        _authService = authService;
	}

   

    private async void ButtonClicked(object sender, EventArgs e)
    {
        string userId = UserIdEntry.Text;
        string password = PasswordEntry.Text;
        if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(password))
        {
            await DisplayAlert("Error", "Please enter both user ID and password.", "OK");
        }
        else
        { 
         _authService.Login();
		await Shell.Current.GoToAsync($"//{nameof(MainPage)}");
        }

    }
}