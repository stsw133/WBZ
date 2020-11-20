using System;
using System.IO.Ports;
using System.Threading;
using WBZ.Controls;

namespace WBZ.Globals
{
    class GSM
    {
        /// <summary>
        /// Send
        /// </summary>
        internal static bool SendSMS(string[] numbers)
        {
            bool result = true;

            foreach (var number in numbers)
            {
                try
                {
                    using (SerialPort sp = new SerialPort(Properties.Settings.Default.config_GSM_com))
                    {
                        sp.Open();
                        ///Using AT command to send sms
                        sp.WriteLine("AT" + Environment.NewLine);
                        Thread.Sleep(100);
                        sp.WriteLine("AT+CMGF=1" + Environment.NewLine);
                        Thread.Sleep(100);
                        sp.WriteLine("AT+CSCS=\"GSM\"" + Environment.NewLine);
                        Thread.Sleep(100);
                        sp.WriteLine("AT+CMGS=\"" + "+48" + number + "\"" + Environment.NewLine);
                        Thread.Sleep(100);
                        sp.WriteLine(Properties.Settings.Default.config_GSM_message + Environment.NewLine);
                        Thread.Sleep(100);
                        sp.Write(new byte[] { 26 }, 0, 1);
                        Thread.Sleep(100);
                        var response = sp.ReadExisting();
                        if (response.Contains("ERROR"))
                        {
                            result = false;
                            new MsgWin(MsgWin.Type.MsgOnly, MsgWin.MsgTitle.ERROR, $"Nie udało się wysłać SMS na numer {number}").ShowDialog();
                        }
                    }
                }
                catch (Exception ex)
                {
                    new MsgWin(MsgWin.Type.MsgOnly, MsgWin.MsgTitle.ERROR, ex.Message).ShowDialog();
                    result = false;
                }
            }
            return result;
        }
    }
}
