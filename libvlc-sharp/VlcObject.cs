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


    internal class VlcListHandle : SafeHandle
    {
        public VlcListHandle()
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
                vlc_list_release(this);
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
        private static extern void vlc_list_release(VlcListHandle p_list);
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
                _vlcObject.Dispose();
            }
        }

        ~VlcObject()
        {
            Dispose(false);
        }

        private IList<String> _moduleList;
        public IList<String> ModuleList
        {
            get
            {
                if (_vlcObject.IsInvalid)
                    throw new ApplicationException("VLC object is NULL");

                if (_moduleList == null)
                {
                    _moduleList = new List<String>();
                    using (VlcListHandle ptrList = __vlc_list_find(_vlcObject, VlcObjectType.Module, VlcObjectSearchMode.Anywhere))
                    {
                        if (!ptrList.IsInvalid)
                        {
                            vlc_list_t list = (vlc_list_t)Marshal.PtrToStructure(ptrList.DangerousGetHandle(), typeof(vlc_list_t));
                            for (int i = 0; i < list.i_count; i++)
                            {
                                IntPtr ptrValue = new IntPtr(list.p_values.ToInt32() + i * Marshal.SizeOf(typeof(vlc_value_t)));
                                vlc_value_t value = (vlc_value_t)Marshal.PtrToStructure(ptrValue, typeof(vlc_value_t));
                                vlc_common_t common = (vlc_common_t)Marshal.PtrToStructure(value.p_object, typeof(vlc_common_t));

                                _moduleList.Add(Marshal.PtrToStringAnsi(common.psz_object_name));
                            }
                        }
                    }
                }

                return _moduleList;
            }
        }

        public VlcObject FindObject(VlcObjectType type, VlcObjectSearchMode mode)
        {
            if (_vlcObject.IsInvalid)
                throw new ApplicationException("VLC object is NULL");

            VlcObject o = new VlcObject(__vlc_object_find(_vlcObject, type, mode));
            return o;
        }

		public void GetListChoices(String name, out IList<String> choices)
		{
			if (_vlcObject.IsInvalid)
                throw new ApplicationException("VLC object is NULL");

            choices = new List<String>();

			vlc_value_t a = new vlc_value_t();
            vlc_value_t b = new vlc_value_t();
			__var_Change(_vlcObject, name, VlcChangeAction.GetList, ref a, ref b );
		
            try
            {
                vlc_list_t t = (vlc_list_t)Marshal.PtrToStructure(b.p_list, typeof(vlc_list_t));
                for (int i = 0; i < t.i_count; i++)
                {
                    IntPtr textPtr = new IntPtr(t.p_values.ToInt32() + i * Marshal.SizeOf(typeof(vlc_value_t)));
                    vlc_value_t textValue = (vlc_value_t)Marshal.PtrToStructure(textPtr, typeof(vlc_value_t));
                    choices.Add(Marshal.PtrToStringAnsi(textValue.psz_string));
                }
            }
            finally
            {
                __var_Change(_vlcObject, name, VlcChangeAction.FreeList, ref a, ref b);
            }
		}

        public String GetStringValue(String name)
        {
            if (_vlcObject.IsInvalid)
                throw new ApplicationException("VLC object is NULL");

			vlc_value_t v = new vlc_value_t();
            __var_Get(_vlcObject, name, ref v);
			
			return Marshal.PtrToStringAuto(v.psz_string);
        }

        public void SetStringValue(String name, String val)
        {
            if (_vlcObject.IsInvalid)
                throw new ApplicationException("VLC object is NULL");

            vlc_value_t v = new vlc_value_t();
            v.psz_string = Marshal.StringToHGlobalAnsi(val);
            try
            {
                __var_Set(_vlcObject, name, ref v);
            }
            finally
            {
                Marshal.FreeHGlobal(v.psz_string);
            }
        }

        public Int32 GetIntValue(String name)
        {
            if (_vlcObject.IsInvalid)
                throw new ApplicationException("VLC object is NULL");

            vlc_value_t v = new vlc_value_t();
            __var_Get(_vlcObject, name, ref v);

            return v.i_int;
        }

        public void SetIntValue(String name, Int32 val)
        {
            if (_vlcObject.IsInvalid)
                throw new ApplicationException("VLC object is NULL");

            vlc_value_t v = new vlc_value_t();
            v.i_int = val;
            __var_Set(_vlcObject, name, ref v);
        }

        public Boolean GetBoolValue(string name)
        {
            if (_vlcObject.IsInvalid)
                throw new ApplicationException("VLC object is NULL");

            vlc_value_t v = new vlc_value_t();
            __var_Get(_vlcObject, name, ref v);

            return v.b_bool != 0;
        }

        public void SetBoolValue(String name, Boolean val)
        {
            if (_vlcObject.IsInvalid)
                throw new ApplicationException("VLC object is NULL");

            vlc_value_t v = new vlc_value_t();
            v.b_bool = val ? 1 : 0;
            __var_Set(_vlcObject, name, ref v);
        }

        public String GetConfigStringValue(String name)
        {
            if (_vlcObject.IsInvalid)
                throw new ApplicationException("VLC object is NULL");

            return __config_GetPsz(_vlcObject, name);
        }

        public void SetConfigStringValue(String name, String value)
        {
            if (_vlcObject.IsInvalid)
                throw new ApplicationException("VLC object is NULL");

            __config_PutPsz(_vlcObject, name, value);
        }

        public float GetConfigFloatValue(String name)
        {
            if (_vlcObject.IsInvalid)
                throw new ApplicationException("VLC object is NULL");

            return __config_GetFloat(_vlcObject, name);
        }

        public void SetConfigFloatValue(String name, float value)
        {
            if (_vlcObject.IsInvalid)
                throw new ApplicationException("VLC object is NULL");

            __config_PutFloat(_vlcObject, name, value);
        }

        public Int32 GetConfigIntValue(String name)
        {
            if (_vlcObject.IsInvalid)
                throw new ApplicationException("VLC object is NULL");

            return __config_GetInt(_vlcObject, name);
        }

        public void SetConfigIntValue(String name, Int32 value)
        {
            if (_vlcObject.IsInvalid)
                throw new ApplicationException("VLC object is NULL");

            __config_PutInt(_vlcObject, name, value);
        }

        #region libvlc api
        [DllImport("libvlc")]
        private static extern VlcObjectHandle vlc_current_object(int i_object);

        [DllImport("libvlc")]
        private static extern VlcObjectHandle __vlc_object_find(VlcObjectHandle p_object, VlcObjectType objectType, VlcObjectSearchMode mode);

        [DllImport("libvlc")]
        private static extern VlcListHandle __vlc_list_find(VlcObjectHandle p_object, VlcObjectType objectType, VlcObjectSearchMode mode);

        [DllImport("libvlc")]
        private static extern Int32 __var_Get(VlcObjectHandle p_object, [MarshalAs(UnmanagedType.LPStr)] String name, ref vlc_value_t value);

        [DllImport("libvlc")]
        private static extern Int32 __var_Set(VlcObjectHandle p_object, [MarshalAs(UnmanagedType.LPStr)] String name, ref vlc_value_t value);

        [DllImport("libvlc")]
        private static extern Int32 __var_Change(VlcObjectHandle p_ojbect, [MarshalAs(UnmanagedType.LPStr)] String name, VlcChangeAction action,  ref vlc_value_t valA, ref vlc_value_t valB);

