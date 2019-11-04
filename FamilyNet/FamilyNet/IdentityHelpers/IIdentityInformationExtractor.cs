using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace FamilyNet.IdentityHelpers
{
    public interface IIdentityInformationExtractor
    {
        void GetUserInformation(ISession session, ViewDataDictionary viewData);
    }
}
