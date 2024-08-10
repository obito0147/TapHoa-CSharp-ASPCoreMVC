# Sử dụng image dotnet SDK 8.0 để build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Sao chép file .csproj và khôi phục các phụ thuộc
COPY OnTapCSharp/OnTapCSharp.csproj OnTapCSharp/
RUN dotnet restore "OnTapCSharp/OnTapCSharp.csproj"

# Sao chép toàn bộ mã nguồn và build
COPY OnTapCSharp/ OnTapCSharp/
WORKDIR "/src/OnTapCSharp"
RUN dotnet build "OnTapCSharp.csproj" -c Release -o /app/build

# Publish ứng dụng
FROM build AS publish
RUN dotnet publish "OnTapCSharp.csproj" -c Release -o /app/publish

# Sử dụng image dotnet runtime 8.0 để chạy ứng dụng
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "OnTapCSharp.dll"]
