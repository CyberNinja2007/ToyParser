using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AngleSharp;

namespace Classes
{
    public class ToysParser
    {
        private string _domain;
        private readonly IBrowsingContext _context;

        public ToysParser()
        {
            var configuration = new Configuration().WithDefaultLoader();
            _context = BrowsingContext.New(configuration);
        }

        public async Task<List<ToyInfo>> Parse(string url)
        {
            _domain = url.Split("/")[2];
            
            var document = await _context.OpenAsync(url);
            var pages = document.QuerySelectorAll("a.page-link")
                .Select(x => _domain + x.GetAttribute("href")).ToArray();
            
            List<ToyInfo> toys = new List<ToyInfo>();
            
            foreach (var page in pages)
            { 
                toys.AddRange(await ParseToys(page));
            }

            return toys;
        }

        private async Task<List<ToyInfo>> ParseToys(string url)
        {
            List<ToyInfo> toys = new List<ToyInfo>();
            
            var document = await _context.OpenAsync(url);
            var products = document.QuerySelectorAll("div.product-card > div.row > div.col-12 > a.product-name")
                .Select(x => _domain + x.GetAttribute("href")).ToArray();

            foreach (var product in products)
            {
                toys.Add(await ParseToy(product));
            }
            
            return toys;
        }

        private async Task<ToyInfo> ParseToy(string url)
        {
            var document = await _context.OpenAsync(url);
            
            ToyInfo toy = new ToyInfo();
            
            toy.Region = document.QuerySelector("div.select-city-link > a").TextContent;
            toy.Breadcrumbs = document.QuerySelectorAll("breadcrumb-item")
                .Select(x => _domain + x.GetAttribute("href")).ToArray();
            toy.Name = document.QuerySelector("h1.detail-name").TextContent;
            toy.PriceOld = document.QuerySelector("span.old-price").TextContent;
            toy.PriceNew = document.QuerySelector("span.price").TextContent;
            toy.Availability = document.QuerySelector("span.ok").TextContent;
            toy.Pictures = document.QuerySelectorAll("div.slick-slide > img-fluid").Select(x => _domain + x.GetAttribute("href")).ToArray();
            toy.Url = url;
            
            return toy;
        }
    }
}