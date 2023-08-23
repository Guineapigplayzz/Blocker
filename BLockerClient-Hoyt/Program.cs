namespace BLockerClient_Hoyt
{
	internal static class Program
	{
		/// <summary>
		///  The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main()
		{
			// To customize application configuration such as set high DPI settings or default font,
			// see https://aka.ms/applicationconfiguration.
			ApplicationConfiguration.Initialize();
			

			var keyboardHook = new KeyboardHook();
			keyboardHook.Start();
			Application.Run(new Form1());
			keyboardHook.Stop();
		}
	}


}