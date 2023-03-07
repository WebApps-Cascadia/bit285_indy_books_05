﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using IndyBooks.Models;
using Microsoft.EntityFrameworkCore;

namespace IndyBooks.Controllers
{
    [Route("api/writerapi")] //The base route for all calls to this controller
    public class WriterAPIController : Controller
    {
        private IndyBooksDbContext _db;
        public WriterAPIController(IndyBooksDbContext db) { _db = db; }

        /**
         * BOOK COUNT: returns the count of all the books by a single Author
         */
        //TODO: Write the [HttpGet] annotation with the API route for this call
        [HttpGet("{id}/bookcount")]
        public IActionResult GetCount(long id)
        {
            //TODO: return BadRequest if the id value is not greater than zero
            if(!(id > 0))
            {
                return BadRequest();
            }
            
            //TODO: return NotFound if their are no writers in the db with the id
            if(!_db.Writers.Any(w => w.Id == id))
            {
                return NotFound();
            }
            Writer writer = _db.Writers.Include(w => w.Books).Single(w => w.Id == id);

            //var writers = _db.Writers.Include(w => w.Books).Where(w => w.Id == id);
            //TODO: return OK with the AJAX data as a new object, e.g.,{ id=5, count=6 } for the given writer
            //return Ok(writers.Select(w => new { id = w.Id, count = w.Books.Count }));
            return Ok(new { id = id, count = writer.Books.Count });
        }

        /**
         * READ ALL: Retrieves a collection of writers
         * uses a GET verb with the URL pattern "api/writers"
         */
        [HttpGet]
        public IActionResult Get()
        {
            return Ok(_db.Writers);
        }
        /**
         * DELETE: Removes a particular writer with the given {id}
         * uses a DELETE verb with the URL pattern "api/writers/45"
         */
        [HttpDelete("{id}")]
        public ActionResult Delete(long id)
        {
            //Searchs for record using Any(); if missing -> return NotFound();
            if (!_db.Writers.Any(w => w.Id == id)) { return NotFound(); }
            //Removes the writer with the given id
            _db.Remove(new Writer { Id = id });
            _db.SaveChanges();
            return Accepted();
        }
        /**
         * CREATE: Add a new writer to the collection
         * uses a POST verb with the URL pattern "api/writers"
         */
        [HttpPost]
        public IActionResult Post([FromBody] Writer writer)
        {
         //Tests for an invalid ModelState -> return BadRequest(); 
            if (!ModelState.IsValid) { return BadRequest(); }
         //Makes changes to DbContext, save to Database -> return Accepted(writer);
            _db.Add(writer);
            _db.SaveChanges();
            return Accepted(writer);

        }/**
         * READ ONE: Retrieves a particular writer with the given {id}
         * uses a GET verb with the URL pattern "api/writers/41"
         */
        [HttpGet("{id}")]
        public IActionResult Get(long id) 
        {
         //Tests for missing record using Any() -> return NotFound();
            if (!_db.Writers.Any(w => w.Id == id)) { return NotFound(); }
         //Returns the writer matching the id with all their books
            var writer = _db.Writers
                .Include(w => w.Books)
                .SingleOrDefault(w => w.Id == id);
            return Ok(writer);
        }
        /**
         * UPDATE: Modify a particular writer with the given {id}
         * uses a PUT verb with the URL pattern "api/writers/16"
         */
        [HttpPut("{id}")]
        public ActionResult Put(long id, [FromBody]Writer writer)
        {
            //Test for invalid Model
            if (!ModelState.IsValid) { return BadRequest(); }

            //Tests for missing record (bad ID value)
            if(!_db.Writers.Any(w => w.Id == id)) {return NotFound();}
            //Makes changes to DbContext, save to Database -> return Accepted(writer);
            writer.Id = id;
            _db.Update(writer);
            _db.SaveChanges();
            return Accepted(writer);
        }
       
    }
}

