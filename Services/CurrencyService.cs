using CommunityToolkit.Maui.Views;
using Denomination;
using Denomination.ViewModels;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net.Http;
using System.Text.Json;
using System.Windows.Input;

public class CurrencyService : INotifyPropertyChanged
{
    public List<CurrencyInfo> AllCurrencies { get; set; }
    public ICommand PostToApiCommand { get; }
    public ICommand ResetCommand { get; }

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
                UpdateViewModels();
                OnPropertyChanged(nameof(SelectedCurrency));
                OnPropertyChanged(nameof(SelectedCurrencyInfo));
                OnPropertyChanged(nameof(CoinViewModels));
                OnPropertyChanged(nameof(NoteViewModels));
                OnPropertyChanged(nameof(CoinTotal));
                OnPropertyChanged(nameof(NoteTotal));
                OnPropertyChanged(nameof(GrandTotal));
                OnPropertyChanged(nameof(CurrencySymbol));
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
            foreach (var coin in _coinViewModels)
            {
                coin.PropertyChanged -= CoinViewModel_PropertyChanged;
                coin.PropertyChanged += CoinViewModel_PropertyChanged;
            }
            OnPropertyChanged(nameof(CoinViewModels));
            OnPropertyChanged(nameof(CoinTotal));
            OnPropertyChanged(nameof(GrandTotal));
        }
    }

    public List<NoteViewModel> NoteViewModels
    {
        get => _noteViewModels ?? [];
        private set
        {
            _noteViewModels = value;
            foreach (var note in _noteViewModels)
            {
                note.PropertyChanged -= NoteViewModel_PropertyChanged;
                note.PropertyChanged += NoteViewModel_PropertyChanged;
            }
            OnPropertyChanged(nameof(NoteViewModels));
            OnPropertyChanged(nameof(NoteTotal));
            OnPropertyChanged(nameof(GrandTotal));
        }
    }

    public decimal CoinTotal => CoinViewModels?.Sum(c => c.Total) ?? 0m;
    public decimal NoteTotal => NoteViewModels?.Sum(n => n.Total) ?? 0m;
    public decimal GrandTotal => CoinTotal + NoteTotal;
    public string CurrencySymbol => SelectedCurrency?.CurrencySymbol ?? "";

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
                Coins = new List<decimal> { 0.005m, 0.010m, 0.020m, 0.050m, 0.100m },
                Notes = new List<decimal> { 0.25m, 0.50m, 1m, 5m, 10m, 20m }
            }
        };

        string savedCode = Preferences.Default.Get("SelectedCurrencyCode", AllCurrencies.First().CurrencyCode);
        _selectedCurrency = AllCurrencies.FirstOrDefault(c => c.CurrencyCode == savedCode);
        UpdateViewModels();

        PostToApiCommand = new Command(async () => await PostToApi());
        ResetCommand = new Command(ResetQuantities);
    }

    private void UpdateViewModels()
    {
        CoinViewModels = SelectedCurrency?.Coins?.Select(c => new CoinViewModel(c)).ToList() ?? [];
        NoteViewModels = SelectedCurrency?.Notes?.Select(n => new NoteViewModel(n)).ToList() ?? [];
    }

    private void CoinViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(CoinViewModel.Total))
        {
            OnPropertyChanged(nameof(CoinTotal));
            OnPropertyChanged(nameof(GrandTotal));
        }
    }

    private void NoteViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(NoteViewModel.Total))
        {
            OnPropertyChanged(nameof(NoteTotal));
            OnPropertyChanged(nameof(GrandTotal));
        }
    }

    private void ResetQuantities()
    {
        foreach (var coin in CoinViewModels)
        {
            coin.Quantity = 0;
        }
        foreach (var note in NoteViewModels)
        {
            note.Quantity = 0;
        }
    }

    private async Task PostToApi()

    {
        // Show single authentication popup
        var popup = new AuthenticationPopup();
        await Application.Current.MainPage.ShowPopupAsync(popup);

        if (popup.IsCancelled || string.IsNullOrWhiteSpace(popup.Username) || string.IsNullOrWhiteSpace(popup.Password))
        {
            if (popup.IsCancelled)
            {
                // continue; // User cancelled the
                // 
               return; // Exit the method if cancelled
            }
            else
            {
                await Application.Current.MainPage.DisplayAlert("Error", "Username and password cannot be empty.", "OK");
            }

            return;
        }

       
        bool isAuthenticated = popup.Username == "1" && popup.Password == "1";

        if (!isAuthenticated)
        {
            await Application.Current.MainPage.DisplayAlert("Error", "Authentication Failed: Invalid username or password.", "OK");
            return;
        }
        try
        {
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
                }).Where(n => n.Quantity > 0).ToList(),
                CoinTotal = CoinTotal, // Include CoinTotal
                NoteTotal = NoteTotal, // Include NoteTotal
                GrandTotal = GrandTotal // Include GrandTotal
            };

            System.Diagnostics.Debug.WriteLine(JsonSerializer.Serialize(dto));

            using var client = new HttpClient();
            client.BaseAddress = new Uri("http://localhost:8000/");

            var json = JsonSerializer.Serialize(dto);
            var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

            var response = await client.PostAsync("api/denominations", content);

            if (response.IsSuccessStatusCode)
            {
                await Application.Current.MainPage.DisplayAlert("Success", "Data posted successfully!", "OK");
                ResetQuantities();
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
    public decimal CoinTotal { get; set; } // Added CoinTotal
    public decimal NoteTotal { get; set; } // Added NoteTotal
    public decimal GrandTotal { get; set; } // Added GrandTotal
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

    public string CoinsString => string.Join(", ", Coins);
    public string NotesString => string.Join(", ", Notes);
}

