namespace BackendAPI.Utilities;

/// <summary>
/// Optimal Batch Allocation — Greedy + Borrowing Algorithm
/// 
/// Goal: Divide D (total qty) into batches of max M (machine capacity) where:
///   1. No batch exceeds M (capacity constraint)
///   2. Every batch >= 50% of M (minimum utilization)
///   3. MAXIMIZE number of batches at exactly 100% (M)
///   4. If remainder < 50%, borrow from last full batch to redistribute
///
/// Time Complexity: O(N) where N = number of batches
/// Space Complexity: O(N) for the result array
/// </summary>
public static class BatchOptimizer
{
    public const decimal MIN_EFFICIENCY_THRESHOLD = 50m;      // 50%
    public const decimal OPTIMAL_EFFICIENCY_THRESHOLD = 95m;  // 95%

    /// <summary>
    /// Main method: Greedy allocation with borrowing fallback
    /// Maximizes 100% capacity batches, borrows to fix remainder
    /// </summary>
    public static BatchSuggestion GetOptimalBatchPlan(decimal totalQty, decimal machineCapacity)
    {
        if (totalQty <= 0 || machineCapacity <= 0)
            return new BatchSuggestion();

        var batchSizes = OptimalAllocation(totalQty, machineCapacity);

        decimal minBatch = batchSizes.Min();
        decimal minEfficiency = GetEfficiency(minBatch, machineCapacity);
        int fullBatches = batchSizes.Count(b => b == machineCapacity);

        return new BatchSuggestion
        {
            SuggestedBatches = batchSizes.Count,
            SuggestedBatchSize = batchSizes.Max(),
            BatchSizes = batchSizes,
            MinEfficiency = minEfficiency,
            FullCapacityBatches = fullBatches
        };
    }

    /// <summary>
    /// Core Algorithm: Greedy + Borrow
    /// 
    /// 1. Fill as many full batches (M) as possible
    /// 2. If remainder >= 50% of M → add as last batch ✅
    /// 3. If remainder < 50% of M → borrow from last full batch, redistribute ✅
    /// 4. Edge case: D < 50% of M → single batch [D] ✅
    /// </summary>
    public static List<decimal> OptimalAllocation(decimal D, decimal M)
    {
        var result = new List<decimal>();

        // 50% threshold (ceil for odd M)
        decimal minReq = Math.Ceiling(M / 2);

        // Edge Case: total qty is less than minimum threshold
        if (D < minReq)
        {
            result.Add(D);
            return result;
        }

        int q = (int)(D / M);       // Number of full 100% batches
        decimal R = D % M;          // Remainder

        if (R == 0)
        {
            // Perfect division — all batches at 100%
            for (int i = 0; i < q; i++)
                result.Add(M);
        }
        else if (R >= minReq)
        {
            // Remainder passes 50% rule — no borrowing needed
            for (int i = 0; i < q; i++)
                result.Add(M);
            result.Add(R);
        }
        else
        {
            // *** BORROWING ALGORITHM ***
            // Remainder too small → sacrifice ONE full batch
            for (int i = 0; i < q - 1; i++)
                result.Add(M);

            // Pool the last full batch + remainder
            decimal pool = M + R;

            // Split: keep first as high as possible, last at minimum
            result.Add(pool - minReq);
            result.Add(minReq);
        }

        // Return in descending order
        result.Sort((a, b) => b.CompareTo(a));
        return result;
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
}

/// <summary>
/// Result of batch optimization
/// </summary>
public class BatchSuggestion
{
    public int SuggestedBatches { get; set; } = 1;
    public decimal SuggestedBatchSize { get; set; }
    public List<decimal> BatchSizes { get; set; } = new();
    public decimal MinEfficiency { get; set; }          // Warning ke liye
    public int FullCapacityBatches { get; set; }        // Kitne 100% pe hain
}
