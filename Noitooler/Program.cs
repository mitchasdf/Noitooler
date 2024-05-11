using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Noitooler
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        public static Form1 theForm;
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            theForm = new Form1();
            Application.Run(theForm);
        }

        public static void ButtonClick(object sender, EventArgs e)
        {
            string newText = "";
            // seed stored at noita.exe+{E02F84 - E02F87} L-E
            int result = MemoryReader.ReadMemory("noita", 0xE02F84);
            if (result < 0)
            {
                if (result == -1)
                {
                    newText = "Could not open noita.exe";
                }
                if (result == -2)
                {
                    newText = "Could not read memory of noita.exe";
                }
            }
            else if (result == 0)
            {
                newText = "Seed was 0... Likely wrong, are you still on main menu?";
            }
            else
            {
                newText = "Seed found: " + result.ToString() + ". Opening noitool...";
                System.Diagnostics.Process.Start("https://www.noitool.com/info?seed=" + result.ToString());
            }
            theForm.textBox1.Text = newText;
        }
    }
    class WinAPI
    {
        [DllImport("kernel32.dll")]
        public static extern IntPtr OpenProcess(int dwDesiredAccess, bool bInheritHandle, int dwProcessId);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool ReadProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] lpBuffer, int dwSize, out int lpNumberOfBytesRead);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool CloseHandle(IntPtr hObject);

        public const int PROCESS_VM_READ = 0x0010;
        public const int PROCESS_QUERY_INFORMATION = 0x0400;
    }
    public class MemoryReader
    {
        public static int ReadMemory(string processName, int offset)
        {
            IntPtr baseAddress;
            Process process;
            IntPtr processHandle;
            byte[] buffer;
            try
            {
                process = Process.GetProcessesByName(processName)[0];
                processHandle = WinAPI.OpenProcess(WinAPI.PROCESS_QUERY_INFORMATION | WinAPI.PROCESS_VM_READ, false, process.Id);

                if (processHandle == IntPtr.Zero)
                {
                    return -1;
                }

                baseAddress = process.MainModule.BaseAddress;
                buffer = new byte[4];
            }
            catch (Exception e)
            {
                return -1;
            }

            try
            {

                if (WinAPI.ReadProcessMemory(processHandle, IntPtr.Add(baseAddress, offset), buffer, 4, out int bytesRead))
                {
                    //Console.WriteLine("Read {0} bytes from process {1}", bytesRead, processName);
                    // Here, you can handle the data read from the process's memory
                    WinAPI.CloseHandle(processHandle);
                    return buffer[0] + (buffer[1] << 8) + (buffer[2] << 16) + (buffer[3] << 24);
                }
                else
                {
                    WinAPI.CloseHandle(processHandle);
                    return -2;
                }
            }
            catch (Exception e)
            {
                return -2;
            }
        }
    }

}
