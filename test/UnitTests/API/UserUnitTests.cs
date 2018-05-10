﻿using Macaria.API.Features.Users;
using Macaria.Core.Entities;
using Macaria.Infrastructure.Data;
using Macaria.Infrastructure.Identity;
using Microsoft.EntityFrameworkCore;
using Moq;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace UnitTests.API
{    
    public class UserUnitTests
    {
        protected readonly Mock<IPasswordHasher> _passwordHasherMock;
        protected readonly Mock<ITokenProvider> _tokenProvider;

        public UserUnitTests()
        {
            _passwordHasherMock = new Mock<IPasswordHasher>();
            _tokenProvider = new Mock<ITokenProvider>();

            _passwordHasherMock.Setup(x => x.HashPassword(It.IsAny<byte[]>(), "password"))
                .Returns("password");

            _passwordHasherMock.Setup(x => x.HashPassword(It.IsAny<byte[]>(),"changePassword"))
                .Returns("passwordChanged");

            _tokenProvider.Setup(x => x.Get("quinntynebrown@gmail.com")).Returns("token");
        }

        [Fact]
        public async Task ShouldHandleAuthenticateUserCommandRequest()
        {
            var options = new DbContextOptionsBuilder<MacariaContext>()
                .UseInMemoryDatabase(databaseName: "ShouldHandleAuthenticateUserCommandRequest")
                .Options;

            using (var context = new MacariaContext(options))
            {                
                context.Users.Add(new User()
                {
                    Username = "quinntynebrown@gmail.com",
                    Password = "password",
                });

                context.SaveChanges();

                var handler = new AuthenticateCommand.Handler(context, _tokenProvider.Object, _passwordHasherMock.Object);

                var response = await handler.Handle(new AuthenticateCommand.Request()
                {
                    Username = "quinntynebrown@gmail.com",
                    Password = "password"
                }, default(CancellationToken));

                Assert.Equal("token", response.AccessToken);
            }
        }

        [Fact]
        public async Task ShouldHandleCreateUserCommandRequest() {

            var options = new DbContextOptionsBuilder<MacariaContext>()
                .UseInMemoryDatabase(databaseName: "ShouldHandleCreateUserCommandRequest")
                .Options;

            using (var context = new MacariaContext(options))
            {
                var handler = new CreateUserCommand.Handler(context, _passwordHasherMock.Object);

                var response = await handler.Handle(new CreateUserCommand.Request() {
                    Username = "quinntynebrown@gmail.com",
                    Password ="password"
                }, default(CancellationToken));

                Assert.Single(context.Users.ToList());
                Assert.Equal("quinntynebrown@gmail.com", context.Users.First().Username);
            }
        }

        [Fact]
        public async Task ShouldHandleGetByIdQueryRequest()
        {
            var options = new DbContextOptionsBuilder<MacariaContext>()
                .UseInMemoryDatabase(databaseName: "ShouldHandleGetByIdQueryRequest")
                .Options;

            using (var context = new MacariaContext(options))
            {
                

                context.Users.Add(new Macaria.Core.Entities.User()
                {
                    UserId = 1,
                    Username = "quinntynebrown@gmail.com",
                    Password = "password",
                    
                });

                context.SaveChanges();

                var handler = new GetUserByIdQuery.Handler(context);

                var response = await handler.Handle(new GetUserByIdQuery.Request()
                {
                    UserId = 1
                }, default(CancellationToken));

                Assert.Equal("quinntynebrown@gmail.com", response.User.Username);
            }
        }

        [Fact]
        public async Task ShouldHandleGetUsersQueryRequest()
        {
            var options = new DbContextOptionsBuilder<MacariaContext>()
                .UseInMemoryDatabase(databaseName: "ShouldHandleGetUsersQueryRequest")
                .Options;

            using (var context = new MacariaContext(options))
            {                
                context.Users.Add(new User()
                {
                    UserId = 1,
                    Username = "quinntynebrown@gmail.com",
                    Password = "password",
                    
                });

                context.SaveChanges();

                var handler = new GetUsersQuery.Handler(context);

                var response = await handler.Handle(new GetUsersQuery.Request(), default(CancellationToken));

                Assert.Single(response.Users);
            }
        }

        [Fact]
        public async Task ShouldHandleRemoveUserCommandRequest()
        {
            var options = new DbContextOptionsBuilder<MacariaContext>()
                .UseInMemoryDatabase(databaseName: "ShouldHandleRemoveUserCommandRequest")
                .Options;

            using (var context = new MacariaContext(options))
            {
                

                context.Users.Add(new User()
                {
                    UserId = 1,
                    Username = "quinntynebrown@gmail.com",
                    Password = "password",
                    
                });

                context.SaveChanges();

                var handler = new RemoveUserCommand.Handler(context);

                await handler.Handle(new RemoveUserCommand.Request()
                {
                    UserId = 1
                }, default(CancellationToken));

                Assert.Equal(0, context.Users.Count());
            }
        }

        [Fact]
        public async Task ShouldHandleUpdateUserCommandRequest()
        {
            var options = new DbContextOptionsBuilder<MacariaContext>()
                .UseInMemoryDatabase(databaseName: "ShouldHandleUpdateUserCommandRequest")
                .Options;

            using (var context = new MacariaContext(options))
            {
                

                context.Users.Add(new User()
                {
                    UserId = 1,
                    Username = "quinntynebrown@gmail.com",
                    Password = "password",
                    
                });

                context.SaveChanges();

                var handler = new UpdateUserCommand.Handler(context);

                var response = await handler.Handle(new UpdateUserCommand.Request()
                {
                    UserId = 1,
                    Username = "joe"
                }, default(CancellationToken));

                Assert.Equal(1, response.UserId);
                Assert.Equal("joe", context.Users.Single(x=>x.UserId == 1).Username);
            }
        }

        [Fact]
        public async Task ShouldHandleUserChangePasswordCommandRequest()
        {
            var options = new DbContextOptionsBuilder<MacariaContext>()
                .UseInMemoryDatabase(databaseName: "ShouldHandleUserChangePasswordCommandRequest")
                .Options;

            using (var context = new MacariaContext(options))
            {
                context.Users.Add(new User()
                {
                    UserId = 1,
                    Username = "quinntynebrown@gmail.com",
                    Password = "password" 
                });

                context.SaveChanges();

                var handler = new UserChangePasswordCommand.Handler(context, _passwordHasherMock.Object);

                await handler.Handle(new UserChangePasswordCommand.Request()
                {
                    UserId = 1,
                    Password = "changePassword",
                    ConfirmPassword = "changePassword"
                }, default(CancellationToken));


                Assert.Equal("passwordChanged", context.Users.Single(x => x.UserId == 1).Password);
            }
        }
    }
}
