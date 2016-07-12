using System;
using System.Threading.Tasks;
using Nancy;
using Nancy.Hosting.Self;

namespace ConsoleApplication2
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var host = new NancyHost(new Uri("http://localhost:1234")))
            {
                host.Start();
                Console.WriteLine("Running on http://localhost:1234");
                Console.ReadLine();
            }
        }
    }

    public sealed class MyModule : NancyModule
    {
        public MyModule()
        {
            Get("/", async (_, ct) =>
            {
                await Task.Yield();

                return "hello";
            });
        }
    }
}
