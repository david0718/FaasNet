﻿using FaasNet.Runtime.Builders;
using FaasNet.Runtime.Domains.Enums;
using FaasNet.Runtime.Processors;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace FaasNet.Runtime.Tests
{
    public class WorkflowDefinitionFixture
    {
        [Fact]
        public async Task When_Run_HelloWorld()
        {
            var workflowDefinition = WorkflowDefinitionBuilder.New("helloWorld", "v1", "name", "description")
                .StartsWith(o => o.Inject().Data(new { result = "Hello World!" }).End())
                .Build();
            var stateProcessors = new List<IStateProcessor>
            {
                new InjectStateProcessor()
            };
            var runtimeEngine = new RuntimeEngine(stateProcessors);
            var instance = await runtimeEngine.InstanciateAndLaunch(workflowDefinition, "{}", CancellationToken.None);
            Assert.Equal(WorkflowInstanceStatus.TERMINATE, instance.Status);
            Assert.Equal("Hello World!", instance.Output["result"].ToString());
        }

        [Fact]
        public async Task When_Inject_Persons_And_Filter()
        {
            var workflowDefinition = WorkflowDefinitionBuilder.New("injectAndFilterPersons", "v1", "name", "description")
                .StartsWith(o => o.Inject().Data(new { people = new List<dynamic>
                {
                    new
                    {
                        fname = "John",
                        lname = "Doe",
                        address = "1234 SomeStreet",
                        age = 40
                    },
                    new
                    {
                        fname = "Marry",
                        lname = "Allice",
                        address = "1234 SomeStreet",
                        age = 25
                    },
                    new
                    {
                        fname = "Kelly",
                        lname = "Mill",
                        address = "1234 SomeStreet",
                        age = 30
                    }
                }
                }).SetOutputFilter("{ 'people' : '$.people[?(@.age < 40)]' }").End())
                .Build();
            var stateProcessors = new List<IStateProcessor>
            {
                new InjectStateProcessor()
            };
            var runtimeEngine = new RuntimeEngine(stateProcessors);
            var instance = await runtimeEngine.InstanciateAndLaunch(workflowDefinition, "{}", CancellationToken.None);
            Assert.Equal(WorkflowInstanceStatus.TERMINATE, instance.Status);
            Assert.Equal("{\r\n  \"people\": [\r\n    {\r\n      \"fname\": \"Marry\",\r\n      \"lname\": \"Allice\",\r\n      \"address\": \"1234 SomeStreet\",\r\n      \"age\": 25\r\n    },\r\n    {\r\n      \"fname\": \"Kelly\",\r\n      \"lname\": \"Mill\",\r\n      \"address\": \"1234 SomeStreet\",\r\n      \"age\": 30\r\n    }\r\n  ]\r\n}", instance.OutputStr);
        }
    }
}
