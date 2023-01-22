# ASP.NET Box
A small aspnet core application for troubleshooting

- [Features routes](#features-routes)
  * [Routes](#routes)
  * [Swagger interface](#swagger-interface)
  * [WebSocket](#websocket)
- [Install](#install)
  * [Docker](#docker)
  * [Docker-Compose](#docker-compose)
  * [Kubernetes](#kubernetes)
  * [Kubernetes using HELM](#kubernetes-using-helm)
  * [Notes on the HTTPS support](#notes-on-the-https-support)

# Features routes

## Routes

**Echo**

`GET
​/Echo`
Retuns the provided text

`POST
​/Echo`
Retuns the provided text from the request body

**Payload**

`GET
​/Payload`
Generates a sample response text with the specified length.

`POST
​/Payload`
Measures the received body payload length

**Performance**

`GET
​/Performance​/MemoryAllocate`
Allocates a temporary memory with bytes

`GET
​/Performance​/MemoryCollect`
Runs GC.Collect

`GET
​/Performance​/Cpu`
Benchmarks CPU compute speed by retrieving prime numbers

**RequestInfo**

`GET
​/RequestInfo`

**SqlServer**

`GET
​/SqlServer​/System`
Executes a query on SQL Server using System.Data.SqlClient

`GET
​/SqlServer​/Microsoft`
Executes a query on SQL Server using Microsoft.Data.SqlClient

**SysInfo**

`GET
​/`
Dumps request and system information

**Throw**

`GET
​/Throw`
Thows an unhandled exception

**WaitFor**

`GET
​/WaitFor`
Responds after the specified delay.

## Swagger interface

To ease interaction with the APIs, a Swagger / OpenApi interface is included for convenience. 

![Swagger](resources/docs/swagger_preview.png?raw=true "Swagger")

## WebSocket

WebSocket playground

`​/websocket-echo.html`

![Swagger](resources/docs/websocket_preview.png?raw=true "Swagger")

Websocket echo endpoint

`GET
​/ws`

# Install

## Docker

The app is available as docker image at `akarzazi/aspnetbox`

https://hub.docker.com/r/akarzazi/aspnetbox

```shell
docker run  --publish 8001:80 akarzazi/aspnetbox
```

## Docker-Compose

```shell
docker-compose -f .\aspnetbox.yml up
```

With the https support:

```shell
docker-compose -f .\aspnetbox-with-https.yml up
```

## Kubernetes

```shell
kubectl apply -f .\k8s\as-pod.yaml
```

With the https support:

```shell
kubectl apply -f .\k8s\as-pod-with-https.yaml
```

From github sources:

```shell
kubectl apply -f https://raw.githubusercontent.com/akarzazi/aspnetbox/main/k8s/as-pod.yaml
```

## Kubernetes using HELM

```shell
helm install myaspnetbox .\helm -n myns --create-namespace
```

**Tip: kubectl port forward**

You can reach the K8s pod from the local computer using `kubectl` port forwarding

```shell
kubectl --namespace default port-forward aspnetbox 8002:80
```

Then navigate to `localhost:8002`

## Notes on the HTTPS support

A default self signed PFX certificate is provided as base64 in the scripts.
Feel free to replace it with your own PFX.