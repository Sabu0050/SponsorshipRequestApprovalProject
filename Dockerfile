FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["SponsorshipRequestApprovalProject.sln", "."]
COPY ["Directory.Build.props", "."]
COPY ["Directory.Packages.props", "."]
COPY ["src/SponsorshipRequestApprovalProject.Api/SponsorshipRequestApprovalProject.Api.csproj", "src/SponsorshipRequestApprovalProject.Api/"]
COPY ["src/SponsorshipRequestApprovalProject.Application/SponsorshipRequestApprovalProject.Application.csproj", "src/SponsorshipRequestApprovalProject.Application/"]
COPY ["src/SponsorshipRequestApprovalProject.Domain/SponsorshipRequestApprovalProject.Domain.csproj", "src/SponsorshipRequestApprovalProject.Domain/"]
COPY ["src/SponsorshipRequestApprovalProject.Infrastructure/SponsorshipRequestApprovalProject.Infrastructure.csproj", "src/SponsorshipRequestApprovalProject.Infrastructure/"]

RUN dotnet restore "src/SponsorshipRequestApprovalProject.Api/SponsorshipRequestApprovalProject.Api.csproj"

COPY . .
WORKDIR "/src/src/SponsorshipRequestApprovalProject.Api"
RUN dotnet publish "SponsorshipRequestApprovalProject.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "SponsorshipRequestApprovalProject.Api.dll"]
