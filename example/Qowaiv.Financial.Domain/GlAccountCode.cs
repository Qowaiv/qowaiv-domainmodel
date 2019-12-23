using System;
using System.Collections.Generic;
using System.Text;

namespace Qowaiv.Financial.Domain
{
    public struct GlAccountCode
    {
        private readonly string code;

        public GlAccountCode(string code) => this.code = code;

        public override string ToString() => code;
    }
}
