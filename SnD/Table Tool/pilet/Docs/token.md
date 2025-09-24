## How to create Token

### Rules:

- The squad will look after who in their team can deploy from their Repo
  - You don't need OPT ticket to add another person to your own deployment
- The squad lead will give a Token!

### Squad lead instructions

- Request access to group: https://github.com/orgs/LexisNexis-RBA/teams/icis-mfe-live-deployers 
  - Ping @Sean, @Sanjev in teams
- Go to: https://github.com/settings/personal-access-tokens
- "Generate new token" ![alt text](images/image-10.png)
- ![alt text](images/image-11.png)

> - Set a name that you will remember
> - Set Owner to LexisNexis-RBA
> - Set Expiry to 366 days (and put a calendar event to remind yourself)
> - Set a description that will remind you what this is for

- ![alt text](images/image-12.png)
> - Set to specific repos
> - ADD THESE REPOS AND NOTHING ELSE!

- dsg-icis-common-mfe-deployment-pre-live
- dsg-icis-common-mfe-deployment-live
- dsg-icis-common-mfe-deployment-onboarding

- ![alt text](images/image-13.png)

> Under permissions
> ONLY SET ACTIONS! to read-write

- ![alt text](images/image-14.png)
> Click Generate

### You have a token where to put:

Go to your repo settings

Example:
![alt text](images/image-15.png)

Find the GH_TOKEN var:
![alt text](images/image-16.png)

Paste your new token 
![alt text](images/image-17.png)

> NOTE This is not Gitlab you can't view the secrets after adding them