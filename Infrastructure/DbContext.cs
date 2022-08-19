using Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace infrastructure;

public class DbContext : Microsoft.EntityFrameworkCore.DbContext
{
    public DbContext(DbContextOptions<DbContext> options, ServiceLifetime serviceLifetime) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        //Auto generate ID
        modelBuilder.Entity<Book>()
            .Property(f => f.BookId)
            .ValueGeneratedOnAdd();
        modelBuilder.Entity<Author>()
            .Property(f => f.AuthorId)
            .ValueGeneratedOnAdd();
        modelBuilder.Entity<Category>()
            .Property(f => f.CategoryId)
            .ValueGeneratedOnAdd();
        
        modelBuilder.Entity<Category>()
            .HasKey(f => f.CategoryId);
        
        modelBuilder.Entity<BookCategory>()
            .HasKey(a => new { a.CategoryId, a.BookId });

        modelBuilder.Entity<Book>()
            .HasIndex(b => b.Title);

        modelBuilder.Entity<Book>()
            .Property(b => b.Title)
            .HasColumnType("TEXT")
            .IsUnicode(true)
            .IsRequired(true)
            .HasMaxLength(20);


        //Foregin key to author ID
        modelBuilder.Entity<Book>()
            .HasOne(book => book.Author)
            .WithMany(author => author.Books)
            .HasForeignKey(book => book.AuthorId)
            .OnDelete(DeleteBehavior.Cascade);

        //Many-to-many relation
        modelBuilder.Entity<Book>()
            .HasMany(a => a.BookCategories)
            .WithOne(b => b.Book)
            .OnDelete(DeleteBehavior.Cascade);
        modelBuilder.Entity<Category>()
            .HasMany(a => a.BookCategories)
            .WithOne(b => b.Category)
            .OnDelete(DeleteBehavior.Cascade);
        modelBuilder.Entity<BookCategory>()
            .HasOne(bc => bc.Book)
            .WithMany(b => b.BookCategories);
        modelBuilder.Entity<BookCategory>()
            .HasOne(c => c.Category)
            .WithMany(c => c.BookCategories);

        //Don't auto include author on query
        modelBuilder.Entity<Book>()
            .Ignore(b => b.Author);
        modelBuilder.Entity<BookCategory>()
            .Ignore(bc => bc.Category);
        modelBuilder.Entity<BookCategory>()
            .Ignore(bc => bc.Book);

        modelBuilder.Entity<Person>()
            .HasKey(person => person.Id);
        modelBuilder.Entity<Person>();
        
        modelBuilder.Entity<BorrowedBooks>()
            .HasOne(p => p.Person)
            .WithMany(b => b.Books);
        modelBuilder.Entity<BorrowedBooks>()
            .HasOne(b => b.Book)
            .WithMany(bo => bo.Borrowers);
        
        
        modelBuilder.Entity<Person>()
            .Property(f => f.Id)
            .ValueGeneratedOnAdd();

        modelBuilder.Entity<Person>()
            .HasKey(f => f.Id);
        
        modelBuilder.Entity<BorrowedBooks>()
            .HasKey(a => new { a.BookID, a.PersonID });

        modelBuilder.Entity<Person>()
            .HasIndex(b => b.Name);

        modelBuilder.Entity<Person>()
            .Property(b => b.Name)
            .HasColumnType("TEXT")
            .IsUnicode(true)
            .IsRequired(true)
            .HasMaxLength(20);

        modelBuilder.Entity<BorrowedBooks>()
            .Ignore(bb => bb.Book);
        modelBuilder.Entity<BorrowedBooks>()
            .Ignore(bb => bb.Person);

        

    }

    //Mapping to entity classes
    public DbSet<Author> AuthorTable { get; set; }
    public DbSet<Book> BookTable { get; set; }
    public DbSet<Category> CategoryTable { get; set; }
    public DbSet<BookCategory> BookCategoryJoinTable { get; set; }
    
    public DbSet<BorrowedBooks> BorrowedBooksTable { get; set; }
    
    public DbSet<Person> PersonTable { get; set; }

}