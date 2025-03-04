﻿using Domain.Models;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Activities.Queries
{
    public class GetAllUserActivitiesQuery : IRequest<List<Activity>>
    {
        public int UserId { get; set; }
    }
}
