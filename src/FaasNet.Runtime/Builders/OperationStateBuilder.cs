﻿using FaasNet.Runtime.Domains.Definitions;
using FaasNet.Runtime.Domains.Enums;
using System;

namespace FaasNet.Runtime.Builders
{
    public class OperationStateBuilder : BaseStateBuilder<WorkflowDefinitionOperationState>
    {
        internal OperationStateBuilder() : base(WorkflowDefinitionOperationState.Create())
        {
        }

        public OperationStateBuilder SetActionMode(WorkflowDefinitionActionModes actionMode)
        {
            StateDef.ActionMode = actionMode;
            return this;
        }

        public OperationStateBuilder AddAction(string name, Action<ActionBuilder> callback)
        {
            var builder = new ActionBuilder(name);
            callback(builder);
            StateDef.Actions.Add(builder.Build());
            return this;
        }

        public OperationStateBuilder End()
        {
            StateDef.End = true;
            return this;
        }
    }
}
