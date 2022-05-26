using TestProject.DataAccess;
using TestProject.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace TestProject.Controllers
{   
    [Authorize(Roles = UserRoles.User)]
    [Route("api/[controller]")]
    [ApiController]
    public class UserDetailsController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly IConfiguration _configuration;
        private readonly ApplicationDBContext _context;
        public UserDetailsController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, IConfiguration configuration, ApplicationDBContext context)
        {
            this.userManager = userManager;
            this.roleManager = roleManager;
            _configuration = configuration;
            _context = context;
        }

        [HttpGet]
        [Route("GetUserProfileDetails/{userid}")]
        public async Task<RegistrationDTO> GetUserProfileDetails(string userid)
        {
            var data = await userManager.Users.Where(x => x.Id == userid).SingleOrDefaultAsync();
            RegistrationDTO reg = new RegistrationDTO();
            if (data!=null)
            {
                reg.name = data.UserName;
                reg.emailid = data.Email;
            }

            return reg;
        }

        [HttpPost]
        [Route("PostProductData")]
        public async Task<IActionResult> PostProductData([FromBody] ProductDetailsDTO model)
        {
            if (model.products == null )
            {
                return BadRequest("Data is null");
            }
            List<product> lstproduct = new List<product>();
            if (model.products != null)
            {
                foreach (var s in model.products)
                {
                    product obj = new product();
                    obj.productname = s.productname;
                    obj.sku = s.sku;
                    obj.qty = s.qty;
                    obj.price = s.price;
                    obj.userid = model.userid;
                    obj.isenable = true;
                    lstproduct.Add(obj);

                    _context.products.Add(obj);
                    await _context.SaveChangesAsync();              

                    product_information obj1 = new product_information();
                    obj1.table_product = await _context.products.FindAsync(obj.id); 
                    obj1.product_category = s.product_category;
                    obj1.description = s.description;
                    
                    _context.productinformation.Add(obj1);
                    await _context.SaveChangesAsync();
                }
            }
            return Ok(new Response { Status = "Success", Message = "Data Saved Successfully!" });

        }

        [HttpGet]
        [Route("GetProductList/{userid}")]
        public async Task<List<ProductListDTO>> GetProductList(string userid)
        {
            List<ProductListDTO> lstproductlistdto = new List<ProductListDTO>();
            var productdata = await _context.products.Where(x => x.userid == userid).ToListAsync();
            foreach (var s in productdata)
            {
                var productinfodata = await _context.productinformation.Where(x => x.table_product.id == s.id).SingleOrDefaultAsync();

                ProductListDTO obj = new ProductListDTO();
                obj.productname = s.productname;
                obj.qty = s.qty;
                obj.sku = s.sku;
                obj.price = s.price;
                obj.description = productinfodata.description;
                obj.product_category = productinfodata.product_category;
                lstproductlistdto.Add(obj);
            }
            return lstproductlistdto;
        }

        [HttpPut]
        [Route("PutEnableDisableProduct/{productid}/{productstatus}")]
        public async Task<IActionResult> PutEnableDisableProduct(int productid,bool productstatus)
        {
            var _product = await _context.products
                     .Where(x => x.id == productid)
                     .SingleOrDefaultAsync();
            if (_product == null)
            {
                return BadRequest();
            }
            _product.isenable = productstatus;
            _context.Entry(_product).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();

                return Ok(new Response { Status = "Success", Message = "Status Changed Successfully!" });
            }
            catch (DbUpdateConcurrencyException ex)
            {
                if (!ProductExists(productid))
                {
                    return NotFound();
                }
                else
                {
                    return Ok(new Response { Status = "Failed", Message = "Failed to update Error :" + ex.Message });
                }
            }
        }

        private bool ProductExists(int id)
        {
            return _context.products.Any(e => e.id == id);
        }

        
    }

    #region DTO
    public class RegistrationDTO
    {
        public string name { get; set; }
        public string emailid { get; set; }
    }

    public class ProductDetailsDTO
    {
        public string userid { get; set; }
        public productDTO[] products { get; set; }       
    }

    public class productDTO
    {
        public string productname { get; set; }
        public decimal qty { get; set; }
        public string sku { get; set; }
        public decimal price { get; set; }
        public string description { get; set; }
        public string product_category { get; set; }
    }

    public class ProductListDTO
    {
        public string productname { get; set; }
        public decimal qty { get; set; }
        public string sku { get; set; }
        public decimal price { get; set; }
        public string description { get; set; }
        public string product_category { get; set; }
    }
    #endregion
}
