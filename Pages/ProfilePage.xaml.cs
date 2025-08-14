using Denomination.Services;

namespace Denomination.Pages;

public partial class ProfilePage : ContentPage

{
    private readonly CurrencyService _currencyService;
    private readonly AuthService _authService;
	public ProfilePage(AuthService authService , CurrencyService currencyService)
	{
		InitializeComponent();
		_authService = authService;
        _currencyService = currencyService;

        CurrencyPicker.ItemsSource = _currencyService.AllCurrencies.Select(c => c.Country).ToList();

        // Preselect the current currency
        CurrencyPicker.SelectedItem = _currencyService.SelectedCurrency?.Country;
    }

    private void Logout(object sender, EventArgs e)
    {
		_authService.Logout();
		Shell.Current.GoToAsync($"//{nameof(LoginPage)}");
    }

     private void CurrencyPicker_SelectedIndexChanged(object sender, EventArgs e)
    {
        string selectedCountry = CurrencyPicker.SelectedItem as string;
        var selected = _currencyService.AllCurrencies.FirstOrDefault(c => c.Country == selectedCountry);

        if (selected != null)
        {
            _currencyService.SelectedCurrency = selected;
        }
    }
}