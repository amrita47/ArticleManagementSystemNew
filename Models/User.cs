
using System;
using System.Collections.Generic;

namespace ArticleManagementSystemNew.Models
{
    public class User
    {
        public int Id { get; set; }

        public string Username { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; }

        public ICollection<Content> Contents { get; set; }
            = new List<Content>();
    }
}
