using App.Model;
using App.Service;
using App.Utils;
using Microsoft.UI.Xaml.Media.Imaging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using WinRT.Interop;

namespace App.View.ViewModel
{
    public class ProductViewModel
    {
        private readonly IRepository<Product> _productRepository;
        public FullObservableCollection<Product> Products { get; set; }

        // Reference to the current window for file pickers
        private Microsoft.UI.Xaml.Window _window;

        public ProductViewModel()
        {
            // Use MongoRepository<Product> through the IRepository interface
            _productRepository = new MongoRepository<Product>();

            // Load all products from the repository
            List<Product> listProduct = _productRepository.GetAll();
            Products = new FullObservableCollection<Product>(listProduct);
        }

        // Set the current window reference (call this from page's constructor)
        public void SetWindow(Microsoft.UI.Xaml.Window window)
        {
            _window = window;
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
                    product.ImagePath = imagePath;
                }

                Debug.WriteLine("CHECK PRODUCT BEFRORE CALL API: ", product.ToString());
                // Add to database using repository
                _productRepository.Insert(product);

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
            var results = _productRepository.GetAll()
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
                    product.ImagePath = imagePath;
                }

                // Prepare update values
                var updateValues = new Dictionary<string, object>
                {
                    { "Name", product.Name },
                    { "Price", product.Price },
                    { "ImagePath", product.ImagePath },
                    { "TypeGroup", product.TypeGroup },
                    { "VAT", product.VAT },
                    { "CostPrice", product.CostPrice }
                };

                // Update in database using repository
                _productRepository.UpdateByQuery(
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
                _productRepository.RemoveByQuery(
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

        // Save product image to app's local storage
        private async Task<string> SaveProductImage(StorageFile imageFile, string barCode)
        {
            try
            {
                // Create a folder for product images if it doesn't exist
                StorageFolder localFolder = ApplicationData.Current.LocalFolder;
                StorageFolder imagesFolder = await localFolder.CreateFolderAsync("ProductImages", CreationCollisionOption.OpenIfExists);

                // Generate a filename based on barcode and timestamp to ensure uniqueness
                string fileName = $"{barCode}_{DateTime.Now.Ticks}{Path.GetExtension(imageFile.Name)}";

                // Copy the image file to the app's local storage
                StorageFile newFile = await imageFile.CopyAsync(imagesFolder, fileName, NameCollisionOption.GenerateUniqueName);

                // Return the path to use in ImagePath property
                return newFile.Path;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error saving product image: {ex.Message}");
                throw;
            }
        }

        // Delete a product image
        private async Task DeleteProductImage(string imagePath)
        {
            try
            {
                StorageFile file = await StorageFile.GetFileFromPathAsync(imagePath);
                await file.DeleteAsync();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error deleting product image: {ex.Message}");
                // Don't throw here, we still want to delete the product if the image deletion fails
            }
        }

        // Helper method to load an image from a path and return a BitmapImage
        public static async Task<BitmapImage> LoadImageFromPath(string imagePath)
        {
            if (string.IsNullOrEmpty(imagePath))
                return null;

            try
            {
                StorageFile file = await StorageFile.GetFileFromPathAsync(imagePath);
                using (IRandomAccessStream stream = await file.OpenAsync(FileAccessMode.Read))
                {
                    BitmapImage bitmapImage = new BitmapImage();
                    await bitmapImage.SetSourceAsync(stream);
                    return bitmapImage;
                }
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
                List<Product> filteredProducts = _productRepository.GetFiltered(
                    searchText,
                    productType,
                    productGroup,
                    status,
                    sortOrder
                );

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
    }
}