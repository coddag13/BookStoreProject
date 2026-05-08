using System.Runtime.Serialization;

namespace BookStoreShared.Models
{
    [DataContract]
    public class BookDto
    {
        [DataMember]
        public string BookId { get; set; }

        [DataMember]
        public string Title { get; set; }

        [DataMember]
        public string Author { get; set; }

        [DataMember]
        public int AvailableQuantity { get; set; }

        [DataMember]
        public decimal Price { get; set; }
    }
}
