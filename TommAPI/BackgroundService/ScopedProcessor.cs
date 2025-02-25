//using Microsoft.Extensions.Configuration;
//using Microsoft.Extensions.DependencyInjection;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;
//using TommBLL.Interface;

//namespace TommAPI.BackgroundService
//{
//    public abstract class ScopedProcessor : BackgroundService
//    {
//        private readonly IServiceScopeFactory _serviceScopeFactory;
//        private IConfiguration _configuration;
      
//        public ScopedProcessor(IServiceScopeFactory serviceScopeFactory, IConfiguration configuration)
//        {
//            _serviceScopeFactory = serviceScopeFactory;
//            _configuration = configuration;
           
//        }

//        protected override async Task Process()
//        {
//            using (var scope = _serviceScopeFactory.CreateScope())
//            {
//                await ProcessInScope(scope.ServiceProvider);
//            }
//        }

//        public abstract Task ProcessInScope(IServiceProvider serviceProvider);
//    }
//}
