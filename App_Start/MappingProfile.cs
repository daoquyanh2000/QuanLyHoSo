using AutoMapper;
using QuanLyHoSo.Models;
using QuanLyHoSo.ViewModel;

namespace QuanLyHoSo.App_Start
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<ViewExcelNhanVien, NhanVien>();
            CreateMap<ViewHoSo, HoSo>();
        }
    }
}