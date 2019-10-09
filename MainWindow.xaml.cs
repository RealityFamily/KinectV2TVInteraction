//------------------------------------------------------------------------------
// <copyright file="MainWindow.xaml.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace Microsoft.Samples.Kinect.ControlsBasics
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Windows;
    using System.Windows.Automation.Peers;
    using System.Windows.Automation.Provider;
    using System.Windows.Controls;
    using Microsoft.Kinect;
    using Microsoft.Kinect.Wpf.Controls;
    using Microsoft.Samples.Kinect.ControlsBasics.DataModel;

    /// <summary>
    /// Interaction logic for MainWindow
    /// </summary>
    public partial class MainWindow
    {
        private bool IsPlaying;

        public static List<string> history = new List<string>();
        public static ContentControl var_navigationRegion;

        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindow"/> class. 
        /// </summary>
        public MainWindow()
        {
            this.InitializeComponent();

            var_navigationRegion = navigationRegion;

            CreateData.GetAllVideos();
            CreateData.GetNewsFromSite();
            CreateData.GetGames();

            KinectRegion.SetKinectRegion(this, kinectRegion);

            App app = ((App)Application.Current);
            app.KinectRegion = kinectRegion;

            // Use the default sensor
            this.kinectRegion.KinectSensor = KinectSensor.GetDefault();

            //// Add in display content
            var sampleDataSource = SampleDataSource.GetGroup("Menu");
            history.Add("Menu");
            this.itemsControl.ItemTemplate = (DataTemplate) this.FindResource(sampleDataSource.TypeGroup + "Template");
            this.itemsControl.ItemsSource = sampleDataSource;

            BodyFrameReader bodyFrameReader = this.kinectRegion.KinectSensor.BodyFrameSource.OpenReader();
            bodyFrameReader.FrameArrived += BodyFrameReader_FrameArrived;

            //BackgroungVideo.Source = new Uri(SampleDataSource.GetItem("Video-Main").Parametrs[0].ToString());
            //BackgroungVideo.MediaEnded += BackgroungVideo_MediaEnded;
            //IsPlaying = true;
            //BackgroungVideo.Play();
        }

        private void BackgroungVideo_MediaEnded(object sender, RoutedEventArgs e)
        {
            BackgroungVideo.Stop();
            IsPlaying = true;
            BackgroungVideo.Play();
        }

        private void BodyFrameReader_FrameArrived(object sender, BodyFrameArrivedEventArgs e)
        {
            using(BodyFrame bodyFrame = e.FrameReference.AcquireFrame())
            {
                if (bodyFrame != null)
                {
                    if (bodyFrame.BodyCount > 0)
                    {
                        if (IsPlaying)
                        {
                            MenuButton.Visibility = Visibility.Visible;
                        }
                    } else
                    {
                        if (IsPlaying)
                        {
                            MenuButton.Visibility = Visibility.Collapsed;
                            BackgroungVideo.Visibility = Visibility.Visible;
                            BackgroungVideo.Play();
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Handle a button click from the wrap panel.
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        private void ButtonClick(object sender, RoutedEventArgs e)
        {            
            var button = (Button)e.OriginalSource;
            SampleDataItem sampleDataItem = button.DataContext as SampleDataItem;

            if (sampleDataItem != null)
            {
                if (sampleDataItem.Task == SampleDataItem.TaskType.Page && sampleDataItem.NavigationPage != null)
                {
                    history.Add(sampleDataItem.UniqueId);
                    backButton.Visibility = System.Windows.Visibility.Visible;
                    navigationRegion.Content = Activator.CreateInstance(sampleDataItem.NavigationPage, sampleDataItem.Parametrs);
                } 
                if (sampleDataItem.Task == SampleDataItem.TaskType.ChangeGroup && sampleDataItem.NewGroup != null)
                {
                    history.Add(sampleDataItem.NewGroup);
                    backButton.Visibility = System.Windows.Visibility.Visible;
                    var type = SampleDataSource.GetGroup(sampleDataItem.NewGroup).TypeGroup;
                    this.itemsControl.ItemTemplate = (DataTemplate) this.FindResource(type + "Template");
                    this.itemsControl.ItemsSource = SampleDataSource.GetGroup(sampleDataItem.NewGroup);
                }
                if (sampleDataItem.Task == SampleDataItem.TaskType.Execute && sampleDataItem.Parametrs != null)
                {
                    Process.Start((string) sampleDataItem.Parametrs[0]);

                    ButtonAutomationPeer peer = new ButtonAutomationPeer(backButton);
                    IInvokeProvider invokeProv = peer.GetPattern(PatternInterface.Invoke) as IInvokeProvider;
                    invokeProv.Invoke();
                }
            }
            else
            {
                var selectionDisplay = new SelectionDisplay(button.Content as string);
                this.kinectRegionGrid.Children.Add(selectionDisplay);

                // Selection dialog covers the entire interact-able area, so the current press interaction
                // should be completed. Otherwise hover capture will allow the button to be clicked again within
                // the same interaction (even whilst no longer visible).
                selectionDisplay.Focus();

                // Since the selection dialog covers the entire interact-able area, we should also complete
                // the current interaction of all other pointers.  This prevents other users interacting with elements
                // that are no longer visible.
                this.kinectRegion.InputPointerManager.CompleteGestures();

                e.Handled = true;
            }
        }

        /// <summary>
        /// Handle the back button click.
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        private void GoBack(object sender, RoutedEventArgs e)
        {
            //if (navigationRegion.Content != kinectRegionGrid)
            //{
            //    navigationRegion.Content = this.kinectRegionGrid;
            //} 
            //else
            //{
            history.RemoveAt(history.Count - 1);
            SampleDataItem sampleDataItem = SampleDataSource.GetItem(history[history.Count - 1]);

            if (sampleDataItem == null)
            {
                navigationRegion.Content = this.kinectRegionGrid;
                this.itemsControl.ItemTemplate = (DataTemplate)this.FindResource(SampleDataSource.GetGroup("Menu").TypeGroup + "Template");
                this.itemsControl.ItemsSource = SampleDataSource.GetGroup("Menu");
            }
            else if (sampleDataItem.Task == SampleDataItem.TaskType.Page && sampleDataItem.NavigationPage != null)
            {
                navigationRegion.Content = Activator.CreateInstance(sampleDataItem.NavigationPage, sampleDataItem.Parametrs);
            }
            else if (sampleDataItem.Task == SampleDataItem.TaskType.ChangeGroup && sampleDataItem.NewGroup != null)
            {
                var type_b = SampleDataSource.GetGroup(history[history.Count - 1]).TypeGroup;
                this.itemsControl.ItemTemplate = (DataTemplate)this.FindResource(type_b + "Template");
                this.itemsControl.ItemsSource = SampleDataSource.GetGroup(sampleDataItem.NewGroup);
            }
            if (history[history.Count - 1] == "Menu")
            {
                backButton.Visibility = System.Windows.Visibility.Hidden;
            }
            //}
        }

        private void MenuButton_Click(object sender, RoutedEventArgs e)
        {
            BackgroungVideo.Stop();
            BackgroungVideo.Visibility = Visibility.Collapsed;
            MenuButton.Visibility = Visibility.Collapsed;
            IsPlaying = false;
        }
    }
}
