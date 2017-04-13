using CheeseMVC.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CheeseMVC.ViewModels
{
    public class ViewCategoryViewModel
    {
        public CheeseCategory Category { get; set; }
        public IList<Cheese> Cheeses { get; set; }
    }
}
