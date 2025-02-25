using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TommDAL.ViewModel;

namespace TommBLL.Interface
{
    public interface IHoliday
    {

        Task<SetHolidayResponseViewModel> SetHolidayStatus(HolidayRequestViewModel model);

        Task<HolidayDetailsResponseViewModel> GetHolidayDetails(long UserId);

        Task<bool> DeleteUserHoliday(DeleteHolidayViewModel model);

        Task<List<HolidayDetailsResponseViewModel>> GetHolidayDatesByMonth(long UserId, byte Month, short Year);

        Task<HolidayListResponse> GetUserHolidayList(long UserId, int PageNo, int PageSize);
    }

}
