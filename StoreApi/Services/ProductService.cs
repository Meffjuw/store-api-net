using Api.Entities;
using Dapper;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;

public class ProductService : IProductService
{
  private string connectionString;
  public ProductService(IConfiguration configuration)
  {
    connectionString = configuration.GetConnectionString("Local");
  }

  public async Task<IList<Product>> GetAll(string vendorId)
  {
    using var connection = new SqlConnection(connectionString);

    return (await connection.QueryAsync<Product>(@"
        SELECT 
          a.Id,
          a.Name,
          a.ProductId,
          a.ReleaseDate,
          a.Visible,
          b.VendorId
        FROM [dbo].[Products] a
        JOIN [dbo].[Vendors] b
        ON a.[VendorId] = b.[Id]
        WHERE b.[VendorId] = @vendorId",
      new { vendorId })).AsList();
  }

  public async Task<Product> Get(string productId, string vendorId) {
    using var connection = new SqlConnection(connectionString);

    return (await connection.QueryFirstOrDefaultAsync<Product>(@"
        SELECT 
          a.Id,
          a.Name,
          a.ProductId,
          a.ReleaseDate,
          a.Visible,
          b.VendorId
        FROM [dbo].[Products] a
        JOIN [dbo].[Vendors] b
        ON a.[VendorId] = b.[Id]
        WHERE b.[VendorId] = @vendorId
        AND a.[ProductId] = @productId",
      new { vendorId, productId }));
  }

  public async Task<Product> Create(Product product, string vendorId) {
    using var connection = new SqlConnection(connectionString);

    var key = await connection.ExecuteScalarAsync<int>("EXECUTE [dbo].[insert_product] @ProductId, @Name, @Visible, @ReleaseDate, @vendorId", new {
      product.ProductId,
      product.Name,
      product.Visible,
      product.ReleaseDate,
      vendorId
    });

    product.Id = key;

    return product;
  }

  public async Task<bool> Update(Product product, string vendorId) {
    using var connection = new SqlConnection(connectionString);

    var row = await connection.ExecuteAsync("EXECUTE [dbo].[update_product] @ProductId, @Name, @Visible, @ReleaseDate, @vendorId", new {
      product.ProductId,
      product.Name,
      product.Visible,
      product.ReleaseDate,
      vendorId
    });

    return row != 0;
  }
}