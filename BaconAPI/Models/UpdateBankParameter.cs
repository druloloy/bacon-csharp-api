using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BaconAPI.Models
{
    public class UpdateBankParameter : bank
    {
        public string reason { get; set; }

        public UpdateBankParameter() : base() { }
    }
}