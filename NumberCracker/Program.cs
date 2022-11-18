// See https://aka.ms/new-console-template for more information

using System.ComponentModel;
using System.Net.Sockets;
using System.Text;
using GammaLibrary.Extensions;
using NumberCracker;

for (int a = 0; a < 10; a++)
    for (int b = 0; b < 10; b++)
    for (int c = 0; c < 10; c++)
    {
        var n = new NumberGroup(a, b, c);
        var s = new SolverCore();
        for (int i = 1; i < 1000; i++)
        {
            var guess = s.GetNextGuess();
            var nn = GameCore.GetNumberStatus(n, guess);
            s.ReportNumberStatus(new NumberStatusGroup(nn.A.Status, nn.B.Status, nn.C.Status));
            if (s.TryGetResult() != null)
            {
                var r = s.TryGetResult().Value;
                Console.WriteLine($"{a}{b}{c} took {i} guesses {r.A}{r.B}{r.C}");
                var guesses = s.guesses;
                break;
            }


        }
    }
Console.WriteLine("Done");
for (var i = 0; i < 80; i++)
{
    Task.Run(() => main());
}

void main()
{
    start:
    var tcpClient = new TcpClient();

    tcpClient.Connect("node.yuzhian.com.cn", 31137);
    var networkStream = tcpClient.GetStream();

    var sr = networkStream.CreateStreamReader();
    networkStream.Write("3\n".ToASCIIBytes());
    ReadUntil("> ");
    var correct = "Correct! ";
    var wrong = "Wrong!   ";
    var GREEN = "\033[42m  \033[0m";
    var YELLOW = "\033[43m  \033[0m";
    var WHITE = "\033[47m  \033[0m";
    while (true)
    {
        try
        {
            var sc = new SolverCore();
            for (int i = 0; i < 1000; i++)
            {
                ReadUntil("> ");
                if (sc.TryGetResult() != null)
                {
                    var g1 = sc.TryGetResult().Value;
                    Write($"{g1.A}{g1.B}{g1.C}");
                    break;
                }
                var g = sc.GetNextGuess();
                Write($"{g.A}{g.B}{g.C}");
                var line = ReadLine();
                if (line.StartsWith(correct)) break;
                var info = line.Substring(wrong.Length);
                var i1 = info.IndexOf('4') + 1;
                var i1c = info[i1];
                info = info.Substring(i1);
                var i2 = info.IndexOf('4') + 1;
                var i2c = info[i2];
                info = info.Substring(i2);
                var i3 = info.IndexOf('4') + 1;
                var i3c = info[i3];
                var a = i1c == '2' ? NumberStatus.Green : i1c == '3' ? NumberStatus.Yellow : NumberStatus.White;
                var b = i2c == '2' ? NumberStatus.Green : i2c == '3' ? NumberStatus.Yellow : NumberStatus.White;
                var c = i3c == '2' ? NumberStatus.Green : i3c == '3' ? NumberStatus.Yellow : NumberStatus.White;
                sc.ReportNumberStatus(new NumberStatusGroup(a, b, c));

            }
        }
        catch (Exception e)
        {
            goto start;

        }
    }
    void Write(string s)
    {
        networkStream.Write($"{s}\n".ToASCIIBytes());
    }
    void ReadUntil(string s)
    {
        while (true)
        {
            a:
            for (var index = 0; index < s.ToCharArray().Length; index++)
            {
                var c = s.ToCharArray()[index];
                var read = sr.Read();
                if (read == 'H' && sr.Peek() == 'e')
                {
                    var sb = new StringBuilder();
                    int c1;
                    while ((c1 = sr.Read()) != -1)
                    {
                        sb.Append((char)c1);
                    }

                    var n = sb.ToString().Trim();
                    if (!n.EndsWith("**************************************"))
                    {
                        Console.WriteLine(n);

                    }
                    throw new EndOfStreamException();
                }

                if (read == -1) Console.WriteLine("????");

                //Console.Write((char)read);
                if (c != read) goto a;
            }
            return;
        }
    }

    string ReadLine()
    {
        var s = sr.ReadLine();
        //Console.WriteLine(s);
        return s;
    }
}

Thread.CurrentThread.Join();
n:
for (int a = 9; a < 10; a++)
    for (int b = 8; b < 10; b++)
        for (int c = 8; c < 10; c++)
        {
            var n = new NumberGroup(a, b, c);
            var s = new SolverCore();
            for (int i = 1; i < 1000; i++)
            {
                    var guess = s.GetNextGuess();
                    var nn = GameCore.GetNumberStatus(n, guess);
                    s.ReportNumberStatus(new NumberStatusGroup(nn.A.Status, nn.B.Status, nn.C.Status));
                    if (s.TryGetResult() != null)
                    {
                        var r = s.TryGetResult().Value;
                        Console.WriteLine($"{a}{b}{c} took {i} guesses {r.A}{r.B}{r.C}");
                        var guesses = s.guesses;
                        break;
                    }
                

            }
        }