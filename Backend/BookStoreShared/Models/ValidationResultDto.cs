using System.Runtime.Serialization;

namespace BookStoreShared.Models
{
    [DataContract]
    public class ValidationResultDto
    {
        [DataMember]
        public bool IsValid { get; set; }

        [DataMember]
        public string Message { get; set; }
    }
}
