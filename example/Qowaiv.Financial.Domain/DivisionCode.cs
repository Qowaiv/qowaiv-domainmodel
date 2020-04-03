using System;

namespace Qowaiv.Financial.Domain
{
    public struct DivisionCode : IEquatable<DivisionCode>
    {
        private readonly int code;

        public DivisionCode(int code) => this.code = code;

        public override bool Equals(object obj) => obj is DivisionCode other && Equals(other);
        public bool Equals(DivisionCode other) => code == other.code;
        public override int GetHashCode() => code;

        public static bool operator ==(DivisionCode l, DivisionCode r) => l.Equals(r);
        public static bool operator !=(DivisionCode l, DivisionCode r) => !(l == r);

        public override string ToString() => code.ToString("00000");
    }
}
