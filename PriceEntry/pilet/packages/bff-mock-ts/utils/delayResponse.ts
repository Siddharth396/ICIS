/**
 * Return the response after a delay, unless MOCK_BFF_INCLUDE_DELAYS is false in process.env
 */
function delayResponse<R = any>(response: R, delay: number = 0): Promise<R> {
  //@ts-ignore
  const finalDelay = process.env.MOCK_BFF_INCLUDE_DELAYS === "true" ? delay : 0;

  return new Promise((resolve) => {
    setTimeout(() => resolve(response), finalDelay);
  });
}

export default delayResponse;
