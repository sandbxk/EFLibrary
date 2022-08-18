using System.Reflection;
using Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace infrastructure;

public class LibraryRepository
{
    private DbContextOptions<DbContext> _opts;

    public LibraryRepository()
    {
        _opts = new DbContextOptionsBuilder<DbContext>()
            .UseSqlite("Data source=../Infrastructure/db.db").Options;
    }

    public Book InsertBook(Book book)
    {
        using( var context = new DbContext(_opts, ServiceLifetime.Scoped))
        {
            context.Book.Add(book);
            context.SaveChanges();
            return book;
        }
        
    }

    public List<Book> SelectAllBooks()
    {
        using var context = new DbContext(_opts, ServiceLifetime.Scoped);
        return context.Book.ToList();
    }


    public Book DeleteBook(int id)
    {
        using (var context = new DbContext(_opts, ServiceLifetime.Scoped))
        {
            Book obj = new Book { Id = id };
            context.Book.Remove(obj);
            context.SaveChanges();
            return obj;
        }
    }

    public Author GetFirstAuthor()
    {
        using (var context = new DbContext(_opts, ServiceLifetime.Scoped))
        {
            return context.Author.AsNoTracking().First();
        }
    }


    public Author DeleteAuthor(int id)
    {
        using (var context = new DbContext(_opts, ServiceLifetime.Scoped))
        {
            var author = context.Author.Find(id);
            context.Author.Remove(author ?? throw new InvalidOperationException());
            context.SaveChanges();
            return author;
        }
    }

    public async Task<List<Book>> SelectAllBooksAsync()
    {
        using (var context = new DbContext(_opts, ServiceLifetime.Scoped))
        {
            return await context.Book.AsNoTracking().ToListAsync();
        }
    }


    public Book UpdateBook(Book book)
    {
        using (var context = new DbContext(_opts, ServiceLifetime.Scoped))
        {
            context.Update(book);
            context.SaveChanges();
            return book;
        }
    }


    public void Migrate()
    {
        using (var context = new DbContext(_opts, ServiceLifetime.Scoped))
        {
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();
        }
    }


    public Book PatchBook(Book book)
    {
        using (var context = new DbContext(_opts, ServiceLifetime.Scoped))
        {
            Book existingBook = context.Book.Find(book.Id) ?? throw new InvalidOperationException();
            if (existingBook == null)
            {
                throw new KeyNotFoundException("Could not find by ID " + book.Id);
            }

            foreach (PropertyInfo prop in book.GetType().GetProperties())
            {
                var propertyName = prop.Name;
                var value = prop.GetValue(book);
                if (value != null)
                {
                    existingBook.GetType().GetProperty(propertyName)!.SetValue(existingBook, value);
                }
            }

            context.Update(existingBook);
            context.SaveChanges();
            return existingBook;
        }
    }

    public Book UpsertBook(Book book)
    {
        using (var context = new DbContext(_opts, ServiceLifetime.Scoped))
        {
            var existingEntity = context.Book.Find(book.Id);
            if (existingEntity == null)
            {
                book = InsertBook(book);
            }
            else
            {
                book = PatchBook(book);
            }

            return book;
        }
    }

    public Book GetBookById(int id)
    {
        using (var context = new DbContext(_opts, ServiceLifetime.Scoped))
        {
            return context.Book.Find(id) ?? throw new KeyNotFoundException("Could not find key with ID " + id);
        }
    }

    public Author InsertAuthor(Author author)
    {
        using (var context = new DbContext(_opts, ServiceLifetime.Scoped))
        {
            context.Author.Add(author);
            context.SaveChanges();
            return author;
        }
    }

    public List<Author> GetAuthors()
    {
        using (var context = new DbContext(_opts, ServiceLifetime.Scoped))
        {
            return context.Author.Include(a => a.Books).ToList();
        }
    }

    public List<Book> CustomQuery()
    {
        using (var context = new DbContext(_opts, ServiceLifetime.Scoped))
        {
            var result = from b in context.Book
                where b.Id > 2 
                      && b.AuthorId < 10
                      && EF.Functions.Like(b.Author.Name, "%Bob%")
                select b;
            return result.ToList();
        }
    }
}