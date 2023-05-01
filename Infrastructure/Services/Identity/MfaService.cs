using Application.Services.Identity;
using OtpNet;

namespace Infrastructure.Services.Identity;

public class MfaService : IMfaService
{
    private Totp? _totpProvider = null;

    public byte[] GenerateKeyBytes(int keyLength = 20)
    {
        return KeyGeneration.GenerateRandomKey(keyLength);
    }

    public string GenerateKeyString(int keyLength = 20)
    {
        return Base32Encoding.ToString(GenerateKeyBytes(keyLength));
    }

    public bool IsPasscodeCorrect(string passcode, string? totpKey = null)
    {
        totpKey ??= GenerateKeyString();
        _totpProvider ??= new Totp(Base32Encoding.ToBytes(totpKey), mode: OtpHashMode.Sha512, totpSize: 6);
        
        // TODO: Add configuration for verification window
        return _totpProvider.VerifyTotp(passcode, out var timeMatched, new VerificationWindow(1, 1));
    }
}