using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TommDAL.Models;
using TommDAL.ViewModel;

namespace TommBLL.Interface
{
   public interface IFoods
    {
        Task<List<CategoryMst>> CategoryList();
        Task<List<FoodView>> GetFoodData(int CategoryId);
        Task<Boolean> UpdateFood(FoodCategories model);
    }
}
