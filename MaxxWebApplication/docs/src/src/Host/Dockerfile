FROM artifactory.unibet.com.au/docker-dotnet-sdk/ksp-aspnet:6.0.0
WORKDIR /app
EXPOSE 80
ARG version=0.0.0.0
ENV Serilog__Properties__Release=$version
COPY . .
ENTRYPOINT ["/ksp/ksp.sh", "dotnet", "Kindred.Rewards.Plugin.Host.dll"]
