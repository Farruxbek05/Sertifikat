using Microsoft.AspNetCore.Mvc;
using SkiaSharp;
using QRCoder;

[Route("sertificat/[controller]")]
[ApiController]
public class CertificateController : ControllerBase
{
    [HttpGet("GenerateCertificate")]
        public IActionResult GenerateCertificate(string firstName, string lastName, string groupName)
        {
            
            string qrCodeText = $"https://yourdomain.com/certificates/{Guid.NewGuid()}";

            
            var certificateImage = GenerateCertificateImage(firstName, lastName, groupName, qrCodeText);

            
            return File(certificateImage, "image/png");
        }

        private byte[] GenerateCertificateImage(string firstName, string lastName, string groupName, string qrCodeText)
        {
            int width = 800, height = 600;

            using var bitmap = new SKBitmap(width, height);
            using var canvas = new SKCanvas(bitmap);

            
            var backgroundPaint = new SKPaint
            {
                Shader = SKShader.CreateLinearGradient(
                    new SKPoint(0, 0),
                    new SKPoint(width, height),
                    new SKColor[] { SKColors.LightBlue, SKColors.White },
                    new float[] { 0, 1 },
                    SKShaderTileMode.Clamp)
            };
            canvas.DrawRect(0, 0, width, height, backgroundPaint);

            
            var borderPaint = new SKPaint
            {
                Style = SKPaintStyle.Stroke,
                Color = SKColors.DarkBlue,
                StrokeWidth = 8
            };
            canvas.DrawRect(20, 20, width - 40, height - 40, borderPaint);

            
            var titlePaint = new SKPaint
            {
                Color = SKColors.Black,
                TextSize = 40,
                IsAntialias = true,
                TextAlign = SKTextAlign.Center
            };

            var bodyPaint = new SKPaint
            {
                Color = SKColors.Black,
                TextSize = 28,
                IsAntialias = true,
                TextAlign = SKTextAlign.Center
            };

            var footerPaint = new SKPaint
            {
                Color = SKColors.Black,
                TextSize = 20,
                IsAntialias = true,
                TextAlign = SKTextAlign.Left
            };

            
            canvas.DrawText("Certificate of Farruxbekdan", width / 2, 100, titlePaint);
            canvas.DrawText($"This is sertifikan  task uchun", width / 2, 180, bodyPaint);
            canvas.DrawText($"{firstName} {lastName}", width / 2, 220, titlePaint);
            canvas.DrawText($"From Group: {groupName}", width / 2, 260, bodyPaint);

            
            var qrCode = GenerateQrCodeImage(qrCodeText);
            using var qrBitmap = SKImage.FromEncodedData(qrCode); 
            var qrRect = new SKRect(width - 150, height - 150, width - 30, height - 30);
            canvas.DrawImage(qrBitmap, qrRect); 

            
            string currentDate = DateTime.Now.ToString("yyyy-MM-dd");
            canvas.DrawText($"Date: {currentDate}", 100, height - 100, footerPaint);

           
            using var image = SKImage.FromBitmap(bitmap);
            using var data = image.Encode(SKEncodedImageFormat.Png, 100);
            return data.ToArray(); 
        }

        private byte[] GenerateQrCodeImage(string text)
        {
         
            using var qrGenerator = new QRCodeGenerator();
            var qrCodeData = qrGenerator.CreateQrCode(text, QRCodeGenerator.ECCLevel.Q);
            var qrCode = new PngByteQRCode(qrCodeData);
            return qrCode.GetGraphic(20); 
        }
    }

