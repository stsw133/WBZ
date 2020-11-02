using System;
using System.Net.Mail;
using WBZ.Controls;
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
				new MsgWin(MsgWin.Type.MsgOnly, MsgWin.MsgTitle.ERROR, ex.Message).ShowDialog();
				return false;
			}

			return true;
		}

		/// <summary>
		/// Test
		/// </summary>
		internal static bool TestMail()
		{
			return SendMail(Props.Default.config_Email_Email,
				new string[] { Props.Default.config_Email_Email },
				"", "");
		}
	}
}
