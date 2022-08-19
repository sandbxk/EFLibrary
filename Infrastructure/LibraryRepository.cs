using System.Reflection;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
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
        using (var context = new DbContext(_opts, ServiceLifetime.Scoped))
        {
            context.BookTable.Add(book);
            context.SaveChanges();
            return book;
        }
    }


    public List<Book> SelectAllBooks()
    {
        using (var context = new DbContext(_opts, ServiceLifetime.Scoped))
        {
            return context.BookTable.ToList();
        }
    }

    public List<Book> SelectAllBooksWithCategories()
    {
        using (var context = new DbContext(_opts, ServiceLifetime.Scoped))
        {
            var books = context.BookTable.ToList();
            foreach (var book in books)
            {
                var j = context.BookCategoryJoinTable.Where(j => j.BookId == book.BookId).ToList();
                List<Category> categories = new List<Category>() { };
                foreach (var bookCategory in j)
                {
                    categories.Add(context.CategoryTable.Find(bookCategory.CategoryId));
                }

                book.Categories = categories;
            }
            return books;
        }
    }


    public Book DeleteBook(int id)
    {
        using (var context = new DbContext(_opts, ServiceLifetime.Scoped))
        {
            Book obj = new Book { BookId = id };
            context.BookTable.Remove(obj);
            context.SaveChanges();
            return obj;
        }
    }

    public Author GetFirstAuthor()
    {
        using (var context = new DbContext(_opts, ServiceLifetime.Scoped))
        {
            return context.AuthorTable.AsNoTracking().First();
        }
    }


    public Author DeleteAuthor(int id)
    {
        using (var context = new DbContext(_opts, ServiceLifetime.Scoped))
        {
            var author = context.AuthorTable.Find(id);
            context.AuthorTable.Remove(author ?? throw new InvalidOperationException());
            context.SaveChanges();
            return author;
        }
    }

    public async Task<List<Book>> SelectAllBooksAsync()
    {
        using (var context = new DbContext(_opts, ServiceLifetime.Scoped))
        {
            return await context.BookTable.AsNoTracking().ToListAsync();
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
            Book existingBook = context.BookTable.Find(book.BookId) ?? throw new InvalidOperationException();
            if (existingBook == null)
            {
                throw new KeyNotFoundException("Could not find by ID " + book.BookId);
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
            var existingEntity = context.BookTable.Find(book.BookId);
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
            return context.BookTable.Find(id) ?? throw new KeyNotFoundException("Could not find key with ID " + id);
        }
    }

    public Author InsertAuthor(Author author)
    {
        using (var context = new DbContext(_opts, ServiceLifetime.Scoped))
        {
            context.AuthorTable.Add(author);
            context.SaveChanges();
            return author;
        }
    }

    public List<Author> GetAuthors()
    {
        using (var context = new DbContext(_opts, ServiceLifetime.Scoped))
        {
            return context.AuthorTable.Include(a => a.Books).ToList();
        }
    }


    public Category InsertCategory(Category category)
    {
        using (var context = new DbContext(_opts, ServiceLifetime.Scoped))
        {
            context.CategoryTable.Add(category);
            context.SaveChanges();
            return category;
        }
    }

    public Category FindCategory(int categoryId)
    {
        using (var context = new DbContext(_opts, ServiceLifetime.Scoped))
        {
            return context.CategoryTable.Find(categoryId) ?? throw new InvalidOperationException();
        }
    }

    public IEnumerable<Category> GetAllCategories()
    {
        using (var context = new DbContext(_opts, ServiceLifetime.Scoped))
        {
            return context.CategoryTable.ToList();
        }
    }

    public BookCategory AddBookToCategory(BookCategory bookCategory)
    {
        using (var context = new DbContext(_opts, ServiceLifetime.Scoped))
        {
            context.BookCategoryJoinTable.Add(bookCategory);
            context.SaveChanges();
            return bookCategory;
        }
    }

    public List<Book> BooksAsSQL()
    {
        using (var context = new DbContext(_opts, ServiceLifetime.Scoped))
        {
            return context.BookTable.FromSqlRaw("SELECT * FROM TABLE BOOKTABLE; )").ToList();
        }
    }

    public void InsertIntoBookTable()
    {
        using (var context = new DbContext(_opts, ServiceLifetime.Scoped))
        {
            context.Database.ExecuteSqlRaw("INSERT INTO BookTable (Title, AuthorId) VALUES ('Book', 'Author');");
        }
    }
}
