using System;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace kafkaClient
{
    class Program
    {
        static object lockobject = new object();
        static async Task Main(string[] args)
        {
            int N = 100000000;
            ParallelOptions options = new ParallelOptions();
            options.MaxDegreeOfParallelism = 1;

            var topicData = new TopicData();
            using (var kafkaClient = new MyKafkaProducer("10.0.3.8:9092", "confluent"))
            {
                /*
                 * over 3000 Request per 1sec. too many.
                Parallel.For(0, N,
                    options, (int i, ParallelLoopState loopState) =>
                     {
                         lock (lockobject)
                         {
                             topicData.date = DateTime.UtcNow;
                             topicData.id = i;
                             kafkaClient.produce(JsonConvert.SerializeObject(topicData));
                         }
                     });
                */
                for (int i = 0; i < N; i++)
                {
                    if (i > 0 && i % 15 == 0)
                    {
                        System.Threading.Thread.Sleep(100);
                    }
                    topicData.date = DateTime.UtcNow;
                    topicData.id = i;
                    kafkaClient.produce(JsonConvert.SerializeObject(topicData));
                }
            }
        }
    }
    public class TopicData
    {
        public DateTime date {get; set;}
        public int id { get; set; }
    }
}
