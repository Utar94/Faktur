import { cleanTrim, isNullOrWhiteSpace, trimStart } from "./stringUtils";

export class Credentials implements ICredentials {
  private identifier: string;
  getIdentifier(): string {
    return this.identifier;
  }
  setIdentifier(identifier: string) {
    this.identifier = identifier;
  }

  private secret: string;
  getSecret(): string {
    return this.secret;
  }
  setSecret(secret: string) {
    this.secret = secret;
  }

  constructor(identifier: string = "", secret: string = "") {
    this.identifier = identifier;
    this.secret = secret;
  }

  static parse(credentials?: string): Credentials | undefined {
    credentials = cleanTrim(credentials);
    if (typeof credentials !== "string") {
      return undefined;
    }

    const index: number = credentials.indexOf(":");
    if (index < 0) {
      return new Credentials(credentials);
    }
    return new Credentials(credentials.substring(0, index), credentials.substring(index + 1));
  }
}

export interface ICredentials {
  getIdentifier(): string;
  getSecret(): string;
}

export interface IUrlBuilder {
  getScheme(): string;
  setScheme(scheme: string, inferPort?: boolean): IUrlBuilder;

  getCredentials(): ICredentials | undefined;
  setCredentials(credentials?: ICredentials): IUrlBuilder;
  getHost(): string;
  setHost(host: string): IUrlBuilder;
  getPort(): number;
  setPort(port: number): IUrlBuilder;
  getAuthority(): string;
  setAuthority(authority: string): IUrlBuilder;

  getSegments(): string[];
  setSegments(segments: string[]): IUrlBuilder;
  getPath(): string | undefined;
  setPath(path?: string): IUrlBuilder;

  getQuery(): Map<string, string[]>;
  getQueryString(): string | undefined;
  addQuery(key: string, value: string): IUrlBuilder;
  addQuery(key: string, values: string[]): IUrlBuilder;
  setQuery(key: string, value: string): IUrlBuilder;
  setQuery(key: string, values: string[]): IUrlBuilder;
  setQueryString(queryString?: string): IUrlBuilder;

  getFragment(): string | undefined;
  setFragment(fragment?: string): IUrlBuilder;

  getParameters(): Map<string, string>;
  setParameter(key: string, value?: string): IUrlBuilder;

  build(kind?: UriKind): string;
  buildAbsolute(): string;
  buildRelative(): string;
}

export type UriKind = "Absolute" | "Relative";

export class UrlBuilder implements IUrlBuilder {
  static readonly DEFAULT_SCHEME = "http";
  static readonly DEFAULT_HOST = "localhost";

  private static supportedSchemes: Set<string> = new Set<string>(["http", "https"]);
  static getSupportedSchemes(): string[] {
    return [...UrlBuilder.supportedSchemes];
  }
  static isSchemeSupported(scheme: string): boolean {
    return UrlBuilder.supportedSchemes.has(scheme.trim().toLowerCase());
  }

  private scheme: string = UrlBuilder.DEFAULT_SCHEME;
  getScheme(): string {
    return this.scheme;
  }
  setScheme(scheme: string, inferPort?: boolean): IUrlBuilder {
    if (!UrlBuilder.isSchemeSupported(scheme)) {
      throw new Error(`The scheme '${scheme}' is not supported.`);
    }
    this.scheme = scheme.trim().toLowerCase();
    if (inferPort) {
      this.port = UrlBuilder.inferPort(scheme);
    }
    return this;
  }

  private credentials?: ICredentials;
  getCredentials(): ICredentials | undefined {
    return this.credentials;
  }
  setCredentials(credentials?: ICredentials): IUrlBuilder {
    this.credentials = credentials;
    return this;
  }
  private host: string = UrlBuilder.DEFAULT_HOST;
  getHost(): string {
    return this.host;
  }
  setHost(host: string): IUrlBuilder {
    this.host = cleanTrim(host) ?? UrlBuilder.DEFAULT_HOST;
    return this;
  }
  private port: number = 80;
  getPort(): number {
    return this.port;
  }
  setPort(port: number): IUrlBuilder {
    if (isNaN(port) || port < 0 || port > 65535) {
      throw new Error(`The port '${port}' must be a value between 0 and 65535.`);
    }
    this.port = port;
    return this;
  }
  getAuthority(): string {
    let authority = "";
    if (this.credentials) {
      authority += `${this.credentials.getIdentifier()}:${this.credentials.getSecret()}@`;
    }
    authority += `${this.host}:${this.port}`;
    return authority;
  }
  setAuthority(authority: string): IUrlBuilder {
    const parts: string[] = authority.split("@");
    if (parts.length > 2) {
      throw new Error(`The value '${authority}' is not a valid URL authority.`);
    } else if (parts.length === 2) {
      this.setCredentials(Credentials.parse(parts[0]));
    }

    const endPoint: string = parts[parts.length - 1];
    const index: number = endPoint.indexOf(":");
    if (index < 0) {
      this.setHost(endPoint);
    } else {
      this.setHost(endPoint.substring(0, index));
      this.setPort(Number(endPoint.substring(index + 1)));
    }

    return this;
  }

  private segments: string[] = [];
  getSegments(): string[] {
    return [...this.segments];
  }
  setSegments(segments: string[]): IUrlBuilder {
    this.segments.length = 0;
    segments.forEach((segment) => {
      if (!isNullOrWhiteSpace(segment)) {
        this.segments.push(segment.trim());
      }
    });
    return this;
  }
  getPath(): string | undefined {
    return this.segments.length === 0 ? undefined : `/${this.segments.join("/")}`;
  }
  setPath(path?: string): IUrlBuilder {
    this.setSegments(path?.split("/") ?? []);
    return this;
  }

  private query: Map<string, string[]> = new Map<string, string[]>();
  getQuery(): Map<string, string[]> {
    return new Map<string, string[]>(this.query);
  }
  getQueryString(): string | undefined {
    if (this.query.size === 0) {
      return undefined;
    }
    const parameters: string[] = [];
    this.query.forEach((values, key) => values.forEach((value) => parameters.push([key, value].join("="))));
    return `?${parameters.join("&")}`;
  }
  addQuery(key: string, values: string | string[]): IUrlBuilder {
    if (typeof values === "string") {
      return this.addQuery(key, [values]);
    }
    if (!isNullOrWhiteSpace(key)) {
      key = key.trim();
      const existingValues: string[] = this.query.get(key) ?? [];
      values.forEach((value) => {
        if (!isNullOrWhiteSpace(value)) {
          existingValues.push(value.trim());
        }
      });
      this.setQuery(key, existingValues);
    }
    return this;
  }
  setQuery(key: string, values: string | string[]): IUrlBuilder {
    if (typeof values === "string") {
      return this.setQuery(key, [values]);
    }
    if (!isNullOrWhiteSpace(key)) {
      key = key.trim();
      const newValues: string[] = [];
      values.forEach((value) => {
        if (!isNullOrWhiteSpace(value)) {
          newValues.push(value.trim());
        }
      });
      if (newValues.length > 0) {
        this.query.set(key, newValues);
      } else {
        this.query.delete(key);
      }
    }
    return this;
  }
  setQueryString(queryString?: string): IUrlBuilder {
    queryString = cleanTrim(trimStart(queryString?.trim() ?? "", "?"));
    this.query.clear();
    if (typeof queryString === "string") {
      const parameters: string[] = queryString.split("&");
      parameters.forEach((parameter) => {
        const index: number = parameter.indexOf("=");
        if (index >= 0) {
          this.addQuery(parameter.substring(0, index), parameter.substring(index + 1));
        }
      });
    }
    return this;
  }

  private fragment?: string;
  getFragment(): string | undefined {
    return this.fragment;
  }
  setFragment(fragment?: string): IUrlBuilder {
    fragment = cleanTrim(trimStart(fragment?.trim() ?? "", "#"));
    this.fragment = typeof fragment !== "string" ? undefined : `#${fragment}`;
    return this;
  }

  private parameters: Map<string, string> = new Map<string, string>();
  getParameters(): Map<string, string> {
    return new Map<string, string>(this.parameters);
  }
  setParameter(key: string, value?: string): IUrlBuilder {
    if (isNullOrWhiteSpace(key)) {
      throw new Error("The parameter key is required.");
    }
    key = key.trim();
    if (isNullOrWhiteSpace(value)) {
      this.parameters.delete(key);
    } else {
      this.parameters.set(key, value?.trim() ?? "");
    }
    return this;
  }

  constructor(options?: UrlOptions) {
    options = options ?? {};
    if (options.scheme) {
      this.setScheme(options.scheme, true);
    }
    if (options.host) {
      this.setHost(options.host);
    }
    if (options.port) {
      this.setPort(options.port);
    }
    if (options.path) {
      this.setPath(options.path);
    }
    if (options.queryString) {
      this.setQueryString(options.queryString);
    }
    if (options.fragment) {
      this.setFragment(options.fragment);
    }
    if (options.credentials) {
      this.setCredentials(options.credentials);
    }
  }

  build(kind: UriKind = "Absolute"): string {
    let url: string = "";
    if (kind === "Absolute") {
      url += `${this.scheme}://${this.getAuthority()}`;
    }
    const path: string | undefined = this.getPath();
    if (typeof path === "string") {
      url += path;
    }
    const queryString: string | undefined = this.getQueryString();
    if (typeof queryString === "string") {
      url += queryString;
    }
    const fragment: string | undefined = this.getFragment();
    if (typeof fragment === "string") {
      url += fragment;
    }
    this.parameters.forEach((value, key) => {
      const pattern: string = `\\{${key}\\}`;
      url = url.replace(new RegExp(pattern, "g"), value);
    });
    return url;
  }
  buildAbsolute(): string {
    return this.build("Absolute");
  }
  buildRelative(): string {
    return this.build("Relative");
  }

  private static inferPort(scheme: string): number {
    switch (scheme.trim().toLowerCase()) {
      case "https":
        return 443;
      default:
        return 80;
    }
  }
}

export type UrlOptions = {
  scheme?: string;
  host?: string;
  port?: number;
  path?: string;
  queryString?: string;
  fragment?: string;
  credentials?: ICredentials;
};
