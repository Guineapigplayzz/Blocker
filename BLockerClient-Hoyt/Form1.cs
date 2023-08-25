using Microsoft.Win32;
using System.Diagnostics;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Timers;

namespace BLockerClient_Hoyt
{
	public partial class Form1 : Form
	{
		private DateTime limitEndTime;
		private TcpListener tcpListener;
		private const int HWND_TOPMOST = -1;
		private const uint SWP_NOSIZE = 0x0001;
		private const uint SWP_NOMOVE = 0x0002;
		private const uint SWP_SHOWWINDOW = 0x0040;
        private bool isBackgroundProcessRunning;
		private static bool isFormShowing = false;

		[DllImport("user32.dll")]
		private static extern bool SetWindowPos(IntPtr hWnd, int hWndInsertAfter, int x, int y, int cx, int cy, uint uFlags);

		[DllImport("user32.dll")]
		private static extern IntPtr GetDC(IntPtr hWnd);

		[DllImport("user32.dll")]
		private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

		[DllImport("user32.dll")]
		private static extern bool ShowCursor(bool bShow);

		public Form1()
		{
			string currentDirectory = Directory.GetCurrentDirectory();
			Icon myIcon = new Icon(currentDirectory + "\\vsicon.ico");
			InitializeComponent();
			var notifyIcon = new NotifyIcon();
			notifyIcon.Icon = myIcon;
			notifyIcon.Text = "BlockerClient";
			

			// Create a context menu for the NotifyIcon

			notifyIcon.Visible = true;
			this.ShowInTaskbar = false;

		}

		[STAThread]
		private void Form1_Load(object sender, EventArgs e)
		{



			timer1.Start();
			isFormShowing = false;
		}

			private void Form1_FormClosing(object sender, FormClosingEventArgs e)
			{

				
				

			// Cancel the form closing event to prevent the application from closing
				e.Cancel = true;
				isFormShowing = false;
				Form mainForm = (Form)sender;
				mainForm.Hide();
				this.ShowInTaskbar = false;
				CheckTimeLimit();
			}


		
		protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
		{
			DateTime currentTime = DateTime.Now;
			limitEndTime = DateTime.Today.AddHours(22);
			
			if (currentTime > limitEndTime)
			{
			if (currentTime == DateTime.Today.AddHours(5))
            {
					return base.ProcessCmdKey(ref msg, keyData);
				}
                else
                {
					return true;
				}
			
			}

		return base.ProcessCmdKey(ref msg, keyData);
		}

		protected void Form1_Shown(object sender, EventArgs e)
		{
			

			//base.OnLoad(e);

			// Blur all monitors

			// Hide the mouse cursor
			//ShowCursor(false);
		}

		protected override void OnClosed(EventArgs e)
		{
			base.OnClosed(e);

			// Show the mouse cursor before closing the application
			ShowCursor(true);
		}

		protected override CreateParams CreateParams
		{
			get
			{
				// Extend the window style to disable minimize, maximize, and close buttons
				CreateParams cp = base.CreateParams;
				cp.Style &= ~(int)0x00C00000; // WS_CAPTION
				cp.Style &= ~(int)0x00020000; // WS_MINIMIZEBOX
				cp.Style &= ~(int)0x00010000; // WS_MAXIMIZEBOX
				cp.ClassStyle |= 0x00020000; // CS_NOCLOSE
				return cp;
			}
		}
       

        private void CheckTimeLimit()
		{
			// Check if the current time exceeds the time limit or it's 5 AM
			DateTime currentTime = DateTime.Now;
			limitEndTime = DateTime.Today.AddHours(22);
			int hour = currentTime.Hour;
			string period = (hour < 12) ? "AM" : "PM";
			var a = DateTime.Today.AddHours(29);
			var aam = "AM";
			var b = limitEndTime;
			if (aam == period)
			{
			
				if (currentTime >= a)
				{


				if (this.ShowInTaskbar == true)
				{
					this.ShowInTaskbar = false;
					isFormShowing = false;
					this.Close();
				}
				else
				{
					return;
				}	



				}
			
			


			

			}
            else if (isFormShowing == false || this.ShowInTaskbar == false)
            {
				


					// If the current time is already past 11 PM, add one day to the limit end time
					if (currentTime > limitEndTime)
					{
						if (isFormShowing == false || this.ShowInTaskbar == false)
						{
							// Create and show the monitor form
							foreach (Screen screen in Screen.AllScreens)
							{
								Form monitorForm = new Form();
								monitorForm.FormBorderStyle = FormBorderStyle.None;
								monitorForm.StartPosition = FormStartPosition.Manual;
								monitorForm.Bounds = screen.Bounds;
								monitorForm.WindowState = FormWindowState.Maximized;
								monitorForm.TopMost = true;
								monitorForm.Opacity = 0.75;

								monitorForm.Show();
								this.ShowInTaskbar = true;
							}
						}

						if (isFormShowing == false || this.ShowInTaskbar == false)
						{
							// Create and show the center form
							Times_Up_Screen centerForm = new Times_Up_Screen();
							centerForm.StartPosition = FormStartPosition.CenterScreen;
							centerForm.TopMost = true;

							// Ensure that the center form is always on top of the monitor form
							centerForm.Owner = Application.OpenForms[0];
							centerForm.ShowDialog();
							isFormShowing = true;
						}

				}
			}


		}


        private void timer1_Tick(object sender, EventArgs e)
        {
			CheckTimeLimit();
        }

        private void Form1_Click(object sender, EventArgs e)
        {
			
        }
    }

    public class KeyboardHook
	{
		private const int WH_KEYBOARD_LL = 13;
		private const int WM_KEYDOWN = 0x0100;
		private const int WM_SYSKEYDOWN = 0x0104;

		private delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);

		private LowLevelKeyboardProc _proc;
		private IntPtr _hookID = IntPtr.Zero;

		private DateTime limitEndTime = DateTime.Today.AddHours(22);

		public void Start()
		{
			_proc = HookCallback;
			_hookID = SetHook(_proc);
		}

		public void Stop()
		{
			UnhookWindowsHookEx(_hookID);
		}

		private IntPtr SetHook(LowLevelKeyboardProc proc)
		{
			using (Process curProcess = Process.GetCurrentProcess())
			using (ProcessModule curModule = curProcess.MainModule)
			{
				return SetWindowsHookEx(WH_KEYBOARD_LL, proc, GetModuleHandle(curModule.ModuleName), 0);
			}
		}

		private IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
		{
			if (nCode >= 0 && (wParam == (IntPtr)WM_KEYDOWN || wParam == (IntPtr)WM_SYSKEYDOWN))
			{
				int vkCode = Marshal.ReadInt32(lParam);

				DateTime currentTime = DateTime.Now;

				if (currentTime > limitEndTime )
				{
					if(currentTime >= DateTime.Today.AddHours(5))
                    {
						return CallNextHookEx(_hookID, nCode, wParam, lParam);
					}
                    else
                    {
						// Cancel the key press
						return CallNextHookEx(_hookID, nCode, wParam, lParam);
						//return (IntPtr)1;
					}
				
				}
			}

			return CallNextHookEx(_hookID, nCode, wParam, lParam);
		}

		#region DLL Imports

		[DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);

		[DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		private static extern bool UnhookWindowsHookEx(IntPtr hhk);

		[DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

		[DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		private static extern IntPtr GetModuleHandle(string lpModuleName);

		#endregion
	}
}