using Blazored.LocalStorage;
using System.Net.Http.Json;
using System.Net.Http.Headers;
using System.Text.Json;
using ArggonStores.Web.Models;

namespace ArggonStores.Web.Services;

public interface IProductService
{
    Task<ProductsResult> GetProductsAsync(string? category = null, bool? isActive = true, int pageNumber = 1, int pageSize = 10);
    Task<ProductResult> GetProductByIdAsync(int id);
    Task<ProductResult> CreateProductAsync(CreateProductModel model);
    Task<ProductResult> UpdateProductAsync(EditProductModel model);
    Task<ProductResult> DeleteProductAsync(int id);
}

public class ProductService : IProductService
{
    private readonly HttpClient _httpClient;
    private readonly ILocalStorageService _localStorage;
    private readonly ILogger<ProductService> _logger;
    private readonly JsonSerializerOptions _jsonOptions;

    public ProductService(HttpClient httpClient, ILocalStorageService localStorage, ILogger<ProductService> logger)
    {
        _httpClient = httpClient;
        _localStorage = localStorage;
        _logger = logger;
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true
        };
    }

    public async Task<ProductsResult> GetProductsAsync(string? category = null, bool? isActive = true, int pageNumber = 1, int pageSize = 10)
    {
        try
        {
            await SetAuthorizationHeaderAsync();

            var queryString = $"?pageNumber={pageNumber}&pageSize={pageSize}";
            if (!string.IsNullOrEmpty(category))
                queryString += $"&category={Uri.EscapeDataString(category)}";
            if (isActive.HasValue)
                queryString += $"&isActive={isActive.Value}";

            var response = await _httpClient.GetAsync($"api/products{queryString}");

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var apiResponse = JsonSerializer.Deserialize<JsonElement>(content, _jsonOptions);

                var products = JsonSerializer.Deserialize<List<Product>>(
                    apiResponse.GetProperty("products").GetRawText(), _jsonOptions) ?? new();

                return new ProductsResult
                {
                    IsSuccess = true,
                    Products = products,
                    TotalCount = apiResponse.GetProperty("totalCount").GetInt32(),
                    PageNumber = apiResponse.GetProperty("pageNumber").GetInt32(),
                    PageSize = apiResponse.GetProperty("pageSize").GetInt32(),
                    TotalPages = apiResponse.GetProperty("totalPages").GetInt32()
                };
            }

            var errorContent = await response.Content.ReadAsStringAsync();
            var errorResponse = JsonSerializer.Deserialize<JsonElement>(errorContent, _jsonOptions);
            var errorMessage = errorResponse.TryGetProperty("message", out var messageProp) 
                ? messageProp.GetString() ?? "Failed to retrieve products"
                : "Failed to retrieve products";

            return new ProductsResult
            {
                IsSuccess = false,
                Message = errorMessage
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving products");
            return new ProductsResult
            {
                IsSuccess = false,
                Message = "Network error occurred"
            };
        }
    }

    public async Task<ProductResult> GetProductByIdAsync(int id)
    {
        try
        {
            await SetAuthorizationHeaderAsync();

            var response = await _httpClient.GetAsync($"api/products/{id}");

            if (response.IsSuccessStatusCode)
            {
                var product = await response.Content.ReadFromJsonAsync<Product>(_jsonOptions);
                return new ProductResult
                {
                    IsSuccess = true,
                    Product = product
                };
            }

            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return new ProductResult
                {
                    IsSuccess = false,
                    Message = "Product not found"
                };
            }

            var errorContent = await response.Content.ReadAsStringAsync();
            var errorResponse = JsonSerializer.Deserialize<JsonElement>(errorContent, _jsonOptions);
            var errorMessage = errorResponse.TryGetProperty("message", out var messageProp) 
                ? messageProp.GetString() ?? "Failed to retrieve product"
                : "Failed to retrieve product";

            return new ProductResult
            {
                IsSuccess = false,
                Message = errorMessage
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving product with ID {ProductId}", id);
            return new ProductResult
            {
                IsSuccess = false,
                Message = "Network error occurred"
            };
        }
    }

    public async Task<ProductResult> CreateProductAsync(CreateProductModel model)
    {
        try
        {
            await SetAuthorizationHeaderAsync();

            var response = await _httpClient.PostAsJsonAsync("api/products", model, _jsonOptions);

            if (response.IsSuccessStatusCode)
            {
                var product = await response.Content.ReadFromJsonAsync<Product>(_jsonOptions);
                return new ProductResult
                {
                    IsSuccess = true,
                    Message = "Product created successfully",
                    Product = product
                };
            }

            var errorContent = await response.Content.ReadAsStringAsync();
            return new ProductResult
            {
                IsSuccess = false,
                Message = "Failed to create product"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating product");
            return new ProductResult
            {
                IsSuccess = false,
                Message = "Network error occurred"
            };
        }
    }

    public async Task<ProductResult> UpdateProductAsync(EditProductModel model)
    {
        try
        {
            await SetAuthorizationHeaderAsync();

            var response = await _httpClient.PutAsJsonAsync($"api/products/{model.Id}", model, _jsonOptions);

            if (response.IsSuccessStatusCode)
            {
                var product = await response.Content.ReadFromJsonAsync<Product>(_jsonOptions);
                return new ProductResult
                {
                    IsSuccess = true,
                    Message = "Product updated successfully",
                    Product = product
                };
            }

            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return new ProductResult
                {
                    IsSuccess = false,
                    Message = "Product not found"
                };
            }

            return new ProductResult
            {
                IsSuccess = false,
                Message = "Failed to update product"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating product with ID {ProductId}", model.Id);
            return new ProductResult
            {
                IsSuccess = false,
                Message = "Network error occurred"
            };
        }
    }

    public async Task<ProductResult> DeleteProductAsync(int id)
    {
        try
        {
            await SetAuthorizationHeaderAsync();

            var response = await _httpClient.DeleteAsync($"api/products/{id}");

            if (response.IsSuccessStatusCode)
            {
                return new ProductResult
                {
                    IsSuccess = true,
                    Message = "Product deleted successfully"
                };
            }

            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return new ProductResult
                {
                    IsSuccess = false,
                    Message = "Product not found"
                };
            }

            return new ProductResult
            {
                IsSuccess = false,
                Message = "Failed to delete product"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting product with ID {ProductId}", id);
            return new ProductResult
            {
                IsSuccess = false,
                Message = "Network error occurred"
            };
        }
    }

    private async Task SetAuthorizationHeaderAsync()
    {
        var token = await _localStorage.GetItemAsync<string>("authToken");
        if (!string.IsNullOrEmpty(token))
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }
    }
}