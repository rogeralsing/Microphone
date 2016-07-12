namespace Microphone
module FSharp =
    open System

    let resolveUri (name: string) (scheme: string) (relativePath: string) = 
        Cluster.Client.ResolveUri(name,relativePath,scheme)
        
    let registerService (name: string) (version: string) (cluster: IClusterProvider) (url: Uri) =
        let framework = { new IFrameworkProvider with member x.GetUri() = url } 
        Cluster.RegisterService(framework,cluster,name,version,null)