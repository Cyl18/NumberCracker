using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NumberCracker
{
    struct NumberGroup
    {
        public NumberGroup(int A, int B, int C)
        {
            this.A = A;
            this.B = B;
            this.C = C;
        }

        public int A { get; init; }
        public int B { get; init; }
        public int C { get; init; }

        public void Deconstruct(out int A, out int B, out int C)
        {
            A = this.A;
            B = this.B;
            C = this.C;
        }
    }

    record NumberWithStatus(int Number, NumberStatus Status);
    enum NumberStatus
    {
        Green, Yellow, White
    }

    struct NumberStatusGroup
    {
        public NumberStatusGroup(NumberStatus A, NumberStatus B, NumberStatus C)
        {
            this.A = A;
            this.B = B;
            this.C = C;
        }

        public NumberStatus A { get; init; }
        public NumberStatus B { get; init; }
        public NumberStatus C { get; init; }

        public void Deconstruct(out NumberStatus A, out NumberStatus B, out NumberStatus C)
        {
            A = this.A;
            B = this.B;
            C = this.C;
        }
    }

    record NumberWithStatusGroup(NumberWithStatus A, NumberWithStatus B, NumberWithStatus C);

    class Tree
    {
        public Tree(bool Final, NumberGroup NumberGroup, Dictionary<NumberStatusGroup, Tree> StatusTree, NumberGroup? FinalResult)
        {
            this.Final = Final;
            this.NumberGroup = NumberGroup;
            this.StatusTree = StatusTree;
            this.FinalResult = FinalResult;
        }

        public bool Final { get; set; }
        public NumberGroup NumberGroup { get; init; }
        public Dictionary<NumberStatusGroup, Tree> StatusTree { get; init; }
        public NumberGroup? FinalResult { get; set; }

        public void Deconstruct(out bool Final, out NumberGroup NumberGroup, out Dictionary<NumberStatusGroup, Tree> StatusTree, out NumberGroup? FinalResult)
        {
            Final = this.Final;
            NumberGroup = this.NumberGroup;
            StatusTree = this.StatusTree;
            FinalResult = this.FinalResult;
        }
    }

    internal class SolverCore
    {
        public List<NumberWithStatusGroup> guesses = new List<NumberWithStatusGroup>();
        static Tree TheTree = new Tree(false, new NumberGroup(0,1,2), new Dictionary<NumberStatusGroup, Tree>(), null);
        NumberGroup? lastGuess;
        public NumberGroup GetNextGuess()
        {
                var tree = TheTree;
                var flag = false;
                foreach (var numberWithStatusGroup in guesses)
                {
                    var g = new NumberStatusGroup(numberWithStatusGroup.A.Status, numberWithStatusGroup.B.Status,
                        numberWithStatusGroup.C.Status);
                    flag = true;
                    if (tree.StatusTree.ContainsKey(g))
                    {
                        tree = tree.StatusTree[g];
                    }
                    else
                    {
                        flag = false;
                        break;
                    }
                }

                if (flag)
                {
                    lastGuess = tree.NumberGroup;
                    return tree.NumberGroup;
                }
            
            if (lastGuess == null)
            {
                        var n = new NumberGroup(0,1,2);
                        lastGuess = n;
                        return n;
            }
            var allPossibles = new List<NumberGroup>();
            for (int a = 0; a < 10; a++)
                for (int b = 0; b < 10; b++)
                    for (int c = 0; c < 10; c++)
                    {
                        var n = new NumberGroup(a, b, c);
                        if (guesses.Where(x => x.A.Status is NumberStatus.Green).Any(x => x.A.Number == a)) continue;
                        if (guesses.Where(x => x.B.Status is NumberStatus.Green).Any(x => x.B.Number == b)) continue;
                        if (guesses.Where(x => x.C.Status is NumberStatus.Green).Any(x => x.C.Number == c)) continue;
                        if (guesses.SelectMany(x => new[] { x.A, x.B, x.C }).Where(x => x.Status == NumberStatus.White).Any(x => x.Number == a || x.Number == b || x.Number == c)) continue;
                        if (guesses.Any(x => x.A.Number == a && x.B.Number == b && x.C.Number == c)) continue;

                        allPossibles.Add(n);
                    }

            var nn = new Dictionary<NumberGroup, List<int>>();
            foreach (var a in allPossibles)
            {
                nn[a] = new List<int>();
            }
            foreach (var a in allPossibles)
            {
                var ve = 0;
                //if (guesses.Any(x => x.A.Number == a.A || x.B.Number == a.B || x.C.Number == a.C)) ve -= 50;
                foreach (var b in allPossibles)
                {
                    var ba = GameCore.GetNumberStatus(a, b);
                    var entropy = 0;
                    var e1 = 2;
                    var e2 = 1;
                    if (ba.A.Status == NumberStatus.Yellow) entropy += e1;
                    if (ba.A.Status == NumberStatus.White) entropy += e2;
                    if (ba.B.Status == NumberStatus.Yellow) entropy += e1;
                    if (ba.B.Status == NumberStatus.White) entropy += e2;
                    if (ba.C.Status == NumberStatus.Yellow) entropy += e1;
                    if (ba.C.Status == NumberStatus.White) entropy += e2;
                    entropy += ve;
                    if (guesses.Select(x => x.A).Any(x => x.Number == a.A)) entropy -= 50;
                    if (guesses.Select(x => x.B).Any(x => x.Number == a.B)) entropy -= 50;
                    if (guesses.Select(x => x.C).Any(x => x.Number == a.C)) entropy -= 50;
                    
                    //if (guesses.SelectMany(x => new[] {x.A, x.B, x.C}).Any(x =>  x.Status == NumberStatus.Yellow && l.Contains(x.Number))) entropy -= 20;

                    nn[a].Add(entropy);
                }
            }

            var guess = nn.MaxBy(x => x.Value.Sum()).Key;
            lastGuess = guess;
            return guess;
        }

        public NumberGroup? TryGetResult()
        {
            lock (TheTree)
            {
                var tree = TheTree;
                var flag = false;
                foreach (var numberWithStatusGroup in guesses)
                {
                    var g = new NumberStatusGroup(numberWithStatusGroup.A.Status, numberWithStatusGroup.B.Status,
                        numberWithStatusGroup.C.Status);
                    flag = true;
                    if (tree.StatusTree.ContainsKey(g))
                    {
                        tree = tree.StatusTree[g];
                        flag = false;
                        break;
                    }
                }

                if (flag && tree.Final)
                {
                    return tree.FinalResult;
                }
            }

            var ar = guesses.Select(x => x.A).Where(x => x.Status == NumberStatus.Green).ToArray();
            var br = guesses.Select(x => x.B).Where(x => x.Status == NumberStatus.Green).ToArray();
            var cr = guesses.Select(x => x.C).Where(x => x.Status == NumberStatus.Green).ToArray();
            var allPossibles = new List<NumberGroup>();
            for (int a = 0; a < 10; a++)
                for (int b = 0; b < 10; b++)
                    for (int c = 0; c < 10; c++)
                    {
                        var n = new NumberGroup(a, b, c);
                        // if (guesses.SelectMany(x => new[] { x.A, x.B, x.C }).Where(x => x.Status == NumberStatus.White).Any(x => x.Number == a || x.Number == b || x.Number == c)) continue;
                        // //if (guesses.Any(x => x.A.Number == a || x.B.Number == b || x.C.Number == c)) continue;
                        // var a1 = guesses.Select(x => x.A).Where(x => x.Status == NumberStatus.Green).ToArray();
                        // if (a1.Any() && a != a1[0].Number) continue;
                        // var b1 = guesses.Select(x => x.B).Where(x => x.Status == NumberStatus.Green).ToArray();
                        // if (b1.Any() && b != b1[0].Number) continue;
                        // var c1 = guesses.Select(x => x.C).Where(x => x.Status == NumberStatus.Green).ToArray();
                        // if (c1.Any() && c != c1[0].Number) continue;
                        // var yellows = guesses.SelectMany(x => new[] {x.A, x.B, x.C})
                        //     .Where(x => x.Status == NumberStatus.Yellow).Select(x => x.Number).ToArray();
                        // if (yellows.Any())
                        // {
                        //     if (!yellows.Contains(a) && !yellows.Contains(b) && !yellows.Contains(c)) continue;
                        //     
                        //     
                        // }
                        var flag = true;
                        foreach (var guess in guesses)
                        {
                            if (GameCore.GetNumberStatus(n, new NumberGroup(guess.A.Number, guess.B.Number, guess.C.Number)) != guess)
                            {
                                flag = false;
                                break;
                            }
                        }

                        if (flag)
                        {
                            allPossibles.Add(n);

                        }
                    }

            if (allPossibles.Count == 1) return allPossibles.First();

            if (ar.Length == 0 || br.Length == 0 || cr.Length == 0)
            {
                return null;
            }
            return new NumberGroup(ar[0].Number, br[0].Number, cr[0].Number);
        }

        public void ReportNumberStatus(NumberStatusGroup g)
        {
            var tree1 = TheTree;
            foreach (var numberWithStatusGroup in guesses)
            {
                var g1 = new NumberStatusGroup(numberWithStatusGroup.A.Status, numberWithStatusGroup.B.Status,
                    numberWithStatusGroup.C.Status);

                tree1 = tree1.StatusTree[g1];

            }

            guesses.Add(new NumberWithStatusGroup(
                new NumberWithStatus(lastGuess.Value.A, g.A),
                new NumberWithStatus(lastGuess.Value.B, g.B),
                new NumberWithStatus(lastGuess.Value.C, g.C)
            ));
            if (!tree1.StatusTree.ContainsKey(g))
            {
                if (TryGetResult() == null)
                {
                var g11 = GetNextGuess();
                tree1.StatusTree[g] = new Tree(false, g11, new Dictionary<NumberStatusGroup, Tree>(), null);

                }
                else
                {
                tree1.StatusTree[g] = new Tree(true, TryGetResult().Value, new Dictionary<NumberStatusGroup, Tree>(), TryGetResult().Value);

                }
            }
        }
    }
}
