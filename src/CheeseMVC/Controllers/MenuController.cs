using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using CheeseMVC.Models;
using CheeseMVC.ViewModels;
using CheeseMVC.Data;
using Microsoft.EntityFrameworkCore;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace CheeseMVC.Controllers
{
    public class MenuController : Controller
    {
        private CheeseDbContext context;

        public MenuController(CheeseDbContext dbContext)
        {
            context = dbContext;
        }

        public IActionResult Index()
        {
            List<Menu> menus = context.Menus.ToList();

            return View(menus);
        }

        public IActionResult Add()
        {
            AddMenuViewModel addMenuViewModel = new AddMenuViewModel();
            return View(addMenuViewModel);
        }

        [HttpPost]
        public IActionResult Add(AddMenuViewModel addMenuViewModel)
        {
            if (ModelState.IsValid)
            {

                Menu newMenu = new Menu
                {
                    Name = addMenuViewModel.Name,
                };

                context.Menus.Add(newMenu);
                context.SaveChanges();

                return Redirect($"/Menu/ViewMenu/{newMenu.ID}");
            }

            return View(addMenuViewModel);
        }

        public IActionResult Remove()
        {
            ViewBag.menus = context.Menus.ToList();
            return View();
        }

        [HttpPost]
        public IActionResult Remove(int menuId)
        {
            Menu theMenu = context.Menus.Single(m => m.ID == menuId);
            context.Menus.Remove(theMenu);
            context.SaveChanges();

            return Redirect("/Menu");
        }

        public IActionResult ViewMenu(int id)
        {
            Menu menu = context.Menus.Single(m => m.ID == id);

            List<CheeseMenu> items = context
                .CheeseMenus
                .Include(item => item.Cheese)
                .Where(cm => cm.MenuID == id)
                .ToList();

            ViewMenuViewModel viewMenuViewModel = new ViewMenuViewModel
            {
                Menu = menu,
                Items = items
            };

            return View(viewMenuViewModel);

        }

        public IActionResult AddItem(int id)
        {
            Menu menu = context.Menus.Single(m => m.ID == id);
            AddMenuItemViewModel addMenuItemViewModel = new AddMenuItemViewModel(menu, context.Cheeses.ToList());

            return View(addMenuItemViewModel);
        }

        [HttpPost]
        public IActionResult AddItem(AddMenuItemViewModel addMenuItemViewModel)
        {
            if (ModelState.IsValid)
            {

                IList<CheeseMenu> existingItems = context.CheeseMenus
                    .Where(cm => cm.CheeseID == addMenuItemViewModel.CheeseID)
                    .Where(cm => cm.MenuID == addMenuItemViewModel.MenuID).ToList();

                if (existingItems.Count == 0)
                {
                    CheeseMenu newCheeseMenu = new CheeseMenu
                    {
                        CheeseID = addMenuItemViewModel.CheeseID,
                        MenuID = addMenuItemViewModel.MenuID
                    };
                    context.CheeseMenus.Add(newCheeseMenu);
                    context.SaveChanges();

                }

                return Redirect($"/Menu/ViewMenu/{addMenuItemViewModel.MenuID}");

            }

            return View(addMenuItemViewModel);
        }

        public IActionResult RemoveItem(int id)
        {
            Menu menu = context.Menus.Single(m => m.ID == id);

            List<CheeseMenu> items = context
                .CheeseMenus
                .Include(item => item.Cheese)
                .Where(cm => cm.MenuID == id)
                .ToList();

            RemoveMenuItemViewModel removeMenuItemViewModel = new RemoveMenuItemViewModel
            {
                Menu = menu,
                Items = items
            };

            return View(removeMenuItemViewModel);
        }

        [HttpPost]
        public IActionResult RemoveItem(int[] cheeseIds, int menuId)
        {
            foreach (int cheeseId in cheeseIds)
            {
                /* ???? See if it can be done simpler than below, but this does not function as is
                CheeseMenu theCheeseMenu = context.CheeseMenus.Single(cm => cm.CheeseID == cheeseId & cm.MenuID == menuId);
                context.CheeseMenus.Remove(theCheeseMenu);
                */

                
                List<CheeseMenu> cheeseMenus = context.CheeseMenus
                    .Where(cm => cm.CheeseID == cheeseId)
                    .Where(cm => cm.MenuID == menuId)
                    .ToList();
                if (cheeseMenus.Count != 0)
                {
                    CheeseMenu theCheeseMenu = cheeseMenus[0];
                    context.CheeseMenus.Remove(theCheeseMenu);
                }
                

            }

            context.SaveChanges();

            return Redirect($"/Menu/ViewMenu/{menuId}");
        }
    
    }
}
