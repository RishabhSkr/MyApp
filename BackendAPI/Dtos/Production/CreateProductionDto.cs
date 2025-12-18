using System.ComponentModel.DataAnnotations;

namespace BackendAPI.Dtos.Production
{
    public class CreateProductionDto
    {
        [Required]
        public int SalesOrderId { get; set; }
    }
}