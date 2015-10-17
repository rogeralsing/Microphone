# Microphone

## Self announcing microservices using NancyFx or Web Api**

Microphone is a lightweight framework to run self hosting REST services using **Web Api** or **NancyFx** ontop of a **Consul** cluster.
Each service will start out by allocating a free port to run on, once the service is started, it will register itself in the local Consul agent.

## Create a service

### Web Api
```csharp
    class Program
    {
        static void Main(string[] args)
        {
            Bootstrap.Start("OtherService","v1");
            Console.ReadLine();
        }
    }

    
    public class DefaultController : AutoRegisterApiController
    {
        public string Get()
        {
            return "Service2";
        }
    }
```

### NancyFx

```csharp
    class Program
    {
        private static void Main(string[] args)
        {
            Bootstrap.Start("MyService","v1");           
            Console.ReadLine();
        }
    }

    public class MyService : AutoRegisterModule
    {
        public MyService()
        {
            Get["/"] = _ => "Hello";
        }
    }
```
