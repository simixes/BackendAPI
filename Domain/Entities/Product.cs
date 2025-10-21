namespace BackendAPI.Domain.Entities
{
    public class Product
    {
        public int Id { get; set; }                // Unikt ID i databasen (auto-genereras)
        public string Name { get; set; } = "";     // Produktnamn
        public string Description { get; set; } = ""; // Beskrivning
        public int Price { get; set; }             // Pris i kronor
        public string Image { get; set; } = "";    // Bildväg, t.ex. "/images/t-shirt.png"
        public string UrlSlug { get; set; } = "";  // URL-slug, t.ex. "t-shirt"

        public ICollection<Category> Categories { get; set; } = new List<Category>();
    }
}
