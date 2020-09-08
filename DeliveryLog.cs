using System;

namespace kafkaClient
{
    public class DeliveryLog
    {
        public DateTime timeStamp { get; set; }
        public MyDeliveyResult myDeliveyResult { get; set; }
    }
    public class MyDeliveyResult
    {
        public DateTime topicTimeStamp { get; set; }
        public string topic { get; set; }
        public long offset { get; set; }
        public int partition { get; set; }
        public string message { get; set; }

    }

}
