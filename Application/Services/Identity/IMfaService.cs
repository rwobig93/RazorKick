namespace Application.Services.Identity;

public interface IMfaService
{
    public byte[] GenerateKeyBytes(int keyLength = 20);
    public string GenerateKeyString(int keyLength = 20);
    public bool IsPasscodeCorrect(string passcode);
}