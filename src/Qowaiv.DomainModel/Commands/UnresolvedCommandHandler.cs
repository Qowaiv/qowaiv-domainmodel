using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Runtime.Serialization;

namespace Qowaiv.DomainModel.Commands
{
    /// <summary>The exception that is thrown when a command handler type could not be resolved.</summary>
    [Serializable]
    public class UnresolvedCommandHandler : InvalidOperationException
    {
        /// <summary>The command hander type that could not be resolved.</summary>
        public Type CommandHandlerType { get; }

        /// <summary>Initializes a new instance of the <see cref="UnresolvedCommandHandler"/> class.</summary>
        public UnresolvedCommandHandler(Type commandHandlerType)
            : this(string.Format(CultureInfo.CurrentCulture, QowaivDomainModelMessages.UnresolvedCommandHandler_Type, commandHandlerType))
            => CommandHandlerType = commandHandlerType;

        /// <summary>Initializes a new instance of the <see cref="UnresolvedCommandHandler"/> class.</summary>
        [ExcludeFromCodeCoverage/* Required Exception constructor for inheritance. */]
        public UnresolvedCommandHandler() : this(QowaivDomainModelMessages.UnresolvedCommandHandler) { }

        /// <summary>Initializes a new instance of the <see cref="UnresolvedCommandHandler"/> class.</summary>
        [ExcludeFromCodeCoverage/* Required Exception constructor for inheritance. */]
        public UnresolvedCommandHandler(string message) : base(message) { }

        /// <summary>Initializes a new instance of the <see cref="UnresolvedCommandHandler"/> class.</summary>
        [ExcludeFromCodeCoverage/* Required Exception constructor for inheritance. */]
        public UnresolvedCommandHandler(string message, Exception innerException)
            : base(message, innerException) { }

        /// <summary>Initializes a new instance of the <see cref="UnresolvedCommandHandler"/> class.</summary>
        protected UnresolvedCommandHandler(SerializationInfo info, StreamingContext context) 
            : base(info, context)
        {
            Guard.NotNull(info, nameof(info));
            CommandHandlerType = Type.GetType(info.GetString(nameof(CommandHandlerType)));
        }

        /// <inheritdoc />
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(Guard.NotNull(info, nameof(info)), context);
            info.AddValue(nameof(CommandHandlerType), CommandHandlerType?.AssemblyQualifiedName);
        }
    }
}
