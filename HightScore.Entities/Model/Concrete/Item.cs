﻿using HightScore.Entities.Model.Abstract;

namespace HightScore.Entities.Model.Concrete
{
    public class Item : BaseEntity
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime RelaseDate { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? Iframe { get; set; }
        public string? photo { get; set; }
        public ICollection<ItemCategory> ItemCategories { get; set; } = new List<ItemCategory>();
        public ICollection<ItemPlatform> ItemPlatforms { get; set; }
    }
}