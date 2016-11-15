using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.QueryParsers;
using Lucene.Net.Search;
using Lucene.Net.Store;
using Version = Lucene.Net.Util.Version;
using System.IO;
using FileDB = SearchDemo.Data.DemoDB.File;

namespace SearchDemo.BLL.Services.ServiceImpl
{
    public static class LuceneFileSearch
    {
        #region Field
        private static string _luceneDir = AppDomain.CurrentDomain.BaseDirectory + @"\" + "lucene_index";
        private static FSDirectory _directoryTemp;
        private static FSDirectory _directory
        {
            get
            {
                if (_directoryTemp == null) _directoryTemp = FSDirectory.Open(new DirectoryInfo(_luceneDir));
                if (IndexWriter.IsLocked(_directoryTemp)) IndexWriter.Unlock(_directoryTemp);
                var lockFilePath = Path.Combine(_luceneDir, "write.lock");
                if (System.IO.File.Exists(lockFilePath)) System.IO.File.Delete(lockFilePath);
                return _directoryTemp;
            }
        }
        #endregion

        public static IEnumerable<FileDB> GetAllIndexRecords()
        {
            // validate search index
            if (!System.IO.Directory.EnumerateFiles(_luceneDir).Any()) return new List<FileDB>();

            // set up lucene searcher
            var searcher = new IndexSearcher(_directory, false);
            var reader = IndexReader.Open(_directory, false);
            var docs = new List<Document>();
            var term = reader.TermDocs();
            while (term.Next()) docs.Add(searcher.Doc(term.Doc));
            reader.Dispose();
            searcher.Dispose();
            return _mapLuceneToDataList(docs);
        }
        /// <summary>
        /// Able to search by partial words
        /// </summary>
        /// <param name="input"></param>
        /// <param name="fieldName"></param>
        /// <returns></returns>
        #region Public Methods
        public static IEnumerable<FileDB> Search(string input, string fieldName = "")
        {
            if (string.IsNullOrEmpty(input)) return new List<FileDB>();
            // replaces all dashes "-", adds "*" (star) after each word, so can search by partial words.
            var terms = input.Trim().Replace("-", " ").Split(' ')
                .Where(x => !string.IsNullOrEmpty(x)).Select(x => x.Trim() + "*");
            input = string.Join(" ", terms);

            return _search(input, fieldName);
        }

        /// <summary>
        /// Default search, does not modify search query
        /// </summary>
        /// <param name="input"></param>
        /// <param name="fieldName"></param>
        /// <returns></returns>
        public static IEnumerable<FileDB> SearchDefault(string input, string fieldName = "")
        {
            return string.IsNullOrEmpty(input) ? new List<FileDB>() : _search(input, fieldName);
        }
        public static void AddUpdateLuceneIndex(IEnumerable<FileDB> files)
        {
            // init lucene
            var analyzer = new StandardAnalyzer(Version.LUCENE_30);
            using (var writer = new IndexWriter(_directory, analyzer, IndexWriter.MaxFieldLength.UNLIMITED))
            {
                // add data to lucene search index (replaces older entry if any)
                foreach (var file in files) _addToLuceneIndex(file, writer);

                // close handles
                analyzer.Close();
                writer.Dispose();
            }
        }
        public static void AddUpdateLuceneIndex(FileDB file)
        {
            AddUpdateLuceneIndex(new List<FileDB> { file });
        }

        /// <summary>
        /// Remove record by ID, call this after delete database table rows 
        /// </summary>
        /// <param name="record_id"></param>
        public static void ClearLuceneIndexRecord(int record_id)
        {
            // init lucene
            var analyzer = new StandardAnalyzer(Version.LUCENE_30);
            using (var writer = new IndexWriter(_directory, analyzer, IndexWriter.MaxFieldLength.UNLIMITED))
            {
                // remove older index entry
                var searchQuery = new TermQuery(new Term("Id", record_id.ToString()));
                writer.DeleteDocuments(searchQuery);

                // close handles
                analyzer.Close();
                writer.Dispose();
            }
        }

