FROM microsoft/dotnet:1.1.1-sdk

RUN mkdir -p /dotnetapp

COPY HiP-FeatureToggle /dotnetapp
WORKDIR /dotnetapp

EXPOSE 5002

WORKDIR /dotnetapp
RUN dotnet restore --no-cache

ENTRYPOINT ["dotnet", "run"]
