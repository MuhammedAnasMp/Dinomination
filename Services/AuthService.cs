using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Denomination.Services
{
    public class AuthService
    {
        private const string AuthSateKey = "AuthState";
        public async Task<bool> IsAuthenticatedAsync()
        {
            await Task.Delay(2000);
            var authState = Preferences.Default.Get<bool>(AuthSateKey, false);
            return authState;
        }

        public void Login()
        {
            Preferences.Default.Set<bool>(AuthSateKey, true);
        }

        public void Logout() 
        {
            Preferences.Default.Remove(AuthSateKey);
        }

    }
}