        /// <summary>
        /// Remove whole index, call whenever database schema changes, or want to clear the whole index 
        /// </summary>
        /// <returns></returns>
        public static bool ClearLuceneIndex()
        {
            try
            {
                var analyzer = new StandardAnalyzer(Version.LUCENE_30);
                using (var writer = new IndexWriter(_directory, analyzer, true, IndexWriter.MaxFieldLength.UNLIMITED))
                {
                    // remove older index entries
                    writer.DeleteAll();

                    // close handles
                    analyzer.Close();
                    writer.Dispose();
                }
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// Optimize index and proper lock/unlock file
        /// </summary>
        public static void Optimize()
        {
            var analyzer = new StandardAnalyzer(Version.LUCENE_30);
            using (var writer = new IndexWriter(_directory, analyzer, IndexWriter.MaxFieldLength.UNLIMITED))
            {
                analyzer.Close();
                writer.Optimize();
                writer.Dispose();
            }
        }
        #endregion

        #region Private Methods
        private static void _addToLuceneIndex(FileDB file, IndexWriter writer)
        {
            // remove older index entry
            var searchQuery = new TermQuery(new Term("Id", file.ID.ToString()));
            writer.DeleteDocuments(searchQuery);

            // add new index entry
            var doc = new Document();

            // add lucene fields mapped to db fields
            doc.Add(new Field("Id", file.ID.ToString(), Field.Store.YES, Field.Index.NOT_ANALYZED));
            doc.Add(new Field("Name", file.Name, Field.Store.YES, Field.Index.ANALYZED));
            doc.Add(new Field("ContentType", file.ContentType, Field.Store.YES, Field.Index.ANALYZED));
            doc.Add(new Field("Size", file.Size.ToString(), Field.Store.YES, Field.Index.ANALYZED));
            doc.Add(new Field("Link", file.Link, Field.Store.YES, Field.Index.ANALYZED));
            doc.Add(new Field("CreatedDate", file.CreatedDate.ToString(), Field.Store.YES, Field.Index.ANALYZED));
            doc.Add(new Field("Dimensions", file.Dimensions, Field.Store.YES, Field.Index.ANALYZED));
            doc.Add(new Field("Resolution", file.Resolution, Field.Store.YES, Field.Index.ANALYZED));



            // add entry to index
            writer.AddDocument(doc);
        }
        private static FileDB _mapLuceneDocumentToData(Document doc)
        {
            //Convert string to datetime
            DateTime createDate;
            DateTime.TryParse(doc.Get("CreatedDate"), out createDate);

            return new FileDB
            {
                ID = Convert.ToInt32(doc.Get("Id")),
                Name = doc.Get("Name"),
                ContentType = doc.Get("ContentType"),
                Size = Convert.ToInt32(doc.Get("Size")),
                Link = doc.Get("Link"),
                CreatedDate = createDate,
                Dimensions = doc.Get("Dimensions"),
                Resolution = doc.Get("Resolution")
            };
        }
        private static IEnumerable<FileDB> _mapLuceneToDataList(IEnumerable<Document> hits)
        {
            return hits.Select(_mapLuceneDocumentToData).ToList();
        }
        private static IEnumerable<FileDB> _mapLuceneToDataList(IEnumerable<ScoreDoc> hits,
            IndexSearcher searcher)
        {
            return hits.Select(hit => _mapLuceneDocumentToData(searcher.Doc(hit.Doc))).ToList();
        }

        private static Query parseQuery(string searchQuery, QueryParser parser)
        {
            Query query;
            try
            {
                query = parser.Parse(searchQuery.Trim());
            }
            catch (ParseException)
            {
                query = parser.Parse(QueryParser.Escape(searchQuery.Trim()));
            }
            return query;
        }

        private static IEnumerable<FileDB> _search(string searchQuery, string searchField = "")
        {
            // validation
            if (string.IsNullOrEmpty(searchQuery.Replace("*", "").Replace("?", ""))) return new List<FileDB>();

            // set up lucene searcher
            using (var searcher = new IndexSearcher(_directory, false))
            {
                var hits_limit = 1000;
                var analyzer = new StandardAnalyzer(Version.LUCENE_30);

                // search by single field
                if (!string.IsNullOrEmpty(searchField))
                {
                    var parser = new QueryParser(Version.LUCENE_30, searchField, analyzer);
                    var query = parseQuery(searchQuery, parser);
                    var hits = searcher.Search(query, hits_limit).ScoreDocs;
                    var results = _mapLuceneToDataList(hits, searcher);
                    analyzer.Close();
                    searcher.Dispose();
                    return results;
                }
                // search by multiple fields (ordered by RELEVANCE)
                else
                {
                    var parser = new MultiFieldQueryParser
                        (Version.LUCENE_30, new[] 
                        {
                            "Id",
                            "Name",
                            "ContentType",
                            "Size",
                            "Link",
                            "CreatedDate",
                            "Dimensions",
                            "Resolution"
                        }, analyzer);
                    var query = parseQuery(searchQuery, parser);
                    var hits = searcher.Search
                    (query, null, hits_limit, Sort.RELEVANCE).ScoreDocs;
                    var results = _mapLuceneToDataList(hits, searcher);
                    analyzer.Close();
                    searcher.Dispose();
                    return results;
                }
            }
        }
        #endregion
    }
}
