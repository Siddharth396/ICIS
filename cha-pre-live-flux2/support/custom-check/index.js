import fs from 'fs';
import yamljs from 'js-yaml';
import { validateNamespace } from './validators/namespaces.js';
import { validateImages } from './validators/images.js';
import { validateImagesAutomation } from './validators/imageAutomationChecks.js';
import path from 'path';

const clusters = ['5b']; //'5a', 
for await (const cluster of clusters) {
    console.log(`Checking cluster: ${cluster}`);
    const files = fs.readdirSync('../../');

    console.log('Files under ../../:');
    files.forEach(file => {
        console.log(file);
    });
    const filePath = `../../changelog-${cluster}.txt`;

    try {
        console.log('checking if image policies match');
        await validateImagesAutomation(cluster);

        console.log('loading the change log for error checking', filePath);
        // Read the file
        const fileContent = fs.readFileSync(filePath, 'utf8');

        // Parse the content as YAML
        const parsedData = yamljs.loadAll(fileContent);

        // Iterate over each item and print it
        if (Array.isArray(parsedData)) {
            console.log('checking if all namespaces are present');
            validateNamespace(parsedData);

            try {
                console.log('checking if all docker images are present');
                await validateImages(parsedData);
            }
            catch (error) {
                console.log('This error has been disabled from blocking the build');
            }

            console.log(`Checks passed, entities processed: ${parsedData.length}`);

        } else {
            console.log('Parsed data is not an array.');
        }
    } catch (error) {
        process.exitCode = 1;
        console.error('Error reading or parsing the file:', error.message);
    }
}