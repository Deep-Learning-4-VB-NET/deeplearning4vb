# Phase 2 design: the managed ND4VB tensor engine

Status: agreed design for phase 2 (owner's decision: a pure managed engine written in VB, multi-target
.NET Framework 4.8.1 and .NET 10). Every phase-2 contributor follows this document and
[PORTING_STATUS.md](../PORTING_STATUS.md).

## Goal

ND4J's Java API (`INDArray`, `Nd4j`, the op classes, `Transforms`, workspaces, data buffers) is the
surface everything above it uses: DataVec's NDArray parts, `deeplearning4j-nn`, `deeplearning4j-core`,
NLP, model import. In Java the API is a thin layer: every operation is executed by the native C++
library libnd4j through JavaCPP (`NativeOps`). That library is not in this repository and has no .NET
counterpart. Phase 2 therefore has two halves:

1. **Port the Java API** (`unported/nd4j/nd4j-backends/nd4j-api-parent/nd4j-api`, about 1,400 files)
   with the component rename: `org.nd4j` becomes `DeepLearning4VB.org.nd4vb`.
2. **Write a managed backend** that takes the place of `nd4j-native` + libnd4j: data storage, the op
   executioner, the numeric kernels, BLAS and random numbers, in VB, in the new namespace
   `DeepLearning4VB.org.nd4vb.linalg.managed`. The Java CPU backend in
   `unported/nd4j/nd4j-backends/nd4j-backend-impls/nd4j-native` (`CpuBackend`, `CpuNDArrayFactory`,
   `NDArray`, `CpuMemoryManager`, ...) is the structural model: where it calls `NativeOps`, the managed
   backend calls managed kernels.

## Milestones

| Milestone | Delivers | Main parts |
| --- | --- | --- |
| M1 | `INDArray` you can create, view, reshape, permute, read, write and apply element-wise operations to | foundation interfaces and types; data buffers; `Shape`; indexing; `BaseNDArray`, `NDArrayFactory`, `Nd4j`; op base classes; transform, scalar, pairwise and broadcast ops; the managed executioner and kernels |
| M2 | Reductions, index reductions, matrix multiply, random numbers | reduce/indexaccum/reduce3/summarystats ops; BLAS (gemm, gemv, level 1); RNG and distributions |
| M3 | Advanced indexing and shape operations | `get`/`put` with intervals and points, concat/stack/tile, sort, shape custom ops, boolean indexing |
| M4 | Neural-network kernels | convolution, pooling, batch norm, LSTM/GRU custom ops, losses, activations, updaters, weight init, datasets |
| M5 | SameDiff | `SameDiff`, `SDVariable`, gradients (`doDiff`), the generated op namespaces, FlatBuffers serialization |

After M1 the 16 DataVec files that need `INDArray` can be finished. Phase 3 (neural networks) needs M1 to M4;
SameDiff-based features need M5.

## Fences for parts that come later

Port every Java class once, completely, in its final place. Members that depend on a later milestone
or on a library we do not have yet stay in the file inside a conditional-compilation block, so nothing
is lost and the later milestone only has to enable and finish them:

| Fence | Contents | Enabled in |
| --- | --- | --- |
| `#If SAMEDIFF Then ... #End If` | Members that use `SameDiff`, `SDVariable` or the SameDiff function factory: op constructors taking `SameDiff`, `doDiff`, `calculateOutputDataTypes` for graphs, `DifferentialFunction` graph members | M5 |
| `#If MODEL_IMPORT Then ... #End If` | Members that use TensorFlow or ONNX protobuf types (`NodeDef`, `AttrValue`, `GraphDef`, `Onnx.*`): `initFromTensorFlow`, `initFromOnnx`, attribute mappings | phase 3 (samediff-import) |
| `#If FLATBUFFERS Then ... #End If` | FlatBuffers serialization (`toFlatArray`, `createFromFlatArray`, `org.nd4j.graph.*` usage) | M5 |

Rules: the fenced text is the converter's original text (do not spend time fixing it); keep simple
members that only return strings (`opName`, `tensorflowName`, `onnxName`) outside the fences; a fenced
member that an interface requires gets a non-fenced implementation that throws
`NotSupportedException("<Member> needs SameDiff (phase 2 milestone M5)")`. Native-only members
(JavaCPP `Pointer`, `address()`, `NativeOps`, `Nd4j.getNativeOps()`) are deleted rather than fenced,
and listed in the class's XML comment.

## Managed backend architecture

### Storage and data buffers

- A data buffer's elements live in a typed .NET array owned by a `ManagedStorage` object
  (`org.nd4vb.linalg.managed.storage`): `Single()` for FLOAT, `Double()` for DOUBLE, `UInt16()` bit
  patterns for HALF and BFLOAT16 (converted with `ArrayUtil.toHalf`/`toBFloat16` and their inverses),
  `SByte()` for INT8, `Byte()` for UINT8, `Short()`/`UShort()` for INT16/UINT16, `Integer()`/`UInteger()`
  for INT32/UINT32, `Long()`/`ULong()` for INT64/UINT64, `Boolean()` for BOOL, and `String()` for UTF8.
- Every storage offers typed fast paths (`Floats`, `Doubles`, ...) and the generic accessors
  `GetDouble(i)`, `SetDouble(i, v)`, `GetLong(i)`, `SetLong(i, v)`; conversions follow Java's
  primitive casts (truncation toward zero, wrap-around for integers, `NaN` to 0 for integer targets).
- `DataBuffer` (the ported interface) is implemented by `ManagedDataBuffer` over a storage, an
  element offset and a length, so views share storage like Java's view buffers.
- The shape information is ND4J's `Long()` format, unchanged: `[rank, shape..., stride..., extras,
  elementWiseStride, order]` where `extras` carries the data type flags and `order` is `AscW("c"c)`
  or `AscW("f"c)`. `Shape` ports unchanged on top of it.

### Kernels and op math

- `org.nd4vb.linalg.managed.kernels` holds the numeric loops, independent of the op classes: strided
  element iteration over a shape-info, broadcasting, tensor-along-dimension (TAD) iteration for
  reductions along dimensions, the reduction loops, and BLAS (gemm, gemv, level 1).
- `org.nd4vb.linalg.managed.ops` holds the math of each op, keyed by the Java op name (`opName()` of the
  ported op class, for example `"sigmoid"`, `"add"`, `"sum"`): transform functions, pairwise and scalar
  functions, reduce definitions (start value, map, update, merge, post-process), index-reduce and
  reduce3 definitions. The delegate and definition types both sides use are in
  `src/DeepLearning4VB/org/nd4vb/linalg/managed/OpContracts.vb`.
- The managed `OpExecutioner` dispatches an op by its type (transform, scalar, pairwise, broadcast,
  reduce, index reduce, ...) and `opName()`: it looks the math up in the op math registry and runs the
  matching kernel over the op's `x`, `y`, `z` arrays and dimensions. Custom ops (`DynamicCustomOp`) are
  dispatched by `opName()` to managed implementations as they are written; an unimplemented op throws
  `NotSupportedException` naming the op and the milestone that will add it.
- Floating-point math is done in `Double` and rounded to the target type; integer ops use integer
  arithmetic with Java's wrap-around; booleans are stored as `Boolean` and computed as 0/1.

### Everything else in the backend

- **Workspaces**: `MemoryWorkspace` and the workspace manager keep their API and scoping rules, but
  memory always comes from the garbage-collected heap; workspaces only track scope and statistics.
- **Devices and memory managers**: one CPU device; `AffinityManager`, `MemoryManager`,
  `ConstantHandler` and the TAD manager are simple managed implementations (constant and TAD caches
  are dictionaries).
- **Random numbers**: a managed generator with a 64-bit seed; sequences are deterministic per seed but
  are not bit-identical to libnd4j's.
- **Performance**: correctness first. Contiguous `Single`/`Double` fast paths, `Parallel.For` above a
  size threshold, and `System.Numerics.Vector(Of T)` SIMD paths on .NET 10 only (`#If NET Then`), so
  the .NET Framework build needs no extra package. gemm is blocked and parallel.
- **Backend discovery**: `Nd4j` loads `ManagedBackend` (the counterpart of `CpuBackend`) by default
  through `ND4VBClassLoading`; there are no native libraries to load.

## Package layout

| Namespace | Contents |
| --- | --- |
| `DeepLearning4VB.org.nd4vb.linalg.*` | the ported Java API (`api.ndarray`, `api.buffer`, `api.shape`, `api.ops`, `factory`, `indexing`, ...) |
| `DeepLearning4VB.org.nd4vb.linalg.managed` | the managed backend: `ManagedBackend`, `ManagedNDArray`, `ManagedNDArrayFactory`, `ManagedOpExecutioner`, `ManagedDataBuffer`, memory and workspace managers |
| `DeepLearning4VB.org.nd4vb.linalg.managed.storage` | typed storages |
| `DeepLearning4VB.org.nd4vb.linalg.managed.kernels` | numeric loops and BLAS |
| `DeepLearning4VB.org.nd4vb.linalg.managed.ops` | op math registry |
| `DeepLearning4VB.org.nd4vb.autodiff.*` | SameDiff (M5; `DifferentialFunction` arrives in M1 with its graph members fenced) |

## Testing

- The upstream tests are in `unported/nd4j/nd4j-backends/nd4j-tests` (225 files). Port the tests of
  each milestone with the code (for M1: `Nd4jTestsC`/`Nd4jTestsF` parts on creation, views, element-wise
  ops; `linalg/shape`, `linalg/api/ndarray`, `linalg/api/buffer`, `DataTypeTest`).
- Kernels and op math get their own unit tests against hand-computed values.
- Every test runs on both target frameworks; `dotnet test DeepLearning4VB.slnx` must stay green.
