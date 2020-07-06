using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OsuGallery
{
    public partial class Form1 : Form
    {
        //some global variables
        BackgroundWorker worker;
        private List<string> foundElements = new List<string>();
        private string songsFolder;

        //counter for found elements
        int ctr = 0;
        int folderCount = 0;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //get osu path form registry
            txtOsuLocation.Text = getOsuFilePath();

            //initialize worker
            worker = new BackgroundWorker();
            worker.DoWork += new DoWorkEventHandler(findElements);
            worker.ProgressChanged += new ProgressChangedEventHandler(progressBar);
            worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(findComplete);
            worker.WorkerReportsProgress = true;
            worker.WorkerSupportsCancellation = true;
        }

        private void findElements(object sender, DoWorkEventArgs e)
        {
            //count folders for progress count
            folderCount = Directory.GetDirectories(songsFolder + "\\").Length;

            //iterate on every dir of \Songs\
            foreach (string item in Directory.GetDirectories(songsFolder + "\\"))
            {
                foundElements.Add(item);

                ctr++;

                if (worker.CancellationPending)
                {
                    return;
                }

                worker.ReportProgress((int)((double)ctr / folderCount * 100));
            }
            worker.ReportProgress(1);
        }

        private void progressBar(object sender, ProgressChangedEventArgs e)
        {
            lblStatus.Text = String.Format("Found {0} items. ({1}% done)", ctr, e.ProgressPercentage);
        }

        private void findComplete(object sender, RunWorkerCompletedEventArgs e)
        {
            foreach (string file in foundElements)
            {
                lstDirectories.Items.Add(file.Replace(songsFolder + "\\", string.Empty));
            }

            lblStatus.Text = "Found " + ctr + " items";

            btnCancel.Visible = false;

            enableEverything(true);

            if (foundElements.Count > 0)
            {
                lstDirectories.SelectedIndex = 0;
            }
        }

        private void enableEverything(bool x)
        {
            txtSearch.Enabled = x;
            btnSearch.Enabled = x;
            btnNext.Enabled = x;
            btnPrev.Enabled = x;
            btnReset.Enabled = x;
            button1.Enabled = x;
            lstDirectories.Enabled = x;
        }

        private static string getBGPath(string path)
        {
            using (StreamReader file = File.OpenText(path))
            {
                string line;
                while ((line = file.ReadLine()) != null)
                {
                    if (Regex.IsMatch(line, "^//Background and Video events"))
                    {
                        line = file.ReadLine();
                        string[] items = line.Split(',');
                        if (items[0] == "0")
                        {
                            string tmp = "\\" + items[2].Replace("\"", string.Empty);
                            return tmp;
                        }
                    }
                }
                return null;
            }
        }

        private static string getOsuFilePath()
        {
            using (RegistryKey osuRegistry = Registry.ClassesRoot.OpenSubKey("osu\\DefaultIcon"))
            {
                if (osuRegistry != null)
                {
                    string osuRegistryKey = osuRegistry.GetValue(null).ToString();
                    string osuFilePath = osuRegistryKey.Remove(0, 1);
                    osuFilePath = osuFilePath.Remove(osuFilePath.Length - 11);
                    return osuFilePath;
                }
                else
                {
                    MessageBox.Show("osu! directory cannot be found, please look for it :)");
                    return null;
                }
            }
        }

        private void btnLocateOsu_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog folder = new FolderBrowserDialog();
            folder.ShowNewFolderButton = false;
            folder.RootFolder = Environment.SpecialFolder.MyComputer;
            folder.Description = "Select YOUR osu! ROOT directory:";
            folder.SelectedPath = txtOsuLocation.Text;
            DialogResult path = folder.ShowDialog();
            if (path == DialogResult.OK)
            {
                //check if osu!.exe is present
                if (!File.Exists(folder.SelectedPath + "\\osu!.exe"))
                {
                    MessageBox.Show("Not a valid osu! directory!", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    btnLocateOsu_Click(sender, e);
                    return;
                }
            }

            songsFolder = getSongsFolder(folder.SelectedPath);

            lblSongsFolder.Text = songsFolder + "\\";

            txtOsuLocation.Text = folder.SelectedPath + "\\";
        }

        private string getSongsFolder(string rootFolder) 
        {
            string windowsUsername = Environment.UserName;
            string osuCfgFile = string.Format("osu!.{0}.cfg", windowsUsername);
            string[] cfgFileLines;
            try
            {
                cfgFileLines = File.ReadAllLines(String.Format("{0}\\{1}", rootFolder, osuCfgFile));
            }
            catch (Exception)
            {
                MessageBox.Show("Can't find the osu! songs folder, select the proper osu! ROOT directory");

                return "--";
            }            

            string whatToFind = "^BeatmapDirectory";
            foreach (string line in cfgFileLines)
            {
                if (Regex.IsMatch(line, whatToFind))
                {
                    string[] tmp = line.Split('=');

                    return tmp[1].Trim();
                }
            }

            return "--";
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            if (getSongsFolder(txtOsuLocation.Text) == "--" || lblSongsFolder.Text == "unknown" || lblSongsFolder.Text == "--\\")
            {
                btnLocateOsu_Click(sender, e);
            }

            ctr = 0;
            folderCount = 0;
            foundElements.Clear();

            btnLocateOsu.Enabled = false;
            txtOsuLocation.Enabled = false;

            lblStatus.Text = "Locating files...";

            btnCancel.Visible = true;
            lstDirectories.Items.Clear();

            worker.RunWorkerAsync();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            if (worker.IsBusy)
            {
                worker.CancelAsync();
            }
        }

        private void lstImageFileLocations_SelectedIndexChanged(object sender, EventArgs e)
        {
            string dir = lstDirectories.SelectedItem.ToString();

            loadImages(dir);

            UpdateButtonNavigationState();
        }

        private void UpdateButtonNavigationState()
        {
            int listLength = lstDirectories.Items.Count;
            int currentIndex = lstDirectories.SelectedIndex;

            if (currentIndex == 0)
            {
                btnPrev.Enabled = false;
            }
            else if (currentIndex == listLength - 1)
            {
                btnNext.Enabled = false;
            }
            else
            {
                btnNext.Enabled = true;
                btnPrev.Enabled = true;
            }
        }

        private void NavigateList(object sender, EventArgs e)
        {
            bool fromPrev = (sender == btnPrev) ? true : false;

            int imagesCount = lstImages.Items.Count;

            if (imagesCount <= 1)
            {
                if (!fromPrev)
                {
                    lstDirectories.SelectedIndex += 1;
                }
                else
                {
                    lstDirectories.SelectedIndex -= 1;
                }
            }
            else
            {
                int imagesIndex = lstImages.SelectedIndex;

                if (imagesIndex != 0)
                {
                    if (imagesIndex != imagesCount - 1)
                    {
                        if (!fromPrev)
                        {
                            lstImages.SelectedIndex += 1;
                        }
                        else
                        {
                            lstImages.SelectedIndex -= 1;
                        }
                    }
                    else
                    {
                        if (fromPrev)
                        {
                            lstImages.SelectedIndex -= 1;
                        }
                        else
                        {
                            lstDirectories.SelectedIndex += 1;
                        }
                    }
                }
                else
                {
                    if (fromPrev)
                    {
                        lstDirectories.SelectedIndex -= 1;
                    }
                    else
                    {
                        lstImages.SelectedIndex += 1;
                    }
                }
            }
        }

        private void loadImages(string dir)
        {
            lstImages.Items.Clear();

            string fullPath = songsFolder + "\\" + dir;

            List<string> bgElements = new List<string>();
            foreach (string file in Directory.GetFiles(fullPath))
            {
                if (Regex.IsMatch(file, "osu$"))
                {
                    string bg = getBGPath(file);

                    if (bg != null && !bgElements.Contains(bg))
                    {
                        bgElements.Add(bg);
                    }
                }
            }

            foreach (string item in bgElements)
            {
                lstImages.Items.Add(item);
            }

            try
            {
                lstImages.Enabled = true;
                lstImages.SelectedIndex = 0;
            }
            catch (Exception)
            {
                lstImages.Enabled = false;

                pbxDisplayImage.Image = null;
            }
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            string searchKey = txtSearch.Text.Trim();

            lstDirectories.BeginUpdate();
            enableEverything(false);
            lstDirectories.Items.Clear();

            if (!string.IsNullOrEmpty(searchKey))
            {
                int foundctr = 0;
                foreach (string key in foundElements)
                {
                    string lowKey = key.Replace(songsFolder + "\\", string.Empty).ToLower();
                    string lolSKey = searchKey.ToLower();

                    if (lowKey.Contains(lolSKey))
                    {
                        lstDirectories.Items.Add(key.Replace(songsFolder + "\\", string.Empty));
                        foundctr++;
                    }
                }
                lblStatus.Text = searchKey + " has " + foundctr + " results";
            }
            else
            {
                foreach (string file in foundElements)
                { lstDirectories.Items.Add(file.Replace(songsFolder + "\\", string.Empty)); }
            }

            lstDirectories.EndUpdate();

            if (lstDirectories.Items.Count > 0)
            {
                lstDirectories.SelectedIndex = 0;
            }

            enableEverything(true);
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            string currentSelection;
            try
            {
                currentSelection = lstDirectories.GetItemText(lstDirectories.SelectedItem.ToString());
            }
            catch (Exception)
            {
                currentSelection = null;
            }

            lstDirectories.BeginUpdate();
            lstDirectories.Items.Clear();

            foreach (string file in foundElements)
            {
                lstDirectories.Items.Add(file.Replace(songsFolder + "\\", string.Empty));
            }

            txtSearch.Text = "";
            lstDirectories.EndUpdate();

            if (currentSelection != null)
            {
                lstDirectories.SelectedItem = currentSelection;
            }
            else
            {
                lstDirectories.SelectedIndex = 0;
            }

            lblStatus.Text = "Found " + ctr + " folders";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                string folder = lstDirectories.SelectedItem.ToString();
                string image = lstImages.SelectedItem.ToString();

                string fullPath = songsFolder + "\\" + folder + image;

                lblFullFilePath.Text = fullPath;

                fullPath = Path.GetFullPath(fullPath);
                System.Diagnostics.Process.Start("explorer.exe", string.Format("/select,\"{0}\"", fullPath));
            }
            catch (Exception)
            {
                MessageBox.Show("Cannot open file.");
            }
        }

        private void lstImages_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                string folder = lstDirectories.SelectedItem.ToString();
                string image = lstImages.SelectedItem.ToString();
                string fullpath = songsFolder + "\\" + folder + image;

                lblFullFilePath.Text = fullpath;

                pbxDisplayImage.Image = Image.FromFile(fullpath);
            }
            catch (Exception)
            {
                pbxDisplayImage.Image = null;
            }
        }
    }
}
