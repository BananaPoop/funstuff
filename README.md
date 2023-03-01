# Quick Start

## Prerequisites

* Install Pulumi CLI
* Azure Account and subscription

### NextJS

* Node.js, for managing the next.js app

For windows, recommended to run using WSL
<!-- TODO: Move this to it's own page -->
```bash
#Enter a WSL terminal
$sudo apt-get install curl #if required
$curl -o- https://raw.githubusercontent.com/nvm-sh/nvm/master/install.sh | bash #nvm is node version manager, this will be used to manage your node version
$command -v nvm
$exit #You will need to restart your terminal so that nvm becomes available sometimes
#Get back into the WSL terminal
$command -v nvm #If this outputs "nvm" then its working, otherwise something is probably wrong
nvm #The correct output
$nvm ls #This lists node version installed, should be none if you are doing this for the first time
$nvm install --lts #Install LTS version of Node. Recommended for most scenarios

```
## Steps

1. Login to Pulumi, if you want to use Azure Blob Storage for this [guide](pulumi_azure_backend.azcli)