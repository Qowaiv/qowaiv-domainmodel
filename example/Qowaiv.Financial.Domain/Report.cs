using System;

namespace Qowaiv.Financial.Domain
{
    public struct Report : IEquatable<Report>
    {
        public Report(Year year, Month month)
        {
            Year = year;
            Month = month;
        }

        /// <summary>Gets the month component.</summary>
        public Year Year { get; }

        /// <summary>Gets the month component.</summary>
        public Month Month { get; }

        /// <inheritdoc/>
        public override bool Equals(object obj) => obj is Report other && Equals(other);
        
        /// <inheritdoc/>
        public bool Equals(Report other) => Year == other.Year && Month == other.Month;
        
        /// <inheritdoc/>
        public override int GetHashCode() => (Year.GetHashCode() << 16) ^ Month.GetHashCode();

        /// <inheritdoc/>
        public override string ToString() => $"{Year}-{Month:M}";
    }
}
