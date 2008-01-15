using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Atx.LibVLC;
using System.Runtime.InteropServices;
using System.Threading;

namespace test_project
{
    //public class LogForm : Form
    //{
    //    private ListBox _lb = new ListBox();
    //    private VlcInstance _inst;
    //    private EventWaitHandle _stop = new EventWaitHandle(false, EventResetMode.ManualReset);
    //    private Thread _th;
    //    private delegate void DumpMessagesDelegate();

    //    public LogForm(VlcInstance inst)
    //    {
    //        _lb.IntegralHeight = false;
    //        _lb.HorizontalScrollbar = true;
    //        _inst = inst;
    //        _th = new Thread(new ThreadStart(this.ThreadMethod));

    //        this.Load += new EventHandler(this.OnLoad);
    //        this.Layout += new LayoutEventHandler(this.OnLayout);
    //    }

    //    private void OnLoad(object sender, EventArgs e)
    //    {
    //        Controls.Add(_lb);
    //        //_th.Start();
    //    }

    //    private void DumpMessages()
    //    {
    //        foreach (VlcLogMessage vlm in _inst.Log)
    //        {
    //            _lb.Items.Add(String.Format("{0} {1} {2} {3} {4}", vlm.Severity, vlm.Type, vlm.Name, vlm.Header, vlm.Message));
    //            _lb.SetSelected(_lb.Items.Count - 1, true);
    //        }
    //    }

    //    private void ThreadMethod()
    //    {
    //        while (!_stop.WaitOne(1000, false))
    //        {
    //            Invoke(new DumpMessagesDelegate(DumpMessages));
    //        }
    //    }

    //    public void Stop()
    //    {
    //        _stop.Set();
    //        //_th.Join();
    //    }

    //    protected override void OnClosing(CancelEventArgs e)
    //    {
    //        Stop();
    //        base.OnClosing(e);
    //    }

    //    private void OnLayout(object sender, LayoutEventArgs e)
    //    {
    //        _lb.Location = new Point(ClientRectangle.Left, ClientRectangle.Top);
    //        _lb.Size = new Size(ClientRectangle.Width, ClientRectangle.Height);
    //    }
    //}

    public class Form1 : Form
    {
        private VlcInstance _v;
        //private LogForm _logForm;
        private Control c = new Control();
        private Button bnPause = new Button();
		private Button bnAr43 = new Button();
		private Button bnAr169 = new Button();
		private Button bnAr = new Button();
		
        public Form1()
        {
			this.Load += new EventHandler(this.OnLoad);
			this.Layout += new LayoutEventHandler(this.OnLayout);
			
			bnPause.Text = "Pause";
            bnPause.Click += new EventHandler(this.OnPauseClicked);
			
			bnAr43.Text = "4:3";
            bnAr43.Click += new EventHandler(this.OnAr43Clicked);
			
			bnAr169.Text = "16:9";
            bnAr169.Click += new EventHandler(this.OnAr169Clicked);
			
			bnAr.Text = "Source";
            bnAr.Click += new EventHandler(this.OnArClicked);
        }

        private void OnLoad(object sender, EventArgs e)
        {
			Width += 100;
            Controls.Add(c);
            Controls.Add(bnPause);
			Controls.Add(bnAr43);
			Controls.Add(bnAr169);
			Controls.Add(bnAr);

            Text = "libvlc version: " + VlcInstance.VlcLibraryVersion;

            VlcConfig vlcConfig = new VlcConfig();
            vlcConfig.PluginPath = "C:\\Program Files\\VideoLAN\\VLC\\Plugins";

			_v = new VlcInstance(vlcConfig);

			_v.Owner = c.Handle;               
            _v.VlcPlaylist.Add("");
            _v.VlcPlaylist.Play();

            //_logForm = new LogForm(_v);
            //_logForm.Show();
            //_logForm.Top = this.Top;
            //_logForm.Left = this.Right;
        }

        private void OnLayout(object sender, LayoutEventArgs e)
        {
            c.Width = ClientRectangle.Width;
            c.Height = ClientRectangle.Height - 30;
			
            bnPause.Top = c.Bottom;
            bnPause.Height = ClientRectangle.Bottom - c.Bottom;
			
			bnAr43.Top = c.Bottom;
			bnAr43.Left = bnPause.Right+5;
			bnAr43.Height = ClientRectangle.Bottom - c.Bottom;
			
			bnAr169.Top = c.Bottom;
			bnAr169.Left = bnAr43.Right;
			bnAr169.Height = ClientRectangle.Bottom - c.Bottom;
			
			bnAr.Top = c.Bottom;
			bnAr.Left = bnAr169.Right;
			bnAr.Height = ClientRectangle.Bottom - c.Bottom;
        }

        private void OnPauseClicked(object o, EventArgs e)
        {
            VlcObject vout = _v.VlcObject.FindObject(VlcObjectType.VOut, VlcObjectSearchMode.Child);

			IList<string> choices;
            vout.GetListChoices("deinterlace", out choices);
			
			_v.VlcPlaylist.Pause();
        }

        private void OnAr43Clicked(object o, EventArgs e)
		{
			_v.VlcInput.VideoAspectRatio = "4:3";
		}

		private void OnAr169Clicked(object o, EventArgs e)
		{
			_v.VlcInput.VideoAspectRatio = "16:9";
		}
		
		private void OnArClicked(object o, EventArgs e)
		{
            VlcObject vo = _v.VlcObject.FindObject(VlcObjectType.VOut, VlcObjectSearchMode.Child);
            vo.SetStringValue("aspect-ratio", "");
			//_v.Input.VideoAspectRatio = "";
		}
		
		protected override void OnClosing (CancelEventArgs e)
		{
            //_logForm.Stop();
			_v.Dispose();
			base.OnClosing (e);
		}

    }
}