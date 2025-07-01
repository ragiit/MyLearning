using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NDP.Models.ViewModels
{
    // Models/HomeViewModel.cs
    public class HomeViewModel
    {
        public List<Product> LatestProducts { get; set; }
        public List<Portfolio> Portfolios { get; set; }
    }

    // Models/Product.cs
    public class Product
    {
        // Properti sama seperti di Blazor: Id, Name, Slug, Price, ImageUrl, etc.
    }

    // Models/Portfolio.cs
    public class Portfolio
    {
        // Properti sama seperti di Blazor: Id, Name, Description, ImageUrl, etc.
    }
}