import delayResponse from "../utils/delayResponse";

const preferencesResolver = () => ({
  userLanguage: "en-US",
  hp2Migrate: false,
  pssSurveyLastSeen: -1,
  pssSurveyLastCompleted: -1,
  __typename: 'Preferences',
});

const applicationResolver = () => ({
  toggles: {
    name: 'Petchem',
    isOn: true
  }
});

const userResolver = () => ({
  userId: '123',
  isInternal: true,
  entitlements: [],
  features: [],
  isNewHomeUser: false,
  isLNGReportUser: false,
  type: 'GenesisHome',
  hasBetaAccess: true,
  __typename: 'User',
});

export const definitions = `
  type User {
    userId: String
    isInternal: Boolean
    entitlements: [String]
    features: [String]
    isNewHomeUser: Boolean
    isPetchemOnly: Boolean
    type: String
    isLNGReportUser: Boolean
  }

  type Preferences {
    userLanguage: String
    hp2Migrate: Boolean
    pssSurveyLastSeen: Int
    pssSurveyLastCompleted: Int
  }
  
  type Toggles {
    name: String
    isOn: Boolean
  }
  type Application {
    toggles: Toggles!
  }
`;

export const query = `
  preferences: Preferences!
  user: User!
  application: Application!
`;
export const resolvers = {
  queries: {
    preferences: (_: any, args: any) => delayResponse(preferencesResolver(), 1500),
    user: (_: any, args: any) => delayResponse(userResolver(), 1500),
    application: (_: any, args: any) => delayResponse(applicationResolver(), 1500),
  },
};
