using System.Threading.Tasks;
using TommDAL.ViewModel;

namespace TommBLL.Interface
{
   public interface IPlayList
    {
        Task<SpotifyViewModel> GetAccessToken();
        Task<PlayListViewModel> GetPlaylists(SpotifyViewModel model);
    }
}
