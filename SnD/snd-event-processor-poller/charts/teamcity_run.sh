#!/bin/bash

sudo su -

# exit with an error if any erros occur
set -e

# getting the values from the input arguments
environment=$1
processor_image_tag=$2
poller_image_tag=$3

release_name="supdem-processor-${environment}"
if [[ $environment == "systest" ]]; then
	helm_values=systest_values.yaml
elif [[ $environment == "performance" ]]; then
  	helm_values=performance_values.yaml
elif [[ $environment == "uat" ]]; then
  	helm_values=uat_values.yaml
else
	helm_values=values.yaml
fi

namespace="supdem-${environment}"

#To uninstall the deployment, 
#uncomment the following commands & comment the helm upgrade command. 
#echo "-----Uninstalling Deployment for-----" $namespace
#helm uninstall $release_name -n $namespace

echo "Deploying processor image tag: " $processor_image_tag "and poller image tag: " $poller_image_tag
echo "---------------------------------"

helm upgrade --install --wait --timeout 120s $release_name -n $namespace --kubeconfig=/root/.kube/EKS-5b-supdem-agent-conf --set processor.image.tag=$processor_image_tag --set poller.image.tag=$poller_image_tag \
-f $helm_values .

echo "---------- HELM History ----------"
helm3 --kubeconfig=/root/.kube/EKS-5b-supdem-agent-conf history $release_name --namespace $namespace

echo "------------ Pods list ------------"
kubectl --kubeconfig=/root/.kube/EKS-5b-supdem-agent-conf get pods -n $namespace

