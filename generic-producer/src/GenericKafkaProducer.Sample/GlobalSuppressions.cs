// This file is used by Code Analysis to maintain SuppressMessage
// attributes that are applied to this project.
// Project-level suppressions either have no target or are given
// a specific target and scoped to a namespace, type, member, etc.

using System.Diagnostics.CodeAnalysis;

[assembly: SuppressMessage("StyleCop.CSharp.OrderingRules", "SA1201:Elements should appear in the correct order", Justification = "required", Scope = "member", Target = "~F:KafkaProducer.Sample.Program.AvroSerializerConfig")]
[assembly: SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1401:Fields should be private", Justification = "required", Scope = "member", Target = "~F:KafkaProducer.Sample.Program.schemaRegistryConfig")]
[assembly: SuppressMessage("Usage", "CA2200:Rethrow to preserve stack details", Justification = "required", Scope = "member", Target = "~M:KafkaProducer.Sample.Program.Main(System.String[])~System.Threading.Tasks.Task")]
