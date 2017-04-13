using Microsoft.AspNetCore.Mvc;
using CheeseMVC.Models;
using System.Collections.Generic;
using CheeseMVC.ViewModels;
using CheeseMVC.Data;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace CheeseMVC.Controllers
{
    public class CheeseController : Controller
    {
        private CheeseDbContext context;

        public CheeseController(CheeseDbContext dbContext)
        {
            context = dbContext;
        }

        // GET: /<controller>/
        public IActionResult Index()
        {
            IList<Cheese> cheeses = context.Cheeses.Include(c => c.Category).ToList();
           
            return View(cheeses);
        }

        public IActionResult Add()
        {
            AddCheeseViewModel addCheeseViewModel = new AddCheeseViewModel(context.Categories.ToList());
            return View(addCheeseViewModel);
        }

        [HttpPost]
        public IActionResult Add(AddCheeseViewModel addCheeseViewModel)
        {
            if (ModelState.IsValid)
            {
                CheeseCategory newCheeseCategory =
                    context.Categories.Single(c => c.ID == addCheeseViewModel.CategoryID);

                // Add the new cheese to my existing cheeses
                Cheese newCheese = new Cheese
                {
                    Name = addCheeseViewModel.Name,
                    Description = addCheeseViewModel.Description,
                    Category = newCheeseCategory
                };

                context.Cheeses.Add(newCheese);
                context.SaveChanges();

                return Redirect("/Cheese");
            }
            addCheeseViewModel.AddCategories(context.Categories.ToList());

            return View(addCheeseViewModel);
        }

        public IActionResult Remove()
        {
            ViewBag.cheeses = context.Cheeses.ToList();
            return View();
        }

        [HttpPost]
        public IActionResult Remove(int[] cheeseIds)
        {
            foreach (int cheeseId in cheeseIds)
            {
                Cheese theCheese = context.Cheeses.Single(c => c.ID == cheeseId);
                context.Cheeses.Remove(theCheese);
            }

            context.SaveChanges();

            return Redirect("/");
        }

        public IActionResult Edit(int id)
        {
            Cheese editCheese = context.Cheeses.Single(c => c.ID == id);
            AddCheeseViewModel addCheeseViewModel = new AddCheeseViewModel
            {
                Name = editCheese.Name,
                Description = editCheese.Description,
                CheeseId = editCheese.ID,
                Title = "Edit"
            };
            addCheeseViewModel.AddCategories(context.Categories.ToList());

            return View("Add", addCheeseViewModel);
        }

        [HttpPost]
        public IActionResult Edit(int id, AddCheeseViewModel addCheeseViewModel)
        {

            if (ModelState.IsValid)
            {
                Cheese editCheese = context.Cheeses.Single(c => c.ID == id);
                editCheese.Name = addCheeseViewModel.Name;
                editCheese.Description = addCheeseViewModel.Description;
                editCheese.Category = context.Categories.Single(c => c.ID == addCheeseViewModel.CategoryID);
                context.SaveChanges();

                return Redirect("/Cheese");
            }
            addCheeseViewModel.AddCategories(context.Categories.ToList());

            return View("Add", addCheeseViewModel);

        }
    }
}
