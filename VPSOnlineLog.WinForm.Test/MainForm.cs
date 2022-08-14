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
        public MainForm()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var log_service = new LogService();
            log_service.StartLog();
        }

        private void btnLoadData_Click(object sender, EventArgs e)
        {
            var LogFilePath = ConfigurationManager.AppSettings["LogFilePath"];

            if (string.IsNullOrEmpty(LogFilePath)) return;

            if (!File.Exists(LogFilePath))
                File.Create(LogFilePath);

            List<LogModel> AllLogDatas;
            using (StreamReader r = new StreamReader(LogFilePath))
            {
                string json = r.ReadToEnd();
                AllLogDatas = JsonConvert.DeserializeObject<List<LogModel>>(json) ?? new List<LogModel>();

                dataGridViewLog.DataSource = AllLogDatas;
            }
        }
    }
}
