using System.Runtime.Serialization;

namespace BookStoreShared.Models
{
    [DataContract]
    public class PurchaseRequestDto
    {
        [DataMember]
        public string BookTitle { get; set; }

        [DataMember]
        public string Author { get; set; }

        [DataMember]
        public int Quantity { get; set; }

        [DataMember]
        public string AccountId { get; set; }
    }
}
