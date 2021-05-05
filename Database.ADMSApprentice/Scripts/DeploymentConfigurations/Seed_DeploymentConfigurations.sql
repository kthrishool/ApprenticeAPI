--[Seed_DeploymentConfigurations.sql|1.0]
:r .\ChangeControl-DeploymentConfiguration_SeedData.sql --TFS executes prior to deployment, safe to rerun for manual deploy scenario
:r .\ChangeControl-ValidationTest_SeedData.sql
:r .\ChangeControl-ImportConfiguration_SeedData.sql
:r .\Security_Configurations.sql
:r .\ChangeControl-DeploymentTask_SeedData.sql
--[Seed_DeploymentConfigurations.sql|1.0]