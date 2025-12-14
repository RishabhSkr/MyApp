using System.ComponentModel.DataAnnotations;

namespace BackendAPI.Dtos.Bom
{
    public class BomCreateDto
    {
        public int ProductId { get; set; }
        // list of Materials
        [Required]
        [MinLength(1)]
        public List<BomItemDto> BomItems { get; set; }= new ();
    }
}