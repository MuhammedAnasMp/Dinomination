using CommunityToolkit.Maui.Views;

namespace Denomination;

public partial class AuthenticationPopup : Popup
{
    public string Username { get; private set; }
    public string Password { get; private set; }
    public bool IsCancelled { get; private set; }

    public AuthenticationPopup()
    {
        InitializeComponent();
        IsCancelled = true; // Default to cancelled
    }

    private void OnOkClicked(object sender, EventArgs e)
    {
        Username = UsernameEntry.Text;
        Password = PasswordEntry.Text;
        IsCancelled = false;
        Close();
    }

    private void OnCancelClicked(object sender, EventArgs e)
    {
        IsCancelled = true;
        Close();
    }
}