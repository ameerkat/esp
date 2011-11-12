using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Maps.MapControl.WPF;
using Tweet;
using Tweet.Model;

namespace TwitterMap
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MapLayer m_TextLayer;
        public MapLayer m_PushpinLayer;
        private Dictionary<string, string> twitterOptions;
        private TweetProducer twitterProducer;
        private List<TweetConsumer> twitterConsumers;

        public MainWindow()
        {
            InitializeComponent();
            base.Loaded += OnLoaded;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            base.Loaded -= OnLoaded;

            m_TextLayer = new MapLayer();
            m_PushpinLayer = new MapLayer();
            x_Map.Children.Add(m_PushpinLayer);
            x_Map.Children.Add(m_TextLayer);

            twitterOptions = new Dictionary<string, string>();
            twitterOptions.Add("streamUrl", Options.twitterStreamUrl);
            twitterOptions.Add("username", Options.twitterUsername);
            twitterOptions.Add("password", Options.twitterPassword);

            twitterProducer = new TweetProducer(twitterOptions);
            twitterConsumers = new List<TweetConsumer>();

            List<String> defaultKeywords = new List<String>(){"fire", "quake", "flood"};
            keywords.Text = String.Join(", ", defaultKeywords);
            searchKeywords(defaultKeywords);
        }

        private void submit_Click(object sender, RoutedEventArgs e)
        {
            List<String> keywordsList = new List<String>();
            foreach (String keyword in keywords.Text.Split(','))
            {
                String trimmedKeyword = keyword.Trim();
                if (!String.IsNullOrEmpty(trimmedKeyword))
                {
                    keywordsList.Add(trimmedKeyword);
                }
            }
            searchKeywords(keywordsList);
        }

        private void searchKeywords(List<String> keywordsList)
        {
            this.m_PushpinLayer.Children.Clear();
            this.m_TextLayer.Children.Clear();

            if (twitterProducer != null)
            {
                twitterProducer.Stop();
            }

            twitterProducer.SetKeywords(keywordsList);
            twitterProducer.Start();

            foreach (TweetConsumer consumer in twitterConsumers)
            {
                consumer.Stop();
            }

            twitterConsumers.Clear();
            for (int i = 0; i < Options.numberThreads; i++)
            {
                TweetConsumer consumer = new TweetConsumer(twitterProducer, i, this);
                consumer.Start();
                twitterConsumers.Add(consumer);
            }

        }
    }
}