//TODO: Implement and test the following:
        //[DllImport("libvlc")]
        //Int32 __config_GetType(VlcObjectHandle p_object, [MarshalAs(UnmanagedType.LPStr)] String name);

        [DllImport("libvlc")]
        private static extern Int32 __config_GetInt(VlcObjectHandle p_object, [MarshalAs(UnmanagedType.LPStr)] String name);

        [DllImport("libvlc")]
        private static extern void __config_PutInt(VlcObjectHandle p_object, [MarshalAs(UnmanagedType.LPStr)] String name, Int32 value);
        
        [DllImport("libvlc")][return:MarshalAs(UnmanagedType.R4)]
        private static extern float __config_GetFloat(VlcObjectHandle p_object, [MarshalAs(UnmanagedType.LPStr)] String name);

        [DllImport("libvlc")]
        private static extern void __config_PutFloat(VlcObjectHandle p_object, [MarshalAs(UnmanagedType.LPStr)] String name, [MarshalAs(UnmanagedType.R4)] float value);
        
        [DllImport("libvlc")]
        private static extern String __config_GetPsz(VlcObjectHandle p_object, [MarshalAs(UnmanagedType.LPStr)] String name);

        [DllImport("libvlc")]
        private static extern void __config_PutPsz(VlcObjectHandle p_object, [MarshalAs(UnmanagedType.LPStr)] String name, [MarshalAs(UnmanagedType.LPStr)] String value);

        #endregion
    }
}

