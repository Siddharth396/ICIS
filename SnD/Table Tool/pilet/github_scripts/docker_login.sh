DOCKERUSERNAME=$1
DOCKERPASSWORD=$2

docker login -u "${DOCKERUSERNAME}" -p "${DOCKERPASSWORD}" "artifacts.cha.rbxd.ds"