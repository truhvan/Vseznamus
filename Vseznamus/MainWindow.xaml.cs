using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json.Linq;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;

namespace Vseznamus
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        static readonly HttpClient client = new HttpClient();
        static String lang = "ru.";
        static string[] ids;
        static String link = "wikipedia.org";
        static JObject randomizedJson;
        static JObject introJson;
        void OptionButtonsVisibility(Visibility visibility)
        {
            Option1Button.Visibility = visibility;
            Option2Button.Visibility = visibility;
            Option3Button.Visibility = visibility;
            Option4Button.Visibility = visibility;
        }

        void OptionButtonsNameChange(JObject randomizedParsedJson)
        {
            Option1Button.Content = randomizedJson["query"]["random"][0]["title"];
            Option2Button.Content = randomizedJson["query"]["random"][1]["title"];
            Option3Button.Content = randomizedJson["query"]["random"][2]["title"];
            Option4Button.Content = randomizedJson["query"]["random"][3]["title"];
            Option1Button.ToolTip = randomizedJson["query"]["random"][0]["title"];
            Option2Button.ToolTip = randomizedJson["query"]["random"][1]["title"];
            Option3Button.ToolTip = randomizedJson["query"]["random"][2]["title"];
            Option4Button.ToolTip = randomizedJson["query"]["random"][3]["title"];
            Option1Button.Tag = randomizedJson["query"]["random"][0]["id"];
            Option2Button.Tag = randomizedJson["query"]["random"][1]["id"];
            Option3Button.Tag = randomizedJson["query"]["random"][2]["id"];
            Option4Button.Tag = randomizedJson["query"]["random"][3]["id"];
        }


        void GrayTheButtons()
        {
            Option1Button.IsEnabled = !Option1Button.IsEnabled;
            Option2Button.IsEnabled = !Option2Button.IsEnabled;
            Option3Button.IsEnabled = !Option3Button.IsEnabled;
            Option4Button.IsEnabled = !Option4Button.IsEnabled;
            RandomizeButton.IsEnabled = !RandomizeButton.IsEnabled;
        }

        void ShowInfo(string id)
        {
            InfoTextBox.Text = introJson["query"]["pages"][id]["extract"].ToString();
        }

        public MainWindow()
        {
            InitializeComponent();
            OptionButtonsVisibility(Visibility.Hidden);
        }

        async private void RandomizeButton_Click(object sender, RoutedEventArgs e)
        {
            InfoTextBox.Text = "Загрузка...";
            GrayTheButtons();
            HttpResponseMessage response = await client.GetAsync("https://"+lang+link+"/w/api.php?action=query&list=random&rnnamespace=0&rnlimit=4&format=json");
            response.EnsureSuccessStatusCode();
            string responseBody = await response.Content.ReadAsStringAsync();
            string responseBodyUnescaped = Regex.Unescape(responseBody);
            randomizedJson = JObject.Parse(responseBodyUnescaped);
            string[] ids = { randomizedJson["query"]["random"][0]["id"].ToString() + "|", randomizedJson["query"]["random"][1]["id"].ToString() + "|", randomizedJson["query"]["random"][2]["id"].ToString()+"|", randomizedJson["query"]["random"][3]["id"].ToString() };
            HttpResponseMessage response2 = await client.GetAsync("https://" + lang + link + "/w/api.php?action=query&prop=extracts&pageids="+ ids[0] + ids[1] + ids[2] + ids[3] + "&explaintext=true&exintro=true&format=json");
            response2.EnsureSuccessStatusCode();
            string responseBody2 = await response2.Content.ReadAsStringAsync();
            string responseBody2Unescaped = Regex.Unescape(responseBody2);
            string secondLink = "https://" + lang + link + "/w/api.php?action=query&prop=extracts&pageids=" + ids[0] + ids[1] + ids[2] + ids[3] + "&explaintext=true&exintro=true&format=json";
            introJson = JObject.Parse(responseBody2Unescaped);
            GrayTheButtons();
            InfoTextBox.Text = responseBody2Unescaped;
            OptionButtonsVisibility(Visibility.Visible);
            OptionButtonsNameChange(randomizedJson);
            InfoTextBox.Text = "Нажмите на интересующий вас предмет!";
        }

        private void Option1Button_Click(object sender, RoutedEventArgs e)
        {
            ShowInfo(Option1Button.Tag.ToString());
        }

        private void Option2Button_Click(object sender, RoutedEventArgs e)
        {
            ShowInfo(Option2Button.Tag.ToString());
        }

        private void Option3Button_Click(object sender, RoutedEventArgs e)
        {
            ShowInfo(Option3Button.Tag.ToString());
        }

        private void Option4Button_Click(object sender, RoutedEventArgs e)
        {
            ShowInfo(Option4Button.Tag.ToString());
        }
    }
}
