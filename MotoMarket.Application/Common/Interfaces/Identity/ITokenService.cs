using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MotoMarket.Domain.Entities;

namespace MotoMarket.Application.Common.Interfaces.Identity
{
    public interface ITokenService
    {
        Task<string> CreateToken(Domain.Entities.ApplicationUser user);
    }
}
