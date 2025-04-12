using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace App.Utils
{
    public class CopyFolderImager
    {
        public static string GetProjectDirectory()
        {
            // Bắt đầu với đường dẫn của assembly đang thực thi
            string assemblyLocation = Assembly.GetExecutingAssembly().Location;
            string assemblyDirectory = Path.GetDirectoryName(assemblyLocation);

            // In ra để kiểm tra
            Debug.WriteLine($"Assembly location: {assemblyLocation}");
            Debug.WriteLine($"Assembly directory: {assemblyDirectory}");

            // Tạo một đối tượng DirectoryInfo từ thư mục assembly
            DirectoryInfo directory = new DirectoryInfo(assemblyDirectory);

            // Đi ngược lên cho đến khi tìm thấy thư mục App hoặc thư mục chứa file .csproj
            while (directory != null &&
                   directory.Name != "App" &&
                   !Directory.GetFiles(directory.FullName, "*.csproj").Any())
            {
                Debug.WriteLine($"Checking directory: {directory.FullName}");
                directory = directory.Parent;
            }

            // Nếu tìm thấy thư mục App hoặc thư mục chứa file .csproj
            if (directory != null)
            {
                Debug.WriteLine($"Found project directory: {directory.FullName}");
                return directory.FullName;
            }

            // Thử tìm kiếm từ thư mục hiện tại và đi lên cho đến khi tìm thấy thư mục gốc của solution
            string currentDirectory = Directory.GetCurrentDirectory();
            Debug.WriteLine($"Current directory: {currentDirectory}");

            directory = new DirectoryInfo(currentDirectory);
            while (directory != null && !Directory.GetFiles(directory.FullName, "*.sln").Any())
            {
                directory = directory.Parent;
            }

            // Nếu tìm thấy thư mục chứa file .sln
            if (directory != null)
            {
                // Tìm thư mục dự án App trong thư mục solution
                string appDirectory = Path.Combine(directory.FullName, "App");
                if (Directory.Exists(appDirectory))
                {
                    Debug.WriteLine($"Found App directory: {appDirectory}");
                    return appDirectory;
                }
            }

            // Nếu không tìm thấy, có thể thử một phương pháp khác - lưu ở một vị trí cố định
            // Ví dụ: lưu vào thư mục Documents
            string documentsFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            string appAssetsFolder = Path.Combine(documentsFolder, "AppAssets");

            if (!Directory.Exists(appAssetsFolder))
            {
                Directory.CreateDirectory(appAssetsFolder);
            }

            Debug.WriteLine($"Using documents folder fallback: {appAssetsFolder}");
            return documentsFolder;
        }
        public static void CopyProductImagesFolder()
        {
            try
            {
                // Đường dẫn thư mục đích (bin)
                string destinationPath = Path.Combine(AppContext.BaseDirectory, "Assets", "ProductImages");

                // Đảm bảo thư mục Assets tồn tại
                if (!Directory.Exists(Path.Combine(AppContext.BaseDirectory, "Assets")))
                {
                    Directory.CreateDirectory(Path.Combine(AppContext.BaseDirectory, "Assets"));
                }

                // Tìm thư mục nguồn của dự án
                string projectDirectory = GetProjectDirectory();
                if (projectDirectory == null)
                {
                    Debug.WriteLine("Could not find project directory, cannot copy ProductImages folder");
                    return;
                }

                // Đường dẫn thư mục nguồn (Assets gốc)
                string sourcePath = Path.Combine(projectDirectory, "Assets", "ProductImages");

                // Kiểm tra xem thư mục nguồn có tồn tại không
                if (Directory.Exists(sourcePath))
                {
                    // Xóa thư mục đích nếu đã tồn tại
                    if (Directory.Exists(destinationPath))
                    {
                        Directory.Delete(destinationPath, true);
                        Debug.WriteLine($"Deleted existing ProductImages folder: {destinationPath}");
                    }

                    // Tạo lại thư mục đích
                    Directory.CreateDirectory(destinationPath);

                    // Sao chép tất cả các file từ thư mục nguồn sang thư mục đích
                    foreach (string file in Directory.GetFiles(sourcePath))
                    {
                        string fileName = Path.GetFileName(file);
                        string destFile = Path.Combine(destinationPath, fileName);
                        File.Copy(file, destFile, true);
                    }

                    Debug.WriteLine($"Successfully copied ProductImages folder to bin directory: {destinationPath}");
                    Debug.WriteLine($"Copied {Directory.GetFiles(sourcePath).Length} files");
                }
                else
                {
                    Debug.WriteLine($"Source ProductImages folder not found: {sourcePath}");

                    // Tạo thư mục nguồn nếu chưa tồn tại
                    Directory.CreateDirectory(sourcePath);

                    // Đảm bảo thư mục đích cũng tồn tại
                    if (!Directory.Exists(destinationPath))
                    {
                        Directory.CreateDirectory(destinationPath);
                    }

                    Debug.WriteLine($"Created empty ProductImages folders in source and destination");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error copying ProductImages folder: {ex.Message}");
            }
        }
    }
}
