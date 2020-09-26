using System;
using System.Net.Mail;
using System.Windows;
using Props = WBZ.Properties.Settings;

namespace WBZ.Helpers
{
	internal class Mail
	{
		/// <summary>
		/// Send
		/// </summary>
		internal static bool SendMail(string from, string[] to, string subject, string body)
		{
			try
			{
				var mail = new MailMessage
				{
					From = new MailAddress(from),
					Subject = subject,
					Body = body
				};
				foreach (var x in to)
					mail.To.Add(x);

				var SmtpServer = new SmtpClient(Props.Default.config_Email_Host, int.Parse(Props.Default.config_Email_Port))
				{
					Credentials = new System.Net.NetworkCredential(Props.Default.config_Email_Email, Global.Decrypt(Props.Default.config_Email_Password)),
					EnableSsl = true
				};

				SmtpServer.Send(mail);
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
				return false;
			}

			return true;
		}

		/// <summary>
		/// Test
		/// </summary>
		internal static bool TestMail()
		{
			//TODO - testowanie poprawności konfiguracji maili
			/*
			IPHostEntry hostEntry = Dns.GetHostEntry(smtpServerAddress);
			IPEndPoint endPoint = new IPEndPoint(hostEntry.AddressList[0], port);
			using (Socket tcpSocket = new Socket(endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp))
			{
				//try to connect and test the rsponse for code 220 = success
				tcpSocket.Connect(endPoint);
				if (!CheckResponse(tcpSocket, 220))
					return false;

				// send HELO and test the response for code 250 = proper response
				SendData(tcpSocket, string.Format("HELO {0}\r\n", Dns.GetHostName()));
				if (!CheckResponse(tcpSocket, 250))
					return false;

				return true;
			}
			*/
			if (SendMail(Props.Default.config_Email_Email,
					new string[] { Props.Default.config_Email_Email },
					"", ""))
				return true;

			return false;
		}
	}
}
