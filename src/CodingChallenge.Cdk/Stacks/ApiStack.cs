using Amazon.CDK;
using Amazon.CDK.AWS.IAM;
using Amazon.CDK.AWS.SAM;
using CodingChallenge.Api;
using CodingChallenge.Cdk.Extensions;
using CodingChallenge.GetItemsLambda;
using CodingChallenge.Infrastructure;
using CodingChallenge.Infrastructure.Extensions;
using static Amazon.CDK.AWS.SAM.CfnFunction;

namespace CodingChallenge.Cdk.Stacks;
public class ApiStack : Amazon.CDK.Stack
{
    public const string RestApiOutputSuffix = "apistackbase";
    public ApiStack(Construct parent, string id, IStackProps props, AWSAppProject awsApp) : base(parent, id, props)
    {
        var roleArn = GetLambdaRole(awsApp, "apilambdaRole").RoleArn;
        var restApi = GetWebApi(awsApp);
        //SetupApiGateway(awsApp, restApi);
        SetupListLambda(roleArn, awsApp, restApi);
        SetupApiLambda(roleArn, awsApp, restApi);
        

    }

    private CfnFunction SetupListLambda(string roleArn, AWSAppProject awsApplication, CfnApi restApi)
    {
        var repoUri = GetDockerImageUri(awsApplication);
        var functionReference = nameof(ApiGatewayLambdaClass.GetListHandlerAsync).ToLower();

        var baseSettings = new LamdaFunctionCdkSettings
        {
            FunctionNameSuffix =functionReference,
            Memory = 1024,
            ReservedConcurrentExecutions = 2,
            Timeout = 30,
            RoleArn = roleArn,
            ImageUri = repoUri,
            Architectures = new string[] { Amazon.CDK.AWS.Lambda.Architecture.ARM_64.Name },
            HandlerFunctionName = nameof(ApiGatewayLambdaClass.GetListHandlerAsync),
            HandlerClassType = typeof(ApiGatewayLambdaClass)
        };
        var lambdaProp = baseSettings.GetLambdaContainerBaseProps(awsApplication);

        //((lambdaProp.Environment as FunctionEnvironmentProperty).Variables as Dictionary<string, string>).Add(Constants.DATABASE_TYPE_ENV_VAR_KEY, Constants.DATABASE_TYPE_DYNAMODB_ENV_VAR_KEY);
        //lambdaProp.AddEventSourceProperty(eventQueue.GetQueueEventSourceProperty(10, true), "eventKey");

        lambdaProp.AddEventSourceProperty(new EventSourceProperty()
        {
            Type = "Api",
            Properties = new ApiEventProperty()
            {
                Method = "GET",
                Path = "/list",
                RestApiId = restApi.Ref
            }
        }, nameof(ApiGatewayLambdaClass.GetListHandlerAsync));

        return new Amazon.CDK.AWS.SAM.CfnFunction(this, lambdaProp.FunctionName??functionReference, lambdaProp);

    }
    private CfnFunction SetupApiLambda(string roleArn, AWSAppProject awsApplication, CfnApi restApi)
    {
        var repoUri = GetApiDockerImageUri(awsApplication);
        var functionReference = nameof(LambdaEntryPoint).ToLower();

        var baseSettings = new LamdaFunctionCdkSettings
        {
            FunctionNameSuffix =functionReference,
            Memory = 1024,
            ReservedConcurrentExecutions = 2,
            Timeout = 30,
            RoleArn = roleArn,
            ImageUri = repoUri,
            Architectures = new string[] { Amazon.CDK.AWS.Lambda.Architecture.ARM_64.Name },
            HandlerFunctionName = "FunctionHandlerAsync",
            HandlerClassType = typeof(LambdaEntryPoint)
        };
        var lambdaProp = baseSettings.GetLambdaContainerBaseProps(awsApplication);

        //((lambdaProp.Environment as FunctionEnvironmentProperty).Variables as Dictionary<string, string>).Add(Constants.DATABASE_TYPE_ENV_VAR_KEY, Constants.DATABASE_TYPE_DYNAMODB_ENV_VAR_KEY);
        //lambdaProp.AddEventSourceProperty(eventQueue.GetQueueEventSourceProperty(10, true), "eventKey");

       lambdaProp.AddEventSourceProperty(restApi.GetWebApiEventSourceProperty("Any", "/{proxy+}"), "any");

        return new Amazon.CDK.AWS.SAM.CfnFunction(this, lambdaProp.FunctionName??functionReference, lambdaProp);

    }
    
    private string GetDockerImageUri(AWSAppProject awsApplication)
    {
        var dockerImageName = awsApplication.GetCfOutput($"{InfraStack.GetListEcrRepoSuffix}-name");
        var dockerImageEcrDomain = $"{this.Account}.dkr.ecr.{this.Region}.amazonaws.com";
        return $"{dockerImageEcrDomain}/{dockerImageName}:{awsApplication.Version}";
    }
    private string GetApiDockerImageUri(AWSAppProject awsApplication)
    {
        var dockerImageName = awsApplication.GetCfOutput($"{InfraStack.ApiRepoSuffix}-name");
        var dockerImageEcrDomain = $"{this.Account}.dkr.ecr.{this.Region}.amazonaws.com";
        return $"{dockerImageEcrDomain}/{dockerImageName}:{awsApplication.Version}";
    }

    public Role GetLambdaRole(AWSAppProject awsApplication, string namesuffix) =>
     awsApplication.GetLambdaRole(this, namesuffix)
     .AddCodeBuildReportGroupPolicy(awsApplication)
     .AddCloudWatchLogsPolicy(awsApplication)
     .AddCloudWatchLogGroupPolicy(awsApplication)
     .AddDynamoDBPolicy(awsApplication)
     .AddSnsPolicy(awsApplication)
     .AddSqsPolicy(awsApplication)
     .AddS3Policy(awsApplication)
     .AddSsmPolicy(awsApplication);

    private CfnApi GetWebApi(AWSAppProject awsApplication)
    {
        var apiResourceName = awsApplication.GetResourceName("listapi");
        var apiProps = new CfnApiProps()
        {
            Name = apiResourceName,
            StageName = awsApplication.Environment,
            EndpointConfiguration = "REGIONAL",

        };
        var api = new CfnApi(this, apiResourceName, apiProps);
        awsApplication.SetCfOutput(this, RestApiOutputSuffix, api.Ref);
        return api;
    }
}