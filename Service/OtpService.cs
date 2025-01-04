using ecommerce_final.Entities;
using ecommerce_final.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Security.Cryptography;

namespace ecommerce_final.Service
{
    public class OtpService
    {
        private readonly EcommerceFinalContext _context;
        private readonly EmailService _emailService;

        public OtpService(EcommerceFinalContext context, EmailService emailService)
        {
            _context = context;
            _emailService = emailService;
        }

        public bool GenerateAndSendOtp(string email, string purpose, out int otpId)
        {

            // Tạo mã OTP
            var otpCode = GenerateOtp();

            // Tạo yêu cầu OTP
            var otpRequest = new Otp
            {
                Email = email,
                OtpCode = otpCode,
                ExpirationTime = DateTime.UtcNow.AddMinutes(5),
                Purpose = purpose,
                CreatedAt = DateTime.UtcNow,
                IsUsed = false,
            };

            _context.Otps.Add(otpRequest);
            _context.SaveChanges();

            otpId = otpRequest.Id;
            // Gửi OTP qua email
            _emailService.SendEmail(email, "Your OTP Code", $"Your OTP is: {otpCode}");

            return true;
        }

        public bool VerifyOtp(string email, int OtpId, string otpCode, string purpose, out string errorMessage)
        {
            // Tìm OTP theo userId và otpId
            var otpRequest = _context.Otps
                .Where(o => o.Email == email && o.Id == OtpId && o.Purpose == purpose)
                .FirstOrDefault();

            if (otpRequest == null)
            {
                errorMessage = "Không tìm thấy yêu cầu OTP.";
                return false;
            }

            // Kiểm tra nếu hết số lần thử
            if (otpRequest.AttempsLeft <= 0)
            {
                errorMessage = "Bạn đã hết số lần thử.";
                return false;
            }

            // Kiểm tra nếu OTP hết hạn
            if (otpRequest.ExpirationTime < DateTime.UtcNow)
            {
                errorMessage = "OTP đã hết hạn.";
                return false;
            }

            // Kiểm tra mã OTP
            if (otpRequest.OtpCode != otpCode)
            {
                otpRequest.AttempsLeft--; // Giảm số lần thử nếu mã sai
                _context.Otps.Update(otpRequest);
                _context.SaveChanges();

                if (otpRequest.AttempsLeft <= 0)
                {
                    errorMessage = "Bạn đã hết số lần thử.";
                }
                else
                {
                    errorMessage = $"OTP không hợp lệ. Bạn còn {otpRequest.AttempsLeft} lần thử.";
                }
                return false;
            }

          
            errorMessage = null; // Không có lỗi
            return true;
        }

        private string GenerateOtp()
        {
            return new Random().Next(100000, 999999).ToString(); // Tạo OTP 6 chữ số
        }
    }
}
