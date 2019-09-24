using Flights.Data;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Flights
{
    public static class GlobalResources
    {
        public const string ROLE_ADMIN = "Admin";
        public const string ROLE_CLIENT = "Client";

        /// <summary>
        /// Compte le nombre d'utilisateurs dans un rôle
        /// </summary>
        /// <param name="roleName"></param>
        /// <returns></returns>
        public static long CountInRole(ApplicationDbContext _context, string roleName)
        {
            IdentityRole myRole = _context.Roles.First(r => r.Name == roleName);
            long count = _context.Set<IdentityUserRole<string>>().LongCount(r => r.RoleId == myRole.Id);
            return count;
        }
    }
}
