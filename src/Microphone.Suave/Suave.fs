namespace Microphone.Suave

module SuaveApi =
    open System
    open Suave
    open Suave.Operators
    open Suave.Filters
    open Suave.Successful
    open Suave.Logging
    open Microphone
    open Microsoft.Extensions.Logging

    type LogaryLogLevel = Suave.Logging.LogLevel

    let resolveUri serviceName (relativeUri: string) scheme =
        Cluster.Client.ResolveUri(serviceName,relativeUri,scheme)

    let healthCheck : WebPart =
        GET 
            >=> path "/status" 
            >=> OK System.Environment.MachineName

    let registerService serviceName uri clusterProvider version (logger : Logger) =
        let ilogger = {new ILogger with
                           member x.BeginScope(state) = null
                           member x.IsEnabled(logLevel) = true
                           member x.Log(logLevel, eventId, state, ``exception``, formatter) = 
                                let suaveLogLevel = match logLevel with
                                                    | LogLevel.Debug -> LogaryLogLevel.Debug
                                                    | LogLevel.Information -> LogaryLogLevel.Info
                                                    | LogLevel.Warning -> LogaryLogLevel.Warn
                                                    | LogLevel.Error -> LogaryLogLevel.Error
                                                    | LogLevel.Critical -> LogaryLogLevel.Fatal
                                                    | _ -> Suave.Logging.LogLevel.Debug
                                0 |> ignore
                                //I have no clue how to wrap the suave logger in the ILogger extension

                      }
        Cluster.RegisterService(uri,clusterProvider,serviceName,version,ilogger)      
    //TODO: we need some way to register the healthCheck above in suave
