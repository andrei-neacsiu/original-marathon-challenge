﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Abstract
{
    public interface IUnitOfWork : IDisposable
    {
        IUserRepository UserRepository { get; }
        IActivityRepository ActivityRepository { get; }
        IMarathonRepository MarathonRepository { get; }
        Task Save();
    }
}
