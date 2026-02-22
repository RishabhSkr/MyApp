namespace BackendAPI.Constants
{
    public static class EventStatus
    {
        public const string CREATED = "Created";
        public const string RELEASED = "Released";
        public const string IN_PROGRESS = "In Progress";
        public const string COMPLETED = "Completed";
        public const string CANCELLED = "Cancelled";
        public const string NEW = "New";
        public const string PLANNED = "Planned";
        public const string SUCCESS = "Success";
        public const string PENDING = "Pending";
        public const string NONE = "None";
        public const string UNKNOWN = "Unknown";

    }

    public static class InventoryMovementType
    {
        public const string PRODUCTION = "Production";
        public const string RESERVE = "Reserve";
        public const string UNUSED_RETURN = "Unused Return";
        public const string CANCEL_RETURN = "Cancel Return";
    }

    public static class ProductionBatchType
    {
        public const string NORMAL = "Normal";
        public const string SHORTFALL = "Shortfall";
        public const string REWORK = "Rework";
    }
}