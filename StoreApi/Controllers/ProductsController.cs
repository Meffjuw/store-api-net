using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Security.Claims;
using System.Threading.Tasks;
using Api.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace StoreApi.Controllers
{
  [ApiController]
  [Authorize]
  [Route("api/[controller]")]
  public class ProductsController : ControllerBase
  {
    private readonly ILogger<ProductsController> _logger;
    private readonly IProductService _productService;

    public ProductsController(ILogger<ProductsController> logger, IProductService productService)
    {
      _logger = logger;
      _productService = productService;
    }

    [HttpGet]
    public async Task<ActionResult<IAsyncEnumerable<Product>>> Get()
    {
      var user = User.FindFirst(ClaimTypes.NameIdentifier);

      if (user == null)
      {
        _logger.LogInformation($"User could not be parsed from Access Token. Request rejected.");
        return BadRequest();
      }

      _logger.LogInformation($"User {user.Value} has requested their products.");

      var products = await _productService.GetAll(user.Value);

      return products.Count > 0 ? (ActionResult)Ok(products) : NoContent();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Product>> GetProduct(string id)
    {
      var user = User.FindFirst(ClaimTypes.NameIdentifier);

      if (user == null)
      {
        _logger.LogInformation($"User could not be parsed from Access Token. Request rejected.");
        return BadRequest();
      }

      _logger.LogInformation($"User {user.Value} has requested Product with Id: {id}.");

      var product = await _productService.Get(id, user.Value);

      return product != null ? (ActionResult)Ok(product) : NotFound();
    }

    [HttpPost]
    public async Task<ActionResult<Product>> Post([FromBody] Product product) {
      var user = User.FindFirst(ClaimTypes.NameIdentifier);

      if (user == null)
      {
        _logger.LogInformation($"User could not be parsed from Access Token. Request rejected.");
        return BadRequest();
      }

      Product temp = null;

      try {
        temp = await _productService.Create(product, user.Value);
      } catch (SqlException sqlEx) {
        if(sqlEx.Number == 2601 || sqlEx.Number == 2627) {
          return BadRequest(new {
            error = "Duplicate ProductId",
            message = $"The ProductId {product.ProductId} is already in use."
          });
        }
      }
      catch(Exception e) {
        throw e;
      }

      return temp != null ? (ActionResult)Created(Request.GetEncodedUrl() + "/" + temp.ProductId, temp) : BadRequest();
    }

    [HttpPut]
    public async Task<IActionResult> Put([FromBody] Product product) {
      var user = User.FindFirst(ClaimTypes.NameIdentifier);

      if (user == null)
      {
        _logger.LogInformation($"User could not be parsed from Access Token. Request rejected.");
        return BadRequest();
      }

      bool temp = false;

      try {
        temp = await _productService.Update(product, user.Value);
      } catch (SqlException sqlEx) {
        if(sqlEx.Number == 2601 || sqlEx.Number == 2627) {
          return BadRequest(new {
            error = "Duplicate ProductId",
            message = $"The ProductId {product.ProductId} is already in use."
          });
        }
      }
      catch(Exception e) {
        throw e;
      }

      return temp
        ? (IActionResult)Ok()
        : (IActionResult)NotFound();
    }
  }
}
