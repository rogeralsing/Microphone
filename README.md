<img src="/Resources/microphone.png" height="80">

# Microphone - Self announcing services

**Microphone** is a lightweight framework to run self hosting REST services using **Web Api** or **NancyFx** ontop of a **Consul** cluster.
Each service will start out by allocating a free port to run on, once the service is started, it will register itself in the local Consul agent.

## Install from Nuget

**WebApi bootstrapper**

```bat
PM> Install-Package Microphone.WebApi
```

**NancyFx bootstrapper**

```bat
PM> Install-Package Microphone.Nancy
```

## Create a service

### Web Api
```csharp
    class Program
    {
        static void Main(string[] args)
        {
            Bootstrap.Start("WebApiService","v1");
            Console.ReadLine();
        }
    }

    public class DefaultController : ApiController
    {
        public string Get()
        {
            return "WebApi Service";
        }
    }
```

### NancyFx

```csharp
    class Program
    {
        private static void Main(string[] args)
        {
            Bootstrap.Start("NancyService","v1");           
            Console.ReadLine();
        }
    }

    public class MyService : NancyModule
    {
        public MyService()
        {
            Get["/"] = _ => "Nancy Service";
        }
    }
```

## Service Discovery

If one of your services needs to communicate with another service in the same Consul cluster you can query it for active instances.

```csharp
//inside some WebApi/Nancy endpoint:

var instances = Cluster.FindService("Service2");
var instance = instances.First(); //or use random index to spread load

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
