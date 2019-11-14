using DataTransferObjects;
using DataTransferObjects.Enums;
using FamilyNet.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System;

namespace FamilyNet.IdentityHelpers
{
    public class IdentityInformationExtractor : IIdentityInformationExtractor
    {
        public void GetUserInformation(ISession session, ViewDataDictionary viewData)
        {
            var email = session.GetString(nameof(IdentitySessionKyes.email));
            viewData[nameof(IdentitySessionKyes.email)] =
                String.IsNullOrEmpty(email) ? String.Empty : email;

            var roles = session.GetString(nameof(IdentitySessionKyes.roles));

            if (!String.IsNullOrEmpty(roles))
            {
                var role = roles.Contains(nameof(UserRole.Admin)) ?
                    nameof(UserRole.Admin) : String.Empty;

                if (String.IsNullOrEmpty(role))
                {
                    var allRoles = roles.Split(",");

                    if (allRoles.Length >= 1)
                    {
                        role = allRoles[0];
                    }
                }

                viewData[nameof(IdentitySessionKyes.roles)] = role;
            }
        }

        public void SetUserInformation(ISession session, TokenDTO token)
        {
            session.SetString(nameof(IdentitySessionKyes.id), token.Id);
            session.SetString(nameof(IdentitySessionKyes.email), token.Email);
            session.SetString(nameof(IdentitySessionKyes.roles),
                String.Join(",", token.Roles));

            if (token.PersonId != null)
            {
                session.SetString(nameof(IdentitySessionKyes.personId),
                    token.PersonId.ToString());
            }

            session.SetString(nameof(IdentitySessionKyes.Bearer), token.Token);
        }
    }
}