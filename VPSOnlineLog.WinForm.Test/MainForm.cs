using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using VPSOnlineLog.Libs.BL;
using VPSOnlineLog.Libs.Models;

namespace VPSOnlineLog.WinForm.Test
{
    public partial class MainForm : Form
    {
        const string CompanyFolderName = "KPrasetyo";
        string ProductFolderName;
        string LogFileName;
        string LogFilePath;

        public MainForm()
        {
            InitializeComponent();

            ProductFolderName = ConfigurationManager.AppSettings["ProductFolderName"];
            LogFileName = ConfigurationManager.AppSettings["LogFileName"];

            LogFilePath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), 
                CompanyFolderName, 
                ProductFolderName, 
                LogFileName);
            
            txtLogFilePath.Text = LogFilePath;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var log_service = new LogService();
            log_service.StartLog();
        }

        private void btnLoadData_Click(object sender, EventArgs e)
        {
            BindDataToGrid();
        }

        private void btnClearLog_Click(object sender, EventArgs e)
        {
            File.WriteAllText(LogFilePath, String.Empty);
            BindDataToGrid();
        }

        private void BindDataToGrid() {

            if (string.IsNullOrEmpty(LogFilePath)) return;

            List<LogModel> AllLogDatas;
            using (StreamReader r = new StreamReader(LogFilePath))
            {
                string json = r.ReadToEnd();
                AllLogDatas = JsonConvert.DeserializeObject<List<LogModel>>(json) ?? new List<LogModel>();

                dataGridViewLog.DataSource = AllLogDatas;
                dataGridViewLog.Columns[2].DefaultCellStyle.Format = "dd-MM-yyyy HH:mm:ss";

                dataGridViewLog.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                dataGridViewLog.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                dataGridViewLog.Columns[2].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;

            }
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            BindDataToGrid();
        }
    }
}
