using BackendAPI.Data;
using Microsoft.EntityFrameworkCore;

namespace BackendAPI.Utilities;

/// <summary>
/// Auto-generates human-readable Order Numbers
/// Format: PO-YYYYMMDD-XXXX (e.g., PO-20260221-0001)
/// </summary>
public static class OrderNumberGenerator
{
    /// <summary>
    /// Generate next order number for today
    /// Finds the last order created today and increments the serial
    /// </summary>
    public static async Task<string> GenerateAsync(AppDbContext context)
    {
        string today = DateTime.Now.ToString("yyyyMMdd");
        string prefix = $"PO-{today}-";

        // Find the highest serial number for today
        var lastOrder = await context.ProductionOrders
            .Where(p => p.OrderNumber.StartsWith(prefix))
            .OrderByDescending(p => p.OrderNumber)
            .Select(p => p.OrderNumber)
            .FirstOrDefaultAsync();

        int nextSerial = 1;
        if (lastOrder != null)
        {
            // Extract serial: "PO-20260221-0003" → "0003" → 3 → next = 4
            var parts = lastOrder.Split('-');
            if (parts.Length == 3 && int.TryParse(parts[2], out int lastSerial))
            {
                nextSerial = lastSerial + 1;
            }
        }

        return $"{prefix}{nextSerial:D4}"; // D4 = 4 digit padding (0001, 0002...)
    }
}
