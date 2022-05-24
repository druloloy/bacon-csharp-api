using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BaconAPI.Models
{
    enum Defaults
    {
        LIMIT = 5,
        OFFSET = 0,
        MAX_LIMIT = 15,
        MIN_LIMIT = 1
    }

    public class BankHistoryParameters
    {
        public uint Offset { get; set; } = (uint) Defaults.OFFSET;
        public uint Limit { get; set; } = (uint) Defaults.LIMIT;
        public bool OnlyActive { get; set; } = true;
    }
}