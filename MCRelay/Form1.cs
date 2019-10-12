using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MCRelay
{
    public partial class Form1 : Form
    {
        bool loggedIn = false;
        Process nProcess;
        public Form1()
        {
            InitializeComponent();

        }

        private void Btn_connect_Click(object sender, EventArgs e)
        {
            
            if(!loggedIn)
            {
                nProcess = StartProcess();
                btn_connect.Text = "Disconnect";
            }
            else
            {
                if (nProcess != null)
                {
                    nProcess.CloseMainWindow();
                    AddText("Disconnected!");
                    btn_connect.Text = "Connect";
                }
                else
                { 
                    loggedIn = false;
                    btn_connect.Text = "Connect";
                }
                
            }
            
        }


        Process StartProcess()
        {
            Process nodeProcess = new Process();
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.FileName = "node.exe";
            startInfo.Arguments = "./index.js" + $" {txt_ip.Text} {txt_port.Text} {txt_username.Text} ";  // 164.132.122.180 2048 PizdaBot
            startInfo.UseShellExecute = false;
            startInfo.WindowStyle = ProcessWindowStyle.Hidden;
            startInfo.CreateNoWindow = true;
            startInfo.RedirectStandardOutput = true;
            startInfo.RedirectStandardInput = true;
            nodeProcess.StartInfo = startInfo;

            nodeProcess.EnableRaisingEvents = true;
            nodeProcess.Exited += nodeProcess_Exited;
            nodeProcess.OutputDataReceived += nodeProcess_OutputDataReceived;
            

            nodeProcess.Start();
            nodeProcess.BeginOutputReadLine();
            loggedIn = true;
            return nodeProcess;
        }

        void nodeProcess_Exited(object sender, EventArgs e)
        {
            // Do something when the process exits, if you need to.
            // You'll want to check InvokeRequired before you modify any of your form's controls.

            loggedIn = false;
        }

        void nodeProcess_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (txt_log.InvokeRequired)
            {
                txt_log.Invoke(new Action<string>(AddText), new object[] { e.Data });
                return;
            }
            txt_log.Text += "\n " + e.Data;
        }

        public void AddText(string text)
        {
            string str = "";
            str += ("["+DateTime.Now.ToString("T")+"] " + text + "\r\n");
            if(txt_log.InvokeRequired)
            {
                txt_log.Invoke(new Action<string>(AddText), new object[] { str });
                return;
            }

            txt_log.AppendText(str);
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if(nProcess != null)
            {
                nProcess.CloseMainWindow();
            }
        }

        private void Btn_send_Click(object sender, EventArgs e)
        {
            sendCmd();
        }

        private void Txt_cmd_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.KeyCode == Keys.Enter)
            {
                sendCmd();
            }
        }

        void sendCmd()
        {
            var stream = nProcess.StandardInput;
            stream.WriteLine(txt_cmd.Text);
            txt_cmd.Text = "";
        }
    }
}
