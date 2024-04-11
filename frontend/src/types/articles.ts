import type { Aggregate } from "./aggregate";
import type { SearchPayload, SortOption } from "./search";

export type Article = Aggregate & {
  gtin?: string;
  displayName: string;
  description?: string;
};

export type ArticleSort = "DisplayName" | "Gtin" | "UpdatedOn";

export type ArticleSortOption = SortOption & {
  field: ArticleSort;
};

export type CreateArticlePayload = {
  gtin?: string;
  displayName: string;
  description?: string;
};

export type ReplaceArticlePayload = {
  gtin?: string;
  displayName: string;
  description?: string;
};

export type SearchArticlesPayload = SearchPayload & {
  sort: ArticleSortOption[];
};
