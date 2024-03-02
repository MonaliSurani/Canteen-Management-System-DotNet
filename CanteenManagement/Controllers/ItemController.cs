using CanteenManagement.Models;
using CanteenManagement.ViewModels;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace CanteenManagement.Controllers
{
    public class ItemController : Controller
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly Cart _cart;
        public ItemController(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
            _cart = new Cart();
        }
        public IActionResult AllItems()
        {
            var items= _dbContext.Item.ToList();
            return View(items);
        }

        public IActionResult AddItem(Items item)
        {
            if (ModelState.IsValid)
            {
                Item itm = new Item() { Name = item.Name, Price = item.Price, AvailableItems = item.AvailableItems };

                var addResult = _dbContext.Add(itm);
                _dbContext.SaveChanges();
            }
            return RedirectToAction("AllItems", "Item");
        }

        [HttpGet]
        public async Task<IActionResult> DeleteItem(int Id)
        {
            var item =_dbContext.Item.FirstOrDefault(x => x.Id == Id);
            var result = _dbContext.Item.Remove(item);
            var res= _dbContext.SaveChanges();
            return RedirectToAction("AllItems", "Item");
        }

        [HttpGet]
        public async Task<IActionResult> EditItem(string Id)
        {
            var item = _dbContext.Item.FirstOrDefault(x => x.Id ==Convert.ToInt32(Id));
            Items items = new Items
            {
                Name = item.Name,
                Price = item.Price,
                AvailableItems = item.AvailableItems
            };

            return PartialView("_UpdateItem", items);
        }

        [HttpPost]
        public async Task<IActionResult> EditItem(Items item)
        {
            Item itm = _dbContext.Item.FirstOrDefault(x => x.Id == item.Id);
            itm.Name = item.Name;
            itm.Price = item.Price;
            itm.AvailableItems = item.AvailableItems;

            var update = _dbContext.Item.Update(itm);
            _dbContext.SaveChanges();
            return RedirectToAction("AllItems", "Item");
        }
/*
        private string Upload(UploadImage image)
        {
            string filename = null;
            if (image.ImageFilePath != null)
            {
                //It is setting up the directory
                string uploaddir = Path.Combine(_webHostEnvironment.WebRootPath, "Images");

                //To Generate UniqueImage Name
                filename = Guid.NewGuid().ToString() + "-" + image.ImageFilePath.FileName;

                string filepath = Path.Combine(uploaddir, filename);

                using (var filestream = new FileStream(filepath, FileMode.Create))  //It will create your empty image file as per the given path
                {
                    image.ImageFilePath.CopyTo(filestream);   //This will copy the stream/data of image to be uploaded to empty created file
                }

            }
            return filename;
        }
*/

        public IActionResult AddToCart(int productId)
        {
            var product = _dbContext.Item.Find(productId);

            if (product != null)
            {
                CartItem existingItem = _cart.Items.FirstOrDefault(item => item.Item.Id == productId);

                if (existingItem != null)
                {
                    // If the product is in the cart, increase the quantity
                    existingItem.Quantity++;
                }
                else
                {
                    // If the product is not in the cart, add it with quantity 1
                    _cart.Items.Add(new CartItem { Item = product, Quantity = 1 });
                }

                //var cartItem = new CartItem
                //{
                //    Product = product,
                //    Quantity = 1
                //};

                //// Assume you store the cart in session for simplicity. In a real-world app, you might use a database.
                ////var cart = HttpContext.Session.Get<Cart>("Cart") ?? new Cart();
                //var cartStr = HttpContext.Session.GetString("Cart");
                //var cart = JsonConvert.DeserializeObject<Cart>(cartStr);
                //cart.Items.Add(cartItem);

                //HttpContext.Session.Set("Cart", cart);
            }

            return RedirectToAction("Index");
        }

    }
}
