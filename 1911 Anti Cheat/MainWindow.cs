using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Configuration;

namespace _1911_Anti_Cheat
{
    public partial class AntiCheatForm : Form
    {
        // Game process properties
        string GameProcessName;             // Name of the game process ("hl.exe")
        Process GameProcess;

        // Screen-shot buffer
        Bitmap img = null;                  // Image buffer for the screenshot

        // Timer
        int TimerElapsedSeconds = 0;        // Seconds the timer has been running
        Func<int> TimerLength;              // Random timer duration
        Random TimerRandom;                 // Random num generator (for release `TimerLength`)
        int TimerRandomSeed;                // Random num gen's seed

        // State machine
        enum AntiCheatState
        {
            None,
            FindingGame,
            WaitForReadyClick,
            Running
        }

        AntiCheatState _state = AntiCheatState.None;
        AntiCheatState State
        {
            get => _state;
            set
            {
                if (_state == value)
                {
                    return;
                }

                _state = value;
                btnReady.Enabled = false;
                tmrAntiCheat.Enabled = false;
                tmrFindGame.Enabled = false;
                txtCaptain.Enabled = false;
                txtPlayer.Enabled = false;

                switch (value)
                {
                    case AntiCheatState.FindingGame:
                        btnReady.Text = "Waiting for Game...";
                        if (FindProcess())
                        {
                            State = AntiCheatState.WaitForReadyClick;
                            return;
                        }
                        tmrFindGame.Enabled = true;
                        break;
                    case AntiCheatState.WaitForReadyClick:
                        btnReady.Text = "Ready";
                        btnReady.Enabled = true;
                        txtCaptain.Enabled = true;
                        txtPlayer.Enabled = true;
                        break;
                    case AntiCheatState.Running:
                        btnReady.Text = "Ready";
                        tmrAntiCheat.Enabled = true;
                        break;
                }
            }
        }


        // Useful props/methods
        string GetNames => txtPlayer.Text + " (" + txtCaptain.Text + ")";
        public static string GetTimestamp => "[" + DateTime.Now.ToString("MM-dd-yyyy h-mm-ss") + "]";
        string GetFilenameStart => GetNames + " " + GetTimestamp;
        string ScreencapFilename(string timestamp) => GetFilenameStart + ".jpg";
        string ProcDumpFilename(string timestamp) => GetFilenameStart + ".txt";
        string ConfigFilename(string timestamp, string cfg) => GetFilenameStart + cfg;
        string OutputPath { get => Properties.Settings.Default.OutputPath; set => Properties.Settings.Default.OutputPath = value; }
        


        /// <summary>
        /// Checks if a window still exists
        /// </summary>
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool IsWindow(IntPtr hWnd);

        //=============================================================================
        public AntiCheatForm()
        {
            InitializeComponent();

            if (OutputPath.Length == 0 || !Directory.Exists(OutputPath))
            {
                string tempDirectory = Path.Combine(Path.GetTempPath(), "1911__" + Path.GetRandomFileName());
                Directory.CreateDirectory(tempDirectory);
                OutputPath = tempDirectory;
            }
        }
        
        // On AntiCheat startup
        private void OnStartup(object sender, EventArgs e)
        {
            img = new Bitmap(1, 1);
            GameProcessName = "hl";

#if DEBUG
            // Debug startup
            TimerLength = () => 10; // fixed 10 seconds
            TimerElapsedSeconds = TimerLength();
#else
            // Release startup
            TimerRandomSeed = Guid.NewGuid().GetHashCode();
            TimerRandom = new Random(TimerRandomSeed);
            TimerLength = () => 90 + TimerRandom.Next(61); // (random 1:30 -> 2:30)
            TimerElapsedSeconds = TimerLength();
#endif

            
            // Wait for settings loaded
            Properties.Settings.Default.SettingsLoaded += OnSettingsLoaded;
            Properties.Settings.Default.Reload();

            // Set initial state (find game)
            State = AntiCheatState.FindingGame;
        }
        
        private void OnSettingsLoaded(object sender, SettingsLoadedEventArgs e)
        {
            // Load name/captain
            txtCaptain.Text = Properties.Settings.Default.CaptainName;
            txtPlayer.Text = Properties.Settings.Default.PlayerName;
        }

        // On AntiCheat Exit
        private void OnExit(object sender, FormClosingEventArgs e)
        {
            // Save name/captain
            Properties.Settings.Default.CaptainName = txtCaptain.Text;
            Properties.Settings.Default.PlayerName = txtPlayer.Text;
            Properties.Settings.Default.Save();
            Properties.Settings.Default.Reload();
        }

        //=============================================================================
        // Find the Game Process's Window handle
        private bool FindProcess()
        {
            GameProcess = null;

            Process[] processes = Process.GetProcessesByName(GameProcessName);
            if (processes.Length == 1)
            {
                GameProcess = processes[0];

                // Config Dump
                var configFiles = ConfigFinder.GetConfigFiles(GameProcess.MainWindowHandle);
                foreach (var c in configFiles)
                {
                    string outputPath = Path.Combine(OutputPath, ConfigFilename(GetTimestamp, Path.GetFileName(c)));
                    File.Copy(c, outputPath);
                }

                return true;
            }
            else
            {
                return false;   
            }
        }

        /// <summary>
        /// Get the names of all running processes
        /// </summary>
        string GetProcessDump()
        {
            var sb = new StringBuilder();
            var procs = Process.GetProcesses();
            foreach (var p in procs)
                sb.AppendLine(p.ProcessName);
            return sb.ToString();
        }


        //=============================================================================
        // On "Ready" clicked
        private void btnReady_Click(object sender, EventArgs e)
        {
            if (txtCaptain.Text.Length == 0 || txtPlayer.Text.Length == 0)
            {
                MessageBox.Show("\"Captain\" and \"Player Name\" required", "1911 Anti Cheat", MessageBoxButtons.OK);
                return;
            }

            State = AntiCheatState.Running;
        }

        // Find game timer tick (1 second)
        private void tmrFindGame_Tick(object sender, EventArgs e)
        {
            FindProcess();
            return;
        }

        // Anti-cheat timer tick (1 second)
        private void tmrAntiCheat_Tick(object sender, EventArgs e)
        {

            // Check window handle is valid
            if (GameProcess == null || !IsWindow(GameProcess.MainWindowHandle))
            {
                // If cannot find process, move to "waiting for game" state.
                if (!FindProcess())
                {
                    State = AntiCheatState.FindingGame;
                    return;
                }
            }

            // Check timer ...
            if (--TimerElapsedSeconds <= 0)
            {
                Debug.WriteLine($"AntiCheat - Timer Tick");
                string timestamp = GetTimestamp;

                // Take Game Screenshot
                string imgPath = Path.Combine(OutputPath, ScreencapFilename(timestamp));
                ScreenCapture.TakeScreenshot(GameProcess.MainWindowHandle, ref img);
                img.Save(imgPath, System.Drawing.Imaging.ImageFormat.Jpeg);
                Debug.WriteLine($"   screenshot saved: `{imgPath}`");

                // Running Processes Dump
                string procDump = GetProcessDump();
                string procDumpPath = Path.Combine(OutputPath, ProcDumpFilename(timestamp));
                File.WriteAllText(procDumpPath, procDump);
                Debug.WriteLine($"   proc dump saved: `{procDumpPath}`");

                // Zip and upload to Google Drive
                ZipUploader.DoIt(OutputPath, txtPlayer.Text);

                TimerElapsedSeconds = TimerLength();
                Debug.WriteLine($"   new timer interval: {TimerElapsedSeconds}");
            }
        }
    }
}
