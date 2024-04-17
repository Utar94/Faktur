import { describe, it, expect } from "vitest";

import { Credentials, UrlBuilder, type UrlOptions } from "../urlUtils";

const options: UrlOptions = {
  scheme: " HTTP ",
  host: " host.docker.internal ",
  port: 8080,
  path: "//articles//sku:{sku}//",
  queryString: "?version=42&test=",
  fragment: "#title",
  credentials: new Credentials("admin", "Test123!"),
};

describe("urlUtils.Credentials.constructor", () => {
  it.concurrent("should create the correct Credentials", () => {
    let credentials = new Credentials();
    expect(credentials.getIdentifier()).toBe("");
    expect(credentials.getSecret()).toBe("");

    credentials = new Credentials("admin");
    expect(credentials.getIdentifier()).toBe("admin");
    expect(credentials.getSecret()).toBe("");

    credentials = new Credentials("admin", "Test123!");
    expect(credentials.getIdentifier()).toBe("admin");
    expect(credentials.getSecret()).toBe("Test123!");
  });
});

describe("urlUtils.Credentials.setIdentifier", () => {
  it.concurrent("should set the correct identifier", () => {
    const credentials = new Credentials();
    credentials.setIdentifier("admin");
    expect(credentials.getIdentifier()).toBe("admin");
  });
});

describe("urlUtils.Credentials.setSecret", () => {
  it.concurrent("should set the correct secret", () => {
    const credentials = new Credentials();
    credentials.setSecret("Test123!");
    expect(credentials.getSecret()).toBe("Test123!");
  });
});

describe("urlUtils.Credentials.parse", () => {
  it.concurrent("should return the correct parsed credentials with secret", () => {
    const credentials = Credentials.parse("admin:Test123!");
    expect(credentials?.getIdentifier()).toBe("admin");
    expect(credentials?.getSecret()).toBe("Test123!");
  });

  it.concurrent("should return the correct parsed credentials without secret", () => {
    const credentials = Credentials.parse("admin");
    expect(credentials?.getIdentifier()).toBe("admin");
    expect(credentials?.getSecret()).toBe("");
  });

  it.concurrent("should return undefined when the credentials are null, empty or white space", () => {
    expect(Credentials.parse(undefined)).toBeUndefined();
    expect(Credentials.parse("")).toBeUndefined();
    expect(Credentials.parse("    ")).toBeUndefined();
  });
});

describe("urlUtils.UrlBuilder.build", () => {
  it.concurrent("should build the correct absolute URL", () => {
    const sku = "06900000425";
    const expected = `http://admin:Test123!@host.docker.internal:8080/articles/sku:${sku}?version=42#title`;

    const builder = new UrlBuilder(options).setParameter("sku", sku);

    let url: string = builder.build();
    expect(url).toBe(expected);

    url = builder.build("Absolute");
    expect(url).toBe(expected);

    url = builder.buildAbsolute();
    expect(url).toBe(expected);
  });

  it.concurrent("should build the correct default URL", () => {
    const builder = new UrlBuilder();

    expect(builder.build()).toBe("http://localhost:80");
    expect(builder.build("Absolute")).toBe("http://localhost:80");
    expect(builder.buildAbsolute()).toBe("http://localhost:80");
    expect(builder.build("Relative")).toBe("");
    expect(builder.buildRelative()).toBe("");
  });

  it.concurrent("should build the correct relative URL", () => {
    const sku = "06900000425";
    const expected = `/articles/sku:${sku}?version=42#title`;

    const builder = new UrlBuilder(options).setParameter("sku", sku);

    let url: string = builder.build("Relative");
    expect(url).toBe(expected);

    url = builder.buildRelative();
    expect(url).toBe(expected);
  });
});

describe("urlUtils.UrlBuilder.constructor", () => {
  it.concurrent("should construct the correct UrlBuilder with options", () => {
    const builder = new UrlBuilder(options);
    expect(builder.getScheme()).toBe(options.scheme?.trim().toLowerCase());
    expect(builder.getHost()).toBe(options.host?.trim());
    expect(builder.getPort()).toBe(options.port);
    expect(builder.getPath()).toBe("/articles/sku:{sku}");
    expect(builder.getQueryString()).toBe("?version=42");
    expect(builder.getFragment()).toBe("#title");
    expect(builder.getCredentials()?.getIdentifier()).toBe(options.credentials?.getIdentifier());
    expect(builder.getCredentials()?.getSecret()).toBe(options.credentials?.getSecret());
  });
});

it.concurrent("should construct the correct UrlBuilder without options", () => {
  const builder = new UrlBuilder();
  expect(builder.getScheme()).toBe(UrlBuilder.DEFAULT_SCHEME);
  expect(builder.getCredentials()).toBeUndefined();
  expect(builder.getHost()).toBe(UrlBuilder.DEFAULT_HOST);
  expect(builder.getPort()).toBe(80);
  expect(builder.getPath()).toBeUndefined();
  expect(builder.getQueryString()).toBeUndefined();
  expect(builder.getFragment()).toBeUndefined();
});

describe("urlUtils.UrlBuilder.inferPort", () => {
  it.concurrent("should infer the correct port", () => {
    const builder = new UrlBuilder(options);

    builder.setScheme("http", true);
    expect(builder.getPort()).toBe(80);

    builder.setScheme(" HTTPS ", true);
    expect(builder.getPort()).toBe(443);
  });
});

describe("urlUtils.UrlBuilder.isSchemeSupported", () => {
  it.concurrent("should return false when the scheme is not supported", () => {
    expect(UrlBuilder.isSchemeSupported("")).toBe(false);
    expect(UrlBuilder.isSchemeSupported("  ")).toBe(false);
    expect(UrlBuilder.isSchemeSupported("ftp")).toBe(false);
  });

  it.concurrent("should return true when the scheme is supported", () => {
    expect(UrlBuilder.isSchemeSupported("http")).toBe(true);
    expect(UrlBuilder.isSchemeSupported(" HTTPS ")).toBe(true);
  });
});
