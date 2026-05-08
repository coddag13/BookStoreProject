using System.Runtime.Serialization;

namespace BookStoreShared.Models
{
    [DataContract]
    public class AccountDto
    {
        [DataMember]
        public string AccountId { get; set; }

        [DataMember]
        public string UserName { get; set; }

        [DataMember]
        public decimal Balance { get; set; }
    }
}
