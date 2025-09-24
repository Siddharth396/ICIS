docker build -t kube-ops .
docker tag kube-ops artifacts.cha.rbxd.ds/kube-ops:1.0.4
docker push artifacts.cha.rbxd.ds/kube-ops:1.0.4

#  IN WSL!
# sudo docker run -it --rm -v $(pwd):/app/flux:rw -v $(pwd)/support/cha.sh:/app/cha.sh:rw -w / --entrypoint sh artifacts.cha.rbxd.ds/kube-ops:1.0.3