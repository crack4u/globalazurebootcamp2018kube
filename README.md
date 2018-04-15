# Global Azure Bootcamp - Kubernetes labs

This repository contains a sample app that we're going to deploy to a Kubernetes cluster running on Azure as part of the [Info Support](http://www.infosupport.com) hosted [Global Azure Bootcamp](https://global.azurebootcamp.net/).

Prerequisites:
- An [Azure](https://azure.microsoft.com) subscription
- [kubectl](https://kubernetes.io/docs/tasks/tools/install-kubectl/) on your path

Steps:

1. Create Kubernetes cluster
2. Deploy the application

## Create Kubernetes cluster

To make it easier to deploy a Kubernetes cluster to Azure the tools folder contains a copy of the Azure Container Service engine. It includes a kubernetes.json template that can be run through acs-engine to generate an ARM template that will eventually deploy a Kubernetes cluster with 2 Windows Server 1709 and 2 Linux nodes + 1 Linux master node.

It only requires filling in the following fields:

- dnsPrefix - a simple name that will determine the name of your cluster
- adminPassword - this will be the password of the Administrator account on the Windows nodes
- keyData - public part of your SSH key used to authenticate to the cluster
- clientId + secret - Security information specific to your Azure subscription.

Please refer to the [documentation](https://github.com/Azure/acs-engine/blob/master/docs/kubernetes/windows.md) on how you can best fill in these variables and leave the rest of the variables as they are (since they have been tested ;)).

After the cluster has been successfully created, merge the configuration file created by acs-engine into your kubeconfig using the following command:

```bash
KUBECONFIG=~/.kube/config:<path-to-repo>/tools/_output/<dns-prefix>/kubeconfig/kubeconfig.westeurope.json kubectl config view --flatten > ~/.kube/config 
```

Then make sure that everything works by running the following command:

```bash
kubectl get nodes
```

which should result in output similar to:

```
NAME                        STATUS    ROLES     AGE       VERSION
32416k8s9000                Ready     <none>    18h       v1.9.6
32416k8s9001                Ready     <none>    18h       v1.9.6
k8s-linuxpool1-32416269-0   Ready     agent     18h       v1.9.6
k8s-linuxpool1-32416269-1   Ready     agent     18h       v1.9.6
k8s-master-32416269-0       Ready     master    18h       v1.9.6
```

Finally, let's check if you can access the Kubernetes dashboard by running the following command:

```bash
kubectl proxy
```

And then navigating to http://localhost:8001/ui. The Kubernetes dashboard should now appear. Make sure that you keep the above command running for the remainder of the labs, since it also allows us to check if our individual services are running.

## Deploy the application

The application consists of 3 main parts:
- ShieldHRM WCF service running in IIS on Windows in a container ([jmezach/shieldhrm](https://hub.docker.com/r/jmezach/shieldhrm/)),
- TeamAssembler Backend ASP.NET Core WebAPI running in a Linux container ([jmezach/team-assembler-backend](https://hub.docker.com/r/jmezach/team-assembler-backend/)),
- TeamAssembler Frontend ASP.NET Core MVC app running in a Linux container ([jmezach/team-assembler-frontend](https://hub.docker.com/r/jmezach/team-assembler-frontend/))

Let's deploy these to our Kubernetes cluster:

1. Create a deployment and a service for the ShieldHRM WCF service, making sure the pods are created on the Windows nodes.
2. Make sure the service works by navigating [here](http://localhost:8001/api/v1/namespaces/default/services/shieldhrm-service/proxy/).
3. Next, create a deployment and a service for the backed ASP.NET Core WebAPI. This service requires a persistent volume, so make sure you create that and that you pass the path to the mounted volume through the **OutputFilePath** environment variable. Also ensure that the pods are created on the Linux nodes.
4. Test the service by going [here]( http://localhost:8001/api/v1/namespaces/default/services/shieldhrm-service/proxy/). You should get an empty JSON array back.
5. Finally create a deployment and service for the frontend ASP.NET Core MVC app. Pass the endpoints to the ShieldHRM and BackEnd services through the **ShieldHrmEndpoint** and **BackendEndpoint** environment variables. Also make sure that the frontends are exposed to the internet through a load balancer.
6. Check that everything is working by going to the public IP of the load balancer created in the previous step.