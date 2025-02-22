using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
/*using System.IO.Compression;*/
using System.Windows.Forms;
using ICSharpCode.SharpZipLib.Zip;

namespace Folder_locker
{
    internal class ZippingFIle
    {
       /* public static void ZipFolder(string sourceFolder, string zipPath)
        {
            try
            {
                if (Directory.Exists(sourceFolder))
                {
                    // Ensure the zip file does not exist before creating it
                    if (File.Exists(zipPath))
                    {
                        File.Delete(zipPath);
                    }

                    // Create ZIP file
                    ZipFile.CreateFromDirectory(sourceFolder, zipPath, CompressionLevel.Optimal, false);

                    MessageBox.Show("Folder successfully zipped!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("Source folder does not exist!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public static void UnzipFolder(string zipPath, string extractPath)
        {
            try
            {
                if (File.Exists(zipPath))
                {
                    // Extract ZIP file to destination folder
                    ZipFile.ExtractToDirectory(zipPath, extractPath);

                    MessageBox.Show("Folder successfully unzipped!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("ZIP file does not exist!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
*/
        public static void CreatePasswordProtectedZip(string folderPath, string zipFilePath, string password)
        {
            try
            {
                using (FileStream fsOut = File.Create(zipFilePath))
                {
                    using (ZipOutputStream zipStream = new ZipOutputStream(fsOut))
                    {
                        zipStream.SetLevel(9); // Compression level (0-9)
                        zipStream.Password = password; // Set the password for encryption

                        foreach (string filePath in Directory.GetFiles(folderPath, "*", SearchOption.AllDirectories))
                        {
                            string entryName = filePath.Substring(folderPath.Length + 1); // Relative path inside ZIP
                            ZipEntry newEntry = new ZipEntry(entryName)
                            {
                                AESKeySize = 256, // Use AES-256 encryption
                                DateTime = DateTime.Now
                            };

                            zipStream.PutNextEntry(newEntry);

                            byte[] buffer = File.ReadAllBytes(filePath);
                            zipStream.Write(buffer, 0, buffer.Length);
                            zipStream.CloseEntry();
                        }
                        zipStream.IsStreamOwner = true;
                    }
                }
                MessageBox.Show("ZIP file created successfully!");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        public static void ExtractPasswordProtectedZip(string zipFilePath, string outputFolder, string password)
        {
            try
            {
                using (ZipFile zipFile = new ZipFile(File.OpenRead(zipFilePath)))
                {
                    zipFile.Password = password; // Set the password

                    foreach (ZipEntry entry in zipFile)
                    {
                        if (!entry.IsFile) continue; // Skip directories

                        string outputPath = Path.Combine(outputFolder, entry.Name);
                        Directory.CreateDirectory(Path.GetDirectoryName(outputPath));

                        using (Stream zipStream = zipFile.GetInputStream(entry))
                        using (FileStream fsOut = File.Create(outputPath))
                        {
                            zipStream.CopyTo(fsOut);
                        }
                    }
                }
                MessageBox.Show("ZIP file extracted successfully!");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: Incorrect password or invalid ZIP file.");
            }
        }

    }
}
