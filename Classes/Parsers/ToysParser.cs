using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AngleSharp;
using Classes.Parsers.Abstraction;
using Classes.Toys;

namespace Classes.Parsers
{
    public class ToysParser : IToysParser
    {
        private string _domain;
        private readonly IConfiguration _configuration;
        private readonly List<ToyInfo> _toys;

        public ToysParser()
        {
            _configuration = Configuration.Default.WithDefaultLoader();
            _toys = new List<ToyInfo>();
        }

        public async Task<List<ToyInfo>> ParseAsync(string url)
        {
            var splittedUrl = url.Split("/").Take(3);
            _domain = string.Join("/", splittedUrl);

            var document = await BrowsingContext.New(_configuration).OpenAsync(url);
            var pages = document.QuerySelectorAll("a.page-link").ToArray();
            var pageCount = Convert.ToInt32(pages[^2].TextContent);
            var pageLink = _domain + pages[^1].GetAttribute("href");
            var pageLinks = new string[pageCount];

            for (int i = 1; i <= pageCount; i++)
            {
                pageLinks[i - 1] = pageLink.Remove(pageLink.Length - 1, 1) + i;
            }

            foreach (var page in pageLinks)
            {
                await ParseToysAsync(page);
            }

            return _toys;
        }

        private async Task ParseToysAsync(string url)
        {
            var toys = new List<ToyInfo>();
            var document = await BrowsingContext.New(_configuration).OpenAsync(url);
            var products = document.QuerySelectorAll("div.product-card > div.row > div.col-12 > a.product-name")
                .Select(x => _domain + x.GetAttribute("href")).ToArray();

            foreach (var product in products)
            {
                toys.Add(await ParseToyAsync(product));
            }

            _toys.AddRange(toys);
        }

        private async Task<ToyInfo> ParseToyAsync(string url)
        {
            var configuration = new Configuration().WithDefaultLoader();
            var document = await BrowsingContext.New(configuration).OpenAsync(url);

            ToyInfo toy = new ToyInfo
            {
                Region = document.QuerySelector("div.select-city-link > a").TextContent.Trim(),
                Name = document.QuerySelector("h1.detail-name").TextContent,
                PriceNew = document.QuerySelector("span.price").TextContent,
                Availability = document.QuerySelector("span.ok").TextContent,
                Breadcrumbs = document.QuerySelectorAll("a.breadcrumb-item")
                    .Select(x => _domain + x.GetAttribute("href")).ToArray(),
                Pictures = document.QuerySelectorAll("img.img-fluid")
                    .Select(x => x.GetAttribute("src")).Skip(3).ToArray(),
                Url = url
            };

            try
            {
                toy.PriceOld = document.QuerySelector("span.old-price").TextContent;
            }
            catch
            {
                toy.PriceOld = document.QuerySelector("span.price").TextContent;
            }

            return toy;
        }
    }
}