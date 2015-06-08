# quasardb .NET API

Interfacing with a *quasardb* database from a C# program is extremely straightforward, just create a `QdbCluster` and perform the operations.

    var cluster = new QdbCluster("qdb://127.0.0.1:2836");

OK, now that we have a connection to the cluster, let's store some **binary data**:

    byte[] a, b, c;
    
    QdbBlob myBlob = cluster.Blob('Bob the blob');
    
    myBlob.Put(a);
    myBlob.Update(b);
    c = myBlob.Get();

Want a **queue** in your database?

    byte[] a, b, c;

    QdbQueue myQueue = cluster.Queue('Andrew the queue');

    myQueue.PushBack(a);
    myQueue.PushBack(b);
    c = myQueue.PopFront();

Want **atomic integers** now?   

    long a, b, c;

    QdbInteger myInt = cluster.Integer('Roger the integer');

    myInt.Put(a);
    c = myInt.Add(b);

What else? a **set** maybe?

    byte[] a, b, c;

    QdbHashSet mySet = cluster.HashSet('Janet the set');

    mySet.Insert(a);
    mySet.Remove(b);
    bool hasC = mySet.Contains(c);

