using AutoMapper;
using CustomerProductService.Data;
using CustomerProductService.Models;
using ProductData;
using ProductOrderFacade.Models;
using ProductRepository.Data;
using ProductRepository.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CustomerProductService
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<ProductDto, ProductUpdateDto>();
            CreateMap<ProductUpdateDto, ProductDto>();
            CreateMap<ProductDto, ProductRepoModel>();
            CreateMap<ProductRepoModel, ProductDto>();
            CreateMap<Product, ProductRepoModel>();
            CreateMap<ProductRepoModel, Product>();
            CreateMap<ProductInfoDto, ProductInfoRepoModel>();
            CreateMap<ProductInfoRepoModel, ProductInfoDto>();
            CreateMap<BrandDto, BrandModel>();
            CreateMap<BrandModel, BrandDto>();
        }
    }
}
