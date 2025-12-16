# Icis Generic Kafka Producer
=======================

## Overview
--------

This repo contains two implementations of Kafka Producer libraries that can write events to respective topics. 

### Parts of Solution 
1. GenericKafkaProducer - a library to write kafka event in AVRO or JSON format by converting string message value to GenericRecord
2. GenericKafkaProducer.Test - test cases for GenericKafkaProducer project
3. GenericKafkaProducer.Sample - a sample application to invoke GenericKafkaProducer library
4. SpecificKafkaProducer - a library to write kafka event in AVRO format via ISpecificRecord
5. SpecificKafkaProducer.Test - test cases for SpecificKafkaProducer project
6. SpecificKafkaProducer.Sample - a sample application to invoke SpecificKafkaProducer library

### How to try sample projects
The projects are in .NET 5.0 and created using VS2019. The only external dependencies (other than NuGet packages from the public feed) is Kafka.

1. Have Kafka and the schema registry available, edit the appSettings.json in each project as required to configure the brokers and schema registry
2. To try GenericKafkaProducer sample, start GenericKafkaProducer.Sample
3. To try SpecificKafkaProducer sample, start SpecificKafkaProducer.Sample


### How to inject in projects
Nuget packages will be created for both GenericKafkaProducer and SpecificKafkaProducer and published to Nuget - https://artifacts.cha.rbxd.ds/.

Stash link - 
Teamcity link - 
Nexus link - 

####

### Circuit Breaker in Kafka Producer
Circuit breaker is use to stop the trigger mechanism of client system whenever any depended system is down.
In case our API for fetching the data is down, we would not want to read the messages from SQS and push them to DLQ as all 
messages would be pushed to DLQ in order to avoid it. We stop reading the messages from source when the dependent systems are down.

To enable circuit breaker pattern the consuming application if implements the IErrorHandler Interfaces and registers in the DI, 
the circuit breaker constructor will be called where the consuming application has to handle the errors in the way desired.
###
