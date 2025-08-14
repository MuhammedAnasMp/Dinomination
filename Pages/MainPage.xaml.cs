namespace Denomination.Pages
{
    public partial class MainPage : ContentPage
    {

        private readonly CurrencyService _currencyService;
        public MainPage(CurrencyService currencyService)
        {
            InitializeComponent();
            _currencyService = currencyService;

            BindingContext = _currencyService;

        }


    }

}
