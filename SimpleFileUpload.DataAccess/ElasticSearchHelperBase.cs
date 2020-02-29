﻿using Nest;
using SimpleFileUpload.Entity;
using System;
using System.Collections.Generic;
using System.Text;

namespace SimpleFileUpload.DataAccess
{
	public class ElasticSearchBase<T> where T : class, IElasticIndexModel
	{
		protected readonly ConnectionSettings ConnectionSettings;
		protected readonly ElasticClient ElasticClient;
		protected readonly string IndexName;
		public ElasticSearchBase(string indexName)
		{
			IndexName = indexName;
			ConnectionSettings = new ConnectionSettings(new Uri("http://localhost:9200/")).DefaultIndex(indexName);
			ElasticClient = new ElasticClient(ConnectionSettings);
		}
		//public virtual bool IndexExists()
		//{
		//	ElasticClient.exists
		//}

		public virtual void Index(T item, string indexName)
		{
			//var status = ElasticClient.Index<T>(item, indexDescriptor =>
			//{
			//	indexDescriptor.Index(indexName);
			//	indexDescriptor.Id(item.Id);
			//	return indexDescriptor;
			//});
			var response = ElasticClient.IndexDocument<T>(item);
		}

		public virtual void IndexBulk(IEnumerable<T> items)
		{
			var response = ElasticClient.IndexMany(items, IndexName);
			//var descriptor = new BulkDescriptor();
			//foreach (var item in items)
			//{
			//	descriptor.Index<T>(op => op.Document(item));
			//}
			//var result = ElasticClient.Bulk(descriptor);
		}

		public virtual T Find(int id)
		{
			var response = ElasticClient.Get<T>(id, finder =>
			{
				finder.Index(IndexName);
				return finder;
			});
			return response.Source;
		}

		public virtual IEnumerable<T> GetAll()
		{
			var count = ElasticClient.Count<T>().Count;
			var response = ElasticClient.Search<T>(s => s
					.Index(IndexName)
					.From(0)
					.Size(Convert.ToInt32(count)/40)
					.MatchAll()
					);
			return response.Documents;
		}

	}
}