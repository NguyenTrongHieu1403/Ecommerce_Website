namespace ecommerce_final.Tool
{
    public static class UploadFileToFolder
    {
        public static string Upload(IFormFile file, string folderName)
        {
            if (file == null || file.Length == 0)
            {
                return string.Empty; // File không hợp lệ
            }

            try
            {
                // Loại bỏ ký tự không hợp lệ khỏi tên file
                var originalFileName = Path.GetFileNameWithoutExtension(file.FileName);
                var extension = Path.GetExtension(file.FileName);
                var safeFileName = string.Concat(originalFileName.Split(Path.GetInvalidFileNameChars()));
                var fileName = $"{DateTime.Now.Ticks}_{safeFileName}{extension}";

                // Đảm bảo thư mục đích tồn tại
                var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Images", folderName);
                Directory.CreateDirectory(folderPath);

                // Tạo đường dẫn đầy đủ
                var fullPath = Path.Combine(folderPath, fileName);

                // Ghi file vào thư mục
                using (var myFile = new FileStream(fullPath, FileMode.Create))
                {
                    file.CopyTo(myFile);
                }

                return fileName; // Trả về tên file đã upload
            }
            catch (Exception ex)
            {
                // Ghi log lỗi tại đây nếu cần (ví dụ: logger.LogError(ex))
                return string.Empty; // Hoặc ném lỗi tùy theo yêu cầu
            }
        }
    }
}

/*namespace ecommerce_final.Tool
{
    public static class UploadFileToFolder
    {
        public static string Upload(IFormFile file, string folderName)
        {
            if (file == null || file.Length == 0)
            {
                return string.Empty; // Không có tệp hoặc tệp rỗng
            }

            try
            {
                // Đảm bảo thư mục lưu trữ tồn tại
                var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Hinh", folderName);
                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath); // Tạo thư mục nếu chưa tồn tại
                }

                // Tạo tên tệp duy nhất
                var fileName = $"{DateTime.Now.Ticks}_{Path.GetFileName(file.FileName)}";
                var fullPath = Path.Combine(folderPath, fileName);

                // Lưu tệp vào thư mục
                using (var fileStream = new FileStream(fullPath, FileMode.Create))
                {
                    file.CopyTo(fileStream);
                }

                // Trả về đường dẫn tương đối để sử dụng
                return $"/Hinh/{folderName}/{fileName}";
            }
            catch (Exception ex)
            {
                // Log lỗi (tùy chỉnh theo nhu cầu)
                Console.WriteLine($"Error uploading file: {ex.Message}");
                return string.Empty;
            }
        }
    }
}
*/