FROM mcr.microsoft.com/dotnet/sdk:6.0-focal AS build

WORKDIR /source
COPY . .
RUN dotnet restore "EasySave_G8_CONS/EasySave_G8_CONS.csproj" --disable-parallel
RUN dotnet publish "EasySave_G8_CONS/EasySave_G8_CONS.csproj" -c release -o /app --no-restore


FROM mcr.microsoft.com/dotnet/sdk:6.0-focal
WORKDIR /app 
COPY --from=build /app ./

EXPOSE 5000
ENTRYPOINT ["dotnet", "EasySave_G8_CONS.dll"]