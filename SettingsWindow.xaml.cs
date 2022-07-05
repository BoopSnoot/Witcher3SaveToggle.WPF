using System.ComponentModel;
using System.Configuration;
using System.IO;
using System.Windows;
using Ookii.Dialogs.Wpf;

namespace Witcher3SaveToggle;

public partial class SettingsWindow {
    public SettingsWindow() {
        InitializeComponent();
        BinLocTextBox.Text = ConfigurationManager.AppSettings["WitcherBinLoc"];
        SaveLocTextBox.Text = ConfigurationManager.AppSettings["WitcherSaveLoc"];
    }

    private void SettingsWindow_OnLoaded(object sender, RoutedEventArgs e) {
        MinWidth = ActualWidth;
        MinHeight = ActualHeight;
        MaxHeight = ActualHeight;
    }

    private void BinLocBrowse_OnClick(object sender, RoutedEventArgs e) {
        VistaOpenFileDialog openFileDialog = new() {
            Filter = @"Witcher 3 executable (witcher3.exe)|witcher3.exe|EXE files (*.exe)|*.exe|All files (*.*)|*.*",
            FilterIndex = 1,
            RestoreDirectory = true,
            Title = Properties.Resources.BinLoc_Label
        };

        if (openFileDialog.ShowDialog() != true) return;
        BinLocTextBox.Text = openFileDialog.FileName;
    }

    private void SaveLocBrowse_OnClick(object sender, RoutedEventArgs e) {
        VistaFolderBrowserDialog folderBrowserDialog = new() {
            Description = Properties.Resources.SaveLoc_Label,
            UseDescriptionForTitle = true
        };

        if (folderBrowserDialog.ShowDialog() != true) return;
        SaveLocTextBox.Text = folderBrowserDialog.SelectedPath;
    }

    public static bool IsBinLocValid(string binLoc) {
        return binLoc != "";
    }

    public static bool IsSaveLocValid(string saveLoc) {
        return Directory.Exists(saveLoc);
    }

    private void SettingsWindow_OnClosing(object sender, CancelEventArgs e) {
        if (!IsSaveLocValid(SaveLocTextBox.Text) || !IsBinLocValid(BinLocTextBox.Text)) {
            e.Cancel = true;
            MessageBox.Show("Please make sure all the fields are set", "Missing fields");
        }
        else {
            var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

            config.AppSettings.Settings["WitcherBinLoc"].Value = BinLocTextBox.Text;
            config.AppSettings.Settings["WitcherSaveLoc"].Value = SaveLocTextBox.Text;

            config.Save(ConfigurationSaveMode.Modified);
            ConfigurationManager.RefreshSection("appSettings");
        }
    }
}