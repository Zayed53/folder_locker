using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Security.AccessControl;



namespace Folder_locker
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        string path;
        

        private void button1_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog openFolderDialog = new FolderBrowserDialog();
            if (openFolderDialog.ShowDialog() == DialogResult.OK)
            {
                //Uri uri = new Uri(openFileDialog.FileName);

                path = openFolderDialog.SelectedPath;
                textBox1.Text = path;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(path))
            {
                foreach (string file in Directory.GetFiles(path))
                {
                    AESHelper.EncryptFile(file);
                }

                SetFolderPermissions(true);
                MessageBox.Show("Folder locked successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("Please select a folder first.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(path))
            {
                
               
                    SetFolderPermissions(false);

                    foreach (string file in Directory.GetFiles(path, "*.locked"))
                    {
                        AESHelper.DecryptFile(file);
                    }

                    MessageBox.Show("Folder unlocked successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("Please select a folder first.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            
        }

        private void SetFolderPermissions(bool allowAccess)
        {
            try
            {

                string folderPath = path;
                string adminUserName = Environment.UserName;// getting your adminUserName
                DirectorySecurity ds = Directory.GetAccessControl(folderPath);
                FileSystemAccessRule fsa = new FileSystemAccessRule(adminUserName, FileSystemRights.FullControl, AccessControlType.Deny);

                if (allowAccess)
                {
                    ds.AddAccessRule(fsa);
                }
                else
                {
                    ds.RemoveAccessRule(fsa);
                }

                //ds.AddAccessRule(fsa);
                Directory.SetAccessControl(folderPath, ds);
                if (allowAccess)
                {
                    MessageBox.Show("UnLocked");
                }
                else
                {
                    MessageBox.Show("Locked");
                }
                
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void unlock()
        {
            try
            {

                string folderPath = path;
                string adminUserName = Environment.UserName;// getting your adminUserName
                DirectorySecurity ds = Directory.GetAccessControl(folderPath);
                FileSystemAccessRule fsa = new FileSystemAccessRule(adminUserName, FileSystemRights.FullControl, AccessControlType.Deny);

                ds.RemoveAccessRule(fsa);
                Directory.SetAccessControl(folderPath, ds);
                MessageBox.Show("UnLocked");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnZip_Click(object sender, EventArgs e)
        {
            
                if (!string.IsNullOrEmpty(path))
                {
                    
                    string zipPath = Path.Combine(Path.GetDirectoryName(path), Path.GetFileName(path) + ".zip");

                    ZippingFIle.CreatePasswordProtectedZip(path, zipPath, "abs");
                    SetFolderPermissions(true);
                }
            
        }

        private void btnUnzip_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "ZIP Files|*.zip";
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string zipPath = openFileDialog.FileName;
                    string extractPath = Path.Combine(Path.GetDirectoryName(zipPath), Path.GetFileNameWithoutExtension(zipPath));

                    ZippingFIle.ExtractPasswordProtectedZip(zipPath, extractPath, "abs");
                    SetFolderPermissions(false);
                }
            }
        }

        
    }
}
