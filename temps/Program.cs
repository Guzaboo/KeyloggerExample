using System;
using System.Threading;
using System.Windows.Forms;

public class Program
{
	public static void Main()
    {
        Application.SetUnhandledExceptionMode(UnhandledExceptionMode.ThrowException);
        Form1 form = new Form1();
        Application.Run(/*form*/);
        //form.Show();
        //new ManualResetEvent(false).WaitOne();

        //Uncomment the 3 above lines to test the reader in a visible form.
    }
}