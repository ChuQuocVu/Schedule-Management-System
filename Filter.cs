using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Schedule_Management
{
    public partial class Filter : Form
    {
        public Filter()
        {
            InitializeComponent();
        }

        private void Filter_Load(object sender, EventArgs e)
        {
            txt_TimeIn.Text = "-";
            txt_TImeOut.Text = "-";
            txt_Status.Text = "-";
            txt_FilterName.Text = "";
        }

        private void btn_Filter_Click(object sender, EventArgs e)
        {
            loadStudentActitvites();
        }

        private void loadStudentActitvites()
        {
            bool isFirst = true;
            bool isName = false;
            txt_TimeIn.Text = "-";
            txt_TImeOut.Text = "-";
            txt_Status.Text = "-";
            string date = datePicker1.Text;

            if (txt_FilterName.Text == "")
            {
                AutoClosingMessageBox_Filter.Show("Name is empty!", "", 1500);
                return;
            }

            Form1 mainWindow = new Form1();

            for (int r = 0; r < mainWindow.dataGridViewData.Rows.Count; r++)
            {
                if (mainWindow.dataGridViewData.Rows[r].Cells[1].Value.ToString().Trim().Substring(0, 9) == date)
                {
                    if (mainWindow.dataGridViewData.Rows[r].Cells[2].Value.ToString().Trim() == txt_FilterName.Text)
                    {
                        isName = true;
                        if (isFirst)
                        {
                            txt_TimeIn.Text = mainWindow.dataGridViewData.Rows[r].Cells[1].Value.ToString().Trim().Substring(0, 9);
                            txt_Status.Text = mainWindow.dataGridViewData.Rows[r].Cells[4].Value.ToString().Trim();
                            isFirst = false;
                        }
                        else
                        {
                            txt_TImeOut.Text = mainWindow.dataGridViewData.Rows[r].Cells[1].Value.ToString().Trim().Substring(0, 9);
                        }
                    }
                }
            }
            if (!isName)
            {
                AutoClosingMessageBox_Filter.Show($"Can't find '{txt_FilterName.Text}' in system's history!", "", 1500);
            }
        }

        private void datePicker1_ValueChanged(object sender, EventArgs e)
        {
            loadStudentActitvites();
        }
    }

    public class AutoClosingMessageBox_Filter
    {
        System.Threading.Timer _timeoutTimer;
        string _caption;
        AutoClosingMessageBox_Filter(string text, string caption, int timeout)
        {
            _caption = caption;
            _timeoutTimer = new System.Threading.Timer(OnTimerElapsed,
                null, timeout, System.Threading.Timeout.Infinite);
            using (_timeoutTimer)
                MessageBox.Show(text, caption);
        }
        public static void Show(string text, string caption, int timeout)
        {
            new AutoClosingMessageBox_Filter(text, caption, timeout);
        }
        void OnTimerElapsed(object state)
        {
            IntPtr mbWnd = FindWindow("#32770", _caption); // lpClassName is #32770 for MessageBox
            if (mbWnd != IntPtr.Zero)
                SendMessage(mbWnd, WM_CLOSE, IntPtr.Zero, IntPtr.Zero);
            _timeoutTimer.Dispose();
        }
        const int WM_CLOSE = 0x0010;
        [System.Runtime.InteropServices.DllImport("user32.dll", SetLastError = true)]
        static extern IntPtr FindWindow(string lpClassName, string lpWindowName);
        [System.Runtime.InteropServices.DllImport("user32.dll", CharSet = System.Runtime.InteropServices.CharSet.Auto)]
        static extern IntPtr SendMessage(IntPtr hWnd, UInt32 Msg, IntPtr wParam, IntPtr lParam);
    }
}
