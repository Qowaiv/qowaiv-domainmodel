namespace Qowaiv.DomainModel.Tracking
{
    /// <summary>Implements <see cref="ITrackableChange"/> for changing a property.</summary>
    public class PropertyChanged : ITrackableChange
    {
        /// <summary>Initializes a new instance of the <see cref="PropertyChanged"/> class.</summary>
        /// <param name="properties">
        /// The property collection that contains the property.
        /// </param>
        /// <param name="propertyName">
        /// The name of the property.
        /// </param>
        /// <param name="value">
        /// The new value.
        /// </param>
        public PropertyChanged(PropertyCollection properties, string propertyName, object value)
        {
            this.properties = Guard.NotNull(properties, nameof(properties));
            this.propertyName = Guard.NotNullOrEmpty(propertyName, nameof(propertyName));
            originalValue = this.properties[this.propertyName];
            this.value = value;
        }

        private readonly PropertyCollection properties;
        private readonly string propertyName;
        private readonly object originalValue;
        private readonly object value;

        /// <inheritdoc />
        public void Apply() => properties[propertyName] = value;

        /// <inheritdoc />
        public void Rollback() => properties[propertyName] = originalValue;
    }
}
