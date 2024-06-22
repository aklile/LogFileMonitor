using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LogFileMonitor
{
    public partial class Form1 : Form
    {
        private string targetFilePath = ""; 
        private FileSystemWatcher fileWatcher;
        private Timer timer;
        private string previousFileContent = ""; 
        public Form1()
        {
            InitializeComponent();
            fileWatcher = new FileSystemWatcher();
            fileWatcher.NotifyFilter = NotifyFilters.LastWrite;
            fileWatcher.Filter = "*.txt"; // Only monitor .txt files
            fileWatcher.Changed += FileWatcher_Changed;

            timer = new Timer();
            timer.Interval = 15000; // 15 seconds
            timer.Tick += Timer_Tick;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Text Files (*.txt)|*.txt";
            openFileDialog.Title = "Select the Target Text File";

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                targetFilePath = openFileDialog.FileName;
                StartMonitoring();
            }

        }
        private void StartMonitoring()
        {
            if (!string.IsNullOrEmpty(targetFilePath) && File.Exists(targetFilePath))
            {
                fileWatcher.Path = Path.GetDirectoryName(targetFilePath);
            
                fileWatcher.EnableRaisingEvents = true;

              
                LogMessage($"Started monitoring file: {targetFilePath}\n");
                timer.Start();
            }
            else
            {
                MessageBox.Show("Please select a valid text file.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void FileWatcher_Changed(object sender, FileSystemEventArgs e)
        {
            if (e.ChangeType == WatcherChangeTypes.Changed)
            {
                // Ensure it's the target file
                if (string.Compare(e.FullPath, targetFilePath, StringComparison.OrdinalIgnoreCase) == 0)
                {
                   
                    string currentFileContent = File.ReadAllText(targetFilePath);

                    
                    if (currentFileContent != previousFileContent)
                    {
                        
                        LogMessage($"File {e.FullPath} has been modified. New content: {currentFileContent}\n");

                        
                        previousFileContent = currentFileContent;
                    }
                }
            }
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            if (!File.Exists(targetFilePath))
            {
                LogMessage($"File {targetFilePath} no longer exists. Monitoring stopped.\n");
                fileWatcher.EnableRaisingEvents = false;
                timer.Stop();
                return;
            }
            

            // For demonstration purposes, simply log the check
            LogMessage($"Checking file {targetFilePath} at {DateTime.Now.ToString()}.\n");
        }

        private void LogMessage(string message)
        {
           
            if (textBox1.InvokeRequired)
            {
                // If called from a different thread, invoke the method on the UI thread
                textBox1.Invoke(new Action<string>(LogMessage), message);
            }
            else
            {
                // Update the TextBox directly if on the UI thread
                textBox1.AppendText($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} - {message}\n");
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }
    }
}
