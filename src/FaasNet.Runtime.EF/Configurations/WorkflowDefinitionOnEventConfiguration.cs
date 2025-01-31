﻿using FaasNet.Runtime.Domains.Definitions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Newtonsoft.Json;
using System.Linq;

namespace FaasNet.Runtime.EF.Configurations
{
    public class WorkflowDefinitionOnEventConfiguration : IEntityTypeConfiguration<WorkflowDefinitionOnEvent>
    {
        public void Configure(EntityTypeBuilder<WorkflowDefinitionOnEvent> builder)
        {
            builder.Property<int>("Id").ValueGeneratedOnAdd();
            builder.HasKey("Id");
            builder.Property(p => p.EventRefs)
                .HasConversion(p => string.Join(",", p), p => p.Split(',', System.StringSplitOptions.None).Where(s => !string.IsNullOrWhiteSpace(s)).ToList());
            builder.Property(p => p.EventDataFilter)
                .HasConversion(p => JsonConvert.SerializeObject(p), p => JsonConvert.DeserializeObject<WorkflowDefinitionEventDataFilter>(p));
            builder.HasMany(_ => _.Actions).WithOne().OnDelete(DeleteBehavior.NoAction);
        }
    }
}
