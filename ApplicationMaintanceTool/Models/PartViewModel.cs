using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Web;

namespace ApplicationMaintanceTool.Models
{
    public class PartViewModel
    {
        public string id { get; set; }

        public string name { get; set; }


        public PartViewModel()
        { 

        }

        public override string ToString() => base.ToString() + this.id + this.name;

    }
}