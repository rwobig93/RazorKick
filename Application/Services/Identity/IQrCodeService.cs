namespace Application.Services.Identity;

public interface IQrCodeService
{
    public string GenerateQrCodeSrc(string textToEncode);
}