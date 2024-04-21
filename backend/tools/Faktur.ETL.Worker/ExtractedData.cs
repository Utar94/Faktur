using Faktur.Contracts.Articles;
using Faktur.Contracts.Banners;
using Faktur.Contracts.Products;
using Faktur.Contracts.Receipts;
using Faktur.Contracts.Stores;

namespace Faktur.ETL.Worker;

internal record ExtractedData(IEnumerable<Article> Articles, IEnumerable<Banner> Banners, IEnumerable<Store> Stores, IEnumerable<Product> Products, IEnumerable<Receipt> Receipts);
