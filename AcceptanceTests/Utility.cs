using System;
using System.Diagnostics;

namespace AcceptanceTests
{
	public static class Utility
	{
		public static string ExecuteCmd(string arguments, string workingDirectory, out int exitCode)
		{
			// Create the Process Info object with the overloaded constructor
			// This takes in two parameters, the program to start and the
			// command line arguments.
			// The arguments parm is prefixed with "@" to eliminate the need
			// to escape special characters (i.e. backslashes) in the
			// arguments string and has "/C" prior to the command to tell
			// the process to execute the command quickly without feedback.
			ProcessStartInfo _info =
				new ProcessStartInfo("cmd", "/C \"" + arguments + "\"");

			//Set Working Directory
			_info.WorkingDirectory = workingDirectory;

			// The following commands are needed to redirect the
			// standard output.  This means that it will be redirected
			// to the Process.StandardOutput StreamReader.
			_info.RedirectStandardOutput = true;
			_info.RedirectStandardError = true;

			// Set UseShellExecute to false.  This tells the process to run
			// as a child of the invoking program, instead of on its own.
			// This allows us to intercept and redirect the standard output.
			_info.UseShellExecute = false;

			// Set CreateNoWindow to true, to supress the creation of
			// a new window
			_info.CreateNoWindow = true;

			// Create a process, assign its ProcessStartInfo and start it
			Process _p = new Process();
			_p.StartInfo = _info;
			_p.Start();

			// Capture the results in a string
			string _processResults = _p.StandardOutput.ReadToEnd();
			string _processError = _p.StandardError.ReadToEnd();

			if (!string.IsNullOrWhiteSpace(_processError))
				throw new System.Exception(_processError);

			_p.WaitForExit();
			exitCode = _p.ExitCode;

			// Close the process to release system resources
			_p.Close();

			// Return the output stream to the caller
			return _processResults;
		}

		public static string ExecuteCmd(string arguments, out int exitCode)
		{
			return ExecuteCmd(arguments, null, out exitCode);
		}
	}
}