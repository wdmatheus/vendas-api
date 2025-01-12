# syntax=docker/dockerfile:1

FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/nightly/aspnet:9.0-noble-chiseled AS runtime

FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/nightly/sdk:9.0-noble AS build

WORKDIR /source

ARG TARGETARCH

COPY ./*.sln .
COPY ./*.props .
COPY ./src/*/*.csproj .

RUN for file in $(ls *.csproj); do mkdir -p src/${file%.*}/ && mv $file ./src/${file%.*}/; done

RUN dotnet restore -r linux-$TARGETARCH ./src/Vendas.Api/Vendas.Api.csproj

COPY . .

RUN dotnet publish ./src/Vendas.Api/Vendas.Api.csproj -c Release -r linux-$TARGETARCH --no-restore -o /dist && \
  rm /dist/*.Development.json

FROM runtime as final

WORKDIR /app

COPY --link --from=build /dist .

ENTRYPOINT ["./Vendas.Api"]
