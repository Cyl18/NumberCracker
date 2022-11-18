using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NumberCracker
{
    internal class GameCore
    {
        static Dictionary<(NumberGroup, NumberGroup), NumberWithStatusGroup> Cache;
        static NumberWithStatusGroup[,,,,,] Cache2;
        
        static GameCore()
        {
            Cache = new Dictionary<(NumberGroup, NumberGroup), NumberWithStatusGroup>();
            Cache2 = new NumberWithStatusGroup[10,10,10,10,10,10];
            for (int a = 0; a < 10; a++)
            for (int b = 0; b < 10; b++)
            for (int c = 0; c < 10; c++)
            for (int a1 = 0; a1 < 10; a1++)
            for (int b1 = 0; b1 < 10; b1++)
            for (int c1 = 0; c1 < 10; c1++)
            {
                var g1 = new NumberGroup(a, b, c);
                var g2 = new NumberGroup(a1, b1, c1);
                //Cache[(g1, g2)] = GetNumberStatusInternal(g1, g2);
                Cache2[a,b,c,a1,b1,c1] = GetNumberStatusInternal(g1, g2);
            }
        }

        public static NumberWithStatusGroup GetNumberStatus(NumberGroup trueValue, NumberGroup guessValue)
        {
            return Cache2[trueValue.A, trueValue.B, trueValue.C, guessValue.A, guessValue.B, guessValue.C];
        }
        public static NumberWithStatusGroup GetNumberStatusInternal(NumberGroup trueValue, NumberGroup guessValue)
        {
            var numbers = new[] {trueValue.A, trueValue.B, trueValue.C};
            var a = new NumberWithStatus(guessValue.A,
                guessValue.A == trueValue.A ? NumberStatus.Green :
                numbers.Contains(guessValue.A) ? NumberStatus.Yellow : NumberStatus.White);
            if (numbers.Contains(guessValue.A)) numbers = numbers.Where(x => x != guessValue.A).ToArray();
            var b =
                    new NumberWithStatus(guessValue.B,
                        guessValue.B == trueValue.B ? NumberStatus.Green :
                        numbers.Contains(guessValue.B) ? NumberStatus.Yellow : NumberStatus.White)
                ;
            if (numbers.Contains(guessValue.B)) numbers = numbers.Where(x => x != guessValue.B).ToArray();
            var c =
                    new NumberWithStatus(guessValue.C,
                        guessValue.C == trueValue.C ? NumberStatus.Green :
                        numbers.Contains(guessValue.C) ? NumberStatus.Yellow : NumberStatus.White)
                ;
            if (numbers.Contains(guessValue.C)) numbers = numbers.Where(x => x != guessValue.C).ToArray();

            return new NumberWithStatusGroup(a
                ,b,c
                );
        }
    }
}
