#Micro Service Lab .NET Starter Kit

###Introduction

You are going to deploy micro services to azure. Each service might consume and publish messages to the shared bus.
To persist service state, eventstore is used.

###Azure

Prequisites
- Azure SDK 2.2
- Azure subscription

The starter kit contains two roles. One worker and one web role.
The worker role consumes messages, and may publish new ones. The web role could host UI 
for your service or/and HTTP apis your service might expose.

You could host these kind of services in your worker role, you also could let your web site do stuff in the background(iick), 
but to get started you got two roles. The new WebJob could have been used as well...

You could also skip the web role and use Web Sites, your pick!

###Register Services

Each service could register itself, to let every one know what services that are out there!
To register a service you'll need an url to a service endpoint. 

###Lab.Worker

The worker is responsible for registering the service.
In the Setup you'll find register and how to create a bus for message consumtion.
 
###Lab.Web

The web project is responsible for hosting your endpoints. This might be HTTP Apis or plain HTML views to show service data.

###Getting Started

1. You need publishing info for your cloud service
1.1 Create a cloud service
1.2 Download publishing info
1.3 Setup info for your project

2. The Queue needs a connection
2.1 In setup, configure your connection

3. Register the service
3.1 In Setup you configure the registration

