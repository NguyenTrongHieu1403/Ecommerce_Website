using ecommerce_final.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ecommerce_final.Models
{
    public class ProductCreateViewModel
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Summary { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public int Sold { get; set; }
        public int TaxonomyId { get; set; }
        public int DiscountId { get; set; }
        public bool IsNew { get; set; }
        public IFormFile Img { get; set; }
        public IEnumerable<SelectListItem> Taxonomies { get; set; }
        public IEnumerable<SelectListItem> Discounts { get; set; }
        public IEnumerable<Attribute> Attributes { get; set; }
        public List<int> AttributeValues { get; set; }
    }


}
