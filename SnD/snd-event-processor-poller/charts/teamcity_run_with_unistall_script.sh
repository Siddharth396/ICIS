#!/bin/bash

sudo su -

# exit with an error if any erros occur
set -e

# getting the values from the input arguments
environment=$1

release_name="supdem-processor-${environment}"

namespace="supdem-${environment}"

#To uninstall the deployment
echo "-----Uninstalling Deployment for-----" $namespace
helm --kubeconfig=/root/.kube/EKS-5b-supdem-agent-conf ls -n $namespace 
helm --kubeconfig=/root/.kube/EKS-5b-supdem-agent-conf uninstall $release_name -n $namespace 
echo "-----Uninstall completed-------"
