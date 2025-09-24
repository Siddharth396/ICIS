
export const validateNamespace = (parsedData) => {
    const namespaces = parsedData.filter(x => x.kind === "Namespace").map(x => x.metadata.name);
    let missingNameSpaces = [];
    parsedData.forEach((item, index) => {
        if (item.metadata && item.metadata.namespace) {
            if (!namespaces.includes(item.metadata.namespace)) {
                missingNameSpaces.push(item.metadata.namespace);
            }
        }
    });
    missingNameSpaces = missingNameSpaces.filter(x => x !== "flux-system" && x !== "default" && x !== "monitoring" && x !== "logging" && x !== "kube-system" && x !== "verify-systest");
    if (missingNameSpaces.length > 0) {
        console.log(`Missing Namespaces: ${missingNameSpaces.join(', ')}`);
        throw new Error(`Missing Namespaces: ${missingNameSpaces.join(', ')}`);
    }
}

