namespace Microphone
module FSharp =
    open System

    let resolveUri (name: string) (scheme: string) (relativePath: string) = 
        Cluster.Client.ResolveUri(name,relativePath,scheme)
        
    let registerService (name: string) (version: string) (cluster: IClusterProvider) (url: Uri) =
        Cluster.RegisterService(url,cluster,name,version,null)