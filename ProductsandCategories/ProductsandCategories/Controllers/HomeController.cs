using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using LoginAndRegistration.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ProductsandCategories.Models;

namespace ProductsandCategories.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private MyDBContext dbContext;

        public HomeController(ILogger<HomeController> logger, MyDBContext context)
        {
            _logger = logger;
            dbContext = context;
        }

        //###############################################################################
            //Products
        //###############################################################################
        [HttpGet("CreateProducts")]
        public IActionResult CreateProducts()
        {
            return View();
        }
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        [HttpPost("CreateProducts")]
        public IActionResult CreateProducts(Products products)
        {
            dbContext.Productss.Add(products);

            dbContext.SaveChanges();

            return Redirect("ViewProducts");
        }

        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        [HttpGet("ViewProducts")]
        public IActionResult ViewProducts()
        {
            List<Products> products = dbContext.Productss.ToList();

            return View(products);
        }
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        [HttpGet("DetailsProducts/{id}")]
        public IActionResult DetailsProducts(int id)
        {
            Products p = dbContext.Productss
                .Include(pr => pr.Get_Categories)
                .ThenInclude(rn => rn.Categories)
                .FirstOrDefault(po => po.ProductsId == id);

            List<int> kk = p.Get_Categories.Select(gc => gc.CategoriesId).ToList();

            List<Categories> categories = dbContext.Categoriess.Where(c => !kk.Contains( c.CategoriesId) ).ToList();
            
            ViewBag.Categories = categories;

            ViewBag.ProductId = p.ProductsId;

            return View("DetailsProducts",p);
        }
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        [HttpPost("Link_Product_Category")]
         public IActionResult Link_Product_Category(int productsId, int categoriesId)
        {
            Products_and_Categories prod_cat = new Products_and_Categories();
            prod_cat.ProductsId = productsId;
            prod_cat.CategoriesId = categoriesId;
            dbContext.Productss_and_Categoriess.Add(prod_cat);
            dbContext.SaveChanges();

            return Redirect("~/DetailsProducts/"+productsId);

        }
        [HttpGet("Delete_Products/{id}")]
        public IActionResult Delete_Products(int id)
        {
            Products products = new Products() { ProductsId = id };
            dbContext.Productss.Attach(products);
            dbContext.Productss.Remove(products);
            dbContext.SaveChanges();
            return Redirect("~/ViewProducts");
        }
        //###############################################################################
        //Categories
        //###############################################################################

        [HttpGet("CreateCategories")]
        public IActionResult CreateCategories()
        {
            return View();
        }
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        [HttpPost("CreateCategories")]
        public IActionResult CreateCategories(Categories categories)
        {
            dbContext.Categoriess.Add(categories);
            dbContext.SaveChanges();

            return Redirect("ViewCategories");
        }
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        [HttpGet("ViewCategories")]
        public IActionResult ViewCategories()
        {
            List<Categories> categories = dbContext.Categoriess.ToList();

            return View(categories);
        }
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        [HttpGet("DetailsCategories/{id}")]
        public IActionResult DetailsCategories(int id)
        {
            Categories c = dbContext.Categoriess
                .Include(ca => ca.Get_Products)
                .ThenInclude(caa => caa.Products)
                .FirstOrDefault(A => A.CategoriesId == id);

            List<int> dd = c.Get_Products.Select(gp => gp.ProductsId).ToList();

            List<Products> products = dbContext.Productss.Where(pr => !dd.Contains(pr.ProductsId)).ToList();

            ViewBag.Products = products;

            ViewBag.CategoriesId = c.CategoriesId;

            return View("DetailsCategories",c);
        }
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        [HttpPost("Link_Category_Product")]
        public IActionResult Link_Category_Product(int categoriesId, int productsId)
        {
            Products_and_Categories cat_prod = new Products_and_Categories();
            cat_prod.ProductsId = productsId;
            cat_prod.CategoriesId = categoriesId;
            
            dbContext.Productss_and_Categoriess.Add(cat_prod);
            dbContext.SaveChanges();


            return Redirect("~/DetailsCategories/"+ categoriesId);
        }
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        [HttpGet("Delete_Categories/{id}")]
        public IActionResult Delete_Categories(int id)
        {
            Categories categories = new Categories() { CategoriesId = id };
            dbContext.Categoriess.Attach(categories);
            dbContext.Categoriess.Remove(categories);
            dbContext.SaveChanges();
            return Redirect("~/ViewCategories");
        }









        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        [HttpGet("Show_Categories/{_productsId}")]
        public IActionResult Show_Categories(int _ProductsId )
        {
            Products_and_Categories pp = dbContext.Productss_and_Categoriess.Find(_ProductsId);
            pp.Products_and_CategoriesId = _ProductsId;
             
            dbContext.SaveChanges();

            return Redirect("~/DetailsCategories/" + _ProductsId );
        }
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~












        //###############################################################################
        //Register AND  Login
        //###############################################################################
        public IActionResult Index()
        {
            return Redirect("ViewProducts");
        }

        [HttpPost("register")]
        public IActionResult Register(User user)
        {
            // Check initial ModelState
            if (ModelState.IsValid)
            {
                // If a User exists with provided email
                if (dbContext.Users.Any(u => u.Email == user.Email))
                {
                    // Manually add a ModelState error to the Email field, with provided
                    // error message
                    ModelState.AddModelError("Email", "Email already in use!");
                    return View(user);

                    // You may consider returning to the View at this point
                }
                PasswordHasher<User> Hasher = new PasswordHasher<User>();
                user.Password = Hasher.HashPassword(user, user.Password);
                dbContext.Users.Add(user);
                dbContext.SaveChanges();
                return Redirect("login");
            }
            return View(user);
            // other code
        }
        [HttpGet("login")]
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost("login")]
        public IActionResult Login(LoginUser userSubmission)
        {
            if (ModelState.IsValid)
            {
                // If inital ModelState is valid, query for a user with provided email
                var userInDb = dbContext.Users.FirstOrDefault(u => u.Email == userSubmission.Email);
                // If no user exists with provided email
                if (userInDb == null)
                {
                    // Add an error to ModelState and return to View!
                    ModelState.AddModelError("Email", "Invalid Email/Password");
                    return View(userSubmission);
                }

                // Initialize hasher object
                var hasher = new PasswordHasher<LoginUser>();

                // verify provided password against hash stored in db
                var result = hasher.VerifyHashedPassword(userSubmission, userInDb.Password, userSubmission.Password);

                // result can be compared to 0 for failure
                if (result == 0)
                {
                    // handle failure (this should be similar to how "existing email" is handled)
                    ModelState.AddModelError("Email", "Invalid Email/Password");
                    return View(userSubmission);
                }
            }
            return View("Success");
        }

        //###############################################################################
                                    //Privacy AND  Error
        //###############################################################################

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
