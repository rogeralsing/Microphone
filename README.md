# Microphone - Self announcing services

**Microphone** is a lightweight framework to run self hosting REST services using **Web Api** or **NancyFx** ontop of a **Consul** cluster.
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

## Service Discovery

If one of your services needs to communicate with another of service in the same Consul cluster you can it query for active instances.

```
var instances = FindService("Service2");
var instance = instances.First(); //or use random index for load balancing

//Use Rest# or similar to call into the remote service
MakeSomeCall("/api/orders",instance.ServiceAddress, instance.ServicePort);
```

## Running your services

Before you start your services, make sure you have an active Consul cluster running on the host machine.

If you are new to Consul, you can bootstrap your test environment using this command:
```
consul agent -server -bootstrap -data-dir /tmp/consul
```

This will give you a single server Consul cluster, this is not recommended for production usage, but it will allow you to use service discovery on your dev machine.


## Diagnostics using Consul REST API

Check service health on Consul agent:

**GET**
```
http://localhost:8500/v1/agent/checks
```

Check all services registered on Consul agent:

**GET**
```
http://localhost:8500/v1/agent/services
```
