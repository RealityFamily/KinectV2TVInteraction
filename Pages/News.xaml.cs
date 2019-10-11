using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace Microsoft.Samples.Kinect.ControlsBasics.Pages
{
    /// <summary>
    /// Логика взаимодействия для News.xaml
    /// </summary>
    public partial class News : UserControl
    {
        private List<string> ImageList = new List<string>();

        public News(string news_page)
        {
            InitializeComponent();

            Left.Height = Left.Width;
            Right.Height = Right.Width;

            HtmlWeb web = new HtmlWeb();
            var HtmlDoc = web.Load(news_page);
            var HtmlImagesList = HtmlDoc.DocumentNode.SelectSingleNode("//ul[@class='uk-slider-items uk-child-width-1-2 uk-child-width-1-3@s uk-child-width-1-5@m uk-grid']").ChildNodes;

            Title.Text = HtmlDoc.DocumentNode.SelectSingleNode("//h1").InnerText.Trim();
            Content.Text = HtmlDoc.DocumentNode.SelectSingleNode("//div[@class='news-item-text uk-margin-bottom']").InnerText.Trim().Replace("&nbsp;","");

            foreach (var HtmlImage in HtmlImagesList)
            {
                if (HtmlImage.Name == "li") {
                    string ImagePath = "https://www.mirea.ru/" + HtmlImage.SelectSingleNode(".//a").Attributes["href"].Value;
                    ImageList.Add(ImagePath);
                }
            }

            BitmapImage image = new BitmapImage();
            image.BeginInit();
            image.UriSource = new Uri(ImageList[0]);
            image.EndInit();

            CurrentImage.Source = image;

            if (ImageList.Count == 1)
            {
                Right.Visibility = System.Windows.Visibility.Hidden;
            }
        }

        private void Button_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            MainWindow.UIInvoked();
            string URI = CurrentImage.Source.ToString();
            int index = ImageList.IndexOf(URI);

            switch (((Button)sender).Name)
            {
            case "Left":
                if (index - 1 >= 0)
                {
                    BitmapImage image = new BitmapImage();
                    image.BeginInit();
                    image.UriSource = new Uri(ImageList[index - 1]);
                    image.EndInit();

                    CurrentImage.Source = image;

                    Right.Visibility = System.Windows.Visibility.Visible;

                    if (index - 1 == 0)
                    {
                        Left.Visibility = System.Windows.Visibility.Hidden;
                    }
                }
                break;
            case "Right":
                if (index + 1 < ImageList.Count && index > -1)
                {
                    BitmapImage image = new BitmapImage();
                    image.BeginInit();
                    image.UriSource = new Uri(ImageList[index + 1]);
                    image.EndInit();

                    CurrentImage.Source = image;

                    Left.Visibility = System.Windows.Visibility.Visible;

                    if(index + 2 == ImageList.Count)
                    {
                        Right.Visibility = System.Windows.Visibility.Hidden;
                    }
                }
                break;
            }
        }

        private void ScrollViewer_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            MainWindow.UIInvoked();
        }
    }
}
