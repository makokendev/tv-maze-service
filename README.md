# TvMaze Service - Coding Challenge

[![Build Status](https://travis-ci.org/joemccann/dillinger.svg?branch=master)](https://travis-ci.org/joemccann/dillinger)

This project is a demo solution for the TvMaze coding challenge outlined in the TvMazeScraper.pdf found in the root of the repository.

The implementation is making use an existing AWS cloud solution template, which allows to quickly spin up AWS services to handle the scraping work and serve the results with a web api.

The basic idea here is to use SQS for populating a queue of scrape request messages. 
A message handler (lamdba function) is then picking up messages from the queue and making the actual scraping work.
The lambda function can then be scaled to finish the scrape work in less than 5 minutes.
DynamoDb table is used for persisting the data. 
AWS API Gateway and .NET 6 Web API serve the data from the same DynamoDb table.

### Technologies

* .NET 6.0
* AWS CDK
* CakeBuild
* AWS SQS/SNS
* AWS DynamoDb
* AWS API Gateway

### Highlights

* [AWS CDK](https://aws.amazon.com/cdk/) framework & [CloudFormation](https://aws.amazon.com/cloudformation/) is used as the Infrastructure as Code (IaC) tool to be able to create relevant infrastructure on AWS.

* [Cake Buid](https://cakebuild.net/) is used as the build automation system. 

* [AWS](https://aws.amazon.com/) is the cloud platform where the solution is realised. 
  * Cloud Native (serverless) components are used such a [SNS](https://aws.amazon.com/sns/), [SQS](https://aws.amazon.com/sns/), [DynamoDB](https://aws.amazon.com/dynamodb/), [API Gateway](https://aws.amazon.com/api-gateway/).

### Prerequisites

In order to deploy this project, an AWS account is required, as well as setting up credentials and named profile.

* Sign up to AWS and create IAM user account and credentials https://docs.aws.amazon.com/cli/latest/userguide/getting-started-prereqs.html
* Install the AWS CLI, directions can be found here: https://docs.aws.amazon.com/cli/latest/userguide/getting-started-install.html
* Configure a named profile, with the name "" https://docs.aws.amazon.com/cli/latest/userguide/cli-configure-profiles.html
* Use the credentials you downloaded in step 1 when creating the user credentials


### Scraper

* The TvMaze Scraper is currently a console application which can be executed locally.
* The console app is adding as many scrape messages as required to a queue.
* The EventQueueProcessor is the message handler, and is in charge of making the api calls to TvMaze
* The results are stored in DynamoDb 

### API

* The API is pulling data from DynamoDb and returning a paged list of results.

### Build

* In this particular cake build project, the purpose of the build pipeline is to produce artifacts which can be deployed to desired platform e.g. AWS.
* The buid pipeline makes sure:
  * A version is determined for the specific version of the code based on git tags & commits
  * Dotnet code is built and all the tests passes.
  * A docker image is created and pushed to private Docker Registry (ECR).
  * "./build/build.sh --target=build" command triggers the build pipeline.

### Deploy

* In this particular cake build project, the purpose of the deploy pipeline is to create relevant serverless infrastructure using CloudFormation if they do not exist and create/update lambda function(s) with the latest corresponding docker images. 
* This pipeline is created with the Continuous Deployment in mind. For Continuous Delivery, a separate deployment artifact can be generated.
* "./build/build.sh --target=deploy" command triggers the deploy pipeline.

### AWS CDK

* This solution contains a single cdk project which contains definitions for three stacks. The stacks can also be defined in separate projects but this setup is chosen to showcase the multi-stack support capability.
* Infra Stack must be deployed first. 
* Database Stack is deployed after infra stack. 
* Main/Application Stack is deployed last as it has dependencies on the Infra & Database stacks.


