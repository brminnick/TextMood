using System;
using System.Data.Linq.Mapping;

namespace TextMood.Shared
{
    public class TextModel
    {
		[Column(Name = nameof(Id), IsPrimaryKey = true, CanBeNull = false, UpdateCheck = UpdateCheck.Never)]
		public string Id { get; set; }

		[Column(Name = nameof(Text), IsPrimaryKey = false, CanBeNull = false, UpdateCheck = UpdateCheck.Never)]
		public string Text { get; set; }

		[Column(Name = nameof(CreatedAt), IsPrimaryKey = false, CanBeNull = false, UpdateCheck = UpdateCheck.Never)]
		public DateTimeOffset CreatedAt { get; set; }

		[Column(Name = nameof(UpdatedAt), IsPrimaryKey = false, CanBeNull = false, UpdateCheck = UpdateCheck.Never)]
		public DateTimeOffset UpdatedAt { get; set; }

		[Column(Name = nameof(IsDeleted), IsPrimaryKey = false, CanBeNull = false, UpdateCheck = UpdateCheck.Never)]
		public bool IsDeleted { get; set; }
    }
}
