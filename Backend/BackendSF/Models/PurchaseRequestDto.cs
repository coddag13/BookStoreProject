namespace BackendSF.Models
{
    public class PurchaseRequestDto
    {
        public string BookTitle { get; set; }
        public string Author { get; set; }
        public int Quantity { get; set; }
        public string AccountId { get; set; }
    }
}
