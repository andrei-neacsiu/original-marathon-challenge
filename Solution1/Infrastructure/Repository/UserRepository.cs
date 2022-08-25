﻿using Application.Abstract;
using Domain.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly DataContext _context;

        public UserRepository(DataContext context)
        {
            _context = context;
        }

        public async Task CreateUser(User user)
        {
            await _context.Users.AddAsync(user);
        }

        public async Task<User> GetUser(int userId)
        {
            var user = await _context.Users.FindAsync(userId);

            return user;
        }

        public async Task<Dictionary<string, string>> GetUserActivityInfo(int Id)
        {
            throw new NotImplementedException();
        }

        public async Task<User> GetUserByLogin(string userName, string password)
        {
            var user = await _context.Users
                .SingleOrDefaultAsync(u => u.UserName == userName && u.Password == password);

            return user;
        }

        public void UpdateUserActivity(int id, string field, string value)
        {
            throw new NotImplementedException();
        }
    }
}
