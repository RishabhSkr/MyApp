namespace BackendAPI.Dtos.Bom
{
    public class BomResponseDto
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public List<BomItemDto> BomItems { get; set; }= new ();
    }
}