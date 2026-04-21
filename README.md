# TCP Assignment

A TCP client-server application built with clean architecture in .NET 8.0.

## Prerequisites

- .NET 8.0 SDK or later

## Running the Application

### Start the Server

```bash
cd Server
dotnet run
# Enter port: 5000
```

### Start the Client

```bash
cd Client
dotnet run
# Enter server IP: 127.0.0.1
# Enter server port: 5000
# Enter message: SetA-Two
```

## Features

- Clean Architecture (Domain, Application, Infrastructure layers)
- AES Encryption/Decryption
- Exception handling
- Async/await networking
- Multiple client support

## Data Lookup Examples

Send queries in format `SetX-KeyName`:

- `SetA-One` → 1 timestamp
- `SetA-Two` → 2 timestamps
- `SetB-Three` → 3 timestamps
- `SetC-Five` → 5 timestamps
- `SetE-Ten` → 10 timestamps

Server sends current timestamp N times with 1-second intervals.

## Build

```bash
dotnet build
```
