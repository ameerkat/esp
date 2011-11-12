using System;
using System.Web.Script.Serialization;
using Tweet.Model;
using System.Threading;
using System.Collections.Generic;
using System.Windows.Threading;
using System.Windows;
using Microsoft.Maps.MapControl.WPF;
using Test;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Input;
using TwitterMap;

namespace Tweet
{
    class TweetConsumer : IConsumer
    {
        private TweetProducer producer;
        private int count;
        private Thread consumer;
        private int threadNumber;
        private MainWindow map;

        public TweetConsumer(TweetProducer producer, int threadNumber, MainWindow map)
        {
            this.map = map;
            this.count = 0; 
            this.producer = producer;
            this.threadNumber = threadNumber;
        }

        public void Start()
        {
            try
            {
                this.consumer = new Thread(new ThreadStart(DoConsume));
                this.consumer.Start();
            }
            catch (OutOfMemoryException e)
            {
                //Print out exception message and return for now, consider actual handling of this exception
                Console.Out.WriteLine(e.Message);
                return;
            }
            catch (ThreadStateException e)
            {
                //Print out exception message and return for now, consider actual handling of this exception
                Console.Out.WriteLine(e.Message);
                return;
            }
        }

        public void Stop()
        {
            try
            {
                if (this.consumer != null)
                {
                    this.consumer.Abort();
                }
            }
            catch (ThreadStateException e)
            {
                //Print out exception message and return for now, consider actual handling of this exception
                Console.Out.WriteLine(e.Message);
                return;
            }
        }

        private void DoConsume()
        {
            String parseMe;
            while (true)
            {
                if ((parseMe = this.producer.ReadLine()) != null)
                {
                    /* 
                     * insert into the tweet database after deserialization for the event detection
                     * and for the API
                     */
                    Status status = this.Deserialize(parseMe);

                    bool containsKeyword = false;
                    foreach (String keyword in this.producer.GetKeywords())
                    {
                        if (status.text.Contains(keyword))
                        {
                            containsKeyword = true;
                            break;
                        }
                    }
                    if (!containsKeyword || status.geo == null)
                    {
                        continue;
                    }
                    this.count++;
                    //Console.Out.WriteLine("Thread " + this.threadNumber + ":\n" + status.ToString());

                    this.map.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal,
                      new Action(
                        delegate()
                        {
                            StatusPushpin pushpin = new StatusPushpin(status);
                            pushpin.MouseEnter += OnMouseEnter;
                            pushpin.MouseLeave += OnMouseLeave;
                            this.map.m_PushpinLayer.AddChild(pushpin, new Location(status.geo.coordinates[0], status.geo.coordinates[1]), PositionOrigin.BottomCenter);
                            
                        }
                    ));
                }
                else
                {
                    //No tweets to consume, wait 1 second
                    //Console.Out.WriteLine("Thread " + this.threadNumber + ": Nothing to consume, waiting...");
                    Thread.Sleep(1000);
                }
            }
        }

        private void OnMouseEnter(object sender, MouseEventArgs e)
        {
            StatusPushpin pushpin = sender as StatusPushpin;

            // scaling will shrink (less than 1) or enlarge (greater than 1) source element
            ScaleTransform st = new ScaleTransform();
            st.ScaleX = 1.4;
            st.ScaleY = 1.4;

            // set center of scaling to center of pushpin
            st.CenterX = (pushpin as FrameworkElement).Height / 2;
            st.CenterY = (pushpin as FrameworkElement).Height / 2;

            pushpin.RenderTransform = st;

            this.map.m_TextLayer.AddChild(pushpin.TextBlock, new Location(pushpin.Status.geo.coordinates[0], pushpin.Status.geo.coordinates[1]));
            
        }

        private void OnMouseLeave(object sender, MouseEventArgs e)
        {
            StatusPushpin pushpin = sender as StatusPushpin;

            this.map.m_TextLayer.Children.Remove(pushpin.TextBlock);

            // remove the pushpin transform when mouse leaves
            pushpin.RenderTransform = null;
        }

        public int Count()
        {
            return this.count;
        }

        public Status Deserialize(String parseMe)
        {
            // Twitter specific deserialization, except all return Model.Status
            // type objects.
            JavaScriptSerializer ser = new JavaScriptSerializer();
            Status tweet = ser.Deserialize<Status>(parseMe);
            return tweet;
        }
    }
}
