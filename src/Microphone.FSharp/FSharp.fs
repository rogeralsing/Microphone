namespace Microphone
module FSharp =
    open Suave

    let resolveService (name: string) = 
        let res = Cluster.Client.ResolveService name
        (res.Host,res.Port)
    
    let registerService (name: string) (version: string) (cluster: IClusterProvider) (url: System.Uri) =
        let framework = { new IFrameworkProvider with member x.GetUri() = url } 
        Cluster.RegisterService(framework,cluster,name,version,null)