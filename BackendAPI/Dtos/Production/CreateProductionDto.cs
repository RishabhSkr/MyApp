using System.ComponentModel.DataAnnotations;

namespace BackendAPI.Dtos.Production
{
    public class CreateProductionDto
    {
        [Required]
        public int SalesOrderId { get; set; }
        [Required]
        public DateTime StartDate { get; set; }
        [Required]
        public DateTime EndDate { get; set; }
    }
}