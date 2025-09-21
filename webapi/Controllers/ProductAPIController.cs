using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using webapi.Models;

namespace webapi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductAPIController : ControllerBase
    {
        static List<Product> products = new List<Product>() {
            new Product(){ Price = 102333, ProductCode = "P123", ProductId = 101, ProductName = "Bag"}
        };


        [HttpGet]
        [Route("getall")]
        [Authorize]
        public IActionResult GetProducts()
        {
            //fetch from db
            return StatusCode(StatusCodes.Status200OK, products);
        }

        [HttpPost]
        [Route("add")]
        [Authorize]
        public IActionResult AddProduct(Product productToInsert)
        {
            products.Add(productToInsert);
            return StatusCode(StatusCodes.Status201Created, true);
        }

        [HttpDelete]
        [Route("delete/{productid}")]
        [Authorize]
        public IActionResult DeleteProduct(int productid)
        {
            var productToDelete = products.Where(d => d.ProductId == productid).FirstOrDefault();

            if (productToDelete != null)
            {
                products.Remove(productToDelete);
                return StatusCode(StatusCodes.Status200OK, true);
            }
            return StatusCode(StatusCodes.Status204NoContent, false);
        }

        [HttpGet]
        [Route("get/{productid}")]
        public IActionResult GetProductByID(int productid)
        {
            var productToEdit = products.Where(d => d.ProductId == productid).FirstOrDefault();

            return StatusCode(StatusCodes.Status200OK, productToEdit);
        }


    }
}
