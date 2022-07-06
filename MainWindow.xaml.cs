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
using System.Windows.Documents;

namespace Witcher3SaveToggle;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow {
	private ObservableCollection<User> Users;

	public MainWindow() {
		InitializeComponent();

		if (!SettingsWindow.IsBinLocValid(ConfigurationManager.AppSettings["WitcherBinLoc"]) ||
			!SettingsWindow.IsSaveLocValid(ConfigurationManager.AppSettings["WitcherSaveLoc"])) {
			new SettingsWindow().ShowDialog();
		}

		Users = new ObservableCollection<User>(LoadUsersFromJson());

		UserGrid.ItemsSource = Users;

		//for (var i = 0; i < Users.Count; i++) {
		//	var row = new TableRow();

		//	row.Cells.Add(new TableCell(new Paragraph(new Run(Users[i].DisplayName))));

		//	var userSelectButton = new Button() {
		//		Content = "Select",
		//		Tag = Users[i].CodeName
		//	};
		//	if (i == 0) {
		//		userSelectButton.SetValue(NameProperty, "button");
		//	}
		//	userSelectButton.Click += UserSelectButton_OnClick;

		//	row.Cells.Add(new TableCell(new BlockUIContainer(userSelectButton)));

		//	var userDeleteButton = new Button() {
		//		Content = "Delete",
		//		Tag = Users[i].CodeName
		//	};
		//	userSelectButton.Click += UserDeleteButton_OnClick;

		//	row.Cells.Add(new TableCell(new BlockUIContainer(userDeleteButton)));

		//	UserTableRowGroup.Rows.Add(row);
		//}
	}

	private static List<User> LoadUsersFromJson() {
		if (!File.Exists(ConfigurationManager.AppSettings["WitcherSaveLoc"] + "\\w3st.json"))
			return new List<User>();

		var jsonString = File.ReadAllText(ConfigurationManager.AppSettings["WitcherSaveLoc"] + "\\w3st.json");
		var users = JsonSerializer.Deserialize<List<User>>(jsonString);

		return users ?? new List<User>();
	}

	private static void StartWitcherBin() {
		Process myProcess = new();

		try {
			myProcess.StartInfo.UseShellExecute = true;
			myProcess.StartInfo.FileName = ConfigurationManager.AppSettings["WitcherBinLoc"];
			myProcess.StartInfo.CreateNoWindow = false;
			myProcess.Start();
		} catch (Exception e) {
			Console.WriteLine(e.Message);
		}
	}

	private void SwitchSave(int whois) {
		var savePath = ConfigurationManager.AppSettings["WitcherSaveLoc"];

		if (Directory.Exists(savePath)) {
			var activeUser = 0;
			for (var i = 0; i < Users.Count; i++) {
				if (!Users[i].IsActive) continue;

				activeUser = i;
				break;
			}

			if (!Directory.GetDirectories(savePath).Contains(savePath + "\\gamesaves." + Users[whois].CodeName) &&
				!Users[whois].IsActive) {
				Directory.CreateDirectory(savePath + "\\gamesaves." + Users[whois].CodeName);
			}

			Directory.Move(savePath + "\\gamesaves", savePath + "\\gamesaves." + Users[activeUser].CodeName);
			Directory.Move(savePath + "\\gamesaves." + Users[whois].CodeName, savePath + "\\gamesaves");

			Users[activeUser].IsActive = false;
			Users[whois].IsActive = true;

			StartWitcherBin();
			Close();
		} else {
			var result = MessageBox.Show("Start Witcher 3 regardless?", "Witcher 3 save data not found",
				MessageBoxButton.YesNo);

			if (result == MessageBoxResult.Yes) StartWitcherBin();
			else Close();
		}
	}

	private void UserSelectButton_OnClick(object sender, RoutedEventArgs e) {
		if (sender is not Button { Tag: User user }) return;
		SwitchSave(Users.IndexOf(user));
	}

	private void UserDeleteButton_OnClick(object sender, RoutedEventArgs e) {
		if (sender is not Button { Tag: User user }) return;

		var result = MessageBox.Show("Are you sure you want to delete " + user.DisplayName + "?",
			"Delete user " + user.DisplayName + "?", MessageBoxButton.YesNo);
		if (result == MessageBoxResult.Yes) Users.Remove(user);
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
			MessageBox.Show("WitcherSaveLoc is invalid", "WitcherSaveLoc is invalid");
		}
	}

	private void MainWindow_OnClosing(object sender, CancelEventArgs e) {
		File.WriteAllText(ConfigurationManager.AppSettings["WitcherSaveLoc"] + "\\w3st.json",
			JsonSerializer.Serialize(Users));
	}

	private void AddUser_OnClick(object sender, RoutedEventArgs e) {
		new AddUserWindow().ShowDialog();
	}

    private void UserGrid_AddingNewItem(object sender, AddingNewItemEventArgs e) {

    }
}