# About

This project can be used to run `WhiteSource` scan for any `.NET application` using docker.
Because WhiteSource scans `.dll` and `.exe` files, we need the production version of those files, and we can take them from the production docker image of our components.
We are using the [WhiteSource Unified Agent](https://whitesource.atlassian.net/wiki/spaces/WD/pages/804814917/Unified+Agent+Overview) to run the scan.

## Configure environment variables

In the below table you can find all environment variables.

| Environment variable | Required | Example for `bff-starter` | Description |
| -------------------- | -------- | ------------------------ | ----------- |
| WS_PROJECT_NAME | YES | Subscriber Starter BFF | The name of the project to update. It should be unique for each project. |
| DOCKER_IMAGE         | YES      | artifacts.cha.rbxd.ds/sb/sb-bff | Docker image name where the .dll and .exe files are |
| DOCKER_TAGNAME       | YES      | %dep.SubscriberPlatform_GettingStarted_Starter_BFF_Commit.dockerTagname% | Docker image tag - version of the code. |
| CODE_FOLDER_PATH     | YES      | /opt/bff    | Location inside docker image where the .dll and .exe files are |
| WS_API_KEY | YES | secret |  A unique identifier of your organization. Used to identify the organization in plugins. |
| WS_USER_KEY | NO | secret | Unique identifier of the user that can be generated from the Profile page in your WhiteSource account. NOTE: Required only if Enforce user level access is selected in the Integrate page. |
| WS_INCLUDES_PATTERN | YES | `**/*.exe **/*.dll` | Which files to include in the scan (file extensions, file names. folder names, etc.). These parameters can receive a list of arguments delimited by a comma, semicolon, or space. Read more [here](https://whitesource.atlassian.net/wiki/spaces/WD/pages/1544880156/Unified+Agent+Configuration+Parameters#Includes/Excludes-Glob-Patterns)  |
| HTTP_PROXY | YES | http://outboundproxycha.cha.rbxd.ds:3128 | This is not required if you don't run the scan behind a proxy. In TeamCity we are behind a proxy, and that's why it is required |