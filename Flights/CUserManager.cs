using Flights.Data.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Flights
{
    public class CUserManager<TUser> : UserManager<TUser> where TUser : Person
    {
        public static readonly string ADMINISTRATOR = "Administrator";

        /// <summary>
        /// Constructeur par défaut d'une instance <see cref="Flights.CUserManager{TUser}"/>
        /// </summary>
        /// <param name="store"></param>
        /// <param name="optionsAccessor"></param>
        /// <param name="passwordHasher"></param>
        /// <param name="userValidators"></param>
        /// <param name="passwordValidators"></param>
        /// <param name="keyNormalizer"></param>
        /// <param name="errors"></param>
        /// <param name="services"></param>
        /// <param name="logger"></param>
        public CUserManager(IUserStore<TUser> store, IOptions<IdentityOptions> optionsAccessor, IPasswordHasher<TUser> passwordHasher, IEnumerable<IUserValidator<TUser>> userValidators, IEnumerable<IPasswordValidator<TUser>> passwordValidators, ILookupNormalizer keyNormalizer, IdentityErrorDescriber errors, IServiceProvider services, ILogger<UserManager<TUser>> logger) : base(store, optionsAccessor, passwordHasher, userValidators, passwordValidators, keyNormalizer, errors, services, logger)
        {
        }

        /// <summary>
        /// Création d'un utilisateur
        /// </summary>
        /// <param name="user">Utilisateur</param>
        /// <returns></returns>
        public override Task<IdentityResult> CreateAsync(TUser user)
        {
            if (user.UserName != ADMINISTRATOR)
                user.UserName = user.Surname.ToLower() + "." + user.Name.ToLower();
            return base.CreateAsync(user);
        }

        /// <summary>
        /// Création d'un utilisateur
        /// </summary>
        /// <param name="user">Utilisateur</param>
        /// <param name="password">Mot de passe</param>
        /// <returns></returns>
        public override Task<IdentityResult> CreateAsync(TUser user, string password)
        {
            if (user.UserName != ADMINISTRATOR)
                user.UserName = user.Surname.ToLower() + "." + user.Name.ToLower();
            return base.CreateAsync(user, password);
        }

        /// <summary>
        /// Mise à jour d'un utilisateur
        /// </summary>
        /// <param name="user">Utilisateur</param>
        /// <returns></returns>
        public override Task<IdentityResult> UpdateAsync(TUser user)
        {
            if (user.UserName != ADMINISTRATOR)
                user.UserName = user.Surname.ToLower() + "." + user.Name.ToLower();
            return base.UpdateAsync(user);
        }

        /// <summary>
        /// Archivage d'un compte utilisateur
        /// </summary>
        /// <param name="user">Utilisateur</param>
        /// <returns></returns>
        public override Task<IdentityResult> DeleteAsync(TUser user)
        {
            user.Status = EntitiesEnums.EStatus.ARCHIVED;
            return base.UpdateAsync(user);
        }
    }
}
