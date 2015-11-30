using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Win32;
using Quasardb;

namespace StreamDemo
{

    public partial class MainWindow : Window, IStreamCopyObserver
    {
        readonly CancellationTokenSource _cancellationTokenSource;

        public MainWindow()
        {
            InitializeComponent();

            _cancellationTokenSource = new CancellationTokenSource();
        }

        private async void OnUploadButtonClicked(object sender, RoutedEventArgs e)
        {
            var dlg = new OpenFileDialog();
            var ok = dlg.ShowDialog(this);
            if (!ok.Value) return;

            try
            {
                await Upload(dlg.FileName);
            }
            catch (Exception ex)
            {
                errorLabel.Content = ex.Message;
            }
        }

        private async void OnDownloadButtonClicked(object sender, RoutedEventArgs e)
        {
            var dlg = new SaveFileDialog();
            var ok = dlg.ShowDialog(this);
            if (!ok.Value) return;

            try
            {
                await Download(dlg.FileName);
            }
            catch (Exception ex)
            {
                errorLabel.Content = ex.Message;
            }
        }

        private void OnCancelClicked(object sender, RoutedEventArgs e)
        {
            _cancellationTokenSource.Cancel();
        }

        private async Task Upload(string fileName)
        {
            using (var input = File.OpenRead(fileName))
            using (var qdb = new QdbCluster(clusterUri.Text))
            using (var output = qdb.Stream(entryAlias.Text).Open(QdbStreamMode.Append))
            {
                await StreamHelper.Copy(input, output, this, _cancellationTokenSource.Token);
            }
        }

        private async Task Download(string fileName)
        {
            using (var output = File.Create(fileName))
            using (var qdb = new QdbCluster(clusterUri.Text))
            using (var input = qdb.Stream(entryAlias.Text).Open(QdbStreamMode.Read))
            {
                await StreamHelper.Copy(input, output, this, _cancellationTokenSource.Token);
            }
        }

        void IStreamCopyObserver.Progress(double percent)
        {
            progressBar.Value = percent;
        }

        void IStreamCopyObserver.Thoughtput(double kbps)
        {
            errorLabel.Content = string.Format("{0:0.0} MB/s", kbps/1000);
        }
    }
}
