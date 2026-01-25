using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MotoMarket.Mobile
{
    public static class Constants
    {
        // PODMIEŃ TE PORTY NA SWOJE Z PRZEGLĄDARKI!
        // Zazwyczaj HTTP to ok. 5000-5200, HTTPS to 7000-7200.
        // W logach z pierwszego pytania widziałem port 5180, więc zakładam taki:
        const string Port = "5180";

        public static string ApiUrl
        {
            get
            {
                // Jeśli to Android (Emulator) -> użyj 10.0.2.2
                if (DeviceInfo.Platform == DevicePlatform.Android)
                {
                    return $"http://10.0.2.2:{Port}";
                }

                // Jeśli to Windows/Mac/iOS -> użyj localhost
                return $"http://localhost:{Port}";
            }
        }
    }
}
