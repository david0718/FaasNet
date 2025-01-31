﻿using FaasNet.Runtime.Domains.Instances;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace FaasNet.Runtime.EF.Configurations
{
    public class WorkflowInstanceAggregateConfiguration : IEntityTypeConfiguration<WorkflowInstanceAggregate>
    {
        public void Configure(EntityTypeBuilder<WorkflowInstanceAggregate> builder)
        {
            builder.HasKey(_ => _.Id);
            builder.Ignore(_ => _.IntegrationEvents);
            builder.Ignore(_ => _.EventRemovedEvts);
            builder.Ignore(_ => _.Output);
            builder.Property(_ => _.Parameters).HasConversion(v => JsonConvert.SerializeObject(v), v => JsonConvert.DeserializeObject<Dictionary<string, string>>(v));
            builder.HasMany(_ => _.States).WithOne().OnDelete(DeleteBehavior.Cascade);
        }
    }
}
