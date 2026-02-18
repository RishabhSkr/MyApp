namespace BackendAPI.Utilities;

/// <summary>
/// Max-Min K Batch Optimizer — Machine Efficiency Maximizer
/// 
/// Goal: Divide total production qty into N batches where:
///   - Each batch's efficiency (batchQty / machineCapacity) is MAXIMIZED
///   - Every batch >= 50% efficiency (MIN threshold)
///   - Target >= 95% efficiency (OPTIMAL threshold)
///   - Even distribution preferred over uneven (no [300,300,300,100] waste)
/// </summary>
public static class BatchOptimizer
{
    public const decimal MIN_EFFICIENCY_THRESHOLD = 50m;      // 50%
    public const decimal OPTIMAL_EFFICIENCY_THRESHOLD = 95m;  // 95%

    /// <summary>
    /// Main method: finds optimal N batches with even distribution
    /// Returns BatchSuggestion with individual batch sizes and efficiency
    /// </summary>
    public static BatchSuggestion GetOptimalBatchPlan(decimal totalQty, decimal machineCapacity)
    {
        if (totalQty <= 0 || machineCapacity <= 0)
            return new BatchSuggestion { TotalQty = totalQty, MachineCapacity = machineCapacity };

        // Step 1: Minimum batches needed
        int n = (int)Math.Ceiling(totalQty / machineCapacity);

        // Step 2: Even distribution — maximize minimum batch size
        var batchSizes = DistributeEvenly(totalQty, n);

        // Step 3: Calculate efficiencies
        decimal minBatch = batchSizes.Min();
        decimal maxBatch = batchSizes.Max();
        decimal minEfficiency = GetEfficiency(minBatch, machineCapacity);
        decimal avgEfficiency = GetEfficiency(totalQty / n, machineCapacity);

        return new BatchSuggestion
        {
            TotalQty = totalQty,
            MachineCapacity = machineCapacity,
            SuggestedBatches = n,
            SuggestedBatchSize = maxBatch,       // largest batch (for reference)
            BatchSizes = batchSizes,
            MinEfficiency = minEfficiency,        // worst batch efficiency
            AvgEfficiency = avgEfficiency,        // average efficiency
            IsOptimal = minEfficiency >= OPTIMAL_EFFICIENCY_THRESHOLD,
            IsAcceptable = minEfficiency >= MIN_EFFICIENCY_THRESHOLD
        };
    }

    /// <summary>
    /// Distributes totalQty evenly across n batches
    /// Example: 1000 / 3 = [334, 333, 333] (not [400, 400, 200])
    /// </summary>
    public static List<decimal> DistributeEvenly(decimal totalQty, int n)
    {
        if (n <= 0) return new List<decimal> { totalQty };

        decimal baseSize = Math.Floor(totalQty / n);
        decimal remainder = totalQty - (baseSize * n);

        var batches = new List<decimal>();
        for (int i = 0; i < n; i++)
        {
            // First 'remainder' batches get +1 extra unit
            batches.Add(i < remainder ? baseSize + 1 : baseSize);
        }
        return batches;
    }

    /// <summary>
    /// Calculates efficiency % for a single batch
    /// efficiency = (batchQty / machineCapacity) × 100
    /// </summary>
    public static decimal GetEfficiency(decimal batchQty, decimal machineCapacity)
    {
        if (machineCapacity <= 0) return 0;
        return Math.Round((batchQty / machineCapacity) * 100, 2);
    }

    /// <summary>
    /// Binary Search: Find max K (minimum batch fill) across given batches
    /// Used for non-uniform machine capacities (future extension)
    /// </summary>
    public static decimal FindMaxMinK(decimal D, List<decimal> batches)
    {
        if (D <= 0 || !batches.Any()) return 0;

        decimal low = 1;
        decimal high = D;
        decimal ans = 0;

        while (low <= high)
        {
            decimal mid = Math.Floor((low + high) / 2);

            if (CanDistribute(mid, D, batches))
            {
                ans = mid;
                low = mid + 1;
            }
            else
            {
                high = mid - 1;
            }
        }
        return ans;
    }

    private static bool CanDistribute(decimal K, decimal D, List<decimal> batches)
    {
        if (K == 0) return true;

        var validBatches = batches.Where(m => m >= K).OrderByDescending(m => m).ToList();
        if (!validBatches.Any()) return false;

        int maxAllowed = (int)(D / K);
        if (maxAllowed == 0) return false;

        var chosen = validBatches.Take(maxAllowed).ToList();
        return chosen.Sum() >= D;
    }
}

/// <summary>
/// Result of batch optimization
/// </summary>
public class BatchSuggestion
{
    public decimal TotalQty { get; set; }
    public decimal MachineCapacity { get; set; }
    public int SuggestedBatches { get; set; } = 1;
    public decimal SuggestedBatchSize { get; set; }      // Reference (max batch)
    public List<decimal> BatchSizes { get; set; } = new();  // Actual sizes per batch
    public decimal MinEfficiency { get; set; }   // Worst batch efficiency %
    public decimal AvgEfficiency { get; set; }   // Average efficiency %
    public bool IsOptimal { get; set; }          // Min efficiency >= 95%
    public bool IsAcceptable { get; set; }       // Min efficiency >= 50%
}
