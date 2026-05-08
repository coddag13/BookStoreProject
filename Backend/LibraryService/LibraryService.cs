using System.Collections.Generic;
using System.Fabric;
using System.Threading.Tasks;
using BookStoreShared.Interfaces;
using BookStoreShared.Models;
using Microsoft.ServiceFabric.Data.Collections;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Remoting.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;

namespace LibraryService
{
    internal sealed class LibraryService : StatefulService, ILibraryService
    {
        private const string BooksDictionaryName = "books";

        public LibraryService(StatefulServiceContext context)
            : base(context)
        {
        }

        public async Task<BookDto> GetBookByTitleAndAuthorAsync(string title, string author)
        {
            var booksDictionary =
                await this.StateManager.GetOrAddAsync<IReliableDictionary<string, BookDto>>(BooksDictionaryName);

            using (var tx = this.StateManager.CreateTransaction())
            {
                var enumerable = await booksDictionary.CreateEnumerableAsync(tx);
                var asyncEnumerator = enumerable.GetAsyncEnumerator();

                while (await asyncEnumerator.MoveNextAsync(CancellationToken.None))
                {
                    var book = asyncEnumerator.Current.Value;

                    if (book.Title == title && book.Author == author)
                    {
                        return book;
                    }
                }

                return null;
            }
        }


        public async Task<bool> DecreaseBookQuantityAsync(string bookId, int quantity)
        {
            var booksDictionary =
                await this.StateManager.GetOrAddAsync<IReliableDictionary<string, BookDto>>(BooksDictionaryName);

            using (var tx = this.StateManager.CreateTransaction())
            {
                var result = await booksDictionary.TryGetValueAsync(tx, bookId);

                if (!result.HasValue)
                {
                    return false;
                }

                var book = result.Value;

                if (book.AvailableQuantity < quantity)
                {
                    return false;
                }

                book.AvailableQuantity -= quantity;

                await booksDictionary.SetAsync(tx, bookId, book);
                await tx.CommitAsync();

                return true;
            }
        }

        protected override IEnumerable<ServiceReplicaListener> CreateServiceReplicaListeners()
        {
            return this.CreateServiceRemotingReplicaListeners();
        }

        protected override async Task RunAsync(CancellationToken cancellationToken)
        {
            var booksDictionary =
                await this.StateManager.GetOrAddAsync<IReliableDictionary<string, BookDto>>(BooksDictionaryName);

            using (var tx = this.StateManager.CreateTransaction())
            {
                await booksDictionary.TryAddAsync(tx, "book-1", new BookDto
                {
                    BookId = "book-1",
                    Title = "React u praksi",
                    Author = "Ivan Horvat",
                    AvailableQuantity = 10,
                    Price = 25
                });

                await booksDictionary.TryAddAsync(tx, "book-2", new BookDto
                {
                    BookId = "book-2",
                    Title = "C# osnove",
                    Author = "Marko Jurić",
                    AvailableQuantity = 7,
                    Price = 30
                });

                await booksDictionary.TryAddAsync(tx, "book-3", new BookDto
                {
                    BookId = "book-3",
                    Title = "Service Fabric vodič",
                    Author = "Ana Kovač",
                    AvailableQuantity = 5,
                    Price = 40
                });


                await tx.CommitAsync();
            }

            await Task.CompletedTask;
        }

    }
}
