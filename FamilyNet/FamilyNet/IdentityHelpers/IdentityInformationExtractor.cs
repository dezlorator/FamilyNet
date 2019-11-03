using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System;

namespace FamilyNet.IdentityHelpers
{
    public class IdentityInformationExtractor : IIdentityInformationExtractor
    {
        public void GetUserInformation(ISession session, ViewDataDictionary viewData)
        {
            var roles = session.GetString("roles");
            var admin = "Admin";
            var role = roles != null ? roles.Contains(admin) ? admin : String.Empty : String.Empty;
            viewData["role"] = role;

            var email = session.GetString("email");
            viewData["email"] = String.IsNullOrEmpty(email) ? String.Empty : email;
        }
    }
}