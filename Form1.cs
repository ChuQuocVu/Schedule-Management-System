using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.IO;
using System.IO.Ports;
using Microsoft.Office.Interop.Excel;
using System.Data.SqlClient;


namespace Schedule_Management
{
    public partial class Form1 : Form
    {

        #region Declare the variables to be used in this program

        // Tạo đối tượng kết nối
        SqlConnection sqlcon = null;

        // Khởi tạo list Student (bao gồm các class Student)
        public List<Student> listStudent;

        //Initialize a list of classID
        public List<Student> classIDList;

        // Số thứ tự sinh viên
        int number = 1;

        // Khởi tạo dataTable (datagridview)
        private System.Data.DataTable dataTable = new System.Data.DataTable();

        // Số lượng port COM đang khả dụng 
        int lenCom = 0;

        // Ngày trong tuần
        public string[] Day_array = { "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday", "Sunday" };
        #endregion

        public Form1()
        {
            InitializeComponent();
            //loadDataGridView();
            string[] baudRate = { "1200", "2400", "4800", "9600", "19200", "38400", "57600", "115200" };
            comboBoxBaudRate.Items.AddRange(baudRate);
            string[] databits = { "6", "7", "8" };
            comboBoxDataBit.Items.AddRange(databits);
            string[] paritybits = { "None", "Odd", "Even" };
            comboBoxParityBit.Items.AddRange(paritybits);
            string[] stopbits = { "1", "1.5", "2" };
            comboBoxStopBit.Items.AddRange(stopbits);
        }

        #region Button Click Events

        // Press Enter Key to Connect SQL Server
        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                buttondatabase.PerformClick();
            }
        }

        // Connect to SQL Server 
        /*
         *  After SQL Server is connected, classIDList will contain class infomation (className & classID) from database
         */
        private void buttondatabase_Click(object sender, EventArgs e)
        {
            // Tạo chuỗi kết nối
            string strcon = String.Format(@"Data Source={0};Initial Catalog={1};Integrated Security=True", textBoxServer.Text, textBoxDatabase.Text);

            if (textBoxTable.Text != "")
            {
                try
                {

                    if (sqlcon == null) sqlcon = new SqlConnection(strcon);

                    if (sqlcon.State == ConnectionState.Closed)
                    {
                        sqlcon.Open();
                        MessageBox.Show("       Connect Sucessfull !");
                        buttondatabase.ForeColor = Color.Red;
                        buttondatabase.Text = "Disconnect to Database";
                    }
                    else if (sqlcon != null && sqlcon.State == ConnectionState.Open)
                    {
                        sqlcon.Close();
                        MessageBox.Show("       Disconnected !");
                        buttondatabase.ForeColor = Color.LimeGreen;
                        buttondatabase.Text = "Connect to Database";
                        sqlcon = null;
                    }

                    // Code tạo đối tượng thực thi truy vấn
                    SqlCommand sqlCmd = new SqlCommand();
                    sqlCmd.CommandType = CommandType.Text;
                    sqlCmd.CommandText = String.Format(@"SELECT * FROM Class_Infomation");

                    // Code để kết nối truy vấn
                    sqlCmd.Connection = sqlcon;

                    SqlDataReader reader = sqlCmd.ExecuteReader(); // Đổ dữ liệu từ database vào biến 'reader'

                    classIDList = new List<Student>(); // Tạo list rỗng

                    // Truyền dữ liệu từ SQL vào List Student
                    while (reader.Read())
                    {
                        classIDList.Add(new Student(reader.GetString(0), reader.GetString(1)));
                    }
                    reader.Close();

                }
                catch (Exception ex)
                {
                    if (ex.Message == "ExecuteReader: Connection property has not been initialized.")
                    {
                        sqlcon = null;
                    }
                    else if (ex.Message == "Invalid object name '" + textBoxTable.Text + "'.")
                    {
                        MessageBox.Show("Table does not exist. No information!");
                    }
                    else
                    {
                        MessageBox.Show(ex.Message);
                        sqlcon = null;
                    }
                }
            }
            else MessageBox.Show("Table is not selected!");
        }

        // Nút bấm xóa dữ liệu trong datagridview 
        private void buttonDelete_Click(object sender, EventArgs e)
        {
            foreach (DataGridViewRow row in dataGridViewData.SelectedRows)
            {
                dataGridViewData.Rows.Remove(row);
                number--;
            }
        }

        // Nút bấm kết nối UART
        private void buttonConnect_Click(object sender, EventArgs e)
        {
            try
            {
                if (labelStatus.Text == "Disconnected")
                {
                    Com.PortName = comboBoxSecCom.Text;
                    Com.Open();
                    Com.DiscardInBuffer(); // Xóa dữ liệu trong buffer
                    buttonConnect.ForeColor = Color.Red;
                    labelStatus.Text = "Connected";
                    labelStatus.ForeColor = Color.LimeGreen;
                    buttonConnect.Text = "Disconnect";
                }
                else
                {
                    Com.Close();
                    buttonConnect.ForeColor = Color.Lime;
                    labelStatus.Text = "Disconnected";
                    labelStatus.ForeColor = Color.Red;
                    buttonConnect.Text = "Connect";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        // Nút bấm lưu file excel
        private void buttonSave_Click(object sender, EventArgs e)
        {
            SaveFileDialog dialog = new SaveFileDialog();
            dialog.Filter = "*xlsx | Excel 2016"; // Chỉ lưu file với đuôi *xlsx

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                exportExcel(dataGridViewData, dialog.FileName);
            }
        }
        #endregion

        #region Support Functions

        private void getDataByClassName(string className)
        {
            string time_range = "";
            DateTime time_in;
            DateTime time_out;
            try
            {
                // Code tạo đối tượng thực thi truy vấn
                SqlCommand sqlCmd = new SqlCommand();
                sqlCmd.CommandType = CommandType.Text;
                sqlCmd.CommandText = String.Format(@"SELECT * FROM {0}", textBoxTable.Text);

                // Code để kết nối truy vấn
                sqlCmd.Connection = sqlcon;

                SqlDataReader reader = sqlCmd.ExecuteReader(); // Đổ dữ liệu từ database vào biến 'reader'

                listStudent = new List<Student>(); // Tạo list rỗng

                // Truyền dữ liệu từ SQL vào List Student
                while (reader.Read())
                {
                    listStudent.Add(new Student(reader.GetString(0), reader.GetString(1)));
                }
                reader.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void getDataByDate(string date)
        {

        }

        #endregion

        #region Setup DataGridView

        // Thêm cột header cho DataGridView
        private void loadDataGridView()
        {
            dataTable.Columns.Add("No.", typeof(int));
            dataTable.Columns.Add("Name", typeof(string));
            dataTable.Columns.Add("ID", typeof(string));
            dataTable.Columns.Add("Date and Time", typeof(DateTime));
            dataGridViewData.DataSource = dataTable;

            for (int i = 0; i < dataGridViewData.Columns.Count; i++)
            {
                dataGridViewData.Columns[i].Frozen = false;
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            string[] ports = SerialPort.GetPortNames(); // Lấy tất cả các COM đang khả dụng trên PC
            if (lenCom != ports.Length)
            {
                lenCom = ports.Length;
                comboBoxSecCom.Items.Clear();
                for (int i = 0; i < lenCom; i++)
                {
                    comboBoxSecCom.Items.Add(ports[i]);
                }
                comboBoxSecCom.Text = ports[0];
            }
        }
        #endregion

        #region Setup UART Box and SQL Server Box

        // Code chọn Baud Rate từ comboBox
        private void comboBoxBaudRate_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Com.IsOpen) Com.Close();
            Com.BaudRate = Convert.ToInt32(comboBoxBaudRate.Text);
        }

        // Code chọn số bit data từ comboBox
        private void comboBoxDataBit_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Com.IsOpen) Com.Close();
            Com.DataBits = Convert.ToInt32(comboBoxDataBit.Text);
        }

        // Code chọn Parity bit từ comboBox
        private void comboBoxParityBit_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Com.IsOpen) Com.Close();
            switch (comboBoxParityBit.SelectedItem.ToString())
            {
                case "Odd":
                    Com.Parity = Parity.Odd;
                    break;
                case "Even":
                    Com.Parity = Parity.Even;
                    break;
                case "None":
                    Com.Parity = Parity.None;
                    break;
            }
        }

        // Code chọn Stop bit từ comboBox
        private void comboBoxStopBit_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Com.IsOpen) Com.Close();
            switch (comboBoxStopBit.SelectedItem.ToString())
            {
                case "1":
                    Com.StopBits = StopBits.One;
                    break;
                case "1.5":
                    Com.StopBits = StopBits.OnePointFive;
                    break;
                case "2":
                    Com.StopBits = StopBits.Two;
                    break;
            }
        }

        /*
         Cài đặt giá trị mặc định của các comboBox và textBox khi mở ứng dụng.
          
         baudRate = { "1200", "2400", "4800", "9600", "19200", "38400", "57600", "115200" };
         databits = { "6", "7", "8" };
         paritybits = { "None", "Odd", "Even" };
         stopbits = { "1", "1.5", "2" };
         */

        private void Form1_Load(object sender, EventArgs e)
        {
            comboBoxBaudRate.SelectedIndex = 7;
            comboBoxDataBit.SelectedIndex = 2;
            comboBoxParityBit.SelectedIndex = 0;
            comboBoxStopBit.SelectedIndex = 0;
            textBoxServer.Text = @"ADMIN-PC"; // Tên SQL server
            textBoxDatabase.Text = "LnD_DataBase"; // Tên Database sử dụng
            textBoxTable.Text = ""; // Tên Table sử dụng
        }
        #endregion

        #region Push data from MCU to PC via UART
        private void OnCom(object sender, SerialDataReceivedEventArgs e)
        {
            Control.CheckForIllegalCrossThreadCalls = false;

            // Read 1 byte will interrupt COM -> Send Data from MCU to PC !
            string iD;
            string name = "No Information";
            DateTime time = DateTime.Now;
            bool check = false;

            // Đọc ID của thẻ từ
            iD = Com.ReadExisting();

            try
            {
                // So sánh ID vừa đọc từ RFID với ID trong database. Nếu ID nào trùng khớp thì gán tên tương ứng trong database cho biến name
                foreach (var item in listStudent)
                {
                    if (String.Compare(iD, item.ID.Trim()) == 0)
                    {
                        name = item.Name;
                        check = true;
                    }
                }

                // Nếu không có ID nào trùng khớp thì biến name = 'No Infomation'
                if (check == false)
                {
                    name = "No Information";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            // Gửi data đến MCU
            Com.WriteLine(name);


            // Đưa dữ liệu lên dataGridView            
            dataGridViewData.Invoke(new System.Action(() =>
            {
                dataGridViewData.Rows.Add(number, name, iD, time.ToString());
                dataGridViewData.FirstDisplayedScrollingRowIndex = dataGridViewData.RowCount - 1;
            }));

            number++;

        }
        #endregion

        #region Code export file Excel
        private void exportExcel(DataGridView gridData, string nameFile)
        {
            Microsoft.Office.Interop.Excel.Application excel;
            Microsoft.Office.Interop.Excel.Workbook workbook;

            try
            {
                excel = new Microsoft.Office.Interop.Excel.Application();
                excel.Visible = false;
                excel.DisplayAlerts = false;

                workbook = excel.Application.Workbooks.Add(Type.Missing);
                excel.Columns.ColumnWidth = 30;

                // Lưu header
                for (int i = 0; i < gridData.Columns.Count; i++)
                {
                    excel.Cells[1, i + 1] = gridData.Columns[i].HeaderText;
                }

                // Lưu dữ liệu từng dòng từ DatagridView vào excel
                for (int i = 0; i < gridData.Rows.Count; i++)
                {
                    for (int j = 0; j < gridData.Columns.Count; j++)
                    {
                        if (dataGridViewData.Rows[i].Cells[j].Value != null)
                        {
                            excel.Cells[i + 2, j + 1] = gridData.Rows[i].Cells[j].Value.ToString();
                        }
                    }
                }

                // Căn lề cho excel và DatagridView
                for (int i = 0; i < gridData.Columns.Count; i++)
                {
                    dataGridViewData.Columns[i].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                    excel.Cells.HorizontalAlignment = Microsoft.Office.Interop.Excel.Constants.xlCenter;
                }


                workbook.SaveAs(nameFile);
                workbook.Close();
                excel.Quit();
                MessageBox.Show("Successful!");
            }
            catch (Exception Ex)
            {
                MessageBox.Show(Ex.Message);
            }
            finally
            {
                workbook = null;
            }
        }

        #endregion


    }

    public class Student
    {
        public string Name { get; set; }
        public string ID { get; set; }

        public Student(string name, string id)
        {
            Name = name;
            ID = id;
        }
    }

    public class Schedule
    {
        public DateTime s_Time { get; set; }
        public string s_Class { set; get; }

        public Schedule(DateTime m_time, string m_class)
        {
            s_Time = m_time;
            s_Class = m_class;
        }
    }
}