namespace FamilyNet.Encoders
{
    public interface IJWTEncoder
    {
        TokenClaims GetTokenData(string token);
    }
}
