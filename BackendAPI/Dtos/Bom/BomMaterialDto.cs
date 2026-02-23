namespace BackendAPI.Dtos.Bom;

public class BomMaterialDto
{
    public string RawMaterialName { get; set; } = string.Empty;
    public decimal QuantityRequired { get; set; }
    public string Uom { get; set; } = string.Empty;
}
