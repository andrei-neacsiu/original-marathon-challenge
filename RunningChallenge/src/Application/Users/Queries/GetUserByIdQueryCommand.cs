﻿using Domain.Models;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Users.Queries
{
    public class GetUserByIdQueryCommand : IRequest<User>
    {
        public int Id { get; set; }
    }
}
