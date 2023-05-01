using Application.Services.Identity;
using OtpNet;

namespace Infrastructure.Services.Identity;

public class MfaService : IMfaService
{
    private readonly Totp _totpProvider;

    public MfaService(Totp totpProvider, byte[]? totpKey = null)
    {
        totpKey ??= KeyGeneration.GenerateRandomKey(20);

        _totpProvider = new Totp(totpKey, mode: OtpHashMode.Sha512, totpSize: 6);
    }

    public byte[] GenerateKeyBytes(int keyLength = 20)
    {
        return KeyGeneration.GenerateRandomKey(keyLength);
    }

    public string GenerateKeyString(int keyLength = 20)
    {
        return Base32Encoding.ToString(GenerateKeyBytes(keyLength));
    }

    public bool IsPasscodeCorrect(string passcode)
    {
        // TODO: Add configuration for verification window
        return _totpProvider.VerifyTotp(passcode, out var timeMatched, new VerificationWindow(1, 1));
    }
}