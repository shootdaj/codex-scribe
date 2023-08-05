using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using Microsoft.Win32;

namespace WPFApp
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void BtnSelectFolder_Click(
            object sender,
            RoutedEventArgs e
        )
        {
            var dialog = new OpenFileDialog();
            dialog.ValidateNames = false;
            dialog.CheckFileExists = false;
            dialog.CheckPathExists = true;
            dialog.FileName = "Folder Selection.";

            if (dialog.ShowDialog() == true)
            {
                var folderPath = Path.GetDirectoryName(dialog.FileName);
                txtSelectedFolder.Text = folderPath;
            }
        }

        private void BtnStart_Click(
            object sender,
            RoutedEventArgs e
        )
        {
            var sourceDirectory = txtSelectedFolder.Text;
            if (string.IsNullOrWhiteSpace(sourceDirectory))
            {
                MessageBox.Show("Please select a folder.");
                return;
            }

            var fileExtensions = txtFileExtensions.Text.Split(',').Select(ext => ext.Trim()).ToArray();
            if (!fileExtensions.Any())
            {
                MessageBox.Show("Please provide at least one file extension.");
                return;
            }

            var codeStringBuilder = new StringBuilder();

            foreach (var fileExtension in fileExtensions)
            foreach (var file in Directory.EnumerateFiles(
                         sourceDirectory,
                         $"*{fileExtension}",
                         SearchOption.AllDirectories
                     ))
            {
                // Exclude files in the bin and obj directories
                if (file.Contains("\\bin\\") || file.Contains("\\obj\\"))
                {
                    continue;
                }

                // Get the relative file path and add it to the StringBuilder
                var relativePath = Path.GetRelativePath(sourceDirectory, file);
                codeStringBuilder.AppendLine($"File: {relativePath}\n---\n{File.ReadAllText(file)}\n");
            }

            var outputText = codeStringBuilder.ToString();

            // Display the output in the TextBox
            txtOutput.Text = outputText;

            var outputFile = Path.Combine(sourceDirectory, "output.txt");
            File.WriteAllText(outputFile, outputText);

            MessageBox.Show($"Codebase has been written to {outputFile}");
        }

        private void BtnCopyToClipboard_Click(
            object sender,
            RoutedEventArgs e
        )
        {
            Clipboard.SetText(txtOutput.Text);
        }
    }
}