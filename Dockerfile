FROM microsoft/dotnet:1.1.1-sdk

RUN mkdir -p /dotnetapp

COPY HiP-FeatureToggle /dotnetapp
WORKDIR /dotnetapp

EXPOSE 5002

WORKDIR /dotnetapp
#RUN (echo "131.234.137.23 tfs-hip.cs.upb.de" >> /etc/hosts) && dotnet restore --no-cache --configfile Nuget.Config

CMD (echo "131.234.137.23 tfs-hip.cs.upb.de" >> /etc/hosts) && dotnet restore --configfile Nuget.Config
#ENTRYPOINT ["dotnet", "run"]
