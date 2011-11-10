using System;

namespace Tweet
{
    interface IProducer
    {
        void Start();
        void Stop();
        String ReadLine();
        int Count();
    }
}
