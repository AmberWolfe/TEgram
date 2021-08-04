﻿using Microsoft.AspNetCore.Mvc;
using Capstone.Models;
using Capstone.Security;
using System.Linq.Expressions;
using System.Linq;
using System;
using System.Collections.Generic;

namespace Capstone.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly ITokenGenerator tokenGenerator;
        private readonly IPasswordHasher passwordHasher;
        private ApplicationDbContext _context;

        public LoginController(ITokenGenerator _tokenGenerator, IPasswordHasher _passwordHasher, ApplicationDbContext context)
        {
            tokenGenerator = _tokenGenerator;
            passwordHasher = _passwordHasher;
            _context = context;
        }

        [HttpPost]
        public IActionResult Authenticate(LoginUser userParam)
        {
            // Default to bad username/password message
            IActionResult result = Unauthorized(new { message = "Username or password is incorrect" });

            // Get the user by username  -- ApplicationDbContext --> User objects --> list of attrbutes
            //foreach loop to look for username in each user object, return user if userparam.username exists
            User user = null;
   
            foreach (User u in _context.Users)
            {
                if(u.Username == userParam.Username)
                {
                    user = u;
                }

            }


            // If we found a user and the password hash matches
            if (user != null && passwordHasher.VerifyHashMatch(user.PasswordHash, userParam.Password, user.Salt))
            {
                // Create an authentication token
                string token = tokenGenerator.GenerateToken(user.UserId, user.Username, user.Role);
                PageData packagedUser = PackageUser(user.UserId, p => p.User.UserId == user.UserId);

                // Create a ReturnUser object to return to the client
                LoginResponse retUser = new LoginResponse() { User = packagedUser, Token = token };

                // Switch to 200 OK
                result = Ok(retUser);
            }

            return result;
        }

        [HttpPost("/register")]
        public IActionResult Register(RegisterUser userParam)
        {
            IActionResult result;

            User existingUser = null;

            foreach (User u in _context.Users)
            {
                if (u.Username == userParam.Username)
                {
                    existingUser = u;
                }

            }
          
            if (existingUser != null)
            {
                return Conflict(new { message = "Username already taken. Please choose a different username." });
            }

            User user = new User
            {
                Username = userParam.Username,
                PasswordHash = userParam.Password,
                Role = userParam.Role
            };

            var retUser = _context.Users.Add(user);
            if (retUser != null)
            {
                result = Created(user.Username, null); //values aren't read on client
            }
            else
            {
                result = BadRequest(new { message = "An error occurred and user was not created." });
            }

            return result;
        }

        private PageData PackageUser(int id, Expression<Func<Photo, bool>> predicate)
        {
            User user = _context.Users.First(u => u.UserId == id);
            PageData data = new PageData();
            data.UserProfileUrl = user.ProfileUrl;
            data.UserId = user.UserId;
            data.Username = user.Username;
            data.Firstname = user.FirstName;
            data.Lastname = user.LastName;

            List<Photo> photos = _context.Photos.Where(predicate).OrderByDescending(p => p.CreatedDate).ToList();
            foreach (var photo in photos)
            {
                data.Photos.Add(new PhotoData
                {
                    Url = photo.Url,
                    UserId = photo.UserId,
                    Comments = null,
                    Likes = null
                });

            }
            return data;
        }

    }
}
