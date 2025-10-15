namespace BackendAPI.Contracts.Products;

public class ProductCreateDto
{
    public string Name { get; set; } = "";
    public string Description { get; set; } = "";
    public int Price { get; set; }
    public string Image { get; set; } = "";
}
