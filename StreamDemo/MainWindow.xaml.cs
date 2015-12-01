using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using Humanizer;
using Humanizer.Bytes;
using Microsoft.Win32;
using Quasardb;

namespace StreamDemo
{
    public partial class MainWindow : Window
    {
        CancellationTokenSource _cancellationTokenSource;

        public MainWindow()
        {
            InitializeComponent();

            _cancellationTokenSource = new CancellationTokenSource();

            cancelButton.Click += OnCancelClicked;
            downloadButton.Click += OnDownloadClicked;
            uploadButton.Click += OnUploadClicked;
        }

        private void OnCancelClicked(object sender, RoutedEventArgs e)
        {
            _cancellationTokenSource.Cancel();
            _cancellationTokenSource = new CancellationTokenSource();
        }

        private async void OnDownloadClicked(object sender, RoutedEventArgs e)
        {
            var dlg = new SaveFileDialog();
            dlg.FileName = entryAlias.Text;
            var ok = dlg.ShowDialog(this);
            if (!ok.Value) return;

            try
            {
                SwitchToRunningMode();
                await Download(dlg.FileName);
                SwitchToReadyMode();
            }
            catch (Exception ex)
            {
                SwitchToErrorMode();
                statusLabel.Content = ex.Message;
            }
        }

        private async Task Download(string fileName)
        {
            using (var output = File.Create(fileName))
            using (var qdb = new QdbCluster(clusterUri.Text))
            using (var input = qdb.Stream(entryAlias.Text).Open(QdbStreamMode.Read))
            {
                await StreamHelper.Copy(input, output, OnProgress, _cancellationTokenSource.Token);
            }
        }

        private async void OnUploadClicked(object sender, RoutedEventArgs e)
        {
            var dlg = new OpenFileDialog();
            var ok = dlg.ShowDialog(this);
            if (!ok.Value) return;

            try
            {
                SwitchToRunningMode();
                await Upload(dlg.FileName);
                SwitchToReadyMode();
            }
            catch (Exception ex)
            {
                SwitchToErrorMode();
                statusLabel.Content = ex.Message;
            }
        }

        private async Task Upload(string fileName)
        {
            using (var input = File.OpenRead(fileName))
            using (var qdb = new QdbCluster(clusterUri.Text))
            using (var output = qdb.Stream(entryAlias.Text).Open(QdbStreamMode.Append))
            {
                await StreamHelper.Copy(input, output, OnProgress, _cancellationTokenSource.Token);
            }
        }

        private void OnProgress(StreamHelper.CopyStatus progress)
        {
            progressBar.Value = 100.0 * progress.BytesCopied / progress.TotalBytes;

            if (progress.BytesCopied == progress.TotalBytes)
                statusLabel.Content = string.Format("{0} copied in {1}",
                    ByteSize.FromBytes(progress.BytesCopied).Humanize("0.#"), progress.Elapsed.Humanize());
            else
                statusLabel.Content = string.Format("{0}/s",
                    ByteSize.FromBytes(progress.BytesCopied/progress.Elapsed.TotalSeconds).Humanize("0.#"));
        }

        private void SwitchToReadyMode()
        {
            clusterUri.IsEnabled = true;
            entryAlias.IsEnabled = true;
            uploadButton.IsEnabled = true;
            downloadButton.IsEnabled = true;
            progressBar.Visibility = Visibility.Hidden;
            cancelButton.Visibility = Visibility.Hidden;
            statusLabel.Foreground = new SolidColorBrush(Colors.Black);
        }

        private void SwitchToRunningMode()
        {
            clusterUri.IsEnabled = false;
            entryAlias.IsEnabled = false;
            uploadButton.IsEnabled = false;
            downloadButton.IsEnabled = false;
            progressBar.Visibility = Visibility.Visible;
            cancelButton.Visibility = Visibility.Visible;
            statusLabel.Foreground = new SolidColorBrush(Colors.White);
        }

        private void SwitchToErrorMode()
        {
            clusterUri.IsEnabled = true;
            entryAlias.IsEnabled = true;
            uploadButton.IsEnabled = true;
            downloadButton.IsEnabled = true;
            progressBar.Visibility = Visibility.Hidden;
            cancelButton.Visibility = Visibility.Hidden;
            statusLabel.Visibility = Visibility.Visible;
            statusLabel.Foreground = new SolidColorBrush(Colors.Red);
        }
    }
}
