using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Windows.Forms;

namespace FindDelete
{

    public partial class Form1 : Form
    {
        public static readonly List<string> ImageExtensions = new List<string> { ".JPG", ".JPE", ".BMP", ".GIF", ".PNG" };

        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {

            FolderBrowserDialog fbd = new FolderBrowserDialog();

            DialogResult result = fbd.ShowDialog();

            var pippo = GetAllFiles(fbd.SelectedPath);

            var test = pippo.GroupBy(x => x.Value).Where(x => x.Count() > 1);

            foreach (var item in pippo)
            {
                dataGridView1.Rows.Add(item.Value, item.Key);
            }

            int rowIndex = 0;
            int count = 2;
            foreach (var item in test)
            {
                foreach (var duplicates in item)
                {
                    dataGridView2.Rows.Add(duplicates.Key, duplicates.Value);

                    if ((count % 2) == 0)
                        dataGridView2.Rows[rowIndex].Cells[1].Style.BackColor = Color.Red;
                    else
                        dataGridView2.Rows[rowIndex].Cells[1].Style.BackColor = Color.Orange;

                    rowIndex++;
                }
                count++;
            }
        }

        private Dictionary<string, string> GetAllFiles(string directoryPath)
        {
            Dictionary<string, string> fileHash = new Dictionary<string, string>();

            using (var md5 = MD5.Create())
            {
                var allFiles = Directory.GetFiles(directoryPath, "*", SearchOption.AllDirectories);

                foreach (var filename in allFiles)
                {
                    using (var stream = File.OpenRead(filename))
                    {
                        var md5Result = md5.ComputeHash(stream);
                        var encoded = Convert.ToBase64String(md5Result);
                        fileHash.Add(filename, encoded);
                    }
                }
            }
            return fileHash;
        }

        private void dataGridView2_CellContentDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            var pippo = dataGridView2.SelectedCells;

            if (pippo.Count == 1)
            {
                int rowIndex = e.RowIndex;

                string filePath = dataGridView2.Rows[rowIndex].Cells[0].Value.ToString();

                if (ImageExtensions.Contains(Path.GetExtension(filePath).ToUpperInvariant()))
                {             
                    FileStream stream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
                    pictureBox1.Image = Image.FromStream(stream);
                    stream.Close();
                }
                else
                    Process.Start(filePath);
            }


        }

        private void DeleteFile_Click(object sender, EventArgs e)
        {
            // Not developed yet.
            throw new NotImplementedException();

            /*
            int selectedFiles = dataGridView2.SelectedCells.Count;

            var pippo = dataGridView2.SelectedCells;
            string files = "";
            foreach (DataGridViewCell item in pippo)
            {
                files += item.Value.ToString() + "\n";
                Console.WriteLine("");
            }

            MessageBox.Show("File selected = " + files);
            */
        }
    }
}
