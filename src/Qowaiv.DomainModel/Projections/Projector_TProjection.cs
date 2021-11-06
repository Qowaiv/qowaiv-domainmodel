namespace Qowaiv.DomainModel.Projections
{
    /// <summary>A projector able to Create a projection based on replaying events.</summary>
    /// <remarks>
    /// By running <see cref="Projector.Project{TProjection}(Projector{TProjection}, System.Collections.Generic.IEnumerable{object})"/>
    /// All Methods with the name `When`, with one argument are triggered if the
    /// argument matches the event to replay.
    /// </remarks>
    /// <typeparam name="TProjection">
    /// The type of the projection.
    /// </typeparam>
    public interface Projector<out TProjection>
    {
        /// <summary>Returns a projection based on the current state of the projector.</summary>
        TProjection Projection();
    }
}
