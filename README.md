# Distributed Tracing Demo

This C# example demonstrates how to using OpenTracing with Jaeger for distributed tracing. For this demo, we based on the [Jaeger Kubernetes](https://github.com/jaegertracing/jaeger-kubernetes) template, with minor modifications to make it work with Win 10 local k8s cluster.

## The Docker Image
To build the docker image and push it to Docker Hub:
```
$ docker build -t peishu/demoapi:v1 DemoApi/DemoApi
$ docker push peisu/demoapi:v1
```

## Install Jaeger (All-in-One) in Kubernetes
```
$ kubectl apply -f jagear-all-in-one-k8s.yaml
```
To access Jaeger UI:
```
http://localhost:8081/search
```

## Install Demo API
```
$ kubectl apply -f demoapi-k8s.yaml
```
To access demo api:
```
http://localhost:8080/api/values
```
## A Sample Jaeger UI Screenshot
![JaegerUIScreenshot](https://user-images.githubusercontent.com/22537533/56534840-13f9af00-6520-11e9-9b43-90954814fd8b.PNG)

## Sample C# Code
The follow C# code snippet illustrates how to configure OpenTracing client with Remote Jaeger instances:
```
services.AddSingleton<ITracer>(serviceProvider =>
{
    string serviceName = serviceProvider.GetRequiredService<IHostingEnvironment>().ApplicationName;
    var samplerConfiguration = new Configuration.SamplerConfiguration(_loggerFactory)
        .WithType(ProbabilisticSampler.Type)
        .WithManagerHostPort("jaeger-agent:5778")
        .WithParam(1);

    var senderConfiguration = new Configuration.SenderConfiguration(_loggerFactory)
        .WithAgentHost("jaeger-agent")
        .WithAgentPort(6831);

    var reporterConfiguration = new Configuration.ReporterConfiguration(_loggerFactory)
        .WithLogSpans(true)
        .WithSender(senderConfiguration);
                
    Configuration config = Configuration.FromEnv(_loggerFactory);

                
    var tracer = config
        .WithSampler(samplerConfiguration)
        .WithReporter(reporterConfiguration)                    
        .GetTracer();
    return tracer;
});
```
