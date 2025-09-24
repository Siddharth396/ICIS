const queryResponseData = (args: any) => {
  return {
    filter: JSON.stringify({
      region: "Europe",
      product: "Styrene",
      tableType: "Outages"
    })
  };
};

const mutationResponseData = (args: any) => {
  return {
    contentBlockId: '1',
    version: '1.1',
  };
};

const populateData = {
  queryResponseData,
  mutationResponseData
}

export default populateData;
