using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLauncher.Model
{
    internal class GameAuth
    {
        public string HID { get; }
        public string Token { get; }

        public GameAuth(string token, string hid)
        {
            HID = hid;
            Token = token;
        }
    }
}
