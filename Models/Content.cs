
using System;

namespace ArticleManagementSystemNew.Models
{
    public class Content
    {
        public int Id { get; set; }

        public string Title { get; set; } = string.Empty;

        public string ContentData { get; set; } = string.Empty;

        public int Author { get; set; }

        public Status Status { get; set; } = Status.Draft;

        public DateTime CreatedAt { get; set; }

        public Language Language { get; set; } = Language.English;

        public int ArticleId { get; set; }

        public User? AuthorUser { get; set; }

        public Article? Article { get; set; }
    }
}
