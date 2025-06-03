#!/bin/bash

dotnet ef database update 0
dotnet ef migrations remove
dotnet ef migrations add herdbase_test
dotnet ef database update