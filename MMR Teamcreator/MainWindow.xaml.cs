using Microsoft.Win32;
using MMR_Teamcreator.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using DirectoryChecker;
using System.Runtime.InteropServices;
using System.Linq;
using System.Windows.Media.TextFormatting;
using DocumentFormat.OpenXml.Drawing.Diagrams;
using MMR_Teamcreator.Model.Streamers_Clash;
using System.Text;

namespace MMR_Teamcreator
{
    enum ProgramModes : int
    {
        TeamBalancing = 0,
        StreamersClash = 1
    };

    public partial class MainWindow : Window
    {
        string ImportFilePath = string.Empty, ExportFilePath = string.Empty;

        bool _bLaneless;

        int _currentIndex = 0; //0 - Top, 1 - Jungle, 2 - Mid, 3 - ADC, 4 - Support
        public static int PlayersPerTeam = 5;

        List<Player> topList;
        List<Player> jungleList;
        List<Player> midList;
        List<Player> adcList;
        List<Player> supportList;
        List<PlayerLaneless> lanelessList;

        StreamersClashResult StreamerResult;

        ProgramModes currentMode = ProgramModes.TeamBalancing;

        public MainWindow()
        {
            InitializeComponent();
            ResizeMode = ResizeMode.NoResize;
            WindowStartupLocation = WindowStartupLocation.CenterScreen;

            TeamBalancing_Canvas.Visibility = Visibility.Visible;
            StreamersClash_Canvas.Visibility = Visibility.Hidden;

            currentMode = ProgramModes.TeamBalancing;
            SwitchMode_Button.Content = "Switch to StreamersClash";

            #region ListStuff
            topList = new List<Player>();
            jungleList= new List<Player>();
            midList= new List<Player>();
            adcList= new List<Player>();
            supportList= new List<Player>();
            lanelessList = new List<PlayerLaneless>();

            StreamerResult = new StreamersClashResult();
            #endregion
        }

        /// <summary>
        /// This Method is deprecated
        /// </summary>
        [Obsolete("This method is not being used due to different approach", true)]
        private void PlayerSelection()
        {
            return;
            string[] _roles = { "top", "jg", "mid", "adc", "supp" };
            Label _ingameNick = new Label();
            _ingameNick.Content = "Ingame nick";
            _ingameNick.Width = 95; _ingameNick.Height = 30;
            _ingameNick.Margin = new Thickness(-this.Width / 2, -this.Height + 45 * 2, 0, 0);
            MainGrid_Grid.Children.Add(_ingameNick);
            Label _rankLabel = new Label();
            _rankLabel.Content = "Rank";
            _rankLabel.Width = 45; _rankLabel.Height = 30;
            _rankLabel.Margin = new Thickness((_ingameNick.Margin.Left + _ingameNick.Width) + _ingameNick.Width * 1.6f, -this.Height + 45 * 2, 0, 0);
            MainGrid_Grid.Children.Add(_rankLabel);
            Label _twitchNick = new Label();
            _twitchNick.Content = "Twitch nick";
            _twitchNick.Width = 95; _twitchNick.Height = 30;
            _twitchNick.Margin = new Thickness((_rankLabel.Margin.Left + _twitchNick.Width) + _rankLabel.Width * 3, -this.Height + 45 * 2, 0, 0);
            MainGrid_Grid.Children.Add(_twitchNick);
            //Width : 200 - Twitch && Ingame 30 - Rank; Height :  20 each
            _currentIndex = 4;
            while (_currentIndex >= 0)
            {
                for (int i = 1; i <= 16; i++)
                {
                    TextBox Ingame = new TextBox();
                    Ingame.Name = $"Ingame_{_roles[_currentIndex]}_{i}";
                    Ingame.Width = 200; Ingame.Height = 20;
                    Ingame.Margin = new Thickness(-this.Width / 2, (-this.Height / 1.4f) + (45 * (i - 1)), 0, 0);
                    Ingame.MouseDoubleClick += OutputTextBoxName;
                    MainGrid_Grid.Children.Add(Ingame);
                    TextBox Rank = new TextBox();
                    Rank.Name = $"Rank_{_roles[_currentIndex]}_{i}";
                    Rank.Width = 30; Rank.Height = 20;
                    Rank.MaxLength = 2;
                    Rank.Margin = new Thickness((Ingame.Margin.Left + Ingame.Width) + 40, (-this.Height / 1.4f) + (45 * (i - 1)), 0, 0);
                    Rank.MouseDoubleClick += OutputTextBoxName;
                    MainGrid_Grid.Children.Add(Rank);
                    TextBox Twitch = new TextBox();
                    Twitch.Name = $"Twitch_{_roles[_currentIndex]}_{i}";
                    Twitch.Width = 200; Twitch.Height = 20;
                    Twitch.Margin = new Thickness((Rank.Margin.Left + Rank.Width) + 210, (-this.Height / 1.4f) + (45 * (i - 1)), 0, 0);
                    Twitch.MouseDoubleClick += OutputTextBoxName;
                    MainGrid_Grid.Children.Add(Twitch);
                }
                _currentIndex--;
            }
            Button _confirm = new Button();
            _confirm.Name = "Confirm_Button";
            _confirm.Content = "OK";
            _confirm.Width = 60; _confirm.Height = 30;
            _confirm.Margin = new Thickness(this.Width / 2, this.Height - 100, 0, 0);
            /*_confirm.Click += OkButtonClick;*/
            MainGrid_Grid.Children.Add(_confirm);
            _currentIndex = 0;
            this.Title = "Toplane";
        }


        private void OutputTextBoxName(object sender, EventArgs e)
        {
            MessageBox.Show(((TextBox)sender).Name);
        }

        private void SwitchMode_Button_Click(object sender, RoutedEventArgs e)
        {
            ImportFilePath = SelectedFile_TextBlock.Text = "No file selected";
            ExportFilePath = ExportFile_TextBlock.Text = "No file selected";

            // Clean lists
            topList.Clear();
            jungleList.Clear();
            midList.Clear();
            adcList.Clear();
            supportList.Clear();
            lanelessList.Clear();

            StreamerResult.TopLaners.Clear();
            StreamerResult.Junglers.Clear();
            StreamerResult.Midlaners.Clear();
            StreamerResult.ADCarries.Clear();
            StreamerResult.Supports.Clear();



            if (currentMode == ProgramModes.TeamBalancing)
            {
                // Switch to Streamers Clash

                StreamersClash_Canvas.Visibility = Visibility.Visible;
                TeamBalancing_Canvas.Visibility = Visibility.Hidden;

                currentMode = ProgramModes.StreamersClash;
                SwitchMode_Button.Content = "Switch to Team Balancing";
            }
            else if (currentMode == ProgramModes.StreamersClash)
            {
                // Switch to Team Balancing
                StreamersClash_Canvas.Visibility = Visibility.Hidden;
                TeamBalancing_Canvas.Visibility = Visibility.Visible;

                currentMode = ProgramModes.TeamBalancing;
                SwitchMode_Button.Content = "Switch to StreamersClash";
            }
        }

        #region TeamBalancing_region
        private void BalanceTeams_Button_Click(object sender, RoutedEventArgs e)
        {
            if (_bLaneless)
            {
                MessageBox.Show("", "", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            bool randomPlayers = false;
            if(ImportFilePath == String.Empty)
            {
                MessageBoxResult res = MessageBox.Show("No file was imported, do you want to create random players?", "NO FILE FOUND", MessageBoxButton.YesNo);
                if(res == MessageBoxResult.No)
                    return;

                randomPlayers = true;
            }
            int count = 0;

            if((count = (topList.Count + jungleList.Count + midList.Count + adcList.Count + supportList.Count)) %5 != 0 && randomPlayers == false)
            {
                MessageBox.Show($"Not all roles were fullfilled!\nActual player count: {count}");
                return;
            }

            TeamMethods.BalanceTeams(topList, jungleList, midList, adcList, supportList, randomPlayers, true);

            string path = ExportFilePath = $@"{DirectoryMethods.RemovePathPart(Environment.CurrentDirectory, 3)}\RFPExcelAPI\RawPlayerDataExport.txt";

            path = (path.Length >= 38) ? $"{HelpMetods.StringRange(path, new int[2] { 0, 42 })}" : path;

            ExportFile_TextBlock.Text = $"{path}";
        }

        private void BalanceLanelessTeams_Button_Click(object sender, RoutedEventArgs e)
        {
            if (!_bLaneless)
            {
                MessageBox.Show("", "", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            bool randomPlayers = false;
            if(ImportFilePath == String.Empty)
            {
                MessageBoxResult res = MessageBox.Show("No file was imported, do you want to create random players?", "NO FILE FOUND", MessageBoxButton.YesNo);
                if (res == MessageBoxResult.No)
                    return;
                randomPlayers = true;
            }

            int count = 0;

            if((count = lanelessList.Count) % PlayersPerTeam != 0 && randomPlayers == false)
            {
                MessageBox.Show($"Not all roles were fullfilled!\nActual player count: {count}");
                return;
            }

            TeamLanelessMethods.BalanceTeams(lanelessList, randomPlayers);
        }


        private void Load_Button_Click(object sender, RoutedEventArgs e)
        {
            topList = new List<Player>();
            jungleList = new List<Player>();
            midList = new List<Player>();
            adcList = new List<Player>();
            supportList = new List<Player>();

            string path = ImportFilePath = $@"{Environment.CurrentDirectory}\RFPExcelAPI\RawPlayerDataImport.txt";
            path = (path.Length >= 38) ? $"{HelpMetods.StringRange(path, new int[2] { 0, 42 })}" : path;
            SelectedFile_TextBlock.Text = path;

            if (ImportFilePath == String.Empty)
            {
                MessageBox.Show("No file was selected!", "File missing", MessageBoxButton.OK);
                return;
            }

            try
            {
                string[] players = File.ReadAllLines(ImportFilePath);

                for (int i = 0; i < 80; i++)
                {
                    string[] player = players[i].Split(':'); // [0] ingame [1] rank [2] twitch

                    if (i < 16)
                        topList.Add(new Player("Top", player[2], player[1], player[0]));
                    else if (i >= 16 && i < 32)
                        jungleList.Add(new Player("Jungle", player[2], player[1], player[0]));
                    else if (i >= 32 && i < 48)
                        midList.Add(new Player("Mid", player[2], player[1], player[0]));
                    else if (i >= 48 && i < 64)
                        adcList.Add(new Player("ADC", player[2], player[1], player[0]));
                    else if (i >= 64)
                        supportList.Add(new Player("Support", player[2], player[1], player[0]));
                }

                _bLaneless = false;
                
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occured\n{ex.Message}", "ERROR", MessageBoxButton.OK);
            }
        }

        private void LoadLaneless_Button_Click(object sender, RoutedEventArgs e)
        {
            lanelessList = new List<PlayerLaneless>();

            try
            {
                if(Directory.Exists($@"{Environment.CurrentDirectory}\ClassicTeams"))
                {
                    string path = ImportFilePath = $@"{Environment.CurrentDirectory}\ClassicTeams\players.txt";
                    path = (path.Length >= 38) ? $"{HelpMetods.StringRange(path, new int[2] { 0, 42 })}" : path;
                    SelectedFile_TextBlock.Text = path;

                    if (ImportFilePath == String.Empty)
                    {
                        MessageBox.Show("No file was selected!", "File missing");
                        return;
                    }

                    string[] players = File.ReadAllLines(ImportFilePath);

                    if (players[0].ToUpper().StartsWith("PER="))
                    {
                        PlayersPerTeam = int.Parse(players[0].ToUpper().Split(new string[] { "PER=" }, StringSplitOptions.None)[1]);

                        string[] pom = new string[players.Length-1];

                        //Debug.WriteLine($"pom length = {pom.Length}\nplayers length = {players.Length}");

                        for(int i = 0; i < pom.Length; i++)
                        {
                            //Debug.WriteLine($"pom[{i}] = players[{i+1}]");
                            pom[i] = players[i + 1];
                        }
                        players = pom;
                    }

                    for (int i = 0; i < players.Length; i++)
                    {
                        string[] player = players[i].Split(':');

                        player[1] = player[1] == "M" ? "MA" : player[1];
                        lanelessList.Add(new PlayerLaneless(player[2], player[1], player[0]));
                    }
                    _bLaneless = true;
                    return;
                }
                Directory.CreateDirectory($@"{Environment.CurrentDirectory}\ClassicTeams");
                throw new Exception("No file was found!");
            }
            catch(Exception ex)
            {
                MessageBox.Show($"An error occured\nMESSAGE:{ex.Message}\nSTACK:{ex.StackTrace}", "ERROR", MessageBoxButton.OK);
            }
        }

        private void SelectedFile_TextBlock_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (HelpMetods.IsPath(SelectedFile_TextBlock.Text))
            {
                MessageBoxResult res = MessageBox.Show($"Full path is\n{SelectedFile_TextBlock.Text}\nCopy to clipboard?", "Loaded path", MessageBoxButton.YesNo);
                if (res == MessageBoxResult.Yes)
                    Clipboard.SetText(ImportFilePath);
            }
            else
                MessageBox.Show("No path selected yet", "Loaded path");
        }

        private void ExportFile_TextBlock_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (HelpMetods.IsPath(ExportFile_TextBlock.Text))
            {
                MessageBoxResult res = MessageBox.Show($"Full path is\n{ExportFile_TextBlock.Text}\nCopy to clipboard?", "Loaded path", MessageBoxButton.YesNo);
                if (res == MessageBoxResult.Yes)
                    Clipboard.SetText(ExportFilePath);
            }
            else
                MessageBox.Show("No path selected yet", "Loaded path");
        }

        private void ImportFromExcel_Button_Click(object sender, RoutedEventArgs e)
        {
            topList = new List<Player>();
            jungleList = new List<Player>();
            midList = new List<Player>();
            adcList = new List<Player>();
            supportList = new List<Player>(); 
            
            try
            {
                if (!File.Exists($@"{Environment.CurrentDirectory}\RFPExcelAPI\RFPExcelAPIImport.py"))
                {
                    MessageBox.Show("Python script is missing!");
                    return;
                }

                Process prg = Process.Start($@"{Environment.CurrentDirectory}\RFPExcelAPI\RFPExcelAPIImport.py");
                prg.WaitForExit();
                MessageBox.Show("Excel import script has finished!");

                string path = ImportFilePath = $@"{Environment.CurrentDirectory}\RFPExcelAPI\RawPlayerDataImport.txt";

                if(!File.Exists(path))
                {
                    MessageBox.Show("File was not created!", "ERROR", MessageBoxButton.OK);
                }
                path = (path.Length >= 38) ? $"{HelpMetods.StringRange(path, new int[2] { 0, 42 })}" : path;
                SelectedFile_TextBlock.Text = path;

                try
                {
                    string[] players = File.ReadAllLines(ImportFilePath);

                    int teamIndexing = (players.Length / 5);

                    for (int i = 0; i < players.Length; i++)
                    {
                        string[] player = players[i].Split(':'); // [0] ingame [1] rank [2] twitch

                        if (i < teamIndexing)
                            topList.Add(new Player("Top", player[2], player[1], player[0]));
                        else if (i >= teamIndexing && i < teamIndexing * 2)
                            jungleList.Add(new Player("Jungle", player[2], player[1], player[0]));
                        else if (i >= teamIndexing * 2 && i < teamIndexing * 3)
                            midList.Add(new Player("Mid", player[2], player[1], player[0]));
                        else if (i >= teamIndexing * 3 && i < teamIndexing * 4)
                            adcList.Add(new Player("ADC", player[2], player[1], player[0]));
                        else if (i >= teamIndexing * 4)
                            supportList.Add(new Player("Support", player[2], player[1], player[0]));
                    }

                }
                catch (Exception ex)
                {
                    MessageBox.Show($"An error occured\n{ex.Message}", "ERROR", MessageBoxButton.OK);
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occured!\n{ex.Message}", "ERROR", MessageBoxButton.OK);
            }
        }

        private void Upload_Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!File.Exists($@"{ExportFilePath}"))
                {
                    MessageBox.Show("No export file was found, use Team balancing first!");
                    return;
                }
                if(!File.Exists($@"{Environment.CurrentDirectory}\RFPExcelAPI\RFPExcelAPIExport.py"))
                {
                    MessageBox.Show("Python script was not found!");
                    return;
                }

                Process prg = Process.Start($@"{Environment.CurrentDirectory}\RFPExcelAPI\RFPExcelAPIExport.py");
                prg.WaitForExit();
                MessageBoxResult res = MessageBox.Show("Uploading script has finished!\nWould you like to exit the app?", "Uploading finished!", MessageBoxButton.YesNo);
                if (res == MessageBoxResult.Yes)
                    Application.Current.Shutdown();
                
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occured\n{ex.Message}", "ERROR", MessageBoxButton.OK);
            }
            //TODO
            //run Python script
        }

        #endregion

        #region StreamersClash
        private void LoadStreamersClash_Button_Click(object sender, RoutedEventArgs e)
        {
            List<StreamersClashPlayer>  streamerTopList     = new List<StreamersClashPlayer>();
            List<StreamersClashPlayer>  streamerJungleList  = new List<StreamersClashPlayer>();
            List<StreamersClashPlayer>  streamerMidList     = new List<StreamersClashPlayer>();
            List<StreamersClashPlayer>  streamerADCList     = new List<StreamersClashPlayer>();
            List<StreamersClashPlayer>  streamerSupportList = new List<StreamersClashPlayer>();

            if (!Directory.Exists($@"{Environment.CurrentDirectory}\StreamersClash"))
                Directory.CreateDirectory($@"{Environment.CurrentDirectory}\StreamersClash");

            string playersPath = ImportFilePath = $@"{Environment.CurrentDirectory}\StreamersClash\players.txt";
            string captainsPath = $@"{Environment.CurrentDirectory}\StreamersClash\captains.txt";

            SelectedFile_TextBlock.Text = (playersPath.Length >= 38) ? $"{HelpMetods.StringRange(playersPath, new int[2] { 0, 42 })}" : playersPath;

            if (ImportFilePath == String.Empty)
            {
                MessageBox.Show("No file was imported", "NO FILE FOUND", MessageBoxButton.OK);
                return;
            }

            string[] content = File.ReadAllLines(playersPath);

            for(int i = 0; i < content.Length; i++)
            {
                // Twitch_Nick:INGAME_NICK:ROLE:SECONDARY_ROLE:RANK
                string[] split = content[i].Split(';');

                StreamersClashPlayer current = new StreamersClashPlayer(role: split[2], secondaryRole: split[3], 
                    twitch: split[0], rank: split[4], ingame: split[1], isCaptain: false);

                Tuple<Roles, Roles> rolesTuple = new Tuple<Roles, Roles>(current.Role, current.SecondaryRole);

                if(rolesTuple.Item1 == Roles.Top || rolesTuple.Item2 == Roles.Top)
                {
                    streamerTopList.Add(current);
                }
                if(rolesTuple.Item1 == Roles.Jungle || rolesTuple.Item2 == Roles.Jungle)
                {
                    streamerJungleList.Add(current);
                }
                if (rolesTuple.Item1 == Roles.Mid || rolesTuple.Item2 == Roles.Mid)
                {
                    streamerMidList.Add(current);
                }
                if (rolesTuple.Item1 == Roles.ADC || rolesTuple.Item2 == Roles.ADC)
                {
                    streamerADCList.Add(current);
                }
                if (rolesTuple.Item1 == Roles.Support || rolesTuple.Item2 == Roles.Support)
                {
                    streamerSupportList.Add(current);
                }
            }

            string[] captainContent = File.ReadAllLines(captainsPath);

            for (int i = 0; i < captainContent.Length; i++)
            {
                // Twitch_Nick:INGAME_NICK:ROLE:RANK
                string[] split = captainContent[i].Split(':');

                StreamersClashPlayer current = new StreamersClashPlayer(role: split[2], twitch: split[0], rank: split[3], ingame: split[1], isCaptain: true);

                switch (current.Role)
                {
                    case Roles.Top:
                        {
                            streamerTopList.Add(current);
                            break;
                        }
                    case Roles.Jungle:
                        {
                            streamerJungleList.Add(current);
                            break;
                        }
                    case Roles.Mid:
                        {
                            streamerMidList.Add(current);
                            break;
                        }
                    case Roles.ADC:
                        {
                            streamerADCList.Add(current);
                            break;
                        }
                    case Roles.Support:
                        {
                            streamerSupportList.Add(current);
                            break;
                        }
                }
            }

            StreamerResult = new StreamersClashResult(streamerTopList, streamerJungleList, streamerMidList, streamerADCList, streamerSupportList);
        }

        private void GenerateStreamersClashFiles_Button_Click(object sender, RoutedEventArgs e)
        {
            if (!Directory.Exists($@"{Environment.CurrentDirectory}\StreamersClash"))
                Directory.CreateDirectory($@"{Environment.CurrentDirectory}\StreamersClash");
            string playersPath = ImportFilePath = $@"{Environment.CurrentDirectory}\StreamersClash\Outplayers.txt";
            string captainsPath = $@"{Environment.CurrentDirectory}\StreamersClash\Outcaptains.txt";

            string[] playersOut = StreamerResult.GetOutput();
            string[] captainsOut = StreamerResult.GetCaptainOutput();

            File.WriteAllLines(playersPath, playersOut, encoding: Encoding.UTF8);
            File.WriteAllLines(captainsPath, captainsOut, encoding: Encoding.UTF8);

            MessageBox.Show("Výsledky zapsány!");
        }
        #endregion
    }
}