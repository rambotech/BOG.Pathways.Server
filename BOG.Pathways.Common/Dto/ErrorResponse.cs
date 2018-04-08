using System;
using System.Collections.Generic;
using System.Text;

namespace BOG.Pathways.Common.Dto
{
    public class ErrorResponse
    {
        public int ErrorLookup { get; set; } = 0;
        public string ErrorMessage { get; set; } = "not specified";
        public string Notes { get; set; } = null;
    }
}
