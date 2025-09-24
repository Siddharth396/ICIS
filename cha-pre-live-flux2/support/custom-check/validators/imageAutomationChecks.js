import fs from 'fs';
import yaml from 'yaml';
import fse from 'fs-extra';

const walkSync = (dir, fileList = []) => {
    fs.readdirSync(dir).forEach(file => {
        const filePath = `${dir}/${file}`;
        if (fs.statSync(filePath).isDirectory()) {
            walkSync(filePath, fileList);
        } else if (file.endsWith('.yaml')) {
            fileList.push(filePath);
        }
    });
    return fileList;
};
const loadImagePolicies = async (cluster) => {
    const dirPath = `../../cha-eks-${cluster}/automations/images`;

    const files = walkSync(dirPath);
    let helmFilesWithAutomation = [];
    for await (const file of files) {
        if (file.includes('kustomization.yaml')) {
            continue;
        }
        const buffer = await fse.readFile(file);
        const strBuffer = buffer.toString();
        const yamlDocs = yaml.parseAllDocuments(strBuffer);
        const first = yamlDocs[0];
        if (!first) {
            continue;
        }

        const kind = Array.from(first.contents.items).find(x => x.key.value === "kind");
        if (!kind || kind.value.value !== "ImagePolicy") {
            continue;
        }
        const metadata = Array.from(first.contents.items).find(x => x.key.value === "metadata");
        if (!metadata) {
            continue;
        }

        // get name
        const imagePolicyName = metadata.value.items.find(x => x.key.value === "name").value.value;
        helmFilesWithAutomation.push({ imagePolicyName });
    }
    helmFilesWithAutomation = helmFilesWithAutomation.filter((value, index, self) =>
        index === self.findIndex((t) => t.imagePolicyName === value.imagePolicyName)
    );
    return helmFilesWithAutomation;
};

const loadHelmFiles = async (cluster) => {
    const dirPath = `../../cha-eks-${cluster}/deployments`;
    const files = walkSync(dirPath);
    let helmFilesWithAutomation = [];
    for await (const file of files) {
        if (file.includes('kustomization.yaml') || file.includes('sboutg') || file.includes('ssplat') || file.includes('verify')) {
            continue;
        }
        const buffer = await fse.readFile(file);
        const strBuffer = buffer.toString();
        const yamlDocs = yaml.parseAllDocuments(strBuffer);
        const first = yamlDocs[0];
        if (!first) {
            continue;
        }

        const spec = Array.from(first.contents.items).find(x => x.key.value === "spec");
        if (!spec) {
            continue;
        }
        const valuesFromSpec = Array.from(spec.value.items).find(x => x.key.value === "values");
        if (!valuesFromSpec) {
            continue;
        }

        const deployables = valuesFromSpec.value.items.filter(x => x.value.items && x.value.items.find(y => y.key && y.key.value === "image"));

        for (const item of deployables) {
            // get the image object first
            const imageConf = item.value.items.find(x => x.key.value === "image");
            const repoConf = imageConf.value.items.find(x => x.key.value === "repository");
            // we need to check if the repo has a comment, if it does we need to add it to a list
            if (repoConf.value.comment) {
                const comment = JSON.parse(repoConf.value.comment);
                if (comment) {
                    const imagePolicyName = comment['$imagepolicy'].replace('flux-system:', '').replace(':name', '');
                    helmFilesWithAutomation.push({ file, imagePolicyName });
                }
            }
        }
    }
    helmFilesWithAutomation = helmFilesWithAutomation.filter((value, index, self) =>
        index === self.findIndex((t) => t.imagePolicyName === value.imagePolicyName)
    );
    return helmFilesWithAutomation;
};
export const validateImagesAutomation = async (cluster) => {
    // we need to load all the helm files and all the image policies as yaml docs
    const imagePolicies = await loadImagePolicies(cluster);
    const helmFiles = await loadHelmFiles(cluster);

    const missingPolicies = [];
    for (const helmFile of helmFiles) {
        // find matching image policy
        const matchingPolicy = imagePolicies.find(policy => policy.imagePolicyName === helmFile.imagePolicyName);
        if (!matchingPolicy) {
            // do something with the matching policy
            missingPolicies.push(helmFile);
        }
    }

    if (missingPolicies.length > 0) {
        // do something with the missing policies
        console.error("Missing image policies:", missingPolicies);
        throw "Missing image policies! They are missing and you will not auto deploy!";
    }
};