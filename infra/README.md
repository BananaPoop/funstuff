# About

This project uses Pulumi to deploy Infrastructure as Code (IaC), Pulumi is similar to Terraform and has some compatibility, but uses normal programming languages to construct the configs, in this project the .NET wrappers are used.

Pulumi uses *stacks* which represents an environment essentially. Stacks will typically use separate resources and run isolated from one another, additionally they can also use their own configs. Stacks can be upgraded when there code changes, as well as created, copied and destroyed.

## Managing Secrets

Stacks may have secrets in their config file. To manage that config options can be set as `secret` and have their values encrypted. They will appear in the config files as their encrypted value. Pulumi supports basic passphrase encryption, however we are using the *Azure Key Vault* encryption where the encryption keys are stored inside of an Azure Key Vault, and Pulumi must login and use the encrypt/decrypt cryptographic operations provides by Key Vault.

### Quick Start

When creating a new stack follow these steps:

1. Create the key with the same name as the stack in the desired key vault. It may make sense to use more than one key vault to isolate access to each stacks keys
2. Grant access to Encrypt/Decrypt operations to relevant users (and CI/CD runners). Encrypt is required to set a value, decrypt is required to read a value (which will be needed when deploying)
3. Run `pulumi stack init <STACK_NAME> --secrets-provider="azurekeyvault://<KEY_VAULT_URL>/keys/<KEY_NAME>"` 
4. When setting (or creating) a secret config option use `pulumi config --secret set <NAME> <VALUE>`

Note the .yaml file will contain the encrypted contents, ideally you should limit access to this. Additionally it is not required to have the key vault accessible to the internet if you know what you are doing. If a key becomes compromised the secrets can by decrypted at a later date if saved by an attacker, ensure you can always rotate any secrets stored in your config.

# Future

Encrypted secrets could be moved out of the yaml and stored directly as secrets in azure key vault and accessed via Pulumi, however then they are not trackable through git. This may be preferred in some scenarios.