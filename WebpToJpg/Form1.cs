using System;
using System.IO;
using System.Text;
using System.Drawing;
using System.Diagnostics;
using System.Windows.Forms;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.VisualBasic.FileIO;
using WebpToJpg.ColorService;
using WebpToJpg.ImageService;
using WebpToJpg.UtilitiesService;

namespace WebpToJpg
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            SetStyle
                (
                ControlStyles.UserPaint |
                ControlStyles.AllPaintingInWmPaint | 
                ControlStyles.OptimizedDoubleBuffer, 
                true
                );
            AllowDrop = true;
            KeyDown += Key_Down;
            DragDrop += Panel1_DragDrop;
            DragEnter += Panel1_DragEnter;
            MouseDown += Form1_MouseDown;
            MouseMove += Form1_MouseMove;
            FormClosed += Form1_FormClosed;
            menuStrip1.MouseDown += Form1_MouseDown;
            menuStrip1.MouseMove += Form1_MouseMove;
            button1.KeyDown += Key_Down;
            colorDialog1.CustomColors = new int[] { -986896, -16338907 };
            toolStripComboBox1.Items.AddRange(Enum.GetNames(typeof(PictureFormat)));
            toolStripComboBox1.SelectedIndex = 0;
        }

        private PictureFormat outputFormat;
        private Point lastPosition;
        private string savepath = null;
        private readonly string logFile = "log.txt";
        private readonly INIFile INI = new INIFile();
        private readonly PictureConverter converter = new PictureConverter();

        private void Key_Down(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Escape)
            {
                Application.Exit();
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            if (INI.FileExists)
            {
                try
                {
                    Location = new Point(INI.Parse("Main", "X"),
                                         INI.Parse("Main", "Y"));
                    toolStripComboBox1.Text = INI.Read("Main", "Format");
                    RandomName.Checked = INI.Parse<bool>("Main", "RandomName");
                    deletePhoto.Checked = INI.Parse<bool>("Main", "DeleteSoucePhoto");
                    openOutputFolder.Checked = INI.Parse<bool>("Main", "OpenOutputFolder");
                    BackColor = ColorConvert.ColorFromHex(INI.Read("Main", "ColorForm"));
                }
                catch (Exception exc)
                {
                    ShowMessageBox(exc.Message);
                }
            }

            menuStrip1.BackColor = BackColor;
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            INI.Write("Main", "X", Location.X);
            INI.Write("Main", "Y", Location.Y);
            INI.Write("Main", "Format", outputFormat);
            INI.Write("Main", "RandomName", RandomName.Checked);
            INI.Write("Main", "DeleteSoucePhoto", deletePhoto.Checked);
            INI.Write("Main", "OpenOutputFolder", openOutputFolder.Checked);
            INI.Write("Main", "ColorForm", ColorConvert.ColorToHex(BackColor));
        }

        private void ShowMessageBox(string message)
        {
            MsgBox messageBox = new MsgBox(message);
            messageBox.Show(this);
        }

        private void OpenFolder(string path)
        {
            if (Directory.Exists(path))
            {
                Process.Start(path);
            }
            else
            {
                ShowMessageBox("Output Folder is not selected");
            }
        }

        private bool DirectoryIsEmpty(string path)
        {
            return Directory.GetFiles(path, "*.*", System.IO.SearchOption.AllDirectories).Length == 0;
        }

        private void DeleteEmptyDirectory(string path, bool recursive = false)
        {
            if (Directory.Exists(path) && DirectoryIsEmpty(path))
            {
                Directory.Delete(path, recursive);
            }
        }

        private string[] GetAllFiles(params string[] paths)
        {
            List<string> result = new List<string>();

            foreach (string path in paths)
            {
                if (File.Exists(path))
                {
                    result.Add(path);
                }
                else
                {
                    string[] files = Directory.GetFiles(path, "*.*", System.IO.SearchOption.AllDirectories);
                    result.AddRange(files);
                }
            }

            return result.ToArray();
        }

        private void Panel1_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = e.Data.GetDataPresent(DataFormats.FileDrop, false) ? DragDropEffects.Move : DragDropEffects.None;
        }

        private async void Panel1_DragDrop(object sender, DragEventArgs e)
        {
            string[] paths = (string[])e.Data.GetData(DataFormats.FileDrop, false);
            await OpenFiles(GetAllFiles(paths));

            foreach (string path in paths)
            {
                DeleteEmptyDirectory(path, true);
            }
        }

        private void Form1_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                int dx = e.Location.X - lastPosition.X;
                int dy = e.Location.Y - lastPosition.Y;
                Location = new Point(Location.X + dx, Location.Y + dy);
            }
        }

        private void Form1_MouseDown(object sender, MouseEventArgs e)
        {
            lastPosition = e.Location;
        }

        private void WriteLog(string message)
        {
            string text = $"[{DateTime.Now}] {message}\n";
            File.AppendAllText(logFile, text, Encoding.Default);
        }

        private async Task OpenFiles(string[] files)
        {
            if (string.IsNullOrEmpty(savepath))
            {
                if (folderBrowserDialog1.ShowDialog() == DialogResult.Cancel)
                {
                    return;
                }
                savepath = folderBrowserDialog1.SelectedPath;
            }

            foreach (string sourcePath in files)
            {
                string destinationFileName = RandomName.Checked ? Utilities.GetRandomName() : Path.GetFileNameWithoutExtension(sourcePath);
                string destinationPath = Path.Combine(savepath, destinationFileName) + $".{outputFormat}";
                bool success = await Task.Run(() => TryConvert(sourcePath, destinationPath, outputFormat));

                if (success && deletePhoto.Checked)
                {
                    FileSystem.DeleteFile(sourcePath, UIOption.OnlyErrorDialogs, RecycleOption.SendToRecycleBin);
                    WriteLog($"Source file {sourcePath} was deleted");
                }
            }

            if (openOutputFolder.Checked)
            {
                OpenFolder(savepath);
            }

            ShowMessageBox("Done!");
        }

        private bool TryConvert(string sourcePath, string destinationPath, PictureFormat outputFormat)
        {
            bool success = true;
            try
            {
                switch (outputFormat)
                {
                    case PictureFormat.jpg:
                        converter.ConvertWebpToJpg(sourcePath, destinationPath);
                        break;
                    case PictureFormat.webp:
                        converter.ConvertJpgToWebp(sourcePath, destinationPath);
                        break;
                }

                if (sourcePath.Equals(destinationPath))
                {
                    success = false;
                }

                WriteLog($"File {sourcePath} convert to {destinationPath}");
            }
            catch (Exception exc)
            {
                if (sourcePath.Equals(destinationPath))
                {
                    success = false;
                }
                else
                {
                    File.Copy(sourcePath, destinationPath, true);
                    WriteLog($"File {sourcePath} copyed to {destinationPath}");
                    WriteLog(exc.Message);
                }
            }
            return success;
        }

        private async void SelectFiles()
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                await OpenFiles(openFileDialog1.FileNames);
            }
            else
            {
                ShowMessageBox("Fucking Idiot!!!!!!");
            }
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            SelectFiles();
        }

        private void ConvertToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SelectFiles();
        }

        private void OpenFolderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFolder(savepath);
        }

        private void SetFolderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                savepath = folderBrowserDialog1.SelectedPath;
            }
        }

        private void DelSourceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            deletePhoto.Checked = !deletePhoto.Checked;
        }

        private void SetColor(Color color)
        {
            BackColor = color;
            menuStrip1.BackColor = color;
        }

        private void ColorFormToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (colorDialog1.ShowDialog() == DialogResult.OK)
            {
                SetColor(colorDialog1.Color);
            }
        }

        private void ExitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void OpenOutputFolderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            openOutputFolder.Checked = !openOutputFolder.Checked;
        }

        private void ToolStripMenuItem2_Click(object sender, EventArgs e)
        {
            RandomName.Checked = !RandomName.Checked;
        }

        private void ToolStripComboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            Enum.TryParse(toolStripComboBox1.Text, out outputFormat);
        }
    }
}