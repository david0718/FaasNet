﻿using FaasNet.Runtime.Domains;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.Runtime.Persistence
{
    public interface IWorkflowDefinitionRepository
    {
        IQueryable<WorkflowDefinitionAggregate> Query();
        Task Add(WorkflowDefinitionAggregate workflowDef, CancellationToken cancellationToken);
        Task<int> SaveChanges(CancellationToken cancellationToken);
    }
}