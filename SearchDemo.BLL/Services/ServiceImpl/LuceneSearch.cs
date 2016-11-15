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
    public static class LuceneSearch
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

        #region Constructor
        #endregion

        #region Public Methods
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
        #endregion
    }
}
