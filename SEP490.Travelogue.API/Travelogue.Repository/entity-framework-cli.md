### 📄 `entity-framework-cli.md`

# Entity Framework Core CLI Commands - Hướng Dẫn Sử Dụng `dotnet ef`

## ✅ 1. Cài đặt `dotnet ef` CLI Tool (chỉ cần làm 1 lần)

```bash
dotnet tool install --global dotnet-ef
```

> Dùng để sử dụng các lệnh `dotnet ef` trong terminal (Visual Studio Code, Git Bash, PowerShell, CMD...).

---

## 🔍 2. Kiểm tra phiên bản `dotnet ef` đã cài

```bash
dotnet ef --version
```

---

## 🛠 3. Tạo Migration mới

```bash
dotnet ef migrations add <MigrationName> --startup-project <StartupProjectPath>
```

### 🔸 Ví dụ:

```bash
dotnet ef migrations add Booking --startup-project ../Travelogue.API
```

---

## 🗃 4. Cập nhật Database theo Migration đã tạo

```bash
dotnet ef database update --startup-project <StartupProjectPath>
```

### 🔸 Ví dụ:

```bash
dotnet ef database update --startup-project ../Travelogue.API
```

---

## ❌ 5. Xoá Migration cuối cùng (nếu chưa cập nhật database)

```bash
dotnet ef migrations remove --startup-project <StartupProjectPath>
```

---

## 📋 6. Liệt kê tất cả các migration đã tạo

```bash
dotnet ef migrations list --startup-project <StartupProjectPath>
```

---

## 🧠 7. Xử lý lỗi: Không tạo được `DbContext` khi chạy lệnh `dotnet ef`

### ❓ Lỗi phổ biến:

```
Unable to create a 'DbContext' of type 'ApplicationDbContext'.
Unable to resolve service for type 'DbContextOptions<ApplicationDbContext>'...
```

### ✅ Cách xử lý: Tạo `ApplicationDbContextFactory`

Tạo class `ApplicationDbContextFactory.cs` trong project chứa `ApplicationDbContext`:

```csharp
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace Travelogue.Repository.Data
{
    public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
    {
        public ApplicationDbContext CreateDbContext(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
            var connectionString = configuration.GetConnectionString("DefaultConnection");

            optionsBuilder.UseSqlServer(connectionString); // Hoặc UseNpgsql nếu dùng PostgreSQL

            return new ApplicationDbContext(optionsBuilder.Options);
        }
    }
}
```

---

## ⚙️ 8. Cấu hình Connection String

Trong `appsettings.json` (của project chứa startup):

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=.;Database=TravelogueDB;Trusted_Connection=True;MultipleActiveResultSets=true"
  }
}
```

---

## 📦 9. Cài đặt gói NuGet cần thiết

Trong `.csproj` của project chứa `DbContext`:

```xml
<PackageReference Include="Microsoft.EntityFrameworkCore.Design" Version="8.0.5" />
<PackageReference Include="Microsoft.EntityFrameworkCore.SqlServer" Version="8.0.5" />
```

(hoặc `Npgsql` nếu dùng PostgreSQL)

---

## 📘 10. Tài liệu chính thức

- EF Core CLI Docs: [https://docs.microsoft.com/en-us/ef/core/cli/dotnet](https://docs.microsoft.com/en-us/ef/core/cli/dotnet)
- EF Core Design-time DbContext Creation: [https://learn.microsoft.com/en-us/ef/core/cli/dbcontext-creation](https://learn.microsoft.com/en-us/ef/core/cli/dbcontext-creation)

---

```

---

### ✅ Hướng dẫn sử dụng

1. Lưu nội dung này vào file: `entity-framework-cli.md`
2. Đặt file ở thư mục gốc dự án hoặc trong thư mục `docs/`
3. Commit cùng code để dễ truy cập và chia sẻ với team

Nếu bạn muốn mình tạo thêm bản tiếng Anh hoặc có ảnh minh hoạ VSCode/Terminal, cứ nói nhé!
```
