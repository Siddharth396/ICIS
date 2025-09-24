

const fetchImage = async (image) => {
    const { repository, tag } = image;
    const server = repository.split('/')[0];
    const path = repository.split('/').slice(1).join('/');

    const searchObject = {
        "action": "coreui_Search",
        "method": "read",
        "data": [
            {
                "formatSearch": false,
                "page": 1,
                "start": 0,
                "limit": 300,
                "filter": [
                    {
                        "property": "attributes.docker.imageName",
                        "value": path
                    },
                    {
                        "property": "attributes.docker.imageTag",
                        "value": tag
                    }
                ]
            }
        ],
        "type": "rpc",
        "tid": 6
    };
    try {
        const fetched = await fetch(`https://${server}/service/extdirect`, {
            "headers": {
                "accept": "*/*",
                "accept-language": "en-GB,en-US;q=0.9,en;q=0.8",
                "content-type": "application/json",
                "nx-anti-csrf-token": "0.6823515615187555",
                "x-nexus-ui": "true",
                "x-requested-with": "XMLHttpRequest"
            },
            "referrer": `https://${server}/`,
            "referrerPolicy": "strict-origin-when-cross-origin",
            "body": JSON.stringify(searchObject),
            "method": "POST",
            "mode": "cors",
            "credentials": "include"
        });
        return await fetched.json();
    } catch (error) {
        console.error(error);
        throw new Error(`Error fetching image: ${image.repository}:${image.tag}`);
    }
};
export const validateImages = async (parsedData) => {
    let missingImages = [];
    let imagesToCheck = [];
    parsedData.forEach((item, index) => {
        if (item.kind === "HelmRelease") {
            const values = item.spec.values;
            for (const key in values) {
                const itemDeploy = values[key];
                for (const keyDeployment in itemDeploy) {
                    if (keyDeployment === "image") {
                        const image = itemDeploy[keyDeployment];
                        if (image) {
                            imagesToCheck.push({ image, namespace: item.metadata.namespace });
                        }
                    }
                }
            }
        }
    });

    // get unique images in array 
    const uniqueImages = Array.from(new Set(imagesToCheck.map(a => a.image)))
        .map(image => {
            return imagesToCheck.find(a => a.image === image);
        });

    for (const item of uniqueImages) {
        const data = await fetchImage(item.image);
        if (!data || !data.result || !data.result.data || data.result.data.length === 0) {
            missingImages.push(`NameSpace: ${item.namespace}, Image: ${item.image.repository}:${item.image.tag}`);
        }
    }
    if (missingImages.length > 0) {
        console.log(`Missing Images: ${missingImages.join(', ')}, please push them first!`);
        throw new Error(`Missing Images: ${missingImages.join(', ')}, please push them first!`);
    }
};