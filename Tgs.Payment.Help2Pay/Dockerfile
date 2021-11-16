#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
ARG BuildConfiguration=Release
RUN --mount=type=secret,id=nuget,dst=/pat cp /pat ~/.nuget/NuGet/NuGet.Config
RUN dotnet nuget remove source 'Microsoft Visual Studio Offline Packages' || true
WORKDIR /src
COPY ["NuGet.config", "."]
COPY ["Tgs.Payment.Help2Pay/Tgs.Payment.Help2Pay.csproj", "Tgs.Payment.Help2Pay/"]
COPY ["Help2Pay.Core/Help2Pay.Core.csproj", "Help2Pay.Core/"]
COPY ["Tgs.Payment.Help2Pay.Client/Tgs.Payment.Help2Pay.Client.csproj", "Tgs.Payment.Help2Pay.Client/"]
RUN dotnet restore "Tgs.Payment.Help2Pay/Tgs.Payment.Help2Pay.csproj"
COPY . .
WORKDIR "/src/Tgs.Payment.Help2Pay"
RUN dotnet build "Tgs.Payment.Help2Pay.csproj" -c ${BuildConfiguration} -o /app/build

FROM build AS publish
ARG BuildConfiguration
RUN dotnet publish "Tgs.Payment.Help2Pay.csproj" -c ${BuildConfiguration} -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Tgs.Payment.Help2Pay.dll"]