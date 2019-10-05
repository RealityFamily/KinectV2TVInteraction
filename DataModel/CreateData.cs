using System;
using System.IO;
using Microsoft.Samples.Kinect.ControlsBasics.Pages;
using HtmlAgilityPack;

namespace Microsoft.Samples.Kinect.ControlsBasics.DataModel
{
    public class CreateData
    {
        public static void GetAllVideos() {
            string fullPath = AppDomain.CurrentDomain.BaseDirectory + @"Videos\";
            string[] AllFiles = Directory.GetFiles(fullPath);

            SampleDataCollection video_group = new SampleDataCollection(
                "Video",
                "Видео",
                SampleDataCollection.GroupType.Video);

            int i = 0;
            foreach (var video in AllFiles)
            {
                video_group.Items.Add(new SampleDataItem(
                    "Video-" + i.ToString(),
                    Path.GetFileNameWithoutExtension(video),
                    string.Empty,
                    SampleDataItem.TaskType.Page,
                    typeof(VideoPage),
                    SampleDataSource.StringToArr(video)));
                i++;
            }

            SampleDataSource.AddToGroups(video_group);
        }

        public static void GetNewsFromSite()
        {
            string URI = "https://www.mirea.ru/news/";

            SampleDataCollection news_group = new SampleDataCollection(
                "News",
                "Новости",
                SampleDataCollection.GroupType.News);

            HtmlWeb web = new HtmlWeb();
            var HtmlDoc = web.Load(URI);
            var NewsList = HtmlDoc.DocumentNode.SelectSingleNode("//div[@id='page-wrapper']/div[2]/div/div[2]/div/div[2]/div[1]/div[1]").ChildNodes;

            int i = 0;
            string name;
            string source;
            string news_page;
            foreach (var News in NewsList)
            {
                if (News.Name == "div") {
                    name = News.SelectSingleNode(".//a[@class='uk-link-reset']").InnerText.Trim();
                    source = "https://www.mirea.ru/" + News.SelectSingleNode(".//img[@class='uk-transition-scale-up uk-transition-opaque']").Attributes["src"].Value;
                    news_page = "https://www.mirea.ru/" + News.SelectSingleNode(".//a[@class='uk-link-reset']").Attributes["href"].Value;

                    news_group.Items.Add(new SampleDataItem(
                        "News-" + i.ToString(),
                        name,
                        source,
                        SampleDataItem.TaskType.Page,
                        typeof(News),
                        news_page));
                    i++;
                }

            }

            SampleDataSource.AddToGroups(news_group);
        }

        public static void GetGames()
        {
            string fullPath = AppDomain.CurrentDomain.BaseDirectory + @"Games\";
            string[] AllDir = Directory.GetDirectories(fullPath);

            SampleDataCollection games_group = new SampleDataCollection(
                "Games",
                "Игры",
                SampleDataCollection.GroupType.Video);

            int i = 0;
            foreach (var Game in AllDir)
            {
                string filePath = "";
                foreach (var file in Directory.GetFiles(Game))
                {
                    if (Path.GetExtension(file) == ".exe")
                    {
                        filePath = file;
                    }
                }

                games_group.Items.Add(new SampleDataItem(
                    "Game-" + i.ToString(),
                    new DirectoryInfo(Game).Name,
                    string.Empty,
                    SampleDataItem.TaskType.Execute,
                    typeof(VideoPage),
                    SampleDataSource.StringToArr(filePath)));
                i++;
            }

            SampleDataSource.AddToGroups(games_group);
        }
    }
}
