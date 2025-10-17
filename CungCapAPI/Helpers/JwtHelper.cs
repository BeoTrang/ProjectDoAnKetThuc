using ModelLibrary;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace CungCapAPI.Helpers
{
    public static class JwtHelper
    {
        public static string GenerateAccessToken(ThongTinNguoiDung thongTinNguoiDung ,  IConfiguration config)
        {
            var key = Encoding.ASCII.GetBytes(config["JWT:Key"]);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim("NguoiDungId", thongTinNguoiDung.NguoiDungId.ToString()),
                    new Claim("TenNguoiDung", thongTinNguoiDung.TenNguoiDung),
                    new Claim("Email", thongTinNguoiDung.Email),
                    new Claim("SoDienThoai", thongTinNguoiDung.VaiTro),
                    new Claim("VaiTro", thongTinNguoiDung.VaiTro)
                }),
                Expires = DateTime.UtcNow.AddMinutes(5),
                Issuer = config["JWT:Issuer"],
                Audience = config["JWT:Audience"],
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public static string GenerateRefreshToken(int size = 32)
        {
            var randomNumber = new byte[size];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
                string base64 = Convert.ToBase64String(randomNumber);
                return base64.Replace("+", "-").Replace("/", "_").Replace("=", "");
            }
        }

    }
}
