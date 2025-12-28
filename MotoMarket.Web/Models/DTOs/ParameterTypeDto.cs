using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MotoMarket.Web.Models.DTOs
{
    public class ParameterTypeDto : DictionaryDto
    {
        public string Unit { get; set; } = string.Empty;
        public string InputType { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public bool IsRequired { get; set; }    
    }
}
