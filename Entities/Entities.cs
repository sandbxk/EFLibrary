﻿namespace Entities;

public class Book
{
    public int BookId { get; set; }
    public string Title { get; set; }
    public Author Author { get; set; }
    
    public List<BookCategory> BookCategories { get; set; }
    public List<Category> Categories { get; set; }
    public List<BorrowedBooks>? Borrowers { get; set; }
    
    public int AuthorId { get; set; }
}

//One book can have many categories and
//one category can contain many books
public class Category
{
    public int CategoryId { get; set; }
    
    public string CategoryName { get; set; }
    public List<BookCategory> BookCategories { get; set; }
    public List<Book> Books { get; set; }

}

//The join table for the many-to-many join
public class BookCategory
{
    public int BookId { get; set; }
    public Book Book { get; set; }
    public int CategoryId { get; set; }
    public Category Category { get; set; }
}

public class Author
{
    public int AuthorId { get; set; }
    
    public string Name { get; set; }
    public List<Book> Books { get; set; }
    
}

public class Person
{
    public int? Id { get; set; }
    public string? Name { get; set; }
    public List<BorrowedBooks>? Books { get; set; }

    public override string ToString()
    {
        return "[+" + Id + "] " + Name;
    }
}

public class BorrowedBooks
{
    public int BookID { get; set; }
    public Book Book { get; set; }
    public int PersonID { get; set; }
    public Person Person { get; set; }
    
    
}
