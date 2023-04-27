using Microsoft.AspNetCore.Mvc;
using NetCoreWebApi.Models;
namespace NetCoreWebApi.Controllers;

/// <summary>
/// 
/// </summary>
public class ProductsController : GenericController<Product>
{

    private readonly ILogger<ProductsController> _logger;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="logger"></param>
    /// <param name="dbContext"></param>
    /// <returns></returns>
    public ProductsController(ILogger<ProductsController> logger, NorthwindContext dbContext) : base(logger, dbContext)
    {
        _logger = logger;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="entity"></param>
    /// <returns></returns>
    protected override int GetId(Product entity) => entity.ProductId;
}
