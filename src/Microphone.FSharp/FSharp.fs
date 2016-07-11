namespace Microphone
module FSharp =

    let resolveService (name: string) = Cluster.Client.ResolveService name
    
    let registerService (name: string) (version: string) (url: System.Uri) (cluster: IClusterProvider) = 
        Cluster.RegisterService({ new IFrameworkProvider with member x.GetUri() = url },cluster,name,version,null)
