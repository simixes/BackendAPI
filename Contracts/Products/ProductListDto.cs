namespace BackendAPI.Contracts.Products;

public class ProductListDto
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string Description { get; set; } = "";
    public int Price { get; set; }
    public string Image { get; set; } = "";
    public string UrlSlug { get; set; } = "";
}