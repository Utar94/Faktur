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

describe("urlUtils.UrlBuilder.authority", () => {
  it.concurrent("should return the correct authority", () => {
    const builder = new UrlBuilder();
    expect(builder.getAuthority()).toBe("localhost:80");
    builder.setCredentials(new Credentials("admin", "Test123!"));
    expect(builder.getAuthority()).toBe("admin:Test123!@localhost:80");
  });

  it.concurrent("should set the correct host", () => {
    const builder = new UrlBuilder();
    builder.setAuthority("www.test.com");
    expect(builder.getCredentials()).toBeUndefined();
    expect(builder.getHost()).toBe("www.test.com");
    expect(builder.getPort()).toBe(80);
  });

  it.concurrent("should set the correct host and port", () => {
    const builder = new UrlBuilder();
    builder.setAuthority("www.test.com:8080");
    expect(builder.getCredentials()).toBeUndefined();
    expect(builder.getHost()).toBe("www.test.com");
    expect(builder.getPort()).toBe(8080);
  });

  it.concurrent("should set the correct host, port and credentials", () => {
    const builder = new UrlBuilder();
    builder.setAuthority("admin:Test123!@www.test.com:8080");
    expect(builder.getCredentials()?.getIdentifier()).toBe("admin");
    expect(builder.getCredentials()?.getSecret()).toBe("Test123!");
    expect(builder.getHost()).toBe("www.test.com");
    expect(builder.getPort()).toBe(8080);
  });

  it.concurrent("should throw an error when the authority is not valid", () => {
    const builder = new UrlBuilder();
    expect(() => builder.setAuthority("admin:P@s$W0rD@www.test.com")).toThrow("The value 'admin:P@s$W0rD@www.test.com' is not a valid URL authority.");
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

describe("urlUtils.UrlBuilder.credentials", () => {
  it.concurrent("should set the correct credentials", () => {
    const builder = new UrlBuilder();
    const credentials = new Credentials("admin", "Test123!");
    builder.setCredentials(credentials);
    expect(builder.getCredentials()?.getIdentifier()).toBe(credentials.getIdentifier());
    expect(builder.getCredentials()?.getSecret()).toBe(credentials.getSecret());
    builder.setCredentials();
    expect(builder.getCredentials()).toBeUndefined();
  });
});

describe("urlUtils.UrlBuilder.fragment", () => {
  it.concurrent("should empty the fragment", () => {
    const builder = new UrlBuilder();
    builder.setFragment("#index");
    builder.setFragment("");
    expect(builder.getFragment()).toBeUndefined();
    builder.setFragment("#index");
    builder.setFragment("  ");
    expect(builder.getFragment()).toBeUndefined();
    builder.setFragment("#index");
    builder.setFragment(" # ");
    expect(builder.getFragment()).toBeUndefined();
  });

  it.concurrent("should set the correct fragment", () => {
    const builder = new UrlBuilder();
    builder.setFragment("#index");
    expect(builder.getFragment()).toBe("#index");
    builder.setFragment(" #  index   ");
    expect(builder.getFragment()).toBe("#index");
  });
});

describe("urlUtils.UrlBuilder.host", () => {
  it.concurrent("should set the correct host", () => {
    const builder = new UrlBuilder();
    builder.setHost("www.test.com");
    expect(builder.getHost()).toBe("www.test.com");
    builder.setHost("  www.test.com  ");
    expect(builder.getHost()).toBe("www.test.com");
  });

  it.concurrent("should set the default host", () => {
    const builder = new UrlBuilder();
    builder.setHost("");
    expect(builder.getHost()).toBe(UrlBuilder.DEFAULT_HOST);
    builder.setHost("  ");
    expect(builder.getHost()).toBe(UrlBuilder.DEFAULT_HOST);
  });
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

describe("urlUtils.UrlBuilder.parameter", () => {
  it.concurrent("should remove an empty parameter", () => {
    const builder = new UrlBuilder();
    builder.setParameter("id", "abc123");
    builder.setParameter("id");
    builder.setParameter("storeId", "maxi");
    builder.setParameter("storeId", "");
    builder.setParameter("articleId", "pain");
    builder.setParameter("articleId", "  ");
  });

  it.concurrent("should set the correct parameter", () => {
    const builder = new UrlBuilder();
    builder.setParameter("id", "test");
    builder.setParameter(" id ", "  abc123  ");
    expect(builder.getParameters().size).toBe(1);
    expect(builder.getParameters().get("id")).toBe("abc123");
  });

  it.concurrent("should throw an error when the key is empty", () => {
    const builder = new UrlBuilder();
    expect(() => builder.setParameter("", "abc123")).toThrowError("The parameter key is required.");
    expect(() => builder.setParameter("  ", "abc123")).toThrowError("The parameter key is required.");
  });
});

describe("urlUtils.UrlBuilder.path", () => {
  it.concurrent("should return the correct path", () => {
    const builder = new UrlBuilder();
    builder.setSegments(["test", "", " page ", "  "]);
    expect(builder.getPath()).toBe("/test/page");
  });

  it.concurrent("should set the correct segments", () => {
    const builder = new UrlBuilder();
    builder.setPath("/test// page /  ");
    expect(builder.getPath()).toBe("/test/page");
    expect(builder.getSegments().length).toBe(2);
    expect(builder.getSegments()[0]).toBe("test");
    expect(builder.getSegments()[1]).toBe("page");
  });
});

describe("urlUtils.UrlBuilder.port", () => {
  it.concurrent("should set the specified port", () => {
    const builder = new UrlBuilder();
    builder.setPort(8080);
    expect(builder.getPort()).toBe(8080);
  });

  it.concurrent("should throw an error when the port is not correct", () => {
    const builder = new UrlBuilder();
    expect(() => builder.setPort(-80)).toThrowError("The port '-80' must be a value between 0 and 65535.");
    expect(() => builder.setPort(808080)).toThrowError("The port '808080' must be a value between 0 and 65535.");
    expect(() => builder.setPort(Number("not_a_number"))).toThrowError("The port 'NaN' must be a value between 0 and 65535.");
  });
});

describe("urlUtils.UrlBuilder.query", () => {
  it.concurrent("should add the correct query value", () => {
    const builder = new UrlBuilder();
    builder.addQuery("id", "abc");
    builder.addQuery("id", " 123 ");
    builder.addQuery("id", "");
    builder.addQuery("id", "  ");
    expect(builder.getQuery().size).toBe(1);
    const ids: string[] | undefined = builder.getQuery().get("id");
    if (ids) {
      expect(ids.length).toBe(2);
      expect(ids[0]).toBe("abc");
      expect(ids[1]).toBe("123");
    } else {
      expect(ids).toBeDefined();
    }
  });

  it.concurrent("should add the correct query value", () => {
    const builder = new UrlBuilder();
    builder.addQuery("id", ["", "abc", "  ", " 123 "]);
    const ids: string[] | undefined = builder.getQuery().get("id");
    if (ids) {
      expect(ids.length).toBe(2);
      expect(ids[0]).toBe("abc");
      expect(ids[1]).toBe("123");
    } else {
      expect(ids).toBeDefined();
    }
  });

  it.concurrent("should return the correct query string", () => {
    const builder = new UrlBuilder();
    builder.addQuery("search_terms", "");
    builder.addQuery("search_operator", "And");
    builder.addQuery("sort", "DisplayName");
    builder.addQuery("sort", "DESC.UpdatedOn");
    expect(builder.getQueryString()).toBe("?search_operator=And&sort=DisplayName&sort=DESC.UpdatedOn");
  });

  it.concurrent("should set the correct query string", () => {
    const builder = new UrlBuilder({ queryString: " ? search_operator=And& search_terms =&sort=DisplayName &sort=DESC.UpdatedOn" });
    expect(builder.getQuery().size).toBe(2);
    const searchOperator: string[] | undefined = builder.getQuery().get("search_operator");
    if (searchOperator) {
      expect(searchOperator.length).toBe(1);
      expect(searchOperator[0]).toBe("And");
    } else {
      expect(searchOperator).toBeDefined();
    }
    const sort: string[] | undefined = builder.getQuery().get("sort");
    if (sort) {
      expect(sort.length).toBe(2);
      expect(sort[0]).toBe("DisplayName");
      expect(sort[1]).toBe("DESC.UpdatedOn");
    } else {
      expect(sort).toBeDefined();
    }
  });

  it.concurrent("should set the correct query value", () => {
    const builder = new UrlBuilder();
    builder.setQuery("id", "");
    builder.setQuery("id", "  ");
    builder.setQuery("id", "abc");
    builder.setQuery("id", " 123 ");
    expect(builder.getQuery().size).toBe(1);
    const ids: string[] | undefined = builder.getQuery().get("id");
    if (ids) {
      expect(ids.length).toBe(1);
      expect(ids[0]).toBe("123");
    } else {
      expect(ids).toBeDefined();
    }
  });

  it.concurrent("should set the correct query values", () => {
    const builder = new UrlBuilder();
    builder.setQuery("id", ["", "abc"]);
    builder.setQuery("id", ["  ", " 123 "]);
    const ids: string[] | undefined = builder.getQuery().get("id");
    if (ids) {
      expect(ids.length).toBe(1);
      expect(ids[0]).toBe("123");
    } else {
      expect(ids).toBeDefined();
    }
  });
});

describe("urlUtils.UrlBuilder.scheme", () => {
  it.concurrent("should set the correct scheme", () => {
    const builder = new UrlBuilder();
    builder.setScheme(" HTTP ");
    expect(builder.getScheme()).toBe("http");
    expect(builder.getPort()).toBe(80);
  });

  it.concurrent("should set the correct scheme and infer the correct port", () => {
    const builder = new UrlBuilder();
    builder.setScheme(" HTTPS ", true);
    expect(builder.getScheme()).toBe("https");
    expect(builder.getPort()).toBe(443);
  });

  it.concurrent("should throw an error when the scheme is not supported", () => {
    const builder = new UrlBuilder();
    expect(() => builder.setScheme("ftp")).toThrowError("The scheme 'ftp' is not supported.");
  });
});

describe("urlUtils.UrlBuilder.segments", () => {
  it.concurrent("should set the correct segments", () => {
    const builder = new UrlBuilder();
    builder.setSegments(["", "  ", "test", "  page  "]);
    expect(builder.getSegments().length).toBe(2);
    expect(builder.getSegments()[0]).toBe("test");
    expect(builder.getSegments()[1]).toBe("page");
    builder.setSegments([]);
    expect(builder.getSegments().length).toBe(0);
  });
});
