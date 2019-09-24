using Flights.Data.Entities;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Flights
{
    public class CSignInManager<TUser> : SignInManager<TUser> where TUser : Person
    {
        /// <summary>
        /// Constructeur par défaut d'une instance <see cref="Flights.CSignInManager{TUser}{TUser}"/>
        /// </summary>
        /// <param name="userManager"></param>
        /// <param name="contextAccessor"></param>
        /// <param name="claimsFactory"></param>
        /// <param name="optionsAccessor"></param>
        /// <param name="logger"></param>
        /// <param name="schemes"></param>
        public CSignInManager(UserManager<TUser> userManager, IHttpContextAccessor contextAccessor, IUserClaimsPrincipalFactory<TUser> claimsFactory, IOptions<IdentityOptions> optionsAccessor, ILogger<SignInManager<TUser>> logger, IAuthenticationSchemeProvider schemes) : base(userManager, contextAccessor, claimsFactory, optionsAccessor, logger, schemes)
        {
        }

        /// <summary>
        /// Retourne une indication permettant de savoir si l'utilisateur peut se connecter
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public override async Task<bool> CanSignInAsync(TUser user)
        {
            if (user.Status == EntitiesEnums.EStatus.ARCHIVED)
                return false;

            return await base.CanSignInAsync(user);
        }

        /// <summary>
        /// Connexion d'un utilisateur
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <param name="isPersistent"></param>
        /// <param name="lockoutOnFailure"></param>
        /// <returns></returns>
        public new async Task<CSignInResult> PasswordSignInAsync(string userName, string password, bool isPersistent, bool lockoutOnFailure)
        {
            var user = await UserManager.FindByNameAsync(userName);

            if (user != null)
            {
                if (user.Status == EntitiesEnums.EStatus.ACTIVE)
                    return new CSignInResult(await base.PasswordSignInAsync(user, password, isPersistent, lockoutOnFailure));
                else
                    return CSignInResult.Disabled;
            }
            else
            {
                return CSignInResult.Unexisting;
            }
        }

        /// <summary>
        /// Connexion d'un utilisateur
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <param name="isPersistent"></param>
        /// <param name="lockoutOnFailure"></param>
        /// <returns></returns>
        public new async Task<CSignInResult> PasswordSignInAsync(TUser user, string password, bool isPersistent, bool lockoutOnFailure)
        {
            if (user != null)
            {
                if (user.Status == EntitiesEnums.EStatus.ACTIVE)
                    return new CSignInResult(await base.PasswordSignInAsync(user, password, isPersistent, lockoutOnFailure));
                else
                    return CSignInResult.Disabled;
            }
            else
            {
                return CSignInResult.Unexisting;
            }
        }
    }
}
