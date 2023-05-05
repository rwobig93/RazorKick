using System.Drawing;
using Application.Services.Identity;
using QRCoder;

namespace Infrastructure.Services.Identity;

public class QrCodeService : IQrCodeService
{
    private readonly QRCodeGenerator _codeGenerator;

    public QrCodeService()
    {
        _codeGenerator = new QRCodeGenerator();
    }

    public string GenerateQrCodeSrc(string textToEncode)
    {
        var qrGenerator = new QRCodeGenerator();
        var qrCodeData = qrGenerator.CreateQrCode(textToEncode, QRCodeGenerator.ECCLevel.Q);
        var qrCode = new BitmapByteQRCode(qrCodeData);
        var qrCodeImage = qrCode.GetGraphic(20);

        return "data:image/png;base64, " + Convert.ToBase64String(qrCodeImage);
        // //Set color by using Color-class types
        // Bitmap qrCodeImage = qrCode.GetGraphic(20, Color.DarkRed, Color.PaleGreen, true);
        //
        // //Set color by using HTML hex color notation
        // Bitmap qrCodeImage = qrCode.GetGraphic(20, "#000ff0", "#0ff000");

        // // Add logo to the center of the QR Code
        // Bitmap qrCodeImage = qrCode.GetGraphic(20, Color.Black, Color.White, (Bitmap)Bitmap.FromFile("C:\\myimage.png"));

        // var encodedData = _codeGenerator.CreateQrCode(textToEncode, QRCodeGenerator.ECCLevel.Q);

    }
}