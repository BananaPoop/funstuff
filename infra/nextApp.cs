using System.Threading.Tasks;
using Pulumi;
using CloudFlare = Pulumi.Cloudflare;
public class NextApp{
    public static async Task Deploy(){
        const string accountId = ""; //TODO get these from the yaml
        const string environment = "";
        var cloudFlareProject = new CloudFlare.PagesProject("nextjsProject", new(){
            AccountId = accountId,
            Name = $"funstuff-nextjsProject-{environment}",
            BuildConfig = new CloudFlare.Inputs.PagesProjectBuildConfigArgs
            {
                BuildCommand = "npm run export",
                DestinationDir = "nextjsBuild",
                RootDir = "../nextjsApp"
            },
            ProductionBranch = "spooky" //Not sure what this does exactly
        });

    }
}