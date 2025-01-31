﻿using FaasNet.Runtime.Domains.Enums;
using FaasNet.Runtime.Domains.Instances;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FaasNet.Gateway.Core.StateMachineInstances.Results
{
    public class StateMachineInstanceDetailsResult
    {
        public StateMachineInstanceDetailsResult()
        {
            States = new List<InstanceStateResult>();
        }

        public string Id { get; set; }
        public string WorkflowDefTechnicalId { get; set; }
        public string WorkflowDefId { get; set; }
        public string WorkflowDefName { get; set; }
        public string WorkflowDefDescription { get; set; }
        public int WorkflowDefVersion { get; set; }
        public DateTime CreateDateTime { get; set; }
        public WorkflowInstanceStatus Status { get; set; }
        public JObject Output { get; set; }
        public ICollection<InstanceStateResult> States { get; set; }

        public static StateMachineInstanceDetailsResult ToDto(WorkflowInstanceAggregate instance)
        {
            return new StateMachineInstanceDetailsResult
            {
                Id = instance.Id,
                WorkflowDefTechnicalId = instance.WorkflowDefTechnicalId,
                CreateDateTime = instance.CreateDateTime,
                Output = instance.Output,
                WorkflowDefDescription = instance.WorkflowDefDescription,
                WorkflowDefName = instance.WorkflowDefName,
                Status = instance.Status,
                WorkflowDefId = instance.WorkflowDefId,
                WorkflowDefVersion = instance.WorkflowDefVersion,
                States = instance.States.Select(s => InstanceStateResult.ToDto(s)).ToList()
            };
        }
    }
}
