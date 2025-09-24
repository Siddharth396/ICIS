# Debugging without pushing to TeamCity

> install/get exe 

`curl -s "https://raw.githubusercontent.com/kubernetes-sigs/kustomize/master/hack/install_kustomize.sh"  | bash`

- Put this exec somewhere in your PATH 
- Example windows: `%userprofile%/bin` 
- Don't forget to add to your PATH!


## Debug cha-eks-5b

`./kustomize build ./cha-eks-5b/service-accounts > changelog-5b-svc-accounts.yaml` 
`./kustomize build ./cha-eks-5b/deployments > changelog-5b-deployment.yaml` 
`./kustomize build ./cha-eks-5b/automations > changelog-5b-automations.yaml` 
`./kustomize build ./cha-eks-5b/namespaces > changelog-5b-namespaces.yaml`  
`./kustomize build ./cha-eks-5a/namespaces > changelog-5a-namespaces.yaml`  
`./kustomize build ./cha-eks-5a/service-accounts > changelog-5a-service-accounts.yaml`  
`./kustomize build ./cha-eks-5a/service-accounts > changelog-5a-svc-accounts.yaml`  
