using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace asp_login.Dtos
{
    public class LineToken
    {

        public required string access_token { get; set; }
        public required string refresh_token { get; set; }
        public required string expires_in { get; set; }
        public required string id_token { get; set; }
        public required string scope { get; set; }

    }
}