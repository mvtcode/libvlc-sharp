using System;
using Atx.LibVLC;

public class Foo
{
    public static void Main(string[] args)
    {
        if (args.Length < 1)
        {
            Console.WriteLine("Please specify a file to play");
            return;
        }

        VlcConfig cfg = new VlcConfig();
        VlcInstance e = new VlcInstance(cfg);
        Console.WriteLine("Engine Init");
        Console.WriteLine("-------------------");
        foreach (VlcLogMessage vlm in e.Log)
            Console.WriteLine("{0} {1} {2} {3} {4}", vlm.Severity, vlm.Type, vlm.Name, vlm.Header, vlm.Message);

        e.Log.Clear();

        int id = e.PlaylistAdd(args[0], "");

        Console.WriteLine("-------------------");
        Console.WriteLine("Added {0}", args[0]);
        Console.WriteLine("-------------------");

        foreach (VlcLogMessage vlm in e.Log)
            Console.WriteLine("{0} {1} {2} {3} {4}", vlm.Severity, vlm.Type, vlm.Name, vlm.Header, vlm.Message);

        e.Log.Clear();
        e.Play(id);

        Console.WriteLine("-------------------");
        Console.WriteLine("Playing {0}", id);
        Console.WriteLine("-------------------");

        foreach (VlcLogMessage vlm in e.Log)
            Console.WriteLine("{0} {1} {2} {3} {4}", vlm.Severity, vlm.Type, vlm.Name, vlm.Header, vlm.Message);

        e.Log.Clear();

        while (e.IsPlaying)
        {
            foreach (VlcLogMessage vlm in e.Log)
                Console.WriteLine("{0} {1} {2} {3} {4}", vlm.Severity, vlm.Type, vlm.Name, vlm.Header, vlm.Message);

            e.Log.Clear();

            Console.Write("{0} / {1}\r", e.Input.Time / 1000, e.Input.Length / 1000);
            System.Threading.Thread.Sleep(5000);
        }
        Console.ReadLine();
    }
}
