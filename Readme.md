#Azure pipelines
![alt text](./images/Multiple-agents.jpg)

1. Create account

    * Start page for azure devops services user guide:  
    https://docs.microsoft.com/en-us/azure/devops/user-guide/?view=vsts
    * Create account:  
    https://docs.microsoft.com/en-us/azure/devops/user-guide/sign-up-invite-teammates?view=vsts
    * Create a project

1. Get the code  
![alt text](./images/Build.jpg)

    * In your project click on menu item `Repos`
    * Check which ways you can create your repository
![alt text](./images/GetCode.png)
    * Click on `import` under heading `or import a repository`
    * Enter URL: `https://github.com/ErickSegaar/TGIF_AzurePipelines.git` and click `import`
    * After the import is done, you should be redirected to your Git repository.  
    *Note: In this workshop we'll use the web interface for any code changes. If you prefer you can swith to Git. Use the `Clone` button on the top right to get your repository's URL.*

1. Create your first build
	* In your repository screen click on the `Set up build` button
    * Select the `.NET Desktop` template and click `Apply`
    ![alt text](./images/SelectTemplateFirstBuild.png)
    * To get familiar with the build check each task
    * Click `Save & queue` and in the new window click `Save & queue` again
    * Click on the new build `#<date>.1` in the top-left to follow its progress:  
    ![alt text](./images/FirstBuild.PNG)

1. Create release for single environment

    ![alt text](./images/Deploy.jpg)
	* In the menu go to `Pipeline` > `Releases` and click on `New pipeline`
    * Select `Empty job`
    * Add the artifact of the build with `Add an artifact`
    * Click on `Stage 1` and change the `Stage name` to `Develop`
    * Click on `Tasks` to define your deployment for `Develop`
    * Click the + button and add a `Powershell` task
    * Click on the `Powershell` task and configure it:
        * Rename the Display name to `Fake deployment`
        * Set `Type` to `inline`
        * Overwrite the `Script` field with:  
        `Write-Host "This task mimics the deployment as we won't go to azure today"`
    * Right-click the `Powershell` task and select `Clone task(s)`
    * Configure the cloned task:
        * Rename the Display name to `Call the application`
        * Change script to:  
        `&"$(System.DefaultWorkingDirectory)/<sourceAlias>/drop/EchoConsole/bin/Release/EchoConsole.exe" "Hello World"`  
        *Note: you can get the `<sourceAlias>` by clicking on your artifact in your release pipeline (see the `Add an artifact` step)*
    ![alt text](./images/Add-powershell.PNG)
    * `Save` the release pipeline
    * Create a new release to deploy your build and check the logs of the deployment
	* Talk about approvals

1. Change the code to use variables

    ![alt text](./images/Multiple-Stages.jpg)
	* Enable CI  
    Go to your build definition and choose `Edit`, `Triggers`, check the box of `Enable continuous integration` and `Save`
	* Enable CD  
	Go to your release pipeline and choose `Edit`, and click on the lightning bolt at the artifact, set the `Continuous deployment trigger` to `Enabled` and `Save`
	* Go to `Repos`, `EchoConsole/Program.cs` and click on `Edit` to change the file to:  
        ```
        static void Main(string[] args)
        {
            if (args.Any())
            {
                Console.WriteLine($"Given argument {args.First()}");
            }

            Console.WriteLine(System.Configuration.ConfigurationManager.AppSettings.Get("ApplicationEnvironment"));
        }
        ```
	* Now commit your changes, with the comment `Changed my program with application settings`
    * **Check your build and once that's finished the new release. Talk about what you see**

1. Change release to inject pipelines for different environment

    * First we're going to install an extenstion from the marketplace:
        * Click on the shopping bag icon in the top-right, then `Browse Marketplace` 
        * Search & install "Replace Tokens"
	* Add 2 more environments with different environment variables:
        * Go to the `Edit` screen of your release pipeline and go to `Tasks`
        * Add the `Replace Tokens` task as the first step of the deployment
        * Go back to the pipeline overview and clone the `Develop` stage twice to:
            * Test
            * Prod
        ![alt text](./images/CloneProdStage.png)
        * Go to `Variables` and add the `ApplicationEnvironment` variable three times, each with a different value and scope (`Develop`, `Test`, `Prod`)
        ![alt text](./images/AddStageScopedVariables.png)
        * **Talk about the variables to understand its scoping and what each value means for each stage in your release pipeline**
        * `Save` your release pipeline
    * Now we're going to `Edit` the EchoConsole/app.release.config file in your repository to use the `ApplicationEnvironment` variable:
        ```
        <?xml version="1.0" encoding="utf-8"?>
        <!--For more information on using transformations see the web.config examples at http://go.microsoft.com/fwlink/?LinkId=214134. -->
        <configuration xmlns:xdt="http://schemas.microsoft.com/XML-Document-Transform">
            <appSettings>
                <add key="ApplicationEnvironment" value="#{ApplicationEnvironment}#" xdt:Transform="Replace"/>
            </appSettings>
        </configuration>
        ```
	- Commit the change
	- **Talk about what happens (build, release pipeline and the values)**

1. Explain different possibilities for releasing, libraries, and taskgroups  
Administering one pipeline can be easy, but what makes it hard is when you have hundreds. How can you make it easier to change multiple envrionments at once?
    * **Talk about versions and drafts**
    * Add a variable group
        * Go to `Pipelines`, `Libraries` and click on `+ Variable Group`
        * Give it the name `GeneralVariables` 
        * Use `+ Add` to create variable `GeneralInfo` and give it a unique value
        * `Save` the variable group
    * Link the variable group to your release pipeline
        * Go to the `Edit` screen of your release pipeline and go to `Variables`, `Variable groups` and click the `Link variable group` button
        * Select `GeneralVariables` and click the `Link` button
        * `Save` your release pipeline
    * `Edit` the following code:
        * In EchoConsole/app.release.config add:  
        `<add key="ApplicationEnvironment" value="#{GeneralInfo}#" xdt:Transform="Insert"/>`
        * In EchoConsole/program.cs add:  
        `Console.WriteLine(System.Configuration.ConfigurationManager.AppSettings.Get("GeneralInfo"));`
    * **Check the release pipeline logs and discuss the changes**

1. Change release to use parrallelization

    ![alt text](./images/Multiple-Nodes.jpg)
	- Add a variable array $(environments) with the value First, Second
	- change agent mode of the latest 2 environments to parrallelization with 2 agents and run it.
    - change the tasklibrary to call the application to `&"$(System.DefaultWorkingDirectory)/_TGIF_Pipelines-.NET Desktop-CI/drop/EchoConsole/bin/Release/EchoConsole.exe" "$(Environments)"`
	- Discuss scenario's for this mode

1. Switch to javascript, yaml and linux

    ![alt text](./images/Multiple-agents.jpg)
    - Clone https://github.com/MicrosoftDocs/pipelines-javascript.git into a new repository
    ```
    git clone https://github.com/MicrosoftDocs/pipelines-javascript.git
    git remote set-url origin https://zwarebats@dev.azure.com/zwarebats/TGIF_Pipelines/_git/xxx
    git push -u origin --all
    ```
    - Create a new build definition, this time use the yaml one. Use the azure repo, next next finish. It will detect the yaml in the project

1. Let's try to use 1 build for all your branches, when a branch comes from a feature branch it cannot be deployed automatically but only manual and only master can go past your test environment
Done?

1. Create tokens
	- create token with right scope
	- explain system and PAT tokens

1. Change build for web application to Private Linux
	- Run the docker container with private agent, attached to your public azure devops
	- change to the new queue with the private agent
	- change up the container to get a extra layer for an environment variable
	- run another private agent with the new container
	- force the build to the later agent
	- Talk about scenario to use private agents, directing build capabilities, how this can be cool with k8s pods and scale

1. Run Smoke test?
    - include smoketest for the validation of the environment variables
    - talk about the importance       
