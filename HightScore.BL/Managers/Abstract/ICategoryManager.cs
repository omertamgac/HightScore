﻿using HightScore.Entities.Model.Concrete;

namespace HightScore.BL.Managers.Abstract
{
    public interface ICategoryManager : IManager<Category>
    {
        public Task<List<Category>> GetAllAsync();
        Task<IEnumerable<Category>> GetAllCategoriesAsync();
    }
}
