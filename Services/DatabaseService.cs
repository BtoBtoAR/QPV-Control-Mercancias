using QPVControlMercancias.Models;
using SQLite;

namespace QPVControlMercancias.Services
{
    public class DatabaseService
    {
        private SQLiteAsyncConnection? _database;
        private readonly string _databasePath;

        public DatabaseService()
        {
            _databasePath = Path.Combine(FileSystem.AppDataDirectory, "products.db");
        }

        private async Task InitializeAsync()
        {
            if (_database != null)
                return;

            _database = new SQLiteAsyncConnection(_databasePath);
            await _database.CreateTableAsync<Product>();
        }

        public async Task<List<Product>> GetProductsAsync()
        {
            await InitializeAsync();
            return await _database!.Table<Product>().ToListAsync();
        }

        public async Task<Product?> GetProductByBarcodeAsync(string barcode)
        {
            await InitializeAsync();
            return await _database!.Table<Product>()
                .Where(p => p.Barcode == barcode)
                .FirstOrDefaultAsync();
        }

        public async Task<Product?> GetProductByIdAsync(int id)
        {
            await InitializeAsync();
            return await _database!.Table<Product>()
                .Where(p => p.Id == id)
                .FirstOrDefaultAsync();
        }

        public async Task<int> SaveProductAsync(Product product)
        {
            await InitializeAsync();
            product.UpdatedAt = DateTime.UtcNow;

            if (product.Id != 0)
            {
                return await _database!.UpdateAsync(product);
            }
            else
            {
                product.CreatedAt = DateTime.UtcNow;
                return await _database!.InsertAsync(product);
            }
        }

        public async Task<int> DeleteProductAsync(Product product)
        {
            await InitializeAsync();
            return await _database!.DeleteAsync(product);
        }

        public async Task<int> GetProductCountAsync()
        {
            await InitializeAsync();
            return await _database!.Table<Product>().CountAsync();
        }
    }
}
