using AndroidSdk;
using MaterialSkin.Controls;
using ScrCpyGUI.Properties;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using static AndroidSdk.Adb;

namespace ScrCpyGUI
{
    public partial class Form1 : MaterialForm
    {
        AndroidSdkManager sdk = null;
        List<AdbDevices> devices1 = new List<AdbDevices>();
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            ActiveControl = label1;
            textBox1.Text = Settings.Default.AndroidSDK;
            textBox2.Text = Settings.Default.ScrCpy;
            if (textBox1.Text == "")
            {
                MessageBox.Show("Enter or select Android SDK Path");
                return;
            }
            if (textBox2.Text == "")
            {
                MessageBox.Show("Enter or select SCRCPY Path");
                return;
            }
            if (textBox1.Text.Length > 0)
            {
                sdk = new AndroidSdkManager(new System.IO.DirectoryInfo(textBox1.Text));
                //sdk.Acquire();
                // Stop/start adb
                //sdk.Adb.KillServer();
                //sdk.Adb.StartServer();

                // Find all ADB attached devices
                List<AdbDevice> devices = sdk.Adb.GetDevices();
                foreach (AdbDevice adbDevice in devices)
                {
                    devices1.Add(new AdbDevices()
                    {
                        Device = adbDevice.Device,
                        Serial = adbDevice.Serial,
                        Product = adbDevice.Product,
                        Model = adbDevice.Model,
                        Emulator = adbDevice.IsEmulator ? "Yes" : "No",
                        ScrCpy = "View"
                    });
                }
                dataGridView1.DataSource = devices1;
                dataGridView1.CellClick += CellClick;
                dataGridView1.Columns.Remove("ScrCpy");
                dataGridView1.Columns.Add(new DataGridViewButtonColumn()
                {
                    Text = "ScrCpy",
                    HeaderText = "ScrCpy",
                    DataPropertyName = "ScrCpy",
                });
                dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                /*dataGridView1.Columns.Add(new DataGridViewButtonColumn()
                {
                    Text = "Capture",
                    HeaderText = "Screenshot",
                    DataPropertyName = "ScreenCapture"
                });*/
            }
        }

        private void RefreshClick(object sender, EventArgs e)
        {
            if (textBox1.Text == "")
            {
                MessageBox.Show("Enter or select Android SDK Path");
                return;
            }
            if (textBox2.Text == "")
            {
                MessageBox.Show("Enter or select SCRCPY Path");
                return;
            }
            if (sdk == null && textBox1.Text.Length > 0)
                sdk = new AndroidSdkManager(new DirectoryInfo(textBox1.Text));
            List<AdbDevice> devices = sdk.Adb.GetDevices();
            devices1.Clear();
            foreach (AdbDevice adbDevice in devices)
            {
                devices1.Add(new AdbDevices()
                {
                    Device = adbDevice.Device,
                    Serial = adbDevice.Serial,
                    Product = adbDevice.Product,
                    Model = adbDevice.Model,
                    Emulator = adbDevice.IsEmulator ? "Yes" : "No",
                    ScrCpy = "View"
                });
            }
            try
            {
                /*dataGridView1.DataSource = devices1;

                dataGridView1.Columns.RemoveAt(5);
                dataGridView1.Columns.Add(new DataGridViewButtonColumn()
                {
                    Text = "ScrCpy",
                    HeaderText = "ScrCpy",
                    DataPropertyName = "ScrCpy"
                });*/
                CurrencyManager cm = (CurrencyManager)this.dataGridView1.BindingContext[devices1];
                if (cm != null)
                {
                    cm.Refresh();
                }
                dataGridView1.CellClick -= CellClick;
                dataGridView1.CellClick += CellClick;
            }
            catch
            {

            }
        }

        private void CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 5)
            {
                Process process = new Process();
                process.StartInfo.FileName = textBox2.Text + @"\scrcpy.exe";
                process.StartInfo.Arguments = " -s " + devices1[e.RowIndex].Serial;
                process.StartInfo.CreateNoWindow = true;
                process.Start();
            }
            if (e.ColumnIndex == 6)
            {
                // Screen capture
                sdk.Adb.ScreenCapture(new FileInfo("screen.png"), devices1[e.RowIndex].Serial);
            }
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            /*if (timer != null)
                timer.Stop();*/
            Settings.Default.AndroidSDK = textBox1.Text;
            Settings.Default.ScrCpy = textBox2.Text;
            Settings.Default.Save();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
            if (textBox1.Text.Length > 0)
                folderBrowserDialog.SelectedPath = textBox1.Text;
            var fsd = folderBrowserDialog.ShowDialog();
            textBox1.Text = folderBrowserDialog.SelectedPath;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
            if (textBox2.Text.Length > 0)
                folderBrowserDialog.SelectedPath = textBox2.Text;
            var fsd = folderBrowserDialog.ShowDialog();
            textBox2.Text = folderBrowserDialog.SelectedPath;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (sdk != null)
            {
                sdk.Adb.KillServer();
                sdk.Adb.StartServer();
                RefreshClick(sender, e);
            }
        }
    }

    public class AdbDevices
    {
        public string Serial { get; set; }
        public string Emulator { get; set; }
        public string Product { get; set; }
        public string Model { get; set; }
        public string Device { get; set; }
        public string ScrCpy { get; set; }
    }
}
