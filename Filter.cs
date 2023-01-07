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
            txt_TimeIn.Text = Form1.f_time_in;
            txt_TImeOut.Text = Form1.f_time_out;
            txt_Status.Text = Form1.f_status;
            txt_FilterName.Text = Form1.f_name;
        }
    }
}
