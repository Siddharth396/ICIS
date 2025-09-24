namespace BusinessLayer.Services.Product
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using BusinessLayer.Services.Models;

    using Infrastructure.SQLDB.Models;

    public interface IProductService
    {
        Task<Response<IEnumerable<Product>>> GetProducts();
    }
}
