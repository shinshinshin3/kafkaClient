using System;
using System.Threading.Tasks;
using Confluent.Kafka;
using Newtonsoft.Json;

namespace kafkaClient
{

    public struct kafkaTopic
    {


    }

    public class MyKafkaProducer : IDisposable
    {
        //private string bootstrapServers;
        public string topic;
        private ProducerConfig producerConfig;
        private IProducer<Null, string> producer;

        //コンストラクタ
        public MyKafkaProducer()
        { }
        public MyKafkaProducer(string bootstrapServers, string topic)
        {
            this.producerConfig = new ProducerConfig
            {
                BootstrapServers = bootstrapServers
                //Debug = "all"
            };
            this.topic = topic;
            this.producer = new ProducerBuilder<Null, string>(this.producerConfig)
                .SetLogHandler(LogHandler)
                .SetErrorHandler(ErrorHandler)
                .Build();
        }

        Action<IProducer<Null, string>, LogMessage> LogHandler = 
        (IProducer<Null, string> i, LogMessage l) => 
        {
            Console.WriteLine($"{DateTime.UtcNow}:{l.Level}:{l.Message}");   
        };

        Action<IProducer<Null, string>, Error> ErrorHandler =
        (IProducer<Null, string> i, Error e) =>
        {
            Console.WriteLine($"{DateTime.UtcNow}:{e.Code}'{e.Reason}");
        };

        Action<DeliveryReport<Null, string>> deliveryHandler =
        (r) => {
            var myDelivaryResult = new MyDeliveyResult()
            {
                topicTimeStamp = r.Timestamp.UtcDateTime,
                topic = r.Topic,
                offset = r.Offset.Value,
                partition = r.TopicPartition.Partition.Value,
                message = r.Message.Value
            };
            Console.WriteLine(JsonConvert.SerializeObject(myDelivaryResult));
        };

        public async Task<MyDeliveyResult> produceAsync(string message)
        {
            var deliveryResult = await this.producer.ProduceAsync(this.topic, new Message<Null, string> { Value = message });
            return new MyDeliveyResult()
            {
                topicTimeStamp = deliveryResult.Timestamp.UtcDateTime,
                topic = deliveryResult.Topic,
                offset = deliveryResult.Offset.Value,
                partition = deliveryResult.Partition.Value,
                message = deliveryResult.Message.Value
            };
        }
        public void produce(string message)
        {
            this.producer.Produce(this.topic, new Message<Null, string> { Value = message }, deliveryHandler);
            //return handler.Error.isError;
        }

        public void Dispose()
        {
            this.producer.Flush(TimeSpan.FromSeconds(10));
        }
    }
}
