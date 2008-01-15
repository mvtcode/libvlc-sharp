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

        VlcConfig vlcConfig = new VlcConfig();
        vlcConfig.PluginPath = "C:\\Program Files\\VideoLAN\\VLC\\Plugins";

        VlcInstance e = new VlcInstance(vlcConfig);

        Console.WriteLine("Engine Init");
        Console.WriteLine("-------------------");
        foreach (VlcLogMessage vlm in e.VlcLog)
            Console.WriteLine("{0} {1} {2} {3} {4}", vlm.Severity, vlm.Type, vlm.Name, vlm.Header, vlm.Message);

        e.VlcLog.Clear();

        int id = e.VlcPlaylist.Add(args[0], "");

        Console.WriteLine("-------------------");
        Console.WriteLine("Added {0}", args[0]);
        Console.WriteLine("-------------------");

        foreach (VlcLogMessage vlm in e.VlcLog)
            Console.WriteLine("{0} {1} {2} {3} {4}", vlm.Severity, vlm.Type, vlm.Name, vlm.Header, vlm.Message);

        e.VlcLog.Clear();
        e.VlcPlaylist.Play(id);

        Console.WriteLine("-------------------");
        Console.WriteLine("Playing {0}", id);
        Console.WriteLine("-------------------");

        foreach (VlcLogMessage vlm in e.VlcLog)
            Console.WriteLine("{0} {1} {2} {3} {4}", vlm.Severity, vlm.Type, vlm.Name, vlm.Header, vlm.Message);

        e.VlcLog.Clear();

        while (e.VlcPlaylist.IsPlaying)
        {
            foreach (VlcLogMessage vlm in e.VlcLog)
                Console.WriteLine("{0} {1} {2} {3} {4}", vlm.Severity, vlm.Type, vlm.Name, vlm.Header, vlm.Message);

            e.VlcLog.Clear();

            System.Threading.Thread.Sleep(5000);
            if ( !e.VlcInput.IsInvalid )
                Console.Write("{0} / {1}\r", e.VlcInput.Time / 1000, e.VlcInput.Length / 1000);
        }
        Console.ReadLine();
    }
}
