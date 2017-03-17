# quasardb .NET API

This is the quasardb API for .NET.


### Introduction

Here is a quick intro, please review [the official documentation](https://doc.quasardb.net/dotnet) for more detail.

Connecting to a cluster:

    var cluster = new QdbCluster("qdb://127.0.0.1:2836");

Insert and retrieve binary data:

    cluster.Blob("my blob").Put(myByteArray);
    var myByteArray = cluster.Blob("my blob").Get();

Push and pop from a queue:

    cluster.Deque("my double-ended queue").PushBack(myByteArray);
    myByteArray = cluster.Deque("my double-ended queue").PopFront();

Create and update an atomic integer:

    cluster.Integer("my counter").Put(1934);
    cluster.Integer("my counter").Add(22);

[Got to the official documentation](https://doc.quasardb.net/dotnet)

### Compiling

Download the following files to your local copy of the repository:

* `Quasardb.Native\win32\qdb_api.dll` from [qdb-2.0.0-windows-32bit-c-api.zip](https://download.quasardb.net/quasardb/)
* `Quasardb.Native\win64\qdb_api.dll` from [qdb-2.0.0-windows-64bit-c-api.zip](https://download.quasardb.net/quasardb/)
* `Quasardb.Tests\qdbd.exe` from [qdb-2.0.0-windows-32bit-server.zip](https://download.quasardb.net/quasardb/) or [qdb-2.0.0-windows-64bit-server.zip](https://download.quasardb.net/quasardb/)

Open the solution `Quasardb.sln` in Visual Studio.
