# DeepLearning4VB

A Visual Basic .NET port of [Eclipse Deeplearning4j](https://github.com/deeplearning4j/deeplearning4j)
(DL4J, ND4J, DataVec, RL4J) for .NET Framework 4.8.1 and Visual Studio 2026.

> **Project status: early port, under development.**
> The repository holds a machine translation of the whole DL4J code base (about 4,900 files) plus a
> hand-ported core that compiles and is covered by unit tests. Only the code in `src/` builds and
> runs. Everything in `unported/` is raw converter output that does not compile yet.
> [PORTING_STATUS.md](PORTING_STATUS.md) has the module-by-module picture and the porting rules.

## Requirements

- Visual Studio 2026 with the ".NET desktop development" workload (Visual Studio 2022 17.14 or
  newer also works). The solution uses the `.slnx` format.
- .NET SDK 9.0.200 or newer for command-line builds (`dotnet build`, `dotnet test`).
- .NET Framework 4.8.1 Developer Pack (installed with the Visual Studio workload).

## Build and test

Open `DeepLearning4VB.slnx` in Visual Studio and build, or from a terminal:

```bash
dotnet build DeepLearning4VB.slnx
```

```bash
dotnet test DeepLearning4VB.slnx
```

The library is produced at `src/DeepLearning4VB/bin/<Configuration>/net481/DeepLearning4VB.dll`.
The tests also show up in the Visual Studio Test Explorer.

## Repository layout

| Path | Contents |
| --- | --- |
| `DeepLearning4VB.slnx` | Visual Studio 2026 solution: the library and its test project. |
| `src/DeepLearning4VB/` | The ported library (class library, `net481`). Namespaces mirror the Java packages, for example `org.nd4j.common.primitives.Pair`, so a Java file maps 1:1 to a VB file. |
| `src/DeepLearning4VB/JavaCompat/` | Small equivalents of the Java and Guava APIs the ported code relies on: `java.util.Arrays`, `Collections`, `LinkedHashMap`/`LinkedHashSet`, `PriorityQueue`, `Properties`, `String.format`, `System.getProperty`. |
| `src/DeepLearning4VB/org/slf4j/` | A minimal SLF4J-style logging facade (`Logger`, `LoggerFactory`). Route log output anywhere by setting `LoggerFactory.Provider`. |
| `tests/DeepLearning4VB.Tests/` | MSTest project: the DL4J unit tests ported to MSTest plus new tests for the ported code. |
| `unported/` | The raw Java-to-VB converter output for the rest of DL4J, kept in the upstream module layout (`<module>/src/main/java/...`) so each file can be diffed against its Java original. Not part of the build. |
| `Directory.Build.props` | Shared compiler settings. Integer overflow wraps around as in Java (`RemoveIntegerChecks`). |

## What is ported and working

The `nd4j-common` module, the dependency-free utilities every other DL4J module builds on:

- `org.nd4j.common.base`: `Preconditions` (custom `%tag` formatters register through `Preconditions.registerFormat`).
- `org.nd4j.common.primitives`: `Pair`, `Triple`, `Quad` and their immutable variants, `Counter`, `CounterMap`, `Atomic`, `AtomicBoolean`, `AtomicDouble`, `Optional`, `SynchronizedObject`.
- `org.nd4j.common.collection`: `IntArrayKeyMap`, `IntArrayKeySet`, `MultiDimensionalMap`, `MultiDimensionalSet`, `CompactHeapStringList`.
- `org.nd4j.common.util`: `ArrayUtil` (about 300 array helpers: shapes, strides, flattening, permutations, half precision and big-endian conversions), `MathUtils`, `Rational`, `Bernoulli`, `Factorial`, `SetUtils`, `Index`, `LinkedMultiValueMap`, `OneTimeLogger`, `ND4JFileUtils`, `Paths`, `InputStreamUtil`, `ThreadUtils`.
- `org.nd4j.common.function`: the functional interfaces and `FunctionalUtils`.
- `org.nd4j.common.loader`: `Source`, `Loader`, `FileBatch` (zip based, on `System.IO.Compression`).
- `org.nd4j.common.tools`: `BTools`, `InfoLine`, `InfoValues`, `PropertyParser`, `SIS`.
- `org.nd4j.common.config`: the ND4J system-property and environment-variable names.

A small taste:

```vb
Imports DeepLearning4VB.org.nd4j.common.primitives
Imports DeepLearning4VB.org.nd4j.common.util

Dim counter As New Counter(Of String)()
counter.incrementCount("cat", 2)
counter.incrementCount("dog", 5)
Console.WriteLine(counter.argMax())                           ' dog
Console.WriteLine(counter.getProbability("cat"))              ' 0.2857...

Dim strides = ArrayUtil.calcStrides(New Integer() {2, 3, 4})  ' {12, 4, 1}
Dim pair = Pair.of("answer", 42)                              ' Pair(Of String, Integer)
```

## What is not ported yet

Everything that depends on the ND4J native backend (libnd4j through JavaCPP), Apache Spark,
Jackson, the TensorFlow and ONNX bindings, Python, Arrow, Netty/Aeron, or the RL4J environment SDKs.
The native tensor library has no .NET equivalent in this repository, so `INDArray` and all of the
neural-network code (`deeplearning4j-nn`, `deeplearning4j-core`, ...) are still to come. The
blocking dependency of every module is listed in [PORTING_STATUS.md](PORTING_STATUS.md), together
with the step-by-step rules used to port the files that already compile.

## Contributing

Contributions are welcome. Pick a file in `unported/`, follow the porting rules in
[PORTING_STATUS.md](PORTING_STATUS.md), port its unit test as well, and open a pull request.
Questions and requests to join the mailing list: info@app-one.co.uk.

## License

This port is released under the MIT License (see [LICENSE](LICENSE)). Deeplearning4j itself is
licensed under the Apache License 2.0; the ported source files keep their Apache license headers.

*DeepLearning4VB team, part of AppOne Microservices UK Limited.*
