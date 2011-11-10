using System;
using Tweet.Model;

namespace Tweet
{
    interface IConsumer
    {
        void Start();
        void Stop();
        int Count();
        Status Deserialize(String parseMe);
    }
}
