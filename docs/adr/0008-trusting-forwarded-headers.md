# 0008. Trusting forwarded headers

- Status: Proposed
- Date: 2026-10-02
- Deciders: Victor Adeyemi, Hannah Choi

## Context

The rate limiter keys on `RemoteIpAddress`. Behind App Service that is the front end
address, so every caller shares one bucket. The real address is the first entry in
`X-Forwarded-For`.

Trusting that header without knowing which proxies are in front of us means any caller
can set it and get their own bucket, which is worse than the current behaviour.

## Options considered

### Trust the first entry

One line. Trivially spoofed.

### Trust the last entry added by a known proxy

Correct. Needs a list of our own egress addresses, which changes when the platform
scales.

### Use the platform header

App Service does not give us one we can rely on across all SKUs.

## Decision

Not decided. Blocked until we know whether Front Door is going in front of the API,
because that answers the proxy list question for us.
