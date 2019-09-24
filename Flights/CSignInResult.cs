using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Flights
{
    public class CSignInResult : SignInResult
    {
        /// <summary>
        /// Constructeur par défaut
        /// </summary>
        public CSignInResult()
        {

        }

        /// <summary>
        /// Constructeur pour caster une instance de <see cref="SignInResult"/> en <see cref="CSignInResult"/>
        /// </summary>
        /// <param name="signInResult"></param>
        public CSignInResult(SignInResult signInResult)
        {
            this.IsLockedOut = signInResult.IsLockedOut;
            this.IsNotAllowed = signInResult.IsNotAllowed;
            this.RequiresTwoFactor = signInResult.RequiresTwoFactor;
            this.Succeeded = signInResult.Succeeded;
        }

        public static CSignInResult Disabled { get { return new CSignInResult { IsDisabled = true }; } }
        public static CSignInResult Unexisting { get { return new CSignInResult { DoesNotExists = true }; } }
        public bool IsDisabled { get; protected set; }
        public bool DoesNotExists { get; protected set; }
    }
}
