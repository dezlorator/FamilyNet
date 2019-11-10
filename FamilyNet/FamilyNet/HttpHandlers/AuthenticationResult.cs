using DataTransferObjects;

namespace FamilyNet.HttpHandlers
{
    public class AuthenticationResult
    {
        public bool Success { get; set; }
        public TokenDTO Token { get; set; }
    }
}
