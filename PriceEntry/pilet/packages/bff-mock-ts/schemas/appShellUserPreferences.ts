import delayResponse from "../utils/delayResponse";

const preferencesResolver = () => ({
  userLanguage: "en-US",
  hp2Migrate: false,
});

const applicationResolver = () => ({
  toggles: {
    name: "Petchem",
    isOn: true,
  },
});

const userResolver = () => ({
  userId: "test-user-id",
  isInternal: true,
  entitlements: [],
  isNewHomeUser: false,
  isPetchemOnly: false,
  type: "something",
  isLNGReportUser: true,
});

export const definitions = `
  type User {
    userId: String
    isInternal: Boolean
    entitlements: [String]
    isNewHomeUser: Boolean
    isPetchemOnly: Boolean
    type: String
    isLNGReportUser: Boolean
  }

  type Preferences {
    userLanguage: String
    hp2Migrate: Boolean
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
    preferences: () => delayResponse(preferencesResolver(), 1500),
    user: () => delayResponse(userResolver(), 1500),
    application: () => delayResponse(applicationResolver(), 1500),
  },
};
