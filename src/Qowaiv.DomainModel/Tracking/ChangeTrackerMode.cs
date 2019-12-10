namespace Qowaiv.DomainModel.Tracking
{
    /// <summary>Represents the mode of a <see cref="ChangeTracker"/>.</summary>
    public enum ChangeTrackerMode
    {
        /// <summary>All changes trigger a validation (and potentially a rollback). (Default)</summary>
        None,

        /// <summary>Buffers changes, until the moment a validation (and potential rollback) is triggered.</summary>
        Buffering,

        /// <summary>Initializes an aggregate and does not support a potential rollback.</summary>
        Initialization,
    }
}
