FROM ghcr.io/yannh/kubeconform:latest AS kc
FROM alpine:3.20

COPY --from=kc /kubeconform /usr/local/bin/kubeconform
WORKDIR /app
COPY ./cha.sh /app/cha.sh
RUN apk add curl git bash

RUN curl -s "https://raw.githubusercontent.com/kubernetes-sigs/kustomize/master/hack/install_kustomize.sh" | bash
RUN kubeconform --version || true
RUN ./kustomize --version || true
RUN curl -s https://fluxcd.io/install.sh | bash

ENTRYPOINT ["/app/cha.sh"]