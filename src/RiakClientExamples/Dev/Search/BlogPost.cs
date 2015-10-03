namespace RiakClientExamples.Dev.Search
{
    using System;
    using System.Collections.Generic;

    public class BlogPost : IModel
    {
        private readonly string title;
        private string author;
        private string content;
        private ISet<string> keywords;
        private DateTime datePosted;
        private Boolean published;

        public BlogPost(
            string title,
            string author,
            string content,
            ISet<string> keywords,
            DateTime datePosted,
            bool published)
        {
            if (string.IsNullOrWhiteSpace(title))
            {
                throw new ArgumentNullException("title", "title is required");
            }
            this.title = title;

            if (string.IsNullOrWhiteSpace(author))
            {
                throw new ArgumentNullException("author", "author is required");
            }
            this.author = author;

            if (string.IsNullOrWhiteSpace(content))
            {
                throw new ArgumentNullException("content", "content is required");
            }
            this.content = content;

            this.keywords = keywords;
            this.datePosted = datePosted;
            this.published = published;
        }

        public string ID
        {
            get { return CreateSlug(); }
        }

        public string Title
        {
            get { return title; }
        }

        public string Author
        {
            get { return author; }
        }

        public string Content
        {
            get { return content; }
        }

        public IEnumerable<string> Keywords
        {
            get { return keywords; }
        }

        public DateTime DatePosted
        {
            get { return datePosted; }
        }

        public bool Published
        {
            get { return published; }
        }

        private string CreateSlug()
        {
            return "TODO";
        }
    }
}
