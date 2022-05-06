using Rocky.Models;

namespace Rocky.ViewModels
{
    public class DetailsVM
    {
        public DetailsVM()
        {
            Product = new Product();
        }

        public Product Product { get; set; }

        public bool ExistsInCart { get; set; }
    }
}
