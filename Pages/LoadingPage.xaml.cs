using Denomination.Services;

namespace Denomination.Pages;

public partial class LoadingPage : ContentPage
{
	private readonly AuthService _authService;
    private readonly CurrencyService _currencyService;
    public LoadingPage(AuthService autService, CurrencyService currencyService)
	{
		InitializeComponent();
		_authService = autService;

	}

	protected async override void OnNavigatedTo(NavigatedToEventArgs args)
	{
		base.OnNavigatedTo(args);

		if (await _authService.IsAuthenticatedAsync())
		{
            //logged in.
         await  Shell.Current.GoToAsync($"//{nameof(MainPage)}");
        }
		else
		{	
            await	Shell.Current.GoToAsync($"//{nameof(LoginPage)}");
        }
	}
}