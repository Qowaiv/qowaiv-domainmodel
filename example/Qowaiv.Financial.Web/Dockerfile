#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["example/Qowaiv.Financial.Web/Qowaiv.Financial.Web.csproj", "example/Qowaiv.Financial.Web/"]
RUN dotnet restore "example/Qowaiv.Financial.Web/Qowaiv.Financial.Web.csproj"
COPY . .
WORKDIR "/src/example/Qowaiv.Financial.Web"
RUN dotnet build "Qowaiv.Financial.Web.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Qowaiv.Financial.Web.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Qowaiv.Financial.Web.dll"]