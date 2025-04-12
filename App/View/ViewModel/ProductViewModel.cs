using App.Model;
using App.Service;
using App.Utils;
using BarcodeStandard;
using Microsoft.UI.Xaml.Media.Imaging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using WinRT.Interop;

namespace App.View.ViewModel
{
    public class ProductViewModel
    {
        public IDao _dao;
        private readonly string _imagesDirectoryPath;
        private readonly string _imagesRelativePath;

        public FullObservableCollection<Product> Products { get; set; }

        public Microsoft.UI.Xaml.Window _window;

        public ProductViewModel()
        {
            _dao = Services.GetKeyedSingleton<IDao>();
            List<Product> products = _dao.Products.GetAll();

            Products = new FullObservableCollection<Product>(products);
            foreach (var product in Products)
            {
                // Load image from relative path
                product.TotalPrice = product.Price * product.Inventory;

                // Tạo barcode cho mỗi sản phẩm
                if (product.BarCodeBitmap == null)
                {
                    product.BarCodeBitmap = new BitmapImage();
                    _ = product.LoadBarcodeAsync(product.BarCode);
                }
            }

            _window = new Microsoft.UI.Xaml.Window();

            // Get the project directory instead of the bin directory
            string projectDirectory = GetProjectDirectory();

            // Đường dẫn tuyệt đối đến thư mục Images trong project
            _imagesDirectoryPath = Path.Combine(projectDirectory, "Assets", "ProductImages");

            // Đường dẫn tương đối để lưu vào database
            _imagesRelativePath = Path.Combine("Assets", "ProductImages");

            //Debug.WriteLine("duong dan den project: ", projectDirectory);

            //Debug.WriteLine("CHECK DUONG DAN TUYET DOI DEN PROJECT: ", _imagesDirectoryPath);
            //Debug.WriteLine("CHECK DUONG DAN TUONG DOI DEN DATABASE: ", _imagesRelativePath);

            // Đảm bảo thư mục tồn tại
            if (!Directory.Exists(_imagesDirectoryPath))
            {
                Directory.CreateDirectory(_imagesDirectoryPath);
            }
        }

        

        // Helper method to get the actual project directory
        private static string GetProjectDirectory()
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

        public async Task NewFilter(Dictionary<string, object> filter, Dictionary<string, int>? sort = null)
        {
            List<Product> filteredProduct = _dao.Products.GetByQuery(filter, null, sort);
            if (filteredProduct != null)
            {
                Products.Clear();
                foreach (Product product in filteredProduct)
                {
                    Products.Add(product);
                }
            }
        }

        // Create (Add) a new product
        public async Task<bool> AddProduct(Product product, StorageFile imageFile = null)
        {
            try
            {
                // Handle image if provided
                if (imageFile != null)
                {
                    string imagePath = await SaveProductImage(imageFile, product.BarCode);
                    product.ImagePath = "ms-appx:///" + imagePath;
                }

                Debug.WriteLine("CHECK PRODUCT BEFRORE CALL API: ", product.ToString());
                // Add to database using repository
                _dao.Products.Insert(product);

                // Add to observable collection
                Products.Add(product);

                return true;
            }
            catch (Exception ex)
            {
                // Log the exception
                System.Diagnostics.Debug.WriteLine($"Error adding product: {ex.Message}");
                return false;
            }
        }

        // Read (Get) a product by BarCode
        public Product GetProductByBarCode(string barCode)
        {
            // Use UpdateByQuery to query by BarCode
            var parameters = new Dictionary<string, object> { { "BarCode", barCode } };

            // Modify RemoveByQuery to support querying
            var results = _dao.Products.GetAll()
                .Where(p => p.BarCode == barCode)
                .ToList();

            return results.FirstOrDefault();
        }

        // Update an existing product
        public async Task<bool> UpdateProduct(Product product, StorageFile newImageFile = null)
        {
            try
            {
                // Find the index of the product in the collection
                int index = Products.IndexOf(Products.FirstOrDefault(p => p.BarCode == product.BarCode));

                if (index == -1)
                    return false;

                // Handle new image if provided
                if (newImageFile != null)
                {
                    // Delete old image if it exists
                    if (!string.IsNullOrEmpty(product.ImagePath))
                    {
                        await DeleteProductImage(product.ImagePath);
                    }

                    // Save new image
                    string imagePath = await SaveProductImage(newImageFile, product.BarCode);
                    product.ImagePath = "ms-appx:///" + imagePath;
                }

                // Prepare update values
                var updateValues = new Dictionary<string, object>
                {
                    { "Name", product.Name },
                    { "Price", product.Price },
                    { "ImagePath", product.ImagePath },
                    { "TypeGroup", product.TypeGroup },
                    { "Vat", product.Vat },
                    { "CostPrice", product.CostPrice }
                };

                // Update in database using repository
                _dao.Products.UpdateByQuery(
                    updateValues,
                    "BarCode = @BarCode",
                    new Dictionary<string, object> { { "BarCode", product.BarCode } }
                );

                // Update in observable collection
                Products[index] = product;

                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error updating product: {ex.Message}");
                return false;
            }
        }

