function checkEnvVariables(variables) {
  let allVariablesSet = true;
  for (let i = 0; i < variables.length; i++) {
    if (!process.env[variables[i]]) {
      console.log(`${variables[i]} is not set`);
      allVariablesSet = false;
    }
  }
  return allVariablesSet;
}

module.exports = {
  checkEnvVariables,
};
