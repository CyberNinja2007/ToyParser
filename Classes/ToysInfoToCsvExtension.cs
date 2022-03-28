using System.Text;

namespace Classes
{
    public static class ToysInfoToCsvExtension
    {
        public static string ToCsv(this ToyInfo toy)
        {
            StringBuilder result = new StringBuilder(toy.Url + ';' + toy.Region + ';');
            
            foreach (var breadcrumb in toy.Breadcrumbs)
            {
                result.Append(breadcrumb + ' ');
            }

            result.Append(';' + toy.Name + ';');

            foreach (var picture in toy.Pictures)
            {
                result.Append(picture + ' ');
            }

            result.Append(';' + toy.PriceOld + ';' + toy.PriceNew + ';' + toy.Availability);

            return result.ToString();
        }
    }
}