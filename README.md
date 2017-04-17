<img src="/Resources/microphone.png" height="80">

# Microphone - Self announcing services

**Microphone** is a lightweight framework to run self hosting REST services using **Web Api** or **NancyFx** ontop of a **Consul** or **ETCD** cluster.
Each service will start out by allocating a free port to run on, once the service is started, it will register itself in the local cluster provider.

## Install from Nuget

**WebApi bootstrapper**

```bat
PM> Install-Package Microphone.AspNet
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
            var options = new ConsulOptions();
            var loggerFactory = new LoggerFactory();
            var logger = loggerFactory.CreateLogger("logger");
            var provider = new ConsulProvider(loggerFactory, Options.Create(options));
            Cluster.RegisterService(new Uri($"http://localhost"), provider, "orders", "v1", logger);
            Console.ReadLine();
        }
    }

    public class OrdersController : ApiController
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
            Cluster.Bootstrap(new NancyProvider(), new ConsulProvider(), "customers", "v1");
            Console.ReadLine();
        }
    }

    public class CustomersService : NancyModule
    {
        public MyService()
        {
            Get["/"] = _ => "Nancy Service";
        }
    }
```

## Cluster providers

**Consul**
```
Cluster.Bootstrap(new WebApiProvider(), new ConsulProvider(), "my-service", "v1");
```

The Consul provider also works together with Ebays "Fabio" load balancer https://github.com/eBay/fabio
```
Cluster.Bootstrap(new WebApiProvider(), new ConsulProvider(useEbayFabio:true), "my-service", "v1");
```

**ETCD**
```
Cluster.Bootstrap(new WebApiProvider(), new EtcdProvider(), "my-service", "v1");
```

## Service Discovery

If one of your services needs to communicate with another service in the same Consul cluster you can query it for active instances.

```csharp
//inside some WebApi/Nancy endpoint:

//automatically load balanced over service instances
var instance = await Cluster.FindServiceInstanceAsync("orders"); 

//Use Rest# or similar to call into the remote service
MakeSomeCall("/api/orders",instance.ServiceAddress, instance.ServicePort);
```

## Running your services

Before you start your services, make sure you have an active cluster running on the host machine.

#### Consul Cluster

If you are new to Consul, you can bootstrap your test environment using this command:
```
consul agent -server -bootstrap -data-dir /tmp/consul -bind=127.0.0.1
```

This will give you a single server Consul cluster, this is not recommended for production usage, but it will allow you to use service discovery on your dev machine.


#### Diagnostics using Consul REST API

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

#### ETCD Cluster

If you are using the ETCD cluster provider, make sure you have a local ETCD cluster running on your dev machine.

```
etcd.exe
```

#### Key/Value storage

```csharp
    class Person
    {
        public string Name { get; set; }
        public int Age { get; set; }
    }

    class Program
    {
        private static void Main(string[] args)
        {
            Cluster.Bootstrap(new WebApiProvider(), new ConsulProvider(), "orders", "v1");
            Cluster.KVPutAsync("val1", new Person { Name = "Name", Age = 25 }).Wait();
            var res = Cluster.KVGetAsync<Person>("val1").Result;
            Console.ReadLine();
        }
    }
```
