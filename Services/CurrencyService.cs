using System.Collections.Generic;
using System.ComponentModel;
using System.Net.Http;
using System.Text.Json;
using System.Windows.Input;
using Denomination.ViewModels;

public class CurrencyService : INotifyPropertyChanged
{
    public List<CurrencyInfo> AllCurrencies { get; set; }
    public ICommand PostToApiCommand { get; }

    private CurrencyInfo _selectedCurrency;
    private List<CoinViewModel> _coinViewModels;
    private List<NoteViewModel> _noteViewModels;

    public CurrencyInfo SelectedCurrency
    {
        get => _selectedCurrency;
        set
        {
            if (_selectedCurrency != value)
            {
                _selectedCurrency = value;
                UpdateViewModels(); // Update cached ViewModels when currency changes
                OnPropertyChanged(nameof(SelectedCurrency));
                OnPropertyChanged(nameof(SelectedCurrencyInfo));
                OnPropertyChanged(nameof(CoinViewModels));
                OnPropertyChanged(nameof(NoteViewModels));
                Preferences.Default.Set("SelectedCurrencyCode", _selectedCurrency?.CurrencyCode ?? AllCurrencies.First().CurrencyCode);
            }
        }
    }

    public List<CoinViewModel> CoinViewModels
    {
        get => _coinViewModels ?? [];
        private set
        {
            _coinViewModels = value;
            OnPropertyChanged(nameof(CoinViewModels));
        }
    }

    public List<NoteViewModel> NoteViewModels
    {
        get => _noteViewModels ?? [];
        private set
        {
            _noteViewModels = value;
            OnPropertyChanged(nameof(NoteViewModels));
        }
    }

    public List<CurrencyInfo> SelectedCurrencyInfo => SelectedCurrency != null ? new List<CurrencyInfo> { SelectedCurrency } : new List<CurrencyInfo>();

    public event PropertyChangedEventHandler PropertyChanged;
    protected void OnPropertyChanged(string name) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

    public CurrencyService()
    {
        AllCurrencies = new List<CurrencyInfo>
        {
            new CurrencyInfo
            {
                Country = "United Arab Emirates",
                CurrencyCode = "AED",
                CurrencyName = "UAE Dirham",
                CurrencySymbol = "د.إ",
                SubunitName = "Fils",
                SubunitPerUnit = 100,
                Coins = new List<decimal> { 0.25m, 0.50m, 1m },
                Notes = new List<decimal> { 5m, 10m, 20m, 50m, 100m, 200m, 500m, 1000m }
            },
            new CurrencyInfo
            {
                Country = "Kuwait",
                CurrencyCode = "KWD",
                CurrencyName = "Kuwaiti Dinar",
                CurrencySymbol = "د.ك",
                SubunitName = "Fils",
                SubunitPerUnit = 1000,
                Coins = new List<decimal> { 0.005m, 0.010m, 0.020m, 0.050m, 0.100m, 0.250m },
                Notes = new List<decimal> { 0.25m, 0.50m, 1m, 5m, 10m, 20m }
            }
        };

        string savedCode = Preferences.Default.Get("SelectedCurrencyCode", AllCurrencies.First().CurrencyCode);
        _selectedCurrency = AllCurrencies.FirstOrDefault(c => c.CurrencyCode == savedCode);
        UpdateViewModels(); // Initialize ViewModels

        PostToApiCommand = new Command(async () => await PostToApi());
    }

    private void UpdateViewModels()
    {
        // Reset ViewModels when currency changes
        CoinViewModels = SelectedCurrency?.Coins?.Select(c => new CoinViewModel(c)).ToList() ?? [];
        NoteViewModels = SelectedCurrency?.Notes?.Select(n => new NoteViewModel(n)).ToList() ?? [];
    }

    private async Task PostToApi()
    {
        try
        {
            // Prepare the DTO
            var dto = new DenominationDto
            {
                CurrencyCode = SelectedCurrency?.CurrencyCode,
                Coins = CoinViewModels.Select(c => new DenominationItem
                {
                    Denomination = c.Denomination.ToString(),
                    Quantity = c.Quantity,
                    Total = c.Total
                }).Where(c => c.Quantity > 0).ToList(),
                Notes = NoteViewModels.Select(n => new DenominationItem
                {
                    Denomination = n.Denomination.ToString(),
                    Quantity = n.Quantity,
                    Total = n.Total
                }).Where(n => n.Quantity > 0).ToList()
            };

            // Debug: Log the DTO to verify data
            System.Diagnostics.Debug.WriteLine(JsonSerializer.Serialize(dto));

            // Set up HttpClient
            using var client = new HttpClient();
            client.BaseAddress = new Uri("http://localhost:8000/"); // Replace with your API URL

            // Serialize the DTO to JSON
            var json = JsonSerializer.Serialize(dto);
            var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

            // Post to the API
            var response = await client.PostAsync("api/denominations", content); // Replace with your API endpoint path

            // Handle the response
            if (response.IsSuccessStatusCode)
            {
                await Application.Current.MainPage.DisplayAlert("Success", "Data posted successfully!", "OK");
            }
            else
            {
                var error = await response.Content.ReadAsStringAsync();
                await Application.Current.MainPage.DisplayAlert("Error", $"Failed to post data: {error}", "OK");
            }
        }
        catch (Exception ex)
        {
            await Application.Current.MainPage.DisplayAlert("Error", $"An error occurred: {ex.Message}", "OK");
        }
    }
}

public class DenominationDto
{
    public string CurrencyCode { get; set; }
    public List<DenominationItem> Coins { get; set; }
    public List<DenominationItem> Notes { get; set; }
}

public class DenominationItem
{
    public string Denomination { get; set; }
    public int Quantity { get; set; }
    public decimal Total { get; set; }
}
public class CurrencyInfo
{
    public string Country { get; set; }
    public string CurrencyCode { get; set; }
    public string CurrencyName { get; set; }
    public string CurrencySymbol { get; set; }
    public string SubunitName { get; set; }
    public int SubunitPerUnit { get; set; }
    public List<decimal> Coins { get; set; }
    public List<decimal> Notes { get; set; }

    // Computed properties for display
    public string CoinsString => string.Join(", ", Coins);
    public string NotesString => string.Join(", ", Notes);
}

