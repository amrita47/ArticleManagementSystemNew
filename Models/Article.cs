using System;
using System.Collections.Generic;

namespace ArticleManagementSystemNew.Models
{
    public class Article
    {
        public int Id { get; set; }

        public Status Status { get; set; } = Status.Draft;

        public DateTime CreatedAt { get; set; }

        public ICollection<Content> Contents { get; set; }
            = new List<Content>();
    }
}