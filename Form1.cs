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
using System.Data.OleDb;

namespace Schedule_Management
{
    public partial class Form1 : Form
    {

        #region Declare the variables to be used in this program

        // Tạo đối tượng kết nối
        SqlConnection sqlcon = null;

        // Khởi tạo list classIDList (bao gồm các class Student)
        public List<Class> classIDList;

        // Initialize a schedule list of each class
        public List<Schedule> scheduleList;

        // Initialize a dictionary contains class and status
        public Dictionary<string, string> classStatus;

        // Lesson by time in and time out
        public Dictionary<int, string> lessonByTimeIn;
        public Dictionary<int, string> lessonByTimeOut;

        // Số thứ tự
        int number = 1;

        // Flag check the first time start program
        bool IsFirstTime = true;

        // Khởi tạo dataTable (datagridview)
        private System.Data.DataTable dataTable = new System.Data.DataTable();

        // Initialize a table show status of each class. Ex: "DD18DV1 : In class" or "DD18DV1 : None"
        private System.Data.DataTable Class_Status_Table = new System.Data.DataTable();

        // Initialize a table show schedule of all class in week
        private System.Data.DataTable Schedule_Table = new System.Data.DataTable();
        SqlDataAdapter adapter = new SqlDataAdapter();

        // Số lượng port COM đang khả dụng 
        int lenCom = 0;

        // Ngày trong tuần
        public string[] Day_range = { "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday", "Sunday" };

        // Danh sách các lớp học
        public List<string> className_range;

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
            string[] Time_In_range = { "", "07:00 AM", "08:00 AM", "09:00 AM", "10:00 AM", "11:00 AM", "12:00 PM",
                                        "01:00 PM", "02:00 PM", "03:00 PM", "04:00 PM", "05:00 PM", "06:00 PM" };
            comboBoxTimeIn.Items.AddRange(Time_In_range);
            string[] Time_Out_range = { "", "08:00 AM", "09:00 AM", "10:00 AM", "11:00 AM", "12:00 PM",
                                        "01:00 PM", "02:00 PM", "03:00 PM", "04:00 PM", "05:00 PM", "06:00 PM", "07:00 PM" };
            comboBoxTimeOut.Items.AddRange(Time_Out_range);
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
                        AutoClosingMessageBox.Show("       Connect Sucessfull !", "", 1500);
                        buttondatabase.ForeColor = Color.Red;
                        buttondatabase.Text = "Disconnect to Database";
                    }
                    else if (sqlcon != null && sqlcon.State == ConnectionState.Open)
                    {
                        sqlcon.Close();
                        AutoClosingMessageBox.Show("       Disconnected !", "", 1500);
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

                    classIDList = new List<Class>(); // Tạo list rỗng
                    className_range = new List<string>();
                    className_range.Add("");
                    if (IsFirstTime)
                    {
                        classStatus = new Dictionary<string, string>(); // Tạo dict rỗng
                    }

                    // Truyền dữ liệu từ SQL vào List Class
                    while (reader.Read())
                    {
                        classIDList.Add(new Class(reader.GetString(0).Trim(), reader.GetString(1).Trim()));
                    }
                    reader.Close();

                    // CLear & Refresh dataGridViewStatus when click buttondatabase
                    dataGridViewStatus.Rows.Clear();
                    dataGridViewStatus.Refresh();
                    foreach (var item in classIDList)
                    {
                        className_range.Add(item.Name);
                        // If this is the first time start program
                        if (IsFirstTime)
                        {
                            // Truyền PairKeyValue{className, "Out"} vào dictionary classStatus
                            classStatus.Add(item.Name, "Out");

                            // Đẩy dữ liệu trong classStatus lên dataGridViewStatus
                            dataGridViewStatus.Invoke(new System.Action(() =>
                            {
                                dataGridViewStatus.Rows.Add(item.Name, "Out");
                                dataGridViewStatus.FirstDisplayedScrollingRowIndex = dataGridViewStatus.RowCount - 1;
                            }));
                        }
                        else
                        {
                            if (!classStatus.ContainsKey(item.Name))
                            {
                                classStatus.Add(item.Name, "Out");
                            }
                            // Đẩy dữ liệu trong classStatus lên dataGridViewStatus
                            dataGridViewStatus.Invoke(new System.Action(() =>
                            {
                                dataGridViewStatus.Rows.Add(item.Name, classStatus[item.Name]);
                                dataGridViewStatus.FirstDisplayedScrollingRowIndex = dataGridViewStatus.RowCount - 1;
                            }));
                        }
                    }

                    // First time flag: Set this flag to false -> This is not a first time start program
                    IsFirstTime = false;

                    // Load Schedule to dataGridViewSchedule
                    loadScheduleGridView();

                    // Refresh comboBoxClassName before add value
                    comboBoxClassName.Items.Clear();
                    comboBoxClassName.Refresh();

                    // Add value to comboBoxClassName
                    comboBoxClassName.Items.AddRange(className_range.ToArray());
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

        private void buttonSaveSchedule_Click(object sender, EventArgs e)
        {
            if (comboBoxTimeIn.Text != "" && comboBoxTimeOut.Text != "")
            {
                string date_string = datePicker.Text;
                string className = comboBoxClassName.Text;
                string timeIn = comboBoxTimeIn.Text;
                string timeOut = comboBoxTimeOut.Text;
                int lessonIn = 0;
                int lessonOut = 0;

                string dayOfWeek = DateTime.Parse(date_string).ToString("dddd");

                // Get lesson match each time_in and time_out in day
                getLessonByTimeIn_Out();

                foreach (KeyValuePair<int, string> item in lessonByTimeIn)
                {
                    if (item.Value == timeIn)
                    {
                        lessonIn = item.Key;
                    }
                }

                foreach (KeyValuePair<int, string> item in lessonByTimeOut)
                {
                    if (item.Value == timeOut)
                    {
                        lessonOut = item.Key;
                    }
                }

                try
                {
                    SqlCommand sqlCmd = new SqlCommand($"UPDATE Schedule SET {dayOfWeek} = '{className}' " +
                                                        $"WHERE Lesson BETWEEN {lessonIn} AND {lessonOut}", sqlcon);
                    sqlCmd.ExecuteNonQuery();
                    AutoClosingMessageBox.Show("Done!", "", 1500);
                    loadScheduleGridView();
                }
                catch (Exception ex)
                {
                    AutoClosingMessageBox.Show(ex.Message, "", 2000);
                }
            }
            else
            {
                AutoClosingMessageBox.Show("Please fill all infomation !", "Missing Info", 2000);
            }
        }


        #endregion

        #region Support Functions

        private void getDataByClassName(string className)
        {
            int time_range = 0;
            string time_in = "";
            string time_out = "";
            bool IsDataExist = false;
            scheduleList = new List<Schedule>(); // Tạo list rỗng

            try
            {
                // Code tạo đối tượng thực thi truy vấn
                SqlCommand sqlCmd = new SqlCommand();
                sqlCmd.CommandType = CommandType.Text;

                for (int i = 0; i < Day_range.Length; i++)
                {
                    sqlCmd.CommandText = String.Format(@"SELECT Time, {0} FROM Schedule WHERE {1} = '{2}'", Day_range[i], Day_range[i], className);

                    // Code để kết nối truy vấn
                    sqlCmd.Connection = sqlcon;

                    SqlDataReader reader = sqlCmd.ExecuteReader(); // Đổ dữ liệu từ database vào biến 'reader'

                    // Truyền dữ liệu từ SQL vào scheduleList
                    while (reader.Read())
                    {
                        time_range++;
                        if (reader.GetString(0) != null)
                        {
                            if (time_range == 1)
                            {
                                time_in = reader.GetString(0).Substring(0, 8);
                            }
                            else if (time_range == 3)
                            {
                                time_out = reader.GetString(0).Substring(12, 8);
                                IsDataExist = true;
                                time_range = 0;
                            }
                        }

                        if (IsDataExist)
                        {
                            scheduleList.Add(new Schedule(Day_range[i], time_in, time_out));
                            IsDataExist = false;
                        }
                    }
                    reader.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void updateClassStatus(string className, string status)
        {
            foreach (var item in classIDList)
            {
                if (item.Name.Trim() == className)
                {
                    // Update dataGridView
                    dataGridViewStatus.Rows[classIDList.IndexOf(item)].Cells[1].Value = status;
                }
            }
        }

        private void getLessonByTimeIn_Out()
        {
            lessonByTimeIn = new Dictionary<int, string>();
            lessonByTimeOut = new Dictionary<int, string>();
            try
            {
                // Code tạo đối tượng thực thi truy vấn
                SqlCommand sqlCmd = new SqlCommand();
                sqlCmd.CommandType = CommandType.Text;

                sqlCmd.CommandText = String.Format(@"SELECT Lesson, Time FROM Schedule");

                // Code để kết nối truy vấn
                sqlCmd.Connection = sqlcon;

                SqlDataReader reader = sqlCmd.ExecuteReader();

                while (reader.Read())
                {
                    lessonByTimeIn.Add(reader.GetInt32(0), reader.GetString(1).Substring(0, 8));
                    lessonByTimeOut.Add(reader.GetInt32(0), reader.GetString(1).Substring(12, 8));
                }
                reader.Close();
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void createScheduleTableInDataBase(string tableName)
        {
            try
            {
                SqlCommand sqlCmd = new SqlCommand($"CREATE TABLE {tableName} (Lesson int, Time nvarchar(225), " +
                                                    $"Monday nvarchar(225), Tuesday nvarchar(225), " +
                                                    $"Wednesday nvarchar(225), Thursday nvarchar(225), " +
                                                    $"Friday nvarchar(225), Saturday nvarchar(225)," +
                                                    $"Sunday nvarchar(225))", sqlcon);

                sqlCmd.ExecuteNonQuery();
                AutoClosingMessageBox.Show($"Created {tableName}", "", 1000);
            }
            catch(Exception ex)
            {
                if (ex.Message == $"There is already an object named '{tableName}' in the database.")
                {
                    AutoClosingMessageBox.Show($"Rewrite {tableName}", "", 1000);
                    // If tableName is already exist -> Clear table to fill new data
                    clearDataInDataBase(tableName);
                }
            }
        }

        private System.Data.DataTable readDataFromExcel(string filePath)
        {
            /* This function read data from file excel
             * return: a DataTable contain all data rows in file excel
             */

            System.Data.DataTable data = null;
            string excelCon = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + filePath.Trim() + ";Extended Properties=Excel 8.0";
            // Create connection object
            OleDbConnection oledbCon = new OleDbConnection(excelCon);

            try
            {
                oledbCon.Open();
                OleDbCommand cmd = new OleDbCommand("SELECT * FROM [Sheet1$]", oledbCon);
                OleDbDataAdapter olebdAdapter = new OleDbDataAdapter();
                olebdAdapter.SelectCommand = cmd;

                DataSet ds = new DataSet();
                // FIll data from excel to Dataset ds
                olebdAdapter.Fill(ds);
                data = ds.Tables[0];

            }
            catch(Exception ex)
            {
                AutoClosingMessageBox.Show(ex.Message, "", 1500);
            }
            finally
            {
                oledbCon.Close();
            }

            return data;
        }

        private void insertDataIntoDataBase(string tableName, List<object> arg)
        {
            /* This function insert new line data to schedule table in SQL 
             * Parameters: 
             *      - tableName: Name of table use to contain schedule import from excel
             *      - arg: a list contain fields of schedule:
             *              + Lesson [index 0]: int
             *              + Time [index 1]: string
             *              + Monday -> Sunday [index 2 -> 8]: string
             */
            try
            {
                SqlCommand sqlCmd = new SqlCommand($"INSERT INTO {tableName} " +
                    $"(Lesson, Time, Monday, Tuesday, Wednesday, Thursday, Friday, Saturday, Sunday) " +
                    $"VALUE ({arg[0]}, {arg[1]}, {arg[2]}, {arg[3]}, {arg[4]}, {arg[5]}, " +
                    $"{arg[6]}, {arg[7]}, {arg[8]})", sqlcon);
                sqlCmd.ExecuteNonQuery();
            }
            catch(Exception ex)
            {
                AutoClosingMessageBox.Show(ex.Message, "", 1500);
            }
        }

        private void clearDataInDataBase(string tableName)
        {
            /* This function clear all data in Database table*/

            try
            {
                SqlCommand sqlCmd = new SqlCommand($"DELETE FROM {tableName}", sqlcon);
                sqlCmd.ExecuteNonQuery();
            }
            catch(Exception ex)
            {
                AutoClosingMessageBox.Show(ex.Message, "", 1500);
            }
        }

        private void importDataFromDataTableIntoDataBase(string tableName, System.Data.DataTable data)
        {
            /* This function transfer data from DataTable to SQL */

            if (data == null || data.Rows.Count == 0)
            {
                AutoClosingMessageBox.Show("No schedule to import!", "", 2000);
                return;
            }

            List<object> argument = new List<object>();

            try
            {
                foreach (DataRow dataRow in data.Rows)
                {
                    foreach (DataColumn dataColumn in data.Columns)
                    {
                        argument.Add(dataRow[dataColumn].ToString().Trim());
                    }
                    argument[0] = Convert.ToInt32(argument[0]);
                    insertDataIntoDataBase(tableName, argument);
                    argument.Clear();
                }
            }
            catch(Exception ex)
            {
                AutoClosingMessageBox.Show(ex.Message, "", 1500);
            }
        }

        #endregion

        #region Setup DataGridView

        // Thêm cột header cho DataGridView
        private void loadDataGridView()
        {
            dataTable.Columns.Add("No.", typeof(int));
            dataTable.Columns.Add("Date and Time", typeof(DateTime));
            dataTable.Columns.Add("Class", typeof(string));
            dataTable.Columns.Add("ID", typeof(string));
            dataTable.Columns.Add("Status", typeof(string));
            dataGridViewData.DataSource = dataTable;

            for (int i = 0; i < dataGridViewData.Columns.Count; i++)
            {
                dataGridViewData.Columns[i].Frozen = false;
            }
        }

        private void loadClassStatusTable()
        {
            Class_Status_Table.Columns.Add("Class", typeof(string));
            Class_Status_Table.Columns.Add("Status", typeof(string));
            dataGridViewStatus.DataSource = Class_Status_Table;

            for (int i = 0; i < dataGridViewStatus.Columns.Count; i++)
            {
                dataGridViewStatus.Columns[i].Frozen = false;
            }
        }

        private void loadScheduleGridView()
        {
            SqlCommand sqlCmd = new SqlCommand("SELECT * FROM Schedule", sqlcon);
            adapter.SelectCommand = sqlCmd;
            Schedule_Table.Clear();
            adapter.Fill(Schedule_Table);
            dataGridViewSchedule.DataSource = Schedule_Table;
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
            textBoxServer.Text = @"ADMIN-PC"; // SQL Server's name -> This field should not be changed !
            textBoxDatabase.Text = @"LnD_DataBase"; // Database's name -> This field should not be changed !
            textBoxTable.Text = @"Class_Infomation"; // Table's name -> Can be change depend on users
        }
        #endregion

        #region Push data from MCU to PC via UART
        private void OnCom(object sender, SerialDataReceivedEventArgs e)
        {
            Control.CheckForIllegalCrossThreadCalls = false;

            // Read 1 byte will interrupt COM -> Send Data from MCU to PC !
            string iD;
            string name = "No Information";
            string status = "Denied";
            DateTime time = DateTime.Now;
            TimeSpan now = DateTime.Now.TimeOfDay;
            string dateValue = time.ToString("dddd");
            bool check = false;
            bool IsClass = false;

            // Đọc ID của thẻ từ
            iD = Com.ReadExisting();

            try
            {
                // So sánh ID vừa đọc từ RFID với ID trong database. Nếu ID nào trùng khớp thì gán tên tương ứng trong database cho biến name
                foreach (var item in classIDList)
                {
                    if (String.Compare(iD, item.ID.Trim()) == 0)
                    {
                        name = item.Name.Trim();
                        check = true;
                    }
                }

                // Fill in scheduleList with current class infomation
                getDataByClassName(name);

                foreach (var item in scheduleList)
                {
                    // Convert timeIn & timeOut from String to TimeSpan
                    TimeSpan m_timeIn = DateTime.Parse(item.s_TimeIn).TimeOfDay;
                    TimeSpan m_timeOut = DateTime.Parse(item.s_TimeOut).TimeOfDay;

                    if (dateValue == item.s_Day)
                    {
                        IsClass = true;
                        if (now >= m_timeIn && now <= m_timeOut)
                        {
                            if (classStatus[name] == "Out")
                            {
                                classStatus[name] = "In class";
                                if ((now - m_timeIn).TotalMinutes > 15)
                                {
                                    status = "Checkin late";
                                }
                                else
                                {
                                    status = "Checkin";
                                }
                            }
                            else
                            {
                                classStatus[name] = "Out";
                                if (now < m_timeOut)
                                {
                                    status = "Checkout early";
                                }
                                else
                                {
                                    status = "Checkout";
                                }
                            }
                            updateClassStatus(name, classStatus[name]);
                        }
                        else
                        {
                            if (classStatus[name] == "Out")
                            {
                                AutoClosingMessageBox.Show($"It's not a {name} class time right now !", "ACCESS DENIED!", 2000);
                                status = "Denied";
                            }
                            else if (classStatus[name] == "In class")
                            {
                                if (now > m_timeOut)
                                {
                                    classStatus[name] = "Out";
                                    AutoClosingMessageBox.Show($"Class {name} check out LATE ! Please pay attention next time !", "", 2000);
                                    status = "Checkout late";
                                    updateClassStatus(name, classStatus[name]);
                                }
                            }
                        }
                    }
                }

                // Nếu không có ngày nào trong lịch của lớp đó trùng khớp với ngày hôm nay
                // -> hiện thông báo "Không có lớp trong hôm nay"
                if(!IsClass)
                {
                    AutoClosingMessageBox.Show($"{name} doesn't have any class for today !", "", 2000);
                }

                // Nếu không có ID nào trùng khớp thì biến name = 'No Infomation'
                if (check == false)
                {
                    name = "No Information";
                    status = "Denied";
                    AutoClosingMessageBox.Show("No infomation !", "ACCESS DENIED!", 2000);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            // Gửi data đến MCU
            if (status != "Denied")
            {
                Com.WriteLine($"Class: {name}  A");
            }
            else
            {
                Com.WriteLine($"Class: {name}  D");
            }


            // Đưa dữ liệu lên dataGridView            
            dataGridViewData.Invoke(new System.Action(() =>
            {
                dataGridViewData.Rows.Add(number, time.ToString(), name, iD, status);
                dataGridViewData.FirstDisplayedScrollingRowIndex = dataGridViewData.RowCount - 1;
            }));

            number++;
        }
        #endregion

        #region Connect MySQL - Excel
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

        private void Import_Schedule_From_Excel(string filePath)
        {
            /* Name of file Excel should be like this: 'Schedule_{tableName}': 
             * Ex: Schedule_Week1 -> tableName is Week1
             */

            System.Data.DataTable data = readDataFromExcel(filePath);

            // Get tableName from file path
            string[] token = Path.GetFileName(filePath).Split('.');
            string tableName = token[0];
            // Create table contain schedule if it not exist
            createScheduleTableInDataBase(tableName);
            importDataFromDataTableIntoDataBase(tableName, data);
        }

        #endregion

    }

    #region Initialize all classes use in program
    public class Class
    {
        public string Name { get; set; }
        public string ID { get; set; }

        public Class(string name, string id)
        {
            Name = name;
            ID = id;
        }
    }

    public class Schedule
    {
        public string s_TimeIn { get; set; }
        public string s_TimeOut { get; set; }
        public string s_Day { set; get; }

        public Schedule(string m_day, string m_time_in, string m_time_out)
        {
            s_TimeIn = m_time_in;
            s_Day = m_day;
            s_TimeOut = m_time_out;
        }
    }

    public class AutoClosingMessageBox
    {
        System.Threading.Timer _timeoutTimer;
        string _caption;
        AutoClosingMessageBox(string text, string caption, int timeout)
        {
            _caption = caption;
            _timeoutTimer = new System.Threading.Timer(OnTimerElapsed,
                null, timeout, System.Threading.Timeout.Infinite);
            using (_timeoutTimer)
                MessageBox.Show(text, caption);
        }
        public static void Show(string text, string caption, int timeout)
        {
            new AutoClosingMessageBox(text, caption, timeout);
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
    #endregion

}