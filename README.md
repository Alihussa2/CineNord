# CineNord - Eksamensprojekt (System Integration)

## Start databaserne
```
docker compose up -d
```
Stop: `docker compose down`. Nulstil alt (sletter data): `docker compose down -v`.

| Container | Port | Indhold |
|---|---|---|
| postgres | 5432 | rest_db, soap_db, graphql_auth, grpc_db, ws_db, sse_db, warehouse |
| shard0 / shard1 / shard2 | 5440 / 5441 / 5442 | GraphQL-shards (customers + reviews) |
| redis | 6379 | db 0: REST-blacklist, db 1: GraphQL-tokens |

Bruger/password til alle databaser: `cinenord` / `cinenord`.

## API'er
| Mappe | Projekt | URL |
|---|---|---|
| Rest | CineNord.Rest | http://localhost:5101/swagger |
| Soap | CineNord.Soap | http://localhost:5102/BookingService.asmx?wsdl |
| GraphQL | CineNord.GraphQL | http://localhost:5103/graphql |
| GraphQL | CineNord.Etl | Console App (koer med dotnet run) |
| Grpc | CineNord.Grpc | localhost:5104 (HTTP/2, server reflection) |
| WebSockets | CineNord.WebSockets | ws://localhost:5105/ws/seating |
| Sse | CineNord.Sse | http://localhost:5106/v1/screenings/7/seat-events |

Hvert API (undtagen gRPC) har `GET /health`, der viser om databaserne svarer.