        // Delete a product
        public async Task<bool> DeleteProduct(Product product)
        {
            try
            {
                // Delete the image file if it exists
                if (!string.IsNullOrEmpty(product.ImagePath))
                {
                    await DeleteProductImage(product.ImagePath);
                }

                // Delete from database using repository
                _dao.Products.RemoveByQuery(
                    "BarCode = @BarCode",
                    new Dictionary<string, object> { { "BarCode", product.BarCode } }
                );

                // Remove from observable collection
                Products.Remove(product);

                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error deleting product: {ex.Message}");
                return false;
            }
        }

        // Helper method to pick an image file
        public async Task<StorageFile> PickProductImage()
        {
            if (_window == null)
                throw new InvalidOperationException("Window reference is not set. Call SetWindow first.");

            var picker = new FileOpenPicker();
            // Initialize the picker with the window handle
            InitializeWithWindow.Initialize(picker, WindowNative.GetWindowHandle(_window));

            // Configure file picker
            picker.ViewMode = PickerViewMode.Thumbnail;
            picker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
            picker.FileTypeFilter.Add(".jpg");
            picker.FileTypeFilter.Add(".jpeg");
            picker.FileTypeFilter.Add(".png");

            // Open the picker and get the selected file
            return await picker.PickSingleFileAsync();
        }

        // Save product image to project folder and return a relative path
        private async Task<string> SaveProductImage(StorageFile imageFile, string barCode)
        {
            try
            {
                // Generate a filename based on barcode and timestamp to ensure uniqueness
                string fileName = $"{barCode}_{DateTime.Now.Ticks}{Path.GetExtension(imageFile.Name)}";
                string destinationPath = Path.Combine(_imagesDirectoryPath, fileName);

                // Copy the file to the project's image folder
                using (var stream = await imageFile.OpenStreamForReadAsync())
                {
                    using (var fileStream = new FileStream(destinationPath, FileMode.Create))
                    {
                        await stream.CopyToAsync(fileStream);
                    }
                }

                //update ProductImages folder trong bin -> cải tiến UX
                CopyFolderImager.CopyProductImagesFolder();

                // Return the relative path to use in ImagePath property
                string relativePath = Path.Combine(_imagesRelativePath, fileName);
                return relativePath;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error saving product image: {ex.Message}");
                throw;
            }
        }

        // Delete a product image
        private async Task DeleteProductImage(string relativePath)
        {
            try
            {
                // Get the project directory
                string projectDirectory = GetProjectDirectory();

                // Convert relative path to absolute path using project directory
                string absolutePath = Path.Combine(projectDirectory, relativePath.Replace("ms-appx:///", ""));

                if (File.Exists(absolutePath))
                {
                    File.Delete(absolutePath);
                }

                //update ProductImages folder trong bin -> cải tiến UX
                CopyFolderImager.CopyProductImagesFolder();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error deleting product image: {ex.Message}");
                // Don't throw here, we still want to delete the product if the image deletion fails
            }
        }

        // Helper method to load an image from a relative path and return a BitmapImage
        public static async Task<BitmapImage> LoadImageFromPath(string relativePath)
        {
            if (string.IsNullOrEmpty(relativePath))
                return null;

            try
            {
                // Get the project directory using a similar approach as in GetProjectDirectory
                string projectDirectory = GetProjectDirectory();

                // Convert relative path to absolute path
                string absolutePath = Path.Combine(projectDirectory,
                    relativePath.Replace("ms-appx:///", ""));

                if (!File.Exists(absolutePath))
                    return null;

                BitmapImage bitmapImage = new BitmapImage();

                using (FileStream stream = File.OpenRead(absolutePath))
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        await stream.CopyToAsync(memoryStream);
                        memoryStream.Position = 0;

                        // Convert to RandomAccessStream required by BitmapImage
                        using (var randomAccessStream = new InMemoryRandomAccessStream())
                        {
                            using (var outputStream = randomAccessStream.GetOutputStreamAt(0))
                            {
                                using (var dataWriter = new DataWriter(outputStream))
                                {
                                    dataWriter.WriteBytes(memoryStream.ToArray());
                                    await dataWriter.StoreAsync();
                                    await outputStream.FlushAsync();
                                }
                            }

                            await bitmapImage.SetSourceAsync(randomAccessStream);
                        }
                    }
                }

                return bitmapImage;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading image: {ex.Message}");
                return null;
            }
        }

        public async Task FilterProducts(string searchText, string productType, string productGroup, string status, string sortOrder)
        {
            try
            {
                //Debug.WriteLine("ProductViewModel", $"Filters: {searchText}, {productType}, {productGroup}, {status}, {sortOrder}");
                // Use the new GetFiltered method from repository
                //List<Product> filteredProducts = _productRepository.GetFiltered(
                //    searchText,
                //    productType,
                //    productGroup,
                //    status,
                //    sortOrder
                //);
                List<Product> filteredProducts = new List<Product>();
                // Clear and update the collection
                Products.Clear();
                foreach (var product in filteredProducts)
                {
                    Products.Add(product);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error filtering products: {ex.Message}");
            }
        }

        public void LoadProductsByCategory(string TypeGroup)
        {
            Products.Clear();

            var filteredProducts = _dao.Products.GetByQuery(new Dictionary<string, object> { { "TypeGroup", TypeGroup } });
            foreach (var product in filteredProducts)
            {
                // Make sure inventory is at least 0, never negative
                product.Inventory = Math.Max(0, product.Inventory);
                Products.Add(product);
            }
        }
    }
}