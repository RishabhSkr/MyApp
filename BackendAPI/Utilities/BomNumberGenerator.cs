using BackendAPI.Data;
using Microsoft.EntityFrameworkCore;

namespace BackendAPI.Utilities;

/// <summary>
/// Auto-generates BOM Numbers
/// Format: BOM-{PRODUCTCODE}-{XXXX} (e.g., BOM-SWA-0001)
/// </summary>
public static class BomNumberGenerator
{
    /// <summary>
    /// Generate BOM number using product code
    /// </summary>
    public static async Task<string> GenerateAsync(AppDbContext context, string productCode)
    {
        // Uppercase the code for consistency
        string code = productCode.Trim().ToUpper();

        string prefix = $"BOM-{code}-";

        // Find highest serial for this product code prefix
        var lastBom = await context.BOMs
            .Where(b => b.BomNumber.StartsWith(prefix))
            .OrderByDescending(b => b.BomNumber)
            .Select(b => b.BomNumber)
            .FirstOrDefaultAsync();

        int nextSerial = 1;
        if (lastBom != null)
        {
            // Extract serial: "BOM-SWA-0003" → "0003" → 3 → next = 4
            var lastPart = lastBom.Split('-').Last();
            if (int.TryParse(lastPart, out int lastSerial))
            {
                nextSerial = lastSerial + 1;
            }
        }

        return $"{prefix}{nextSerial:D4}";
    }
}
