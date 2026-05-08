using System.Threading.Tasks;
using BookStoreShared.Models;
using Microsoft.ServiceFabric.Services.Remoting;

namespace BookStoreShared.Interfaces
{
    public interface ILibraryService : IService
    {
        Task<BookDto> GetBookByTitleAndAuthorAsync(string title, string author);
        Task<bool> DecreaseBookQuantityAsync(string bookId, int quantity);
    }
}
