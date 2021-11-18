﻿using FaasNet.Gateway.Core.Exceptions;
using FaasNet.Gateway.Core.Extensions;
using FaasNet.Gateway.Core.Resources;
using FaasNet.Gateway.Core.StateMachines.Queries.Results;
using FaasNet.Runtime;
using FaasNet.Runtime.Domains.Definitions;
using FaasNet.Runtime.Persistence;
using MediatR;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.Gateway.Core.StateMachines.Commands.Handlers
{
    public class StartStateMachineCommandHandler : IRequestHandler<StartStateMachineCommand, StateMachineInstanceDetails>
    {
        private readonly IWorkflowDefinitionRepository _workflowDefinitionRepository;
        private readonly IRuntimeEngine _runtimeEngine;

        public StartStateMachineCommandHandler(IWorkflowDefinitionRepository workflowDefinitionRepository, IRuntimeEngine runtimeEngine)
        {
            _workflowDefinitionRepository = workflowDefinitionRepository;
            _runtimeEngine = runtimeEngine;
        }

        public async Task<StateMachineInstanceDetails> Handle(StartStateMachineCommand request, CancellationToken cancellationToken)
        {
            var validationResult = Validate(request);
            var workflowInstance = await _runtimeEngine.InstanciateAndLaunch(validationResult.WorkflowDefinition, request.Input, cancellationToken);
            return StateMachineInstanceDetails.Build(workflowInstance);
        }

        #region Validation

        protected virtual ValidationResult Validate(StartStateMachineCommand request)
        {
            var workflowDef = _workflowDefinitionRepository.Query().FirstOrDefault(w => w.Id == request.Id);
            if (workflowDef == null)
            {
                throw new StateMachineNotFoundException(ErrorCodes.InvalidStateMachineName, string.Format(Global.UnknownStateMachine, request.Id));
            }

            if (!string.IsNullOrWhiteSpace(request.Input))
            {
                if (!request.Input.IsJson())
                {
                    throw new BadRequestException(ErrorCodes.InvalidExecutionInput, Global.InvalidExecutionInput);
                }
            }

            return new ValidationResult(workflowDef, request.Input);
        }

        protected class ValidationResult
        {
            public ValidationResult(WorkflowDefinitionAggregate workflowDefinition, string data)
            {
                WorkflowDefinition = workflowDefinition;
                Data = data;
            }

            public WorkflowDefinitionAggregate WorkflowDefinition { get; }
            public string Data { get; }
        }

        #endregion
    }
}
