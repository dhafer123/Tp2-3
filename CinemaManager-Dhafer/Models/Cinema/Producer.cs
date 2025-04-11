using System.Collections.Generic;

namespace CinemaManager_Dhafer.Models.Cinema
{
    public class Producer
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Nationality { get; set; }
        public string Email { get; set; }

        // Add this navigation property
        public virtual ICollection<Movie> Movies { get; set; }
    }
}