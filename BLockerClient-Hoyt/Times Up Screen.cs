using System.Runtime.InteropServices;

namespace BLockerClient_Hoyt
{
	public partial class Times_Up_Screen : Form
	{
		[DllImport("user32.dll", SetLastError = true)]
		private static extern bool LockWorkStation();
		public Times_Up_Screen()
		{
			InitializeComponent();
			timer1.Start();
		}

		private void Times_Up_Screen_Load(object sender, EventArgs e)
		{
			
		}

		private void button1_Click(object sender, EventArgs e)
		{
			LockWorkStation();
		}

        private void timer1_Tick(object sender, EventArgs e)
        {
			
        }

        private void Times_Up_Screen_FormClosing(object sender, FormClosingEventArgs e)
        {

        }
    }
}
