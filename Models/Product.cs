namespace GoodHamburger.Models
{
    public class Product    
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public double Price { get; set; }

        //constructor
        public Product()
        {
        }

        public Product( string name, double price)
        {            
            Name = name;
            Price = price;
        }
    }
}
