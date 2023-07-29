FROM artifactory.unibet.com.au/docker-dotnet-sdk/ksp-sdk-test:6.0.0
WORKDIR /tests
COPY . .
ENTRYPOINT ["/ksp/ksp.sh", "dotnet", "test", "-v", "diag", "*Tests.dll"]
