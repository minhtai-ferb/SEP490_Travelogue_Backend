using System.Text;
using System.Text.RegularExpressions;
using FirebaseAdmin.Auth;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Travelogue.Repository.Const;
using Travelogue.Repository.Entities;

namespace Travelogue.Repository.Data;

public static class DataSeeder
{
    public static async Task SeedDataAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

        await using var transaction = await unitOfWork.BeginTransactionAsync();

        try
        {
            // var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            // var sqlFilePath = Path.Combine(AppContext.BaseDirectory, "SeedScripts", "SeedScript.sql");

            // if (File.Exists(sqlFilePath))
            // {
            //     var sqlScript = await File.ReadAllTextAsync(sqlFilePath, Encoding.UTF8);

            //     var cleanedScript = SqlCleaner.RemoveSqlComments(sqlScript);

            //     cleanedScript = "SET FOREIGN_KEY_CHECKS=0;\n" + cleanedScript + "\nSET FOREIGN_KEY_CHECKS=1;";

            //     var commands = cleanedScript.Split(";", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

            //     foreach (var command in commands)
            //     {
            //         if (!string.IsNullOrWhiteSpace(command))
            //         {
            //             try
            //             {
            //                 var escapedCommand = command.Replace("{", "{{").Replace("}", "}}"); // ✅ Escape
            //                 await dbContext.Database.ExecuteSqlRawAsync(escapedCommand);
            //             }
            //             catch (FormatException ex)
            //             {
            //                 Console.WriteLine($"🔥 FormatException khi chạy SQL:\n{command}\n❌ Exception: {ex.Message}");
            //                 throw;
            //             }
            //         }
            //     }
            // }

            // Role
            var roleRepo = unitOfWork.RoleRepository;
            var userRepo = unitOfWork.UserRepository;
            var userRoleRepo = unitOfWork.UserRoleRepository;

            var adminRoleName = AppRole.ADMIN;
            var userRoleName = AppRole.USER;
            var moderator = AppRole.MODERATOR;
            var tourGuide = AppRole.TOUR_GUIDE;
            var craftVillageOwner = AppRole.CRAFT_VILLAGE_OWNER;

            // Kiểm tra role tồn tại
            var existingRoles = await roleRepo.GetByNamesAsync(new List<string> { adminRoleName, userRoleName, moderator, tourGuide, craftVillageOwner });
            var existingRoleNames = existingRoles.Select(r => r.Name).ToHashSet();

            var newRoles = new List<Role>();

            if (!existingRoleNames.Contains(adminRoleName))
            {
                newRoles.Add(Role.Create(adminRoleName, skipFormatting: true));
            }

            if (!existingRoleNames.Contains(userRoleName))
            {
                newRoles.Add(Role.Create(userRoleName, skipFormatting: true));
            }

            if (!existingRoleNames.Contains(moderator))
            {
                newRoles.Add(Role.Create(moderator, skipFormatting: true));
            }

            if (!existingRoleNames.Contains(tourGuide))
            {
                newRoles.Add(Role.Create(tourGuide, skipFormatting: true));
            }

            if (!existingRoleNames.Contains(craftVillageOwner))
            {
                newRoles.Add(Role.Create(craftVillageOwner, skipFormatting: true));
            }

            if (newRoles.Count > 0)
            {
                await roleRepo.AddRangeAsync(newRoles);
                await unitOfWork.SaveAsync();

                existingRoles = await roleRepo.GetByNamesAsync(new List<string> { adminRoleName, userRoleName, moderator, tourGuide, craftVillageOwner });
            }

            //var adminRole = existingRoles.FirstOrDefault(r => r.Name == adminRoleName);
            //if (adminRole == null)
            //    throw new Exception("Không tìm thấy role Admin");

            var adminRole = existingRoles.FirstOrDefault(r => r.Name == adminRoleName);
            if (adminRole == null)
            {
                throw new Exception("Không tìm thấy role Admin");
            }

            // User
            var userEmail = "traveloguetayninh@gmail.com";
            var existingUser = await userRepo.GetUserByEmailAsync(userEmail);
            if (existingUser == null)
            {
                existingUser = new User
                {
                    Id = Guid.NewGuid(),
                    Email = userEmail,
                    FullName = "Administrator",
                    IsEmailVerified = true,
                    EmailConfirmed = true,
                    Sex = Entities.Enums.Gender.Male,
                };
                existingUser.SetPassword("string");

                await userRepo.AddAsync(existingUser);
            }

            // Firebase Authentication
            try
            {
                var firebaseUser = await FirebaseAuth.DefaultInstance.GetUserByEmailAsync(userEmail);

                if (!firebaseUser.EmailVerified)
                {
                    var updateRequest = new UserRecordArgs
                    {
                        Uid = firebaseUser.Uid,
                        EmailVerified = true
                    };

                    await FirebaseAuth.DefaultInstance.UpdateUserAsync(updateRequest);
                }
            }
            catch (FirebaseAuthException ex) when (ex.AuthErrorCode == AuthErrorCode.UserNotFound)
            {
                var createUserArgs = new UserRecordArgs
                {
                    Email = userEmail,
                    EmailVerified = true,
                    Password = "string",
                    DisplayName = "Administrator"
                };

                await FirebaseAuth.DefaultInstance.CreateUserAsync(createUserArgs);
            }

            // User - Role
            var userRoleExists = await userRoleRepo.ActiveEntities
                .AnyAsync(ur => ur.UserId == existingUser.Id && ur.RoleId == adminRole.Id);
            if (!userRoleExists)
            {
                var userRole = new UserRole { UserId = existingUser.Id, RoleId = adminRole.Id };
                await userRoleRepo.AddAsync(userRole);
            }

            await unitOfWork.SaveAsync();
            await transaction.CommitAsync();
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            Console.WriteLine($"Lỗi khi seed dữ liệu: {ex.Message}");
            throw;
        }
    }
}

public static class SqlCleaner
{
    public static string RemoveSqlComments(string sqlContent)
    {
        // 1. Xóa các dòng comment bắt đầu bằng "--"
        sqlContent = Regex.Replace(sqlContent, @"^\s*--.*(\r?\n)?", string.Empty, RegexOptions.Multiline);

        // 2. Xóa các comment dạng /*! ... */
        sqlContent = Regex.Replace(sqlContent, @"/\*!.*?\*/;", string.Empty, RegexOptions.Singleline);

        // 3. Xóa comment dạng /* ... */ (nếu cần)
        sqlContent = Regex.Replace(sqlContent, @"/\*.*?\*/", string.Empty, RegexOptions.Singleline);

        // 4. Loại bỏ các dòng trống thừa
        sqlContent = Regex.Replace(sqlContent, @"^\s*$\n|\r", string.Empty, RegexOptions.Multiline);

        return sqlContent.Trim();
    }
}
