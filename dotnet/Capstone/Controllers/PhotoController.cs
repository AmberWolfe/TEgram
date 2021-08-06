﻿using Capstone.ApiResponseObjects;
using Capstone.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Capstone.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PhotoController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly PackagingHelper packagingHelper;
        public PhotoController(ApplicationDbContext context)
        {
            _context = context;
            packagingHelper = new PackagingHelper(context);
        }

        // GET: api/<ValuesController>
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<ValuesController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // GET api/feed
        [AllowAnonymous]
        [HttpGet("feed")]
        public ActionResult<List<PhotoDataResponse>> GetFeed()
        {
            return packagingHelper.PackagePhotos(p => p.UserId > 0);
        }

        // POST api/<ValuesController>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<ValuesController>/5
        //[HttpPut("like/{id}")]
        [HttpPut("{id}")] // Pass the fact we want to change like in the query string
        public bool Put(int id, string action)
        {
            switch (action)
            {
                case "like":
                    return ToggleLike(id);
                case "favorite":
                    return ToggleFavorite(id);
                default:
                    break;
            }
            return false;
        }

        //TODO Feed route /api/photo/feed
        // /api/feed

        // DELETE api/<ValuesController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }

        private bool ToggleLike(int photoId)
        {
            string userIdStr = HttpContext.User?.FindFirstValue("sub")?.ToString() ?? "-1";
            int userId = int.Parse(userIdStr);

            var photo = _context.Photos.Include(p => p.PhotoLikes).FirstOrDefault(p => p.PhotoId == photoId);

            if (photo.PhotoLikes.FirstOrDefault(p => p.UserId == userId) != null)
            {
                photo.PhotoLikes.Remove(_context.Users.Find(userId));
                _context.SaveChanges();
                return false;
            }
            else
            {
                photo.PhotoLikes.Add(_context.Users.Find(userId));
                _context.SaveChanges();
                return true;
            }
        }
        private bool ToggleFavorite(int photoId)
        {
            string userIdStr = HttpContext.User?.FindFirstValue("sub")?.ToString() ?? "-1";
            int userId = int.Parse(userIdStr);

            var photo = _context.Photos.Include(p => p.PhotoFavorites).FirstOrDefault(p => p.PhotoId == photoId);

            if (photo.PhotoFavorites.FirstOrDefault(p => p.UserId == userId) != null)
            {
                photo.PhotoFavorites.Remove(_context.Users.Find(userId));
                _context.SaveChanges();
                return false;
            }
            else
            {
                photo.PhotoFavorites.Add(_context.Users.Find(userId));
                _context.SaveChanges();
                return true;
            }
        }

    }
}
