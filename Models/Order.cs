namespace GoodHamburger.Models
{
    public class Order
    {
        public int Id { get; set; }
        public Sandwich Sandwich { get; set; }
        public List<Extra> Extras { get; set; } = new List<Extra>();
        public double TotalAmount { get; set; }

        //constructor

        public Order()
        {
        }

        public Order(int id, Sandwich sandwich, double totalAmount)
        {
            Id = id;
            Sandwich = sandwich;
            TotalAmount = totalAmount;
        }

        // Method to add an extra to the list
        public void AddExtra(Extra extra)
        {
            // Check for duplicates
            if (Extras.Any(e => e.Name.Equals(extra.Name, StringComparison.OrdinalIgnoreCase)))
                throw new InvalidOperationException($"The extra '{extra.Name}' is already in the order.");

            Extras.Add(extra);
        }

        // Method to remove an extra from the list
        public void RemoveExtra(Extra extra)
        {
            Extras.Remove(extra);
        }

        // Method to update an extra in the list
        public void UpdateExtra(Extra oldExtra, Extra newExtra)
        {
            // Check if the oldExtra exists in the list
            if (!Extras.Contains(oldExtra))
                throw new InvalidOperationException($"The extra '{oldExtra.Name}' does not exist in the order.");

            // Check for duplicates with the newExtra
            if (Extras.Any(e => e.Name.Equals(newExtra.Name, StringComparison.OrdinalIgnoreCase) && e != oldExtra))
                throw new InvalidOperationException($"The extra '{newExtra.Name}' is already in the order.");

            // Replace the oldExtra with the newExtra
            Extras[Extras.IndexOf(oldExtra)] = newExtra;
        }
    }
}
