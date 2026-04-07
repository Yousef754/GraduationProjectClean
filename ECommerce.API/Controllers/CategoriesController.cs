using ECommerce.Domain.Contracts;
using ECommerce.Domain.Entities.ProductModule;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoriesController : ApiBaseController
    {
        private readonly IUnitOfWork _unitOfWork;

        public CategoriesController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet("GetAllCategories")]
        public async Task<ActionResult<IEnumerable<Category>>> GetAll()
        {
            var categories = await _unitOfWork.GetRepository<Category, int>().GetAllAsync();
            return Ok(categories);
        }
    }
}
