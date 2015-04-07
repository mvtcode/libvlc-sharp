# Introduction #

Here are some functions to help developpers using this lib.


# Search for VLC path #

Search libvlc.dll, and validate path (libvlc.dll and subdirectory 'plugins') in registry (if VLC is already installed), in environement's variable PATH, or return application's directory.

```
        /// <summary>
        /// Search path for libvlc and plugins directory using registry, and environement PATH variable
        /// </summary>
        /// <returns>VLC path, or current app path</returns>
        public static string SearchVlcPath()
        {
            //Search VLC - Registry
            RegistryKey regKey = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\VideoLAN\VLC");
            if (regKey != null)
            {
                string path = (string)regKey.GetValue("InstallDir", null);
                if (!string.IsNullOrEmpty(path))
                {
                    if (CheckVlcDirectory(path))
                    {
                        return path;
                    }
                }
            }

            //Search VLC - Path
            string[] envpaths = Environment.GetEnvironmentVariable("PATH").Split(';');
            foreach (string env in envpaths)
            {
                if (CheckVlcDirectory(env))
                {
                    return env;
                }
            }

            //Set program path as VLC Path
            Assembly asm = Assembly.GetEntryAssembly();
            string loc = asm.Location;
            loc = Path.GetDirectoryName(asm.Location);
            return loc;
        }

        /// <summary>
        /// Check if directory is a valid VLC directory : contains libvlc.* (dll or so) and sub-directory 'plugins'
        /// </summary>
        /// <param name="dir">Directory to check</param>
        /// <returns>True if directory is a VLC</returns>
        public static bool CheckVlcDirectory(string dir)
        {
            DirectoryInfo dirinfo = new DirectoryInfo(dir);
            if (dirinfo.Exists)
            {
                if (dirinfo.GetDirectories("plugins").Length == 1)
                {
                    if (dirinfo.GetFiles("libvlc.*").Length > 0)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
```
# Set current environement #
To avoid [DllNotFoundException](http://msdn.microsoft.com/en-us/library/system.dllnotfoundexception.aspx) on Windows, you should have the libvlc.dll either in program's directory, or in one of the environement's variable PATH.
This function can change the PATH variable to add VLC's path to the application environement.

```
        /// <summary>
        /// Set current process environement variable PATH to have the system find the libvlc.dll
        /// </summary>
        /// <param name="vlcpath">Path of libvlc.dll</param>
        public static void SetEnvironement(string vlcpath)
        {
            string path = Environment.GetEnvironmentVariable("path", EnvironmentVariableTarget.Process);
            if (!path.Contains(vlcpath))
            {
                path += ";" + vlcpath;
                Environment.SetEnvironmentVariable("path", path, EnvironmentVariableTarget.Process);
            }
        }
```