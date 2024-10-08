﻿using HighScore.Entities.Model.Concrete;

namespace HighScore.BL.Managers.Abstract
{
    public interface IUserReviewManager : IManager<UserReview>
    {
        public Task<IEnumerable<UserReview>> GetReviewsByItemIdAsync(int itemId);
        public Task<bool> CreateComment(int itemId, string userId, int userRating, string comment);

    }
}
