using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Atx.LibVLC
{
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

////TODO: Struct sizes will be different on linux and os x
//    //HACK: 24-byte structure
//    [StructLayout(LayoutKind.Sequential)]
//    internal struct Critical_Section
//    {
//        Int32 a;
//        Int32 b;
//        Int32 c;
//        Int32 d;
//        Int32 e;
//        Int32 f;
//    }

////TODO: Struct sizes will be different on linux and os x
//    [StructLayout(LayoutKind.Sequential)]
//    internal struct vlc_mutex_t
//    {
//        public IntPtr mutex;
//        public Critical_Section csection;
//        public IntPtr p_this;
//    }

////TODO: Struct sizes will be different on linux and os x
//    [StructLayout(LayoutKind.Sequential)]
//    internal struct vlc_cond_t
//    {
//        volatile Int32  i_waiting_threads;
//        IntPtr          event_obj;
//        IntPtr          SignalObjectAndWait;
//        IntPtr          semaphore;
//        Critical_Section csection;
//        Int32           i_win9x_cv;
//        IntPtr          p_this;
//    }

    [StructLayout(LayoutKind.Sequential)]
    internal class vlc_common_t
    {
        public Int32            i_object_id;
        public VlcObjectType    i_object_type;
        public IntPtr           psz_object_type;
        public IntPtr           psz_object_name;
        public IntPtr           psz_header;
        public Int32            i_flags;
        public bool             b_thread;
        public IntPtr           thread_id;
        /*               Not portable //
        public vlc_mutex_t object_lock;
        public vlc_cond_t object_wait;
        public volatile bool b_error;
        public volatile bool b_die;
        public volatile bool b_dead;
        public volatile bool b_attached;
        public bool b_force;
        public vlc_mutex_t var_lock;
        public Int32 i_vars;
        public IntPtr p_vars;
        public IntPtr p_libvlc;
        public IntPtr p_vlc;
        public volatile Int32 i_refcount;
        public IntPtr p_parent;
        public IntPtr pp_children;
        public volatile Int32 i_children;
        public IntPtr p_private;
        public Int32 be_sure_to_add_VLC_COMMON_MEMBERS_to_struct;
         */
    }

////TODO: Struct sizes will be different on linux and os x
//    [StructLayout(LayoutKind.Sequential)]
//    internal class module_t : vlc_common_t
//    {
//        public IntPtr psz_shortname;
//        public IntPtr psz_longname;
//        public IntPtr psz_help;
//        public IntPtr psz_program;
//        public IntPtr pp_shortcuts;
//        public IntPtr psz_capability;
//        public Int32 i_score;
//        public UInt32 i_cpu;
//        public bool b_unloadable;
//        public bool b_reentrant;
//        public bool b_submodule;
//        public IntPtr pf_activate;
//        public IntPtr pf_deactivate;
//        public IntPtr p_config;
//        public UInt32 i_config_items;
//        public UInt32 i_bool_items;
//        public IntPtr handle;
//        public IntPtr psz_filename;
//        public bool b_builtin;
//        public bool b_loaded;
//        public IntPtr p_symbols;
//    }

    [StructLayout(LayoutKind.Explicit)]
    internal struct vlc_value_t
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
        public Int32 i_count;
        public IntPtr p_values;
        public IntPtr pi_types;
    }
}
