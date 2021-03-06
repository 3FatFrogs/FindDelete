﻿using System;
using System.Collections.Generic;
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

        private Dictionary<string, string> allFiles;

        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            ResetDataGrids();

            FolderBrowserDialog fbd = new FolderBrowserDialog();
            fbd.SelectedPath = @"C:\test\";

            if (fbd.ShowDialog() == DialogResult.OK)
            {
                AddResultsToForm(fbd);
            }
        }

        private void AddResultsToForm(FolderBrowserDialog fbd = null)
        {

            if (fbd != null)
                allFiles = GetAllFiles(fbd.SelectedPath);

            DisplayDuplicates();
        }

        private void DisplayDuplicates()
        {
            dataGridView2.Rows.Clear();

            int rowIndex = 0;
            int count = 2;

            var duplicates = allFiles.GroupBy(x => x.Value).Where(x => x.Count() > 1);

            foreach (var item in duplicates)
            {
                foreach (var d in item)
                {
                    dataGridView2.Rows.Add(d.Key, d.Value);

                    if ((count % 2) == 0)
                        dataGridView2.Rows[rowIndex].Cells[0].Style.BackColor = Color.Red;
                    else
                        dataGridView2.Rows[rowIndex].Cells[0].Style.BackColor = Color.Orange;

                    rowIndex++;
                }
                count++;
            }
        }

        private void ResetDataGrids()
        {
            dataGridView2.DataSource = null;
            dataGridView2.Rows.Clear();
        }

        private Dictionary<string, string> GetAllFiles(string directoryPath)
        {
            Dictionary<string, string> fileHash = new Dictionary<string, string>();

            using (var md5 = MD5.Create())
            {
                var allFiles = Directory.GetFiles(directoryPath, "*", SearchOption.AllDirectories);

                int i = 0;

                foreach (var filename in allFiles)
                {
                    using (var stream = File.OpenRead(filename))
                    {
                        var md5Result = md5.ComputeHash(stream);
                        var encoded = Convert.ToBase64String(md5Result);
                        fileHash.Add(filename, encoded);

                        progressBar1.Value = 100 * ++i / allFiles.Count();
                    }
                }
            }
            return fileHash;
        }

        private void dataGridView2_CellContentDoubleClick(object sender, DataGridViewCellEventArgs e)
        {

            var selectedCelss = dataGridView2.SelectedCells;

            if (selectedCelss.Count == 1)
            {
                int rowIndex = e.RowIndex;

                var value = dataGridView2.Rows[rowIndex].Cells[0].Value;

                if (value != null)
                {
                    string filePath = value.ToString();

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
        }

        private void DeleteFile_Click(object sender, EventArgs e)
        {
            int selectedFiles = dataGridView2.SelectedCells.Count;

            var pippo = dataGridView2.SelectedCells;
            string files = "";
            foreach (DataGridViewCell item in pippo)
            {
                files += item.Value.ToString() + "\n";
                Console.WriteLine("");
            }

            DialogResult delete = MessageBox.Show("File to be deleted = " + files, "Delete file(s)?", MessageBoxButtons.YesNo);
            if (delete == DialogResult.Yes)
            {
                foreach (DataGridViewCell item in pippo)
                {
                    File.Delete(item.Value.ToString());
                    allFiles.Remove(item.Value.ToString());
                }
            }

            pictureBox1.Image = null;
            pictureBox1.Invalidate();

            DisplayDuplicates();
        }


        private void dataGridView2_KeyPress(object sender, KeyPressEventArgs e)
        {
            DisplayPictureFromFilePath();
        }

        private void dataGridView2_SelectionChanged(object sender, EventArgs e)
        {
            DisplayPictureFromFilePath();
        }

        private void DisplayPictureFromFilePath()
        {
            var selectedCelss = dataGridView2.SelectedCells;

            if (selectedCelss.Count == 1 && selectedCelss[0].Value != null)
            {
                string filePath = selectedCelss[0].Value.ToString();

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

        private void dataGridView2_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                var selectedCelss = dataGridView2.SelectedCells;

                string filePath = selectedCelss[0].Value.ToString();

                if (File.Exists(filePath))
                {

                    Process.Start("explorer.exe", "/select, " + filePath);
                }
            }
            else
            {
                DisplayPictureFromFilePath();
            }

        }
    }
}
