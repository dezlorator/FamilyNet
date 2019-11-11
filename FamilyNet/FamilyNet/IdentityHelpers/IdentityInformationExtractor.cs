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
            var orphan = "Orphan";
            var representative = "Representative";
            var volunteer = "Volunteer";
            var charityMaker = "CharityMaker";
            var role = roles != null && roles.Contains(admin) ? admin : String.Empty;

            if(role == "")
            {
                role = roles != null && roles.Contains(orphan) ? orphan : String.Empty;
            }
            if (role == "")
            {
                role = roles != null && roles.Contains(representative) ? representative : String.Empty;
            }
            if (role == "")
            {
                role = roles != null && roles.Contains(volunteer) ? volunteer : String.Empty;
            }
            if (role == "")
            {
                role = roles != null && roles.Contains(charityMaker) ? charityMaker : String.Empty;
            }
            viewData["role"] = role;

            var email = session.GetString("email");
            viewData["email"] = String.IsNullOrEmpty(email) ? String.Empty : email;
        }
    }
}