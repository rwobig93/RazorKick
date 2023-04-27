using Application.Services.Identity;
using OtpNet;

namespace Infrastructure.Services.Identity;

public class MfaService : IMfaService
{
    private Totp _totpProvider;
    private readonly byte[] _totpKey = Base32Encoding.ToBytes(Base32Encoding.ToString(KeyGeneration.GenerateRandomKey(20)));

    public MfaService(Totp totpProvider)
    {
        _totpProvider = new Totp(_totpKey, mode: OtpHashMode.Sha512, totpSize: 6);
    }
}