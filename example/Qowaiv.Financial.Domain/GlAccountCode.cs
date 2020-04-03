using System;

namespace Qowaiv.Financial.Domain
{
    public struct GlAccountCode : IEquatable<GlAccountCode>
    {
        private readonly string code;

        public GlAccountCode(string code) => this.code = code;

        public override bool Equals(object obj) => obj is GlAccountCode other && Equals(other);
        public bool Equals(GlAccountCode other) => code == other.code;
        public override int GetHashCode() => (code ?? "").GetHashCode();

        public static bool operator ==(GlAccountCode l, GlAccountCode r) => l.Equals(r);
        public static bool operator !=(GlAccountCode l, GlAccountCode r) => !(l == r);

        public override string ToString() => code;
    }
}
