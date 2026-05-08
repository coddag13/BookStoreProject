using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookStoreShared.Models
{
    public class PurchaseRequestDto
    {
        public string BookTitle { get; set; }
        public string Author { get; set; }
        public int Quantity { get; set; }
        public string AccountId { get; set; }
    }
}
