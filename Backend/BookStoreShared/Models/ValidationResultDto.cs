using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookStoreShared.Models
{
    public class ValidationResultDto
    {
        public bool IsValid { get; set; }
        public string Message { get; set; }
    }
}

