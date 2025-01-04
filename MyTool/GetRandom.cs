using System.Security.Cryptography;
using System.Text;

namespace ecommerce_final.Tool
{
    public static class GetRandom
    {
        public static string Random(int length = 5)
        {
            var pattern = @"1234567890qazwsxedcrfvtgbyhn@#$%";
            var sb = new StringBuilder();

            using (var rng = RandomNumberGenerator.Create())
            {
                byte[] randomNumber = new byte[4];
                for (int i = 0; i < length; i++)
                {
                    rng.GetBytes(randomNumber);
                    int index = BitConverter.ToInt32(randomNumber, 0) % pattern.Length;
                    if (index < 0) index = -index;  // Đảm bảo index luôn là số dương
                    sb.Append(pattern[index]);
                }
            }

            return sb.ToString();
        }
    }
}
