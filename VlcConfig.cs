/***************************************************************************
 *  VlcConfig.cs
 *  Original Filename: VlcConfig.cs
 *  
 *  Copyright (C) 2007 Scott E Graves <scott.e.graves@gmail.com>
 *  Written by Scott E Graves <scott.e.graves@gmail.com>
 ****************************************************************************/

/*  THIS FILE IS LICENSED UNDER THE MIT LICENSE AS OUTLINED IMMEDIATELY BELOW: 
 *
 *  Permission is hereby granted, free of charge, to any person obtaining a
 *  copy of this software and associated documentation files (the "Software"),  
 *  to deal in the Software without restriction, including without limitation  
 *  the rights to use, copy, modify, merge, publish, distribute, sublicense,  
 *  and/or sell copies of the Software, and to permit persons to whom the  
 *  Software is furnished to do so, subject to the following conditions:
 *
 *  The above copyright notice and this permission notice shall be included in 
 *  all copies or substantial portions of the Software.
 *
 *  THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR 
 *  IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, 
 *  FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE 
 *  AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER 
 *  LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING 
 *  FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER 
 *  DEALINGS IN THE SOFTWARE.
 */

using System;
using System.Collections.Generic;
using System.Text;


namespace libvlc
{
    public class VlcConfig
	{
	    public VlcConfig()
	    {
            _args.Add("-I");
            _args.Add("dummy");
	    }
 
        private string _pluginPath;
	    public string PluginPath
        {
            get
            {
                return _pluginPath;
            }

            set
            {
                _pluginPath = value;
            }
        }

	    private List<string> _args = new List<string>();
	    public string[] Arguments
	    {
		    get
            {
				Int32 count = _args.Count + (PluginPath != null ? 1 : 0);
			    string[] tempArgs = new string[count];
			    _args.CopyTo(tempArgs);
				
				if ( PluginPath != null && PluginPath.Trim().Length > 0 )
					tempArgs[tempArgs.Length-1] = "--plugin-path=" + PluginPath;
				
			    return tempArgs;
		    }
	    }
	}
}
