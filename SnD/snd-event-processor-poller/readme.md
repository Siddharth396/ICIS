# S&D Event Poller

A background service application that polls messages from AWS SQS and sends the messages to S&D Kafka Producer.


### Setup locally
To use event poller in the local development evironemnt, you need to do the following 

#### Configure Default AWS Credentials

``` saml2aws configure -a default --role=arn:aws:iam::512544833523:role/ADFS-DeveloperElevated -p default -r eu-west-1 ```

#### Running via Bash

Build Image:

``` bash docker-build-app.sh event-poller 1.2.1 http://outboundproxycha.cha.rbxd.ds:3128 http://outboundproxycha.cha.rbxd.ds:3128```

Run Container:

``` bash run.sh event-poller event-poller:1.2.1 ```

#### Running via Docker

execute:

``` docker run --rm -it -v %USERPROFILE%\projects\poller\logs:/logs -e "ASPNETCORE_ENVIRONMENT=Dev" -e "ASPNETCORE_URLS=http://+:80" poller:latest $(docker build -t poller --no-cache .)  ```

OR

``` docker compose up --build ```

### Deployment Pipeline

1. Team city - [Build Poller Docker Image](http://teamcity.petchem.cha.rbxd.ds/buildConfiguration/AtecProject_Kubernetes_CommitBuildArtifacts_SnDEventProcessorPoller?branch=%3Cdefault%3E&buildTypeTab=overview&mode=builds)

2. Gitlab - [Deploy via 1CD](https://gitlab.cha.rbxd.ds/snd/charts-deployment/-/pipelines)


### Go-live checklist

For live release please see [Go-live checklist](https://confluence.tools.cha.rbxd.ds/display/SUPDEM/Poller)