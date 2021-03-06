// this is an adaptation of NUnitLite's TcpWriter.cs with an additional 
// overrides and with network-activity UI enhancement

using System;
using System.IO;
using System.Net.Sockets;
using System.Text;
using MonoTouch.UIKit;
using MonoTouch.NUnit.UI;

namespace MonoTouch.NUnit
{
	public class TcpTextWriter : TextWriter, ITestWriter
	{
		private TcpClient client;
		private StreamWriter writer;

		public TcpTextWriter (string hostName, int port)
		{
			if (hostName == null)
				throw new ArgumentNullException ("hostName");
			if ((port < 0) || (port > UInt16.MaxValue))
				throw new ArgumentException ("port");
			
			HostName = hostName;
			Port = port;
			
			UIApplication.SharedApplication.NetworkActivityIndicatorVisible = true;

			try {
				client = new TcpClient (hostName, port);
				writer = new StreamWriter (client.GetStream ());
			} catch {
				UIApplication.SharedApplication.NetworkActivityIndicatorVisible = false;
				throw;
			}
		}

		public string HostName { get; private set; }

		public int Port { get; private set; }
		// we override everything that StreamWriter overrides from TextWriter
		public override System.Text.Encoding Encoding {
			// hardcoded to UTF8 so make it easier on the server side
			get { return System.Text.Encoding.UTF8; }
		}

		public override void Close ()
		{
			UIApplication.SharedApplication.NetworkActivityIndicatorVisible = false;
			writer.Close ();
		}

		protected override void Dispose (bool disposing)
		{
			writer.Dispose ();
		}

		public override void Flush ()
		{
			writer.Flush ();
		}
		// minimum to override - see http://msdn.microsoft.com/en-us/library/system.io.textwriter.aspx
		public override void Write (char value)
		{
			writer.Write (value);
		}

		public override void Write (char[] buffer)
		{
			writer.Write (buffer);
		}

		public override void Write (char[] buffer, int index, int count)
		{
			writer.Write (buffer, index, count);
		}

		public override void Write (string value)
		{
			writer.Write (value);
		}
		// special extra override to ensure we flush data regularly
		public override void WriteLine ()
		{
			writer.WriteLine ();
			writer.Flush ();
		}

		#region ITestWriter implementation

		public void WriteCommentLine (string format, params string[] args)
		{
			WriteLine (format, args);
		}

		#endregion

	}
}