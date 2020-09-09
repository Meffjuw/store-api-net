using System.Collections.Generic;
using System.Threading.Tasks;
using Api.Entities;

public interface IProductService
{
  public Task<IList<Product>> GetAll(string vendorId);

  public Task<Product> Get(string productId, string vendorId);

  public Task<Product> Create(Product product, string vendorId);

  public Task<bool> Update(Product product, string vendorId);
}