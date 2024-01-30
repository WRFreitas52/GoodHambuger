namespace GoodHamburger.Models
{
    public class Extra : Product
    {

        public bool IsDrink { get; set; }

        public Extra()
        {
        }
        public Extra(string name, double price) : base(name, price)
        {
        }
        public Extra(bool isDrink)
        {
            IsDrink = isDrink;
        }

     

    }
}
