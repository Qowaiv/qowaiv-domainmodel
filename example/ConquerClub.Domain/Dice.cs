using Troschuetz.Random;

namespace ConquerClub.Domain
{
    /// <summary>Represents the (default) dice based attack.</summary>
    public static class Dice
    {
        /// <summary>Attack by rolling once.</summary>
        public static AttackResult Attack(Army attacker, Army defender, IGenerator rnd)
        {
            int roll;
            if (attacker >= 4)
            {
                roll = defender >= 2 ? Roll3v2(rnd) : Roll3v1(rnd);
            }
            else if (attacker == 3)
            {
                roll = defender >= 2 ? Roll2v2(rnd) : Roll2v1(rnd);
            }
            else if (attacker == 2)
            {
                roll = defender >= 2 ? Roll1v2(rnd) : Roll1v1(rnd);
            }
            else
            {
                return new AttackResult(attacker, defender);
            }

            switch (roll)
            {
                case +2: defender -= 2; break;
                case +1: defender -= 1; break;
                case -2: attacker -= 2; break;
                case -1: attacker -= 1; break;
                default:
                    attacker -= 1;
                    defender -= 1;
                    break;
            }
            return new AttackResult(attacker, defender);
        }

        /// <summary>Attack will the attacker has at least 4 left and defender has not been defeated.</summary>
        public static AttackResult AutoAttack(Army attacker, Army defender, IGenerator rnd)
        {
            while (attacker >= 4 && defender >= 1)
            {
                var roll = defender >= 2 ? Roll3v2(rnd) : Roll3v1(rnd);

                switch (roll)
                {
                    case +2: defender -= 2; break;
                    case +1: defender -= 1; break;
                    case -2: attacker -= 2; break;
                    case -1: attacker -= 1; break;
                    default:
                        attacker -= 1;
                        defender -= 1;
                        break;
                }
            }
            return new AttackResult(attacker, defender);
        }

        /// <remarks>+37,2% =33,6% -29,3% E: 54,0%</remarks>
        private static int Roll3v2(IGenerator rnd)
        {
            var threshold = rnd.Next(2890 + 2611 + 2275);
            if (threshold < 2890) { return +2; }
            if (threshold < 2890 + 2611) { return 0; }
            else { return -2; }
        }

        /// <remarks>+66,0% -34,0%</remarks>
        private static int Roll3v1(IGenerator rnd)
        {
            return rnd.Next(855 + 441) < 855 ? +1 : -1;
        }

        /// <remarks>+22,8% =32,4% -44,8% E: 39,0%</remarks>
        private static int Roll2v2(IGenerator rnd)
        {
            var threshold = rnd.Next(295 + 420 + 581);
            if (threshold < 295) { return +2; }
            if (threshold < 295 + 420) { return 0; }
            else { return -2; }
        }

        /// <remarks>+57,9% -42,1%</remarks>
        private static int Roll2v1(IGenerator rnd)
        {
            return rnd.Next(125 + 91) < 125 ? +1 : -1;
        }

        /// <remarks>+25,5% -74,5%</remarks>
        private static int Roll1v2(IGenerator rnd)
        {
            return rnd.Next(55 + 161) < 55 ? +1 : -1;
        }

        /// <remarks>+41,7% -58,3%</remarks>
        private static int Roll1v1(IGenerator rnd)
        {
            return rnd.Next(15 + 21) < 15 ? +1 : -1;
        }
    }
}
