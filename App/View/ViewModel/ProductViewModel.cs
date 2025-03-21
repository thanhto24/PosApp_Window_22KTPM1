using App.Model;
using App.Service;
using App.Utils;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Imaging;
using System;
using System.Collections.Generic;
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
        private readonly IDao _dao;
        public FullObservableCollection<Product> categories { get; set; }

        // Reference to the current window for file pickers
        private Microsoft.UI.Xaml.Window _window;

        public ProductViewModel()
        {
            _dao = Services.GetKeyedSingleton<IDao>();
            List<Product> list_product = _dao.Categories.GetAll();
            categories = new FullObservableCollection<Product>(list_product);
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
                // Set a new ID (get the max ID and add 1)
                int nextId = categories.Count > 0 ? categories.Max(p => p.Id) + 1 : 1;
                product.Id = nextId;

                // Handle image if provided
                if (imageFile != null)
                {
                    string imagePath = await SaveProductImage(imageFile, product.BarCode);
                    product.ImagePath = imagePath;
                }

                // Add to database
                _dao.Categories.Insert(product);

                // Add to observable collection
                categories.Add(product);

                return true;
            }
            catch (Exception ex)
            {
                // Log the exception
                System.Diagnostics.Debug.WriteLine($"Error adding product: {ex.Message}");
                return false;
            }
        }

        // Read (Get) a product by ID
        public Product GetProductById(int id)
        {
            return categories.FirstOrDefault(p => p.Id == id);
        }

        // Update an existing product
        public async Task<bool> UpdateProduct(Product product, StorageFile newImageFile = null)
        {
            try
            {
                // Find the index of the product in the collection
                int index = categories.IndexOf(categories.FirstOrDefault(p => p.Id == product.Id));

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

                // Update in database
                var updateValues = new Dictionary<string, object>
                {
                    { "Name", product.Name },
                    { "Price", product.Price },
                    { "ImagePath", product.ImagePath },
                    { "BarCode", product.BarCode },
                    { "TypeGroup", product.TypeGroup },
                    { "VAT", product.VAT },
                    { "CostPrice", product.CostPrice }
                };

                _dao.Categories.UpdateByQuery(
                    updateValues,
                    "ID = @ID",
                    new Dictionary<string, object> { { "ID", product.Id } }
                );

                // Update in observable collection
                categories[index] = product;

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

                // Delete from database
                _dao.Categories.RemoveByQuery(
                    "ID = @ID",
                    new Dictionary<string, object> { { "ID", product.Id } }
                );

                // Remove from observable collection
                categories.Remove(product);

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
    }
}