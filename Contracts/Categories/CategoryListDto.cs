namespace BackendAPI.Contracts.Categories
{
    public class CategoryListDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public string Image { get; set; } = "";
        public string Slug { get; set; } = "";
    }
}