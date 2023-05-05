using Application.Helpers.Runtime;
using Application.Services.Identity;
using OtpNet;

namespace Infrastructure.Services.Identity;

public class MfaService : IMfaService
{
    private Totp? _totpProvider;

    public byte[] GenerateKeyBytes(int keyLength = 20)
    {
        return KeyGeneration.GenerateRandomKey(keyLength);
    }

    public string GenerateKeyString(int keyLength = 20)
    {
        return Base32Encoding.ToString(GenerateKeyBytes(keyLength));
    }

    public bool IsPasscodeCorrect(string passcode, string totpKey, out long timeStampMatched)
    {
        _totpProvider ??= new Totp(Base32Encoding.ToBytes(totpKey), step: 30, mode: OtpHashMode.Sha1, totpSize: 6);
        
        // TODO: Add configuration for verification window and maybe TOTP size / step count
        return _totpProvider.VerifyTotp(passcode, out timeStampMatched, VerificationWindow.RfcSpecifiedNetworkDelay);
    }
}