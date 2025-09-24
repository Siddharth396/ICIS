# Backend for Frontend
The Backend for Frontend (BFF) is a ASP.NET Core web service which exposes a [GraphQL](https://graphql.org/) schema for one web application/component to fetch the required information.

## Dependencies

Following is the [ASCII diagram](http://asciiflow.com/) of the dependencies:

```

Subscriber BFF

┌─────────────────┐           ┌───────────┐       ┌──────────────┐
│                 │           │           │       │              │
│ Identity Server │◄──────────┤ AuthProxy ├──────►│      BFF     │
│                 │           │           │       │              │
└─────────┬───────┘           └─────┬─────┘       └───────┬──────┘
          │                         │                     │
    ┌─────┴───┐                ┌────┴────┐          ┌─────┴───┐
    │         │                │         │          │         │
    │ MS SQL  │                │ MongoDB │          │ MongoDB │
    │         │                │         │          │         │
    └─────────┘                └─────────┘          └─────────┘


Authoring BFF

┌──────────────┐         ┌───────────────────┐           ┌───────────┐       ┌──────────────┐
│              │         │                   │           │           │       │              │
│     ADFS     │◄────────│  Domain Identity  │◄──────────│ AuthProxy │──────►│      BFF     │
│              │         │                   │           │           │       │              │
└──────────────┘         └───────────────────┘           └─────┬─────┘       └───────┬──────┘
                                                               │                     │
                                                               │                     │
                                                          ┌────┴────┐           ┌────┴────┐
                                                          │         │           │         │
                                                          │ MongoDB │           │ MongoDB │
                                                          │         │           │         │
                                                          └─────────┘           └─────────┘
```
