using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.Json;
using ModernWpf.Controls;

namespace Witcher3SaveToggle;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow {
	private ObservableCollection<User> Users;

	public MainWindow() {
		InitializeComponent();

		//Open settings if the required settings are not set
		if (!SettingsWindow.IsBinLocValid(ConfigurationManager.AppSettings["WitcherBinLoc"]) ||
			!SettingsWindow.IsSaveLocValid(ConfigurationManager.AppSettings["WitcherSaveLoc"])) {
			new SettingsWindow().ShowDialog();
		}

		//Load users and pass them to the UserGrid
		Users = new ObservableCollection<User>(LoadUsersFromJson());
		UserGrid.ItemsSource = Users;

	}


	private static List<User> LoadUsersFromJson() {
		//If the json file is missing return an empty list
		if (!File.Exists(ConfigurationManager.AppSettings["WitcherSaveLoc"] + "\\w3st.json"))
			return new List<User>();

		//Read the file and decode the json
		var jsonString = File.ReadAllText(ConfigurationManager.AppSettings["WitcherSaveLoc"] + "\\w3st.json");
		var users = JsonSerializer.Deserialize<List<User>>(jsonString);

		//If the deserialization failed return empty list
		return users ?? new List<User>();
	}

	private static void StartWitcherBin() {
		var processStartInfo = new ProcessStartInfo {
			FileName = ConfigurationManager.AppSettings["WitcherBinLoc"],
			WorkingDirectory = Path.GetDirectoryName(ConfigurationManager.AppSettings["WitcherBinLoc"])
		};
		Process.Start(processStartInfo);
	}

	//Enable the save that is 
	private void SwitchSave(User userToSelect) {
		var savePath = ConfigurationManager.AppSettings["WitcherSaveLoc"];

		if (Directory.Exists(savePath)) {
			User activeUser = Users.Where(user => user.IsActive).FirstOrDefault();

			if (!Directory.GetDirectories(savePath).Contains(savePath + "\\gamesaves." + userToSelect.CodeName) &&
				!userToSelect.IsActive) {
				Directory.CreateDirectory(savePath + "\\gamesaves." + userToSelect.CodeName);
			}

			Directory.Move(savePath + "\\gamesaves", savePath + "\\gamesaves." + activeUser.CodeName);
			Directory.Move(savePath + "\\gamesaves." + userToSelect.CodeName, savePath + "\\gamesaves");

			activeUser.IsActive = false;
			userToSelect.IsActive = true;

			StartWitcherBin();
			Close();
		} else {
			var result = MessageBox.Show("Start Witcher 3 regardless?", "Witcher 3 save data not found", MessageBoxButton.YesNo);

			if (result == MessageBoxResult.Yes) StartWitcherBin();
			else Close();
		}
	}

	private void UserSelectButton_OnClick(object sender, RoutedEventArgs e) {
		if (sender is not Button { Tag: User user }) return;
		SwitchSave(user);
	}

	private void OpenSettings_OnClick(object sender, RoutedEventArgs e) {
		new SettingsWindow().ShowDialog();
	}

	private void TestJSON_OnClick(object sender, RoutedEventArgs e) {
		if (Directory.Exists(ConfigurationManager.AppSettings["WitcherSaveLoc"])) {
			var newUsers = new List<User> {
				new("k8", "k8", true),
				new("boopsnoot", "boopsnoot", false)
			};

			File.WriteAllText(ConfigurationManager.AppSettings["WitcherSaveLoc"] + "\\w3st.json",
				JsonSerializer.Serialize(newUsers));
		} else {
			new ContentDialog {
				Title = "WitcherSaveLoc is invalid",
				Content = "WitcherSaveLoc is invalid",
				PrimaryButtonText = "Exit"
			}.ShowAsync();
		}
	}

	private void MainWindow_OnClosing(object sender, CancelEventArgs e) {
		File.WriteAllText(ConfigurationManager.AppSettings["WitcherSaveLoc"] + "\\w3st.json",
			JsonSerializer.Serialize(Users));
	}

	private void AddUser_OnClick(object sender, RoutedEventArgs e) {
		new AddUserWindow().ShowDialog();
	}
}