//------------------------------------------------------------------------------
// <copyright file="MainWindow.xaml.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace Microsoft.Samples.Kinect.ControlsBasics
{
    using System;
    using System.Collections.Generic;
    using System.Windows;
    using System.Windows.Controls;
    using Microsoft.Kinect;
    using Microsoft.Kinect.Wpf.Controls;
    using Microsoft.Samples.Kinect.ControlsBasics.DataModel;

    /// <summary>
    /// Interaction logic for MainWindow
    /// </summary>
    public partial class MainWindow
    {
        private List<string> history = new List<string>();

        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindow"/> class. 
        /// </summary>
        public MainWindow()
        {
            this.InitializeComponent();

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
                    backButton.Visibility = System.Windows.Visibility.Visible;
                    navigationRegion.Content = Activator.CreateInstance(sampleDataItem.NavigationPage, sampleDataItem.Parametrs);
                } 
                if (sampleDataItem.Task == SampleDataItem.TaskType.ChangeGroup && sampleDataItem.NewGroup != null)
                {
                    history.Add(sampleDataItem.NewGroup);
                    backButton.Visibility = System.Windows.Visibility.Visible;
                    var type = SampleDataSource.GetGroup(sampleDataItem.NewGroup).TypeGroup;
                    switch (type)
                    {
                        case SampleDataCollection.GroupType.Menu:
                            scrollViewer.VerticalScrollBarVisibility = ScrollBarVisibility.Disabled;
                            scrollViewer.HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled;
                            break;
                        case SampleDataCollection.GroupType.News:
                            scrollViewer.VerticalScrollBarVisibility = ScrollBarVisibility.Disabled;
                            scrollViewer.HorizontalScrollBarVisibility = ScrollBarVisibility.Visible;
                            break;
                        case SampleDataCollection.GroupType.Courses:
                            scrollViewer.VerticalScrollBarVisibility = ScrollBarVisibility.Visible;
                            scrollViewer.HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled;
                            break;
                    }
                    this.itemsControl.ItemTemplate = (DataTemplate) this.FindResource(type + "Template");
                    this.itemsControl.ItemsSource = SampleDataSource.GetGroup(sampleDataItem.NewGroup);

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
            if (navigationRegion.Content != kinectRegionGrid)
            {
                navigationRegion.Content = this.kinectRegionGrid;
            } else
            {
                history.RemoveAt(history.Count - 1);
                var type = SampleDataSource.GetGroup(history[history.Count - 1]).TypeGroup;
                switch (type)
                {
                    case SampleDataCollection.GroupType.Menu:
                        scrollViewer.VerticalScrollBarVisibility = ScrollBarVisibility.Disabled;
                        scrollViewer.HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled;
                        break;
                    case SampleDataCollection.GroupType.News:
                        scrollViewer.VerticalScrollBarVisibility = ScrollBarVisibility.Disabled;
                        scrollViewer.HorizontalScrollBarVisibility = ScrollBarVisibility.Visible;
                        break;
                    case SampleDataCollection.GroupType.Courses:
                        scrollViewer.VerticalScrollBarVisibility = ScrollBarVisibility.Visible;
                        scrollViewer.HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled;
                        break;
                }
                this.itemsControl.ItemTemplate = (DataTemplate)this.FindResource(type + "Template");
                this.itemsControl.ItemsSource = SampleDataSource.GetGroup(history[history.Count - 1]);
                if (history[history.Count - 1] == "Menu")
                {
                    backButton.Visibility = System.Windows.Visibility.Hidden;
                }
            }
        }

        private void ItemsControl_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            var temp = itemsControl.ItemTemplate.DataTemplateKey;
            //if (itemsControl.ItemTemplate.DataTemplateKey == )
        }
    }
}
