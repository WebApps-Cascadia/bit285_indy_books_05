using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IndyBooks.Models;
using IndyBooks.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IndyBooks.Controllers
{
    public class AdminController : Controller
    {
        private IndyBooksDbContext _db;
        public AdminController(IndyBooksDbContext db) { _db = db; }

        /***
         * READ       
         */
        [HttpGet]
        public IActionResult Index(long id)
        {
            //TODO: filter books by the id (if passed an id as its Route Parameter),
            //     otherwise use the entire collection of Books, ordered by SKU.
            IEnumerable<Book> books = (id > 0) ? _db.Books.Include(b=>b.Author).Where(b => b.Id == id) :
                                                 _db.Books.Include(b=>b.Author);

            return View("SearchResults", books);
        }
        /***
         * CREATE
         */
        [HttpGet]
        public IActionResult CreateBook()
        {
            //TODO: Build a new CreateBookViewModel with a complete set of Writers from the database
            CreateBookViewModel bookVM = new CreateBookViewModel
            {
                Writers = _db.Writers
            };

            return View(bookVM); //TODO: pass the ViewModel onto the CreateBook View
        }
        [HttpPost]
        public IActionResult CreateBook(CreateBookViewModel bookVM, long id)
        {
            Writer writer;
            //TODO: Build the Writer object using the view Model info
            writer = (bookVM.Name != null) ? new Writer { Name = bookVM.Name } :
                                              _db.Writers.Find(bookVM.AuthorId);


            //TODO: Build the Book using the parameter data and your newly created author.
            Book book = new Book
            {
                Title = bookVM.Title,
                SKU = bookVM.SKU,
                Price = bookVM.Price,
                Author = writer,
                Id = id
            };


            //TODO: Add author and book to their DbSets; SaveChanges
            if (bookVM.Name!= null) _db.Add(writer);
            if (id == 0) _db.Add(book);
            else _db.Update(book);
            _db.SaveChanges();

            //TODO: Show the book by passing the Book's id (rather than 1) to the Index Action 
            return RedirectToAction("Index", new { id = book.Id });
        }
        /***
         * UPDATE (reusing the CreateBook View ) 
         */
         //TODO: Write a method to take a book id, and load book and author info
         //      into the ViewModel for the CreateBook View
         [HttpGet]
         public IActionResult UpdateBook(long id)
        {
            var book = _db.Books.Include(b => b.Author).Single(b => b.Id == id);
            var bookVM = new CreateBookViewModel
            {
                BookId = book.Id,
                AuthorId = book.Author.Id,
                Price = book.Price,
                SKU = book.SKU,
                Title = book.Title,
                Writers = _db.Writers
            };
            return View("CreateBook", bookVM);
            
        }
       

        /***
         * DELETE
         */
        [HttpGet]
        public IActionResult DeleteBook(long id)
        {
            //TODO: Remove the Book associated with the given id number; Save Changes
            _db.Remove(new Book { Id = id });
            _db.SaveChanges();


            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Search() { return View(); }
        [HttpPost]
        public IActionResult Search(SearchViewModel search)
        {
            //Full Collection Search
            IQueryable<Book> foundBooks = _db.Books.Include(b=>b.Author); // start with entire collection

            //Partial Title Search
            if (search.Title != null)
            {
                foundBooks = foundBooks
                            .Where(b => b.Title.Contains(search.Title))
                            .OrderBy(b => b.Author.Name)
                            ;
            }

            //Author's Last Name Search
            if (search.AuthorName != null)
            {
                //Use the Name property of the Book's Author entity
                foundBooks = foundBooks
                            .Where(b => b.Author.Name.EndsWith(search.AuthorName))
                            ;
            }
            //Priced Between Search (min and max price entered)
            if (search.MinPrice > 0 && search.MaxPrice > 0)
            {
                foundBooks = foundBooks
                            .Where(b => b.Price >= search.MinPrice && b.Price <= search.MaxPrice)
                            ;
            }
            //Composite Search Results
            return View("SearchResults", foundBooks);
        }

    }
}
