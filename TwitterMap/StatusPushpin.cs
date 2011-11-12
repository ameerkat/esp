using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Maps.MapControl.WPF;
using Tweet.Model;
using System.Windows.Controls;
using System.Windows.Media;
using System.Globalization;


namespace Test
{
    class StatusPushpin : Pushpin
    {
        public Status Status {get; set;}
        public TextBlock TextBlock { get; set; }


        public StatusPushpin(Status status) :base()
        {
            this.Status = status;
            this.TextBlock = new TextBlock();
            this.TextBlock.Padding = new System.Windows.Thickness(5, 5, 5, 5);
            this.TextBlock.FontSize = 20;
            this.TextBlock.MaxWidth = 300;
            this.TextBlock.TextWrapping = System.Windows.TextWrapping.Wrap;
            this.TextBlock.Background = new SolidColorBrush(Color.FromArgb(200,255,255,255));

            DateTime d = DateTime.ParseExact(this.Status.created_at, "ddd MMM dd HH:mm:ss zzz yyyy", CultureInfo.InvariantCulture);
            this.TextBlock.Text = d.TimeOfDay + "\n" + this.Status.text;
        }


    }
}
