namespace RiakClientExamples.Dev.Search
{
    using System;
    using System.Globalization;
    using RiakClient;
    using RiakClient.Commands.CRDT;

    public class BlogPostRepository : Repository<BlogPost>
    {
        const string titleRegister = "title";
        const string authorRegister = "author";
        const string contentRegister = "content";
        const string keywordsSet = "keywords";
        const string datePostedRegister = "date";
        const string publishedFlag = "published";

        private readonly string bucket;

        public BlogPostRepository(IRiakClient client, string bucket)
            : base(client)
        {
            if (string.IsNullOrWhiteSpace(bucket))
            {
                throw new ArgumentNullException("bucket");
            }

            this.bucket = bucket;
        }

        public override string Save(BlogPost model)
        {
            var mapOp = new UpdateMap.MapOperation();

            mapOp.SetRegister(titleRegister, model.Title);
            mapOp.SetRegister(authorRegister, model.Author);
            mapOp.SetRegister(contentRegister, model.Content);
            mapOp.AddToSet(keywordsSet, model.Keywords);

            string datePostedSolrFormatted =
                model.DatePosted
                    .ToUniversalTime()
                    .ToString("yyyy-MM-dd'T'HH:mm:ss'Z'", CultureInfo.InvariantCulture);

            mapOp.SetRegister(datePostedRegister, datePostedSolrFormatted);

            mapOp.SetFlag(publishedFlag, model.Published);

            // NB: no key so Riak will generate it
            var cmd = new UpdateMap.Builder()
                .WithBucketType(BucketType)
                .WithBucket(this.bucket)
                .WithMapOperation(mapOp)
                .Build();

            RiakResult rslt = client.Execute(cmd);
            CheckResult(rslt);
            MapResponse response = cmd.Response;
            return response.Key;
        }

        protected override string BucketType
        {
            get { return "cms"; }
        }
    }
}
