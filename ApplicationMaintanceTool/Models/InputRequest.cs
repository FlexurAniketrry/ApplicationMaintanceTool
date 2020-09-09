using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ApplicationMaintanceTool.Models
{
    public class InputRequest
    {
        public string iArasUrl { get; set; }

        public string iArasDatabase { get; set; }

        public string iUserId { get; set; }

        public string iPassword { get; set; }

        public string iServerMethod { get; set; }

    }
}