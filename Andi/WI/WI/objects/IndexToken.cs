using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WI.objects
{
    class IndexToken
    {
        public string token;
        public Dictionary<int, int> pages = new Dictionary<int, int>();

        public IndexToken(string Token)
        {
            token = Token;
        }
    }
}
