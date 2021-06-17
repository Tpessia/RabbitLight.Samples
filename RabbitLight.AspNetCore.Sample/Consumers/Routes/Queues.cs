namespace RabbitLight.AspNetCore.Sample.Consumers.Routes
{
    public class Queues
    {
        // Simple Test
        public const string Test1 = "test1";
        public const string Test2 = "test2";
        public const string TestSerially = "test-serially";
        public const string Error = "error";

        // Dead Letter Test
        public const string DLTest = "dl.test";

        // Integrity Test
        public const string Integrity = "integrity";
    }
}
