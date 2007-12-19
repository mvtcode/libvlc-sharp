/***************************************************************************
 *  VlcObject.cs
 *  Original Filename: VlcObject.cs
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
using System.Runtime.InteropServices;


namespace Atx.LibVLC
{
    internal class VlcObjectHandle : SafeHandle
    {
        public VlcObjectHandle()
            : base(IntPtr.Zero, true)
        {
        }

        public override bool IsInvalid
        {
            get
            {
                return handle == IntPtr.Zero;
            }
        }

        protected override bool ReleaseHandle()
        {
            if (!IsInvalid)
            {
                __vlc_object_release(this);
                handle = IntPtr.Zero;
            }

            return true;
        }

        protected override void Dispose(bool disposing)
        {
            ReleaseHandle();
            base.Dispose(disposing);
        }

        [DllImport("libvlc")]
        private static extern void __vlc_object_release(VlcObjectHandle p_object);
    }

    public enum VlcObjectType : int
    {
        Root = -1,
        VLC = -2,
        Module = -3,
        Intf = -4,
        Playlist = -5,
        Item = -6,
        Input = -7,
        Decoder = -8,
        VOut = -9,
        AOut = -10,
        SOut = -11,
        HTTPD = -12,
        Packetizer = -13,
        Encoder = -14,
        Dialogs = -15,
        VLM = -16,
        Announce = -17,
        Demux = -18,
        Access = -19,
        Stream = -20,
        OpenGL = -21,
        Filter = -22,
        VOD = -23,
        SPU = -24,
        TLS = -25,
        SD = -26,
        XML = -27,
        OSDMenu = -28,
        Stats = -29,
        HTTPD_Host = -30,
        Generic = -666,
    }

    [Flags]
    public enum VlcObjectSearchMode : int
    {
        Parent = 1,
        Child = 2,
        Anywhere = 3,
        Strict = 4
    }

    internal enum VlcChangeAction : int
    {
		SetMin = 0x10,
        SetMax = 0x11,
        SetStep = 0x12,
        SetValue = 0x13,
        SetText = 0x14,
        GetText = 0x15,
        AddChoice = 0x20,
        DelChoice = 0x21,
        ClearChoices = 0x22,
        SetDefault = 0x23,
        GetChoices = 0x24,
        FreeChoices = 0x25,
        GetList = 0x26,
        FreeList = 0x27,
        ChoicesCount = 0x28,
        InheritValue = 0x30,
        TriggerCallbacks = 0x35
    }

//TODO: Incomplete type - WARNING
    [StructLayout(LayoutKind.Sequential)]
    public struct vlc_common_t
    {
        public int      i_object_id;
        public int      i_object_type;
        public IntPtr   psz_object_type;
        public IntPtr   psz_object_name;   
        public IntPtr   psz_header;                                                       
        public int      i_flags;                                                           
                                                                            
        ///* Thread properties, if any */                                         
        //vlc_bool_t   b_thread;                                                  
        //vlc_thread_t thread_id;                                                 
                                                                            
        ///* Object access lock */                                                
        //vlc_mutex_t  object_lock;                                               
        //vlc_cond_t   object_wait;                                               
                                                                                
        ///* Object properties */                                                 
        //volatile vlc_bool_t b_error;                  /**< set by the object */ 
        //volatile vlc_bool_t b_die;                   /**< set by the outside */ 
        //volatile vlc_bool_t b_dead;                   /**< set by the object */ 
        //volatile vlc_bool_t b_attached;               /**< set by the object */ 
        //vlc_bool_t b_force;      /**< set by the outside (eg. module_Need()) */ 
                                                                                
        ///* Object variables */                                                  
        //vlc_mutex_t     var_lock;                                               
        //int             i_vars;                                                 
        //variable_t *    p_vars;                                                 
                                                                                
        ///* Stuff related to the libvlc structure */                             
        //libvlc_t *      p_libvlc;                      /**< root of all evil */ 
        //vlc_t *         p_vlc;                   /**< (root of all evil) - 1 */ 
                                                                                
        //volatile int    i_refcount;                         /**< usage count */ 
        //vlc_object_t *  p_parent;                            /**< our parent */ 
        //vlc_object_t ** pp_children;                       /**< our children */ 
        //volatile int    i_children;                                             
                                                                                
        ///* Private data */                                                      
        //void *          p_private;                                              
                                                                                
        ///** Just a reminder so that people don't cast garbage */                
        //int be_sure_to_add_VLC_COMMON_MEMBERS_to_struct;                        
    }

    [StructLayout(LayoutKind.Sequential)]
    struct module_t
    {
        vlc_common_t cmn;
        IntPtr  psz_shortname;                                      /* Module name */
        IntPtr  psz_longname;                           /* Module descriptive name */
        IntPtr  psz_help;                /* Long help string for "special" modules */
        IntPtr  psz_program;        /* Program name which will activate the module */
        IntPtr  pp_shortcuts;//[ MODULE_SHORTCUT_MAX ];    /* Shortcuts to the module */
        IntPtr  psz_capability;                                   /* Capability */
        int     i_score;                           /* Score for each capability */
        UInt32  i_cpu;                             /* Required CPU capabilities */
        bool    b_unloadable;                          /* Can we be dlclosed? */
        bool    b_reentrant;                             /* Are we reentrant? */
        bool    b_submodule;                          /* Is this a submodule? */
        IntPtr  pf_activate;
        IntPtr  pf_deactivate;
        IntPtr  p_config;             /* Module configuration structure */
        UInt32  i_config_items;        /* number of configuration items */
        UInt32  i_bool_items;            /* number of bool config items */
        IntPtr  handle;                             /* Unique handle */
        IntPtr  psz_filename;                     /* Module filename */
        bool    b_builtin;  /* Set to true if the module is built in */
        bool    b_loaded;        /* Set to true if the dll is loaded */
        IntPtr  p_symbols;
    }

    [StructLayout(LayoutKind.Explicit)]
    public struct vlc_value_t
    {
        [FieldOffset(0)]
        public Int32 i_int;
        [FieldOffset(0)]
        public Int32 b_bool;
        [FieldOffset(0)]
        [MarshalAs(UnmanagedType.R4)]
        public float f_float;
        [FieldOffset(0)]
        public IntPtr psz_string;
        [FieldOffset(0)]
        public IntPtr p_address;
        [FieldOffset(0)]
        public IntPtr p_object;
        [FieldOffset(0)]
        public IntPtr p_list;
        [FieldOffset(0)]
        [MarshalAs(UnmanagedType.I8)]
        public Int64 i_time;
        [FieldOffset(0)]
        public IntPtr psz_name;
        [FieldOffset(4)]
        public Int32 i_object_id;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct vlc_list_t
    {
        public int i_count;
        public IntPtr p_values;
        public IntPtr pi_types;
    }

    public class VlcObject : IDisposable
    {
        private bool _disposed = false;
        private VlcObjectHandle _vlcObject;

        internal VlcObject(Int32 ID)
        {
            _vlcObject = vlc_current_object(ID);
        }

        private VlcObject(VlcObjectHandle p)
        {
            _vlcObject = p;
        }

        public void Dispose()
        {
            Dispose(true);
			GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                _disposed = true;
                if (!_vlcObject.IsInvalid)
                    _vlcObject.Dispose();
            }
        }

        ~VlcObject()
        {
            Dispose(false);
        }

        public VlcObject FindObject(VlcObjectType type, VlcObjectSearchMode mode)
        {
            if (_vlcObject.IsInvalid)
                throw new NullReferenceException("VLC object is NULL");

            VlcObject o = new VlcObject(__vlc_object_find(_vlcObject, type, mode));
            return o;
        }

		public void GetListChoices(string name, out IList<string> choices)
		{
			if (_vlcObject.IsInvalid)
                throw new NullReferenceException("VLC object is NULL");

            choices = new List<string>();

			vlc_value_t a = new vlc_value_t();
            vlc_value_t b = new vlc_value_t();
			__var_Change(_vlcObject, name, VlcChangeAction.GetList, ref a, ref b );
		
            vlc_list_t t = (vlc_list_t)Marshal.PtrToStructure(b.p_list, typeof(vlc_list_t));
            for (int i = 0; i < t.i_count; i++)
            {
                IntPtr textPtr = new IntPtr(t.p_values.ToInt32() + i * Marshal.SizeOf(typeof(vlc_value_t)));
                vlc_value_t textValue = (vlc_value_t)Marshal.PtrToStructure(textPtr, typeof(vlc_value_t));
                choices.Add(Marshal.PtrToStringAnsi(textValue.psz_string));
            }

			__var_Change(_vlcObject, name, VlcChangeAction.FreeList, ref a, ref b );
		}

        public string GetStringValue(string name)
        {
            if (_vlcObject.IsInvalid)
                throw new NullReferenceException("VLC object is NULL");

			vlc_value_t v = new vlc_value_t();
            __var_Get(_vlcObject, name, ref v);
			
			return Marshal.PtrToStringAuto(v.psz_string);
        }

        public void SetStringValue(string name, string val)
        {
            if (_vlcObject.IsInvalid)
                throw new NullReferenceException("VLC object is NULL");

            vlc_value_t v = new vlc_value_t();
            v.psz_string = Marshal.StringToHGlobalAnsi(val);

            __var_Set(_vlcObject, name, ref v);
            Marshal.FreeHGlobal(v.psz_string);
        }

        public Int32 GetIntValue(string name)
        {
            if (_vlcObject.IsInvalid)
                throw new NullReferenceException("VLC object is NULL");

            vlc_value_t v = new vlc_value_t();
            __var_Get(_vlcObject, name, ref v);

            return v.i_int;
        }

        public void SetIntValue(string name, Int32 val)
        {
            if (_vlcObject.IsInvalid)
                throw new NullReferenceException("VLC object is NULL");

            vlc_value_t v = new vlc_value_t();
            v.i_int = val;
            __var_Set(_vlcObject, name, ref v);
        }

        public IList<string> ModuleList()
        {
            if (_vlcObject.IsInvalid)
                throw new NullReferenceException("VLC object is NULL");

            IList<string> ret = new List<string>();
            IntPtr ptrList = __vlc_list_find(_vlcObject, VlcObjectType.Module, VlcObjectSearchMode.Anywhere);
            if (ptrList != IntPtr.Zero)
            {
                vlc_list_t list = (vlc_list_t)Marshal.PtrToStructure(ptrList, typeof(vlc_list_t));
                for (int i = 0; i < list.i_count; i++)
                {
                    IntPtr ptrValue = new IntPtr(list.p_values.ToInt32() + i * Marshal.SizeOf(typeof(vlc_value_t)));
                    vlc_value_t value = (vlc_value_t)Marshal.PtrToStructure(ptrValue, typeof(vlc_value_t));
                    vlc_common_t cmn = (vlc_common_t)Marshal.PtrToStructure(value.p_object, typeof(vlc_common_t));
//TODO: Complete implementation from 'zsh.cpp'
                    string s = Marshal.PtrToStringAnsi(cmn.psz_object_name);
                    ret.Add(s);
                }
            }

            return ret;
        }

        #region libvlc api
        [DllImport("libvlc")]
        private static extern VlcObjectHandle vlc_current_object(int i_object);

        [DllImport("libvlc")]
        private static extern VlcObjectHandle __vlc_object_find(VlcObjectHandle p_object, VlcObjectType objectType, VlcObjectSearchMode mode);

        [DllImport("libvlc")]
        private static extern IntPtr __vlc_list_find(VlcObjectHandle p_object, VlcObjectType objectType, VlcObjectSearchMode mode);

        [DllImport("libvlc")]
        private static extern Int32 __var_Get(VlcObjectHandle p_object, [MarshalAs(UnmanagedType.LPStr)] String name, ref vlc_value_t value);

        [DllImport("libvlc")]
        private static extern Int32 __var_Set(VlcObjectHandle p_object, [MarshalAs(UnmanagedType.LPStr)] String name, ref vlc_value_t value);
		
        [DllImport("libvlc")]
        private static extern Int32 __var_Change(
            VlcObjectHandle p_ojbect, 
            [MarshalAs(UnmanagedType.LPStr)] string name,
           VlcChangeAction action, 
            ref vlc_value_t valA, 
            ref vlc_value_t valB);
        #endregion
    }
}

