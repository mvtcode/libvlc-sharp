A set of bindings written in C# to the libvlc api.

Using the API is simple

```
using System;
using Atx.LibVLC;

public class Test
{
  public static void Main(string[] args)
  {
    VlcEngine eng = new VlcEngine();
    int id = eng.Add(args[0], "A file to play");
    eng.Play(id);
    while(eng.IsPlaying)
      Console.Write("{0} / {1}\r", eng.Time / 1000, eng.Length / 1000);
  }
}
```