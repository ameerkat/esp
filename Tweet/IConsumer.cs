using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tweet
{
    interface IConsumer
    {
        void Start();
        void Stop();
        String ReadLine();
        int getCount();
        Model.Status deserialize(String parseMe);
    }
}
