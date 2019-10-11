using Microsoft.Kinect.Wpf.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace Microsoft.Samples.Kinect.ControlsBasics.DataModel
{
    class HandOverHelper
    {
        public bool IsHover { get; private set; }


        public event Action OnHoverStart;
        public event Action OnHoverEnd;

        public HandOverHelper(KinectRegion kinectRegion, Dispatcher d)
        {
            var timer = new DispatcherTimer(TimeSpan.FromMilliseconds(33), DispatcherPriority.Normal, (s, a) =>
            {
                var hoverNow = kinectRegion?.EngagedBodyTrackingIds?.Count > 0;

                if (hoverNow && !IsHover)
                {
                    OnHoverStart?.Invoke();
                }
                else if (IsHover && !hoverNow)
                {
                    OnHoverEnd?.Invoke();
                }
                IsHover = hoverNow;
            }, d);
            //Task.Run(async () =>
            //{
            //    MainWindow.Log("before wait");
            //    await Task.Delay(TimeSpan.FromSeconds(3));
            //    MainWindow.Log("after wait");
            //    OnHoverStart?.Invoke();
            //});
        }

    }
}
