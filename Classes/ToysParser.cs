using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AngleSharp;

namespace Classes
{
    public class ToysParser
    {
        private string _domain;
        private IConfiguration _configuration;
        private readonly IBrowsingContext _context;

        public ToysParser()
        {
            _configuration = new Configuration().WithDefaultLoader();
        }

        public async Task<List<ToyInfo>> Parse(string url)
        {
            var splittedUrl = url.Split("/").Take(3);
            _domain = string.Join("/",splittedUrl);

            var document = await BrowsingContext.New(_configuration).OpenAsync(url);
            var pages = document.QuerySelectorAll("a.page-link").ToArray();
            var pageCount = Convert.ToInt32(pages[^2].TextContent);
            var pageLink = _domain + pages[^1].GetAttribute("href");
            string[] pageLinks = new string[pageCount];
            
            for (int i = 1; i <= pageCount; i++)
            {
                pageLinks[i-1] = pageLink.Remove(pageLink.Length - 1,1) + i;
            }
            
            List<ToyInfo> toys = new List<ToyInfo>();
            
            foreach (var page in pageLinks)
            { 
                toys.AddRange(await ParseToys(page));
            }

            return toys;
        }

        private async Task<List<ToyInfo>> ParseToys(string url)
        {
            List<ToyInfo> toys = new List<ToyInfo>();
            
            var document = await BrowsingContext.New(_configuration).OpenAsync(url);
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
            var configuration = new Configuration().WithDefaultLoader();
            var document = await BrowsingContext.New(configuration).OpenAsync(url);

            ToyInfo toy = new ToyInfo();

            toy.Region = document.QuerySelector("div.select-city-link > a").TextContent.Trim();
            toy.Breadcrumbs = document.QuerySelectorAll("breadcrumb-item")
                .Select(x => _domain + x.GetAttribute("href")).ToArray();
            toy.Name = document.QuerySelector("h1.detail-name").TextContent;

            try
            {
                toy.PriceOld = document.QuerySelector("span.old-price").TextContent;
            }
            catch
            {
                toy.PriceOld = document.QuerySelector("span.price").TextContent;
            }

            toy.PriceNew = document.QuerySelector("span.price").TextContent;
            toy.Availability = document.QuerySelector("span.ok").TextContent;
            toy.Pictures = document.QuerySelectorAll("div.slick-slide > img.img-fluid")
                .Select(x => x.GetAttribute("href")).ToArray();
            toy.Url = url;
            
            return toy;
        }
    }
}