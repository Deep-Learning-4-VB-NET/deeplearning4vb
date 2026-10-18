# DeepLearning4VB

A Visual Basic .NET port of [Eclipse Deeplearning4j](https://github.com/deeplearning4j/deeplearning4j)
for VB.NET developers, targeting .NET Framework 4.8.1 and .NET 10, built with Visual Studio 2026.

> **Project status: early port, under development.**
> The common modules and DataVec, the data pipeline (read, describe, transform and analyse data),
> are ported and covered by 975 unit tests that pass on both target frameworks. The tensor engine
> and the neural networks are not ported yet. Everything in `unported/` is raw converter output that
> does not compile. [PORTING_STATUS.md](PORTING_STATUS.md) has the module-by-module picture, the
> deliberate differences from Java and the porting rules.

## Component names

The components carry "4vb" where the Java originals carry "4j":

| Java original | DeepLearning4VB | Namespace |
| --- | --- | --- |
| Deeplearning4j (DL4J) | DeepLearning4VB (DL4VB) | `DeepLearning4VB.org.deeplearning4vb` |
| ND4J | ND4VB | `DeepLearning4VB.org.nd4vb` |
| DataVec | DataVec | `DeepLearning4VB.org.datavec` |
| RL4J | RL4VB (not ported yet) | |

## Requirements

- Visual Studio 2026 with the ".NET desktop development" workload. The solution uses the `.slnx`
  format.
- .NET SDK 10.0.100 or newer for command-line builds and the .NET 10 target.
- .NET Framework 4.8.1 Developer Pack (installed with the Visual Studio workload) for the
  .NET Framework target.

The only NuGet dependency is System.Text.Json, and only for .NET Framework (it is part of .NET 10).

## Build and test

Open `DeepLearning4VB.slnx` in Visual Studio and build, or from a terminal:

```bash
dotnet build DeepLearning4VB.slnx
```

```bash
dotnet test DeepLearning4VB.slnx
```

The tests run on both target frameworks. The library is produced at
`src/DeepLearning4VB/bin/<Configuration>/net481/DeepLearning4VB.dll` and
`src/DeepLearning4VB/bin/<Configuration>/net10.0/DeepLearning4VB.dll`.

## Repository layout

| Path | Contents |
| --- | --- |
| `DeepLearning4VB.slnx` | Visual Studio 2026 solution: the library and its test project. |
| `src/DeepLearning4VB/` | The library. Namespaces mirror the Java packages with the "4vb" names, for example `org.nd4vb.common.primitives.Pair`, so each Java file maps to one VB file. |
| `src/DeepLearning4VB/JavaCompat/` | Small equivalents of the Java and Guava APIs the ported code relies on (`Arrays`, `Collections`, `LinkedHashMap`, `PriorityQueue`, `Properties`, `String.format`, `TimeUnit`, data streams). |
| `src/DeepLearning4VB/org/joda/`, `com/tdunning/`, `com/clearspring/` | Managed stand-ins for Joda-Time, the t-digest and HyperLogLog++, so no further packages are needed. |
| `src/DeepLearning4VB/org/slf4j/` | A minimal logging facade (`Logger`, `LoggerFactory`). Route log output anywhere by setting `LoggerFactory.Provider`. |
| `tests/DeepLearning4VB.Tests/` | MSTest project: the upstream JUnit tests converted to MSTest plus new tests. |
| `unported/` | The raw Java-to-VB converter output for the rest of Deeplearning4j, in the upstream module layout and with the upstream names, so each file can be compared with its Java original. Not part of the build. |
| `Directory.Build.props` | Shared compiler settings. Integer overflow wraps around as in Java (`RemoveIntegerChecks`). |

## What is ported and working

- **ND4VB common** (`org.nd4vb.common`): tuples, counters, collections, array and math utilities,
  class loading, resources and downloads, archives, validation, and `SerializationUtils` for
  Java-style object serialization (a binary format that works across both frameworks).
- **DL4VB common** (`org.deeplearning4vb.common`): class loading, resource locations, system
  properties, temporary files.
- **DataVec** (`org.datavec.api`, `org.datavec.local`):
  - writables, input splits, and record readers and writers for CSV, lines, regular expressions,
    JSON, LibSVM/SVMLight, file batches and in-memory collections;
  - schemas, column metadata, conditions, filters and more than 60 transforms;
  - data analysis and data-quality reports, with self-contained HTML reports;
  - reductions, joins, sorted ranks and sequence operations (windows, splits, trims);
  - `TransformProcess`, with JSON that matches Java DataVec, so processes saved by Java load here;
  - `LocalTransformExecutor` and `AnalyzeLocal`, which run a `TransformProcess` or an analysis in memory.

A small example: read a CSV file, drop a column, filter rows and encode a category.

```vb
Imports DeepLearning4VB.org.datavec.api.records.reader.impl.csv
Imports DeepLearning4VB.org.datavec.api.split
Imports DeepLearning4VB.org.datavec.api.transform
Imports DeepLearning4VB.org.datavec.api.transform.condition
Imports DeepLearning4VB.org.datavec.api.transform.condition.column
Imports DeepLearning4VB.org.datavec.api.transform.schema
Imports DeepLearning4VB.org.datavec.api.writable
Imports DeepLearning4VB.org.datavec.local.transforms

' people.csv has a header line "name,age,city", then one person per line.
Dim schema = New Schema.Builder().
    addColumnString("name").
    addColumnInteger("age").
    addColumnCategorical("city", "London", "Paris", "Berlin").
    build()

Dim tp = New TransformProcess.Builder(schema).
    removeColumns("name").
    filter(New IntegerColumnCondition("age", ConditionOp.LessThan, 18)).
    categoricalToInteger("city").
    build()

Dim records As New List(Of IList(Of Writable))()
Using reader As New CSVRecordReader(1)            ' 1 = skip the header line
    reader.initialize(New FileSplit("people.csv"))
    While reader.hasNext()
        records.Add(reader.next())
    End While
End Using

Dim result = LocalTransformExecutor.execute(records, tp)   ' age and city index of each adult
Dim json As String = tp.toJson()                          ' the same JSON Java DataVec writes
Dim sameProcess = TransformProcess.fromJson(json)
```

This example is compiled and run as a test (`tests/DeepLearning4VB.Tests/ReadmeExampleTest.vb`).

## What is not ported yet

The tensor engine (`INDArray` and its operations) runs in Java as a native C++ library that has no
.NET counterpart, so the next phase is a managed tensor engine written in VB on top of the ported
ND4J API. The neural networks (`deeplearning4j-nn`, `deeplearning4j-core`), NLP, model import, the
model zoo and reinforcement learning follow it. Modules that only wrap Java platforms (Spark, the
Aeron parameter server, python4j, the RL game environments, Solr) were dropped. The blocking
dependency of every remaining module is listed in [PORTING_STATUS.md](PORTING_STATUS.md).

## Contributing

Contributions are welcome. Pick a file in `unported/`, follow the porting rules in
[PORTING_STATUS.md](PORTING_STATUS.md), port its unit test as well, check that `dotnet test` passes
on both frameworks, and open a pull request. Questions and requests to join the mailing list:
info@app-one.co.uk.

## License

This port is released under the MIT License (see [LICENSE](LICENSE)). Deeplearning4j itself is
licensed under the Apache License 2.0; the ported source files keep their Apache license headers.

*DeepLearning4VB team, part of AppOne Microservices UK Limited.*
