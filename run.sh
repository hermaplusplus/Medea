#!/bin/bash
dotnet restore
dotnet build
dotnet run .\bin\Debug\netcoreapp3.1\Medea.dll