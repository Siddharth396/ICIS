#!/bin/bash
# lf!
allPassed="true"

echo "Building the KuStOmIzE."

echo "Building CHA EKS 5b"
echo "INFO - Building kustomize roles"
./kustomize build ./flux/cha-eks-5b/cluster-roles/ > changelog-5b.txt 2> error-b && echo "---" >> changelog-5b.txt 
echo "INFO - Building kustomize automations (slow)"
./kustomize build ./flux/cha-eks-5b/automations/ >> changelog-5b.txt 2>> error-b && echo "---" >> changelog-5b.txt 
echo "INFO - Building kustomize core-services"
./kustomize build ./flux/cha-eks-5b/core-services/ >> changelog-5b.txt 2>> error-b && echo "---" >> changelog-5b.txt
echo "INFO - Building kustomize icis-services"
./kustomize build ./flux/cha-eks-5b/icis-services/ >> changelog-5b.txt 2>> error-b && echo "---" >> changelog-5b.txt
echo "INFO - Building kustomize namespaces"
./kustomize build ./flux/cha-eks-5b/namespaces/ >> changelog-5b.txt 2>> error-b && echo "---" >> changelog-5b.txt
echo "INFO - Building kustomize service-accounts"
./kustomize build ./flux/cha-eks-5b/service-accounts/ >> changelog-5b.txt 2>> error-b && echo "---" >> changelog-5b.txt
echo "INFO - Building kustomize deployments (slow)"
./kustomize build ./flux/cha-eks-5b/deployments/ >> changelog-5b.txt 2>> error-b
cp changelog-5b.txt changelog-5b.yaml


echo "Building CHA EKS 5a"
echo "INFO - Building kustomize roles"
./kustomize build ./flux/cha-eks-5a/cluster-roles/ > changelog-5a.txt 2> error-a && echo "---" >> changelog-5a.txt
echo "INFO - Building kustomize automations"
./kustomize build ./flux/cha-eks-5a/automations/ >> changelog-5a.txt 2>> error-a && echo "---" >> changelog-5a.txt
echo "INFO - Building kustomize core-services"  
./kustomize build ./flux/cha-eks-5a/core-services/ >> changelog-5a.txt 2>> error-a && echo "---" >> changelog-5a.txt
echo "INFO - Building kustomize icis-services"
./kustomize build ./flux/cha-eks-5a/icis-services/ >> changelog-5a.txt 2>> error-a && echo "---" >> changelog-5a.txt
echo "INFO - Building kustomize namespaces"
./kustomize build ./flux/cha-eks-5a/namespaces/ >> changelog-5a.txt 2>> error-a && echo "---" >> changelog-5a.txt
echo "INFO - Building kustomize service-accounts"
./kustomize build ./flux/cha-eks-5a/service-accounts/ >> changelog-5a.txt 2>> error-a && echo "---" >> changelog-5a.txt
echo "INFO - Building kustomize deployments (slow)"
./kustomize build ./flux/cha-eks-5a/deployments/ >> changelog-5a.txt 2>> error-a

cp changelog-5a.txt changelog-5a.yaml

echo "INFO - Validating kustomize overlays"

kubeconform -strict -schema-location default -schema-location https://raw.githubusercontent.com/datreeio/CRDs-catalog/main/{{.Group}}/{{.ResourceKind}}_{{.ResourceAPIVersion}}.json changelog-5b.yaml >> error-b
kubeconform -strict -schema-location default -schema-location https://raw.githubusercontent.com/datreeio/CRDs-catalog/main/{{.Group}}/{{.ResourceKind}}_{{.ResourceAPIVersion}}.json changelog-5a.yaml >> error-a

sed -i '/Warning/d' error-a
sed -i '/Warning/d' error-b

if [ -s error-b ]; then
    echo "cha-eks-5b: Changes are not correct. Please verify"
    cat error-b
    allPassed="false"
    exit 1
elif [ -s error-a ]; then
    echo "cha-eks-5a: Changes are not correct. Please verify"
    cat error-a
    allPassed="false"
    exit 1
else
    echo "Changes are correct."
    # git add changelog-5b.txt
    # git add changelog-5a.txt
fi
if [ $allPassed = "true" ]; then 
    cp changelog-5b.txt /app/flux/
    cp changelog-5a.txt /app/flux/
    exit 0
fi
exit -1