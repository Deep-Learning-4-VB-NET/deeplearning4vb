# Phase 2 op inventory: ND4J legacy ops and the managed op math registry

Generated from the Java op classes in `unported/nd4j/nd4j-backends/nd4j-api-parent/nd4j-api/src/main/java/org/nd4j/linalg/api/ops/impl/`
(packages `transforms/**`, `scalar/**`, `broadcast/**`, `reduce/**`, `reduce3/**`, `indexaccum/**`, `summarystats/**`) and the
registry in `src/DeepLearning4VB/org/nd4vb/linalg/managed/ops/` (`ManagedOpMath`). See [PHASE2_DESIGN.md](../PHASE2_DESIGN.md).

- **Op kind** is the Java base class (`BaseTransformSameOp` is "transform same", and so on). Transform classes that libnd4j runs
  as pairwise ops when the Java op is given a `y` array are marked "with y" (both forms are registered) or "runs as" (only
  the pairwise form is meaningful).
- **Extra args** is what the Java constructors put into `extraArgs` (as doubles, in order), or the scalar of a scalar op.
- **Implemented** names the `ManagedOpMath` lookup, the `ManagedOpKind` it is registered under, and the source file.
- Custom ops (`DynamicCustomOp`) are listed by name at the end, with a milestone suggestion. The element-wise math of 52 of
  them is already in the registry (`ManagedOpMath.Names(ManagedOpKind.Custom)`, `TryGetCustomFamily`).

## Summary

| Family | Java classes | Implemented |
| --- | ---: | ---: |
| Transforms: same | 23 | 23 |
| Transforms: float | 2 | 2 |
| Transforms: strict | 48 | 48 |
| Transforms: bool | 10 | 10 |
| Transforms: any | 4 | 4 |
| Transforms: other | 1 | 0 |
| Scalar | 21 | 21 |
| Scalar bool | 11 | 11 |
| Broadcast | 12 | 11 |
| Broadcast bool | 6 | 6 |
| Reduce float | 9 | 9 |
| Reduce same | 7 | 7 |
| Reduce long | 3 | 3 |
| Reduce bool | 4 | 4 |
| Reduce3 | 8 | 8 |
| Index reduce | 2 | 2 |
| Summary stats | 2 | 2 |
| **Legacy total** | **173** | **171** |
| Custom ops (`DynamicCustomOp`) | 195 | element-wise math for 52 |

Abstract base classes in these packages (not ops): `BaseReductionBp`, `BaseDynamicCustomBoolReduction`, `BaseDynamicCustomIndexReduction`, `BaseDynamicCustomLongReduction`, `BaseDynamicCustomReduction`, `BaseReduce3Op`, `BaseDynamicTransformOp`, `BaseArithmeticBackpropOp`.

Besides the Java names, the registry keeps libnd4j's legacy index-reduction names `imax`, `imin`, `iamax`, `iamin`
(the Java `IMax`/`IMin`/`IAMax`/`IAMin` classes were replaced by the custom `argmax` family, which is registered too).

## Transforms: same

| Java class | Package | Op kind | `opName()` | Extra args | Implemented |
| --- | --- | --- | --- | --- | --- |
| `CompareAndReplace` | `transforms.comparison` | transform same (runs as pairwise) | `car` | extraArgs {compare, set, eps, mode} | yes: `Pairwise` (pairwise, `PairwiseMath.vb`) |
| `CompareAndSet` | `transforms.comparison` | transform same (with y: pairwise) | `cas` | extraArgs {compare, set, eps, mode} | yes: `Transform` (transform same, `TransformMath.vb`); `Pairwise` (pairwise, `PairwiseMath.vb`) |
| `BinaryMinimalRelativeError` | `transforms.pairwise` | transform same (runs as pairwise) | `BinaryMinimalRelativeError` | extraArgs {thresholdRelative, thresholdAbsolute} | yes: `Pairwise` (pairwise, `PairwiseMath.vb`) |
| `BinaryRelativeError` | `transforms.pairwise` | transform same (runs as pairwise) | `BinaryRelativeError` | extraArgs {threshold} | yes: `Pairwise` (pairwise, `PairwiseMath.vb`) |
| `RelativeError` | `transforms.pairwise` | transform same (runs as pairwise) | `RelativeError` | none | yes: `Pairwise` (pairwise, `PairwiseMath.vb`) |
| `Axpy` | `transforms.pairwise.arithmetic` | transform same (runs as pairwise) | `axpy` | extraArgs {p, n} | yes: `Pairwise` (pairwise, `PairwiseMath.vb`) |
| `CopyOp` | `transforms.pairwise.arithmetic` | transform same (with y: pairwise) | `copy` | none | yes: `Transform` (transform same, `TransformMath.vb`); `Pairwise` (pairwise, `PairwiseMath.vb`) |
| `FModOp` | `transforms.pairwise.arithmetic` | transform same (runs as pairwise) | `fmod` | none | yes: `Pairwise` (pairwise, `PairwiseMath.vb`) |
| `AMax` | `transforms.same` | transform same (runs as pairwise) | `amax_pairwise` | none | yes: `Pairwise` (pairwise, `PairwiseMath.vb`) |
| `AMin` | `transforms.same` | transform same (runs as pairwise) | `amin_pairwise` | none | yes: `Pairwise` (pairwise, `PairwiseMath.vb`) |
| `Abs` | `transforms.same` | transform same | `abs` | none | yes: `Transform` (transform same, `TransformMath.vb`) |
| `Ceil` | `transforms.same` | transform same | `ceil` | none | yes: `Transform` (transform same, `TransformMath.vb`) |
| `Cube` | `transforms.same` | transform same | `cube` | none | yes: `Transform` (transform same, `TransformMath.vb`) |
| `Floor` | `transforms.same` | transform same | `floor` | none | yes: `Transform` (transform same, `TransformMath.vb`) |
| `Max` | `transforms.same` | transform same (runs as pairwise) | `max_pairwise` | none | yes: `Pairwise` (pairwise, `PairwiseMath.vb`) |
| `Min` | `transforms.same` | transform same (runs as pairwise) | `min_pairwise` | none | yes: `Pairwise` (pairwise, `PairwiseMath.vb`) |
| `Negative` | `transforms.same` | transform same | `neg` | none | yes: `Transform` (transform same, `TransformMath.vb`) |
| `OneMinus` | `transforms.same` | transform same | `oneminus` | none | yes: `Transform` (transform same, `TransformMath.vb`) |
| `Reciprocal` | `transforms.same` | transform same | `Reciprocal` | none | yes: `Transform` (transform same, `TransformMath.vb`) |
| `Round` | `transforms.same` | transform same | `round` | none | yes: `Transform` (transform same, `TransformMath.vb`) |
| `Sign` | `transforms.same` | transform same | `sign` | none | yes: `Transform` (transform same, `TransformMath.vb`) |
| `Square` | `transforms.same` | transform same | `square` | none | yes: `Transform` (transform same, `TransformMath.vb`) |
| `TimesOneMinus` | `transforms.same` | transform same | `timesoneminus` | none | yes: `Transform` (transform same, `TransformMath.vb`) |

## Transforms: float

| Java class | Package | Op kind | `opName()` | Extra args | Implemented |
| --- | --- | --- | --- | --- | --- |
| `RSqrt` | `transforms.floating` | transform float | `rsqrt` | none | yes: `Transform` (transform float, `TransformMath.vb`) |
| `Sqrt` | `transforms.floating` | transform float | `sqrt` | none | yes: `Transform` (transform float, `TransformMath.vb`) |

## Transforms: strict

| Java class | Package | Op kind | `opName()` | Extra args | Implemented |
| --- | --- | --- | --- | --- | --- |
| `CubeDerivative` | `transforms.gradient` | transform strict | `cubederivative` | none | yes: `Transform` (transform strict, `TransformMath.vb`) |
| `HardSigmoidDerivative` | `transforms.gradient` | transform strict | `hard_sigmoidderivative` | none | yes: `Transform` (transform strict, `TransformMath.vb`) |
| `HardTanhDerivative` | `transforms.gradient` | transform strict | `hardtanhderivative` | none | yes: `Transform` (transform strict, `TransformMath.vb`) |
| `RationalTanhDerivative` | `transforms.gradient` | transform strict | `rational_tanh_derivative` | none | yes: `Transform` (transform strict, `TransformMath.vb`) |
| `RectifiedTanhDerivative` | `transforms.gradient` | transform strict | `rectified_tanh_derivative` | none | yes: `Transform` (transform strict, `TransformMath.vb`) |
| `SELUDerivative` | `transforms.gradient` | transform strict | `seluderivative` | none | yes: `Transform` (transform strict, `TransformMath.vb`) |
| `SoftSignDerivative` | `transforms.gradient` | transform strict | `softsignderivative` | none | yes: `Transform` (transform strict, `TransformMath.vb`) |
| `ACos` | `transforms.strict` | transform strict | `acos` | none | yes: `Transform` (transform strict, `TransformMath.vb`) |
| `ACosh` | `transforms.strict` | transform strict | `acosh` | none | yes: `Transform` (transform strict, `TransformMath.vb`) |
| `ASin` | `transforms.strict` | transform strict | `asin` | none | yes: `Transform` (transform strict, `TransformMath.vb`) |
| `ASinh` | `transforms.strict` | transform strict | `asinh` | none | yes: `Transform` (transform strict, `TransformMath.vb`) |
| `ATan` | `transforms.strict` | transform strict | `atan` | none | yes: `Transform` (transform strict, `TransformMath.vb`) |
| `ATanh` | `transforms.strict` | transform strict | `atanh` | none | yes: `Transform` (transform strict, `TransformMath.vb`) |
| `Cos` | `transforms.strict` | transform strict | `cos` | none | yes: `Transform` (transform strict, `TransformMath.vb`) |
| `Cosh` | `transforms.strict` | transform strict | `cosh` | none | yes: `Transform` (transform strict, `TransformMath.vb`) |
| `Erf` | `transforms.strict` | transform strict | `erf` | none | yes: `Transform` (transform strict, `TransformMath.vb`) |
| `Erfc` | `transforms.strict` | transform strict | `erfc` | none | yes: `Transform` (transform strict, `TransformMath.vb`) |
| `Exp` | `transforms.strict` | transform strict | `exp` | none | yes: `Transform` (transform strict, `TransformMath.vb`) |
| `Expm1` | `transforms.strict` | transform strict | `expm1` | none | yes: `Transform` (transform strict, `TransformMath.vb`) |
| `GELU` | `transforms.strict` | transform strict | `gelu` | none | yes: `Transform` (transform strict, `TransformMath.vb`) |
| `GELUDerivative` | `transforms.strict` | transform strict | `_geluderivative` | none | yes: `Transform` (transform strict, `TransformMath.vb`) |
| `HardSigmoid` | `transforms.strict` | transform strict | `hard_sigmoid` | none | yes: `Transform` (transform strict, `TransformMath.vb`) |
| `HardTanh` | `transforms.strict` | transform strict | `hardtanh` | none | yes: `Transform` (transform strict, `TransformMath.vb`) |
| `Log` | `transforms.strict` | transform strict | `log` | none | yes: `Transform` (transform strict, `TransformMath.vb`) |
| `Log1p` | `transforms.strict` | transform strict | `log1p` | none | yes: `Transform` (transform strict, `TransformMath.vb`) |
| `LogSigmoid` | `transforms.strict` | transform strict | `logsigmoid` | none | yes: `Transform` (transform strict, `TransformMath.vb`) |
| `Mish` | `transforms.strict` | transform strict | `mish` | none | yes: `Transform` (transform strict, `TransformMath.vb`) |
| `MishDerivative` | `transforms.strict` | transform strict | `_mishderivative` | none | yes: `Transform` (transform strict, `TransformMath.vb`) |
| `PreciseGELU` | `transforms.strict` | transform strict | `precise_gelu` | none | yes: `Transform` (transform strict, `TransformMath.vb`) |
| `PreciseGELUDerivative` | `transforms.strict` | transform strict | `_precise_geluderivative` | none | yes: `Transform` (transform strict, `TransformMath.vb`) |
| `RationalTanh` | `transforms.strict` | transform strict | `rational_tanh` | none | yes: `Transform` (transform strict, `TransformMath.vb`) |
| `RectifiedTanh` | `transforms.strict` | transform strict | `rectified_tanh` | none | yes: `Transform` (transform strict, `TransformMath.vb`) |
| `Rint` | `transforms.strict` | transform strict | `rint` | none | yes: `Transform` (transform strict, `TransformMath.vb`) |
| `SELU` | `transforms.strict` | transform strict | `selu` | none | yes: `Transform` (transform strict, `TransformMath.vb`) |
| `SetRange` | `transforms.strict` | transform strict | `setrange` | extraArgs {min, max} (default 0, 1) | yes: `Transform` (transform strict, `TransformMath.vb`) |
| `Sigmoid` | `transforms.strict` | transform strict | `sigmoid` | none | yes: `Transform` (transform strict, `TransformMath.vb`) |
| `SigmoidDerivative` | `transforms.strict` | transform strict | `_sigmoidderivative` | none | yes: `Transform` (transform strict, `TransformMath.vb`) |
| `Sin` | `transforms.strict` | transform strict | `sin` | none | yes: `Transform` (transform strict, `TransformMath.vb`) |
| `Sinh` | `transforms.strict` | transform strict | `sinh` | none | yes: `Transform` (transform strict, `TransformMath.vb`) |
| `SoftPlus` | `transforms.strict` | transform strict | `softplus` | none | yes: `Transform` (transform strict, `TransformMath.vb`) |
| `SoftSign` | `transforms.strict` | transform strict | `softsign` | none | yes: `Transform` (transform strict, `TransformMath.vb`) |
| `Stabilize` | `transforms.strict` | transform strict | `stabilize` | k (kept in a Java field, not in extraArgs; the registry reads extraArgs(0), default 1) | yes: `Transform` (transform strict, `TransformMath.vb`) |
| `Swish` | `transforms.strict` | transform strict | `swish` | none | yes: `Transform` (transform strict, `TransformMath.vb`) |
| `SwishDerivative` | `transforms.strict` | transform strict | `_swishderivative` | none | yes: `Transform` (transform strict, `TransformMath.vb`) |
| `Tan` | `transforms.strict` | transform strict | `tan` | none | yes: `Transform` (transform strict, `TransformMath.vb`) |
| `TanDerivative` | `transforms.strict` | transform strict | `tanderivative` | none | yes: `Transform` (transform strict, `TransformMath.vb`) |
| `Tanh` | `transforms.strict` | transform strict | `tanh` | none | yes: `Transform` (transform strict, `TransformMath.vb`) |
| `TanhDerivative` | `transforms.strict` | transform strict | `_tanhderivative` | none | yes: `Transform` (transform strict, `TransformMath.vb`) |

## Transforms: bool

| Java class | Package | Op kind | `opName()` | Extra args | Implemented |
| --- | --- | --- | --- | --- | --- |
| `BooleanNot` | `transforms.bool` | transform bool | `bool_not` | none | yes: `Transform` (transform bool, `TransformMath.vb`) |
| `IsFinite` | `transforms.bool` | transform bool | `isfinite` | none | yes: `Transform` (transform bool, `TransformMath.vb`) |
| `IsInf` | `transforms.bool` | transform bool | `isinf` | none | yes: `Transform` (transform bool, `TransformMath.vb`) |
| `IsNaN` | `transforms.bool` | transform bool | `isnan` | none | yes: `Transform` (transform bool, `TransformMath.vb`) |
| `MatchConditionTransform` | `transforms.bool` | transform bool | `match_condition_transform` | extraArgs {compare, eps, mode} | yes: `Transform` (transform bool, `TransformMath.vb`) |
| `Eps` | `transforms.comparison` | transform bool (runs as pairwise bool) | `eps` | none | yes: `Pairwise` (pairwise bool, `PairwiseMath.vb`) |
| `And` | `transforms.pairwise.bool` | transform bool (runs as pairwise bool) | `and` | extraArgs {comparable} (default 0) | yes: `Pairwise` (pairwise bool, `PairwiseMath.vb`) |
| `Not` | `transforms.pairwise.bool` | transform bool (with y: pairwise bool) | `not` | extraArgs {comparable} (default 0) | yes: `Transform` (transform bool, `TransformMath.vb`); `Pairwise` (pairwise bool, `PairwiseMath.vb`) |
| `Or` | `transforms.pairwise.bool` | transform bool (runs as pairwise bool) | `or` | extraArgs {comparable} (default 0) | yes: `Pairwise` (pairwise bool, `PairwiseMath.vb`) |
| `Xor` | `transforms.pairwise.bool` | transform bool (runs as pairwise bool) | `xor` | extraArgs {comparable} (default 0) | yes: `Pairwise` (pairwise bool, `PairwiseMath.vb`) |

## Transforms: any

| Java class | Package | Op kind | `opName()` | Extra args | Implemented |
| --- | --- | --- | --- | --- | --- |
| `Assign` | `transforms.any` | transform any (with y: pairwise) | `old_assign` | none | yes: `Transform` (transform any, `TransformMath.vb`); `Pairwise` (pairwise, `PairwiseMath.vb`) |
| `Set` | `transforms.pairwise` | transform any (with y: pairwise) | `set` | none | yes: `Transform` (transform any, `TransformMath.vb`); `Pairwise` (pairwise, `PairwiseMath.vb`) |
| `PowPairwise` | `transforms.pairwise.arithmetic` | transform any (runs as pairwise) | `pow_pairwise` | none | yes: `Pairwise` (pairwise, `PairwiseMath.vb`) |
| `RemainderOp` | `transforms.pairwise.arithmetic` | transform any (runs as pairwise) | `remainder` | none | yes: `Pairwise` (pairwise, `PairwiseMath.vb`) |

## Transforms: other

| Java class | Package | Op kind | `opName()` | Extra args | Implemented |
| --- | --- | --- | --- | --- | --- |
| `MaxOut` | `transforms` | BaseTransformOp | `maxout` | max (field, unused) | no: Java `opNum()` throws: the op cannot be executed (placeholder class) |

## Scalar

| Java class | Package | Op kind | `opName()` | Extra args | Implemented |
| --- | --- | --- | --- | --- | --- |
| `LeakyReLU` | `scalar` | scalar | `leakyrelu` | scalar = alpha (default 0.01); extraArgs {alpha} | yes: `Scalar` (scalar, `ScalarMath.vb`) |
| `LogX` | `scalar` | scalar | `log_x` | scalar = base; extraArgs {base} | yes: `Scalar` (scalar, `ScalarMath.vb`) |
| `Pow` | `scalar` | scalar | `pow` | scalar = exponent; extraArgs {pow} | yes: `Scalar` (scalar, `ScalarMath.vb`) |
| `PowDerivative` | `scalar` | scalar | `_powderivative` | scalar = exponent; extraArgs {pow} | yes: `Scalar` (scalar, `ScalarMath.vb`) |
| `RectifiedLinear` | `scalar` | scalar | `relu` | scalar = cutoff (default 0) | yes: `Scalar` (scalar, `ScalarMath.vb`) |
| `Relu6` | `scalar` | scalar | `relu6` | scalar = cutoff (default 0) | yes: `Scalar` (scalar, `ScalarMath.vb`) |
| `ReplaceNans` | `scalar` | scalar | `replace_nans` | scalar = replacement; extraArgs {set} | yes: `Scalar` (scalar, `ScalarMath.vb`) |
| `ScalarAdd` | `scalar` | scalar | `add_scalar` | none | yes: `Scalar` (scalar, `ScalarMath.vb`) |
| `ScalarDivision` | `scalar` | scalar | `div_scalar` | none | yes: `Scalar` (scalar, `ScalarMath.vb`) |
| `ScalarFMod` | `scalar` | scalar | `fmod_scalar` | none | yes: `Scalar` (scalar, `ScalarMath.vb`) |
| `ScalarMax` | `scalar` | scalar | `max_scalar` | none | yes: `Scalar` (scalar, `ScalarMath.vb`) |
| `ScalarMin` | `scalar` | scalar | `scalar_min` | none | yes: `Scalar` (scalar, `ScalarMath.vb`) |
| `ScalarMultiplication` | `scalar` | scalar | `mul_scalar` | none | yes: `Scalar` (scalar, `ScalarMath.vb`) |
| `ScalarRemainder` | `scalar` | scalar | `remainder_scalar` | none | yes: `Scalar` (scalar, `ScalarMath.vb`) |
| `ScalarReverseDivision` | `scalar` | scalar | `rdiv_scalar` | none | yes: `Scalar` (scalar, `ScalarMath.vb`) |
| `ScalarReverseSubtraction` | `scalar` | scalar | `rsub_scalar` | none | yes: `Scalar` (scalar, `ScalarMath.vb`) |
| `ScalarSet` | `scalar` | scalar | `set_scalar` | none | yes: `Scalar` (scalar, `ScalarMath.vb`) |
| `ScalarSubtraction` | `scalar` | scalar | `sub_scalar` | none | yes: `Scalar` (scalar, `ScalarMath.vb`) |
| `Step` | `scalar` | scalar | `step` | scalar = cutoff (default 0); extraArgs {cutoff} | yes: `Scalar` (scalar, `ScalarMath.vb`) |
| `ScalarSetValue` | `scalar.comparison` | scalar | `setvalorless_scalar` | none | yes: `Scalar` (scalar, `ScalarMath.vb`) |
| `LeakyReLUDerivative` | `transforms.gradient` | scalar | `leakyreluderivative` | scalar = alpha (default 0.01); extraArgs {alpha} | yes: `Scalar` (scalar, `ScalarMath.vb`) |

## Scalar bool

| Java class | Package | Op kind | `opName()` | Extra args | Implemented |
| --- | --- | --- | --- | --- | --- |
| `ScalarAnd` | `scalar.comparison` | scalar bool | `and_scalar` | none | yes: `Scalar` (scalar bool, `ScalarMath.vb`) |
| `ScalarEps` | `scalar.comparison` | scalar bool | `eps_scalar` | none | yes: `Scalar` (scalar bool, `ScalarMath.vb`) |
| `ScalarEquals` | `scalar.comparison` | scalar bool | `equals_scalar` | none | yes: `Scalar` (scalar bool, `ScalarMath.vb`) |
| `ScalarGreaterThan` | `scalar.comparison` | scalar bool | `greaterthan_scalar` | none | yes: `Scalar` (scalar bool, `ScalarMath.vb`) |
| `ScalarGreaterThanOrEqual` | `scalar.comparison` | scalar bool | `greaterthanorequal_scalar` | none | yes: `Scalar` (scalar bool, `ScalarMath.vb`) |
| `ScalarLessThan` | `scalar.comparison` | scalar bool | `lessthan_scalar` | none | yes: `Scalar` (scalar bool, `ScalarMath.vb`) |
| `ScalarLessThanOrEqual` | `scalar.comparison` | scalar bool | `lessthanorequal_scalar` | none | yes: `Scalar` (scalar bool, `ScalarMath.vb`) |
| `ScalarNot` | `scalar.comparison` | scalar bool | `not_scalar` | none | yes: `Scalar` (scalar bool, `ScalarMath.vb`) |
| `ScalarNotEquals` | `scalar.comparison` | scalar bool | `notequals_scalar` | none | yes: `Scalar` (scalar bool, `ScalarMath.vb`) |
| `ScalarOr` | `scalar.comparison` | scalar bool | `or_scalar` | none | yes: `Scalar` (scalar bool, `ScalarMath.vb`) |
| `ScalarXor` | `scalar.comparison` | scalar bool | `xor_scalar` | none | yes: `Scalar` (scalar bool, `ScalarMath.vb`) |

## Broadcast

| Java class | Package | Op kind | `opName()` | Extra args | Implemented |
| --- | --- | --- | --- | --- | --- |
| `BroadcastAMax` | `broadcast` | broadcast | `broadcast_amax` | none | yes: `Pairwise` (broadcast, `PairwiseMath.vb`) |
| `BroadcastAMin` | `broadcast` | broadcast | `broadcast_amin` | none | yes: `Pairwise` (broadcast, `PairwiseMath.vb`) |
| `BroadcastAddOp` | `broadcast` | broadcast | `broadcastadd` | none | yes: `Pairwise` (broadcast, `PairwiseMath.vb`) |
| `BroadcastCopyOp` | `broadcast` | broadcast | `broadcastcopy` | none | yes: `Pairwise` (broadcast, `PairwiseMath.vb`) |
| `BroadcastDivOp` | `broadcast` | broadcast | `broadcastdiv` | none | yes: `Pairwise` (broadcast, `PairwiseMath.vb`) |
| `BroadcastGradientArgs` | `broadcast` | broadcast | `broadcastgradientargs` | none | no: not element-wise: computes broadcast reduction axes (libnd4j custom op `broadcastgradientargs`, M3) |
| `BroadcastMax` | `broadcast` | broadcast | `broadcast_max` | none | yes: `Pairwise` (broadcast, `PairwiseMath.vb`) |
| `BroadcastMin` | `broadcast` | broadcast | `broadcast_min` | none | yes: `Pairwise` (broadcast, `PairwiseMath.vb`) |
| `BroadcastMulOp` | `broadcast` | broadcast | `broadcastmul` | none | yes: `Pairwise` (broadcast, `PairwiseMath.vb`) |
| `BroadcastRDivOp` | `broadcast` | broadcast | `broadcastrdiv` | none | yes: `Pairwise` (broadcast, `PairwiseMath.vb`) |
| `BroadcastRSubOp` | `broadcast` | broadcast | `broadcastrsub` | none | yes: `Pairwise` (broadcast, `PairwiseMath.vb`) |
| `BroadcastSubOp` | `broadcast` | broadcast | `broadcastsub` | none | yes: `Pairwise` (broadcast, `PairwiseMath.vb`) |

## Broadcast bool

| Java class | Package | Op kind | `opName()` | Extra args | Implemented |
| --- | --- | --- | --- | --- | --- |
| `BroadcastEqualTo` | `broadcast.bool` | broadcast bool | `broadcast_equalto` | none | yes: `Pairwise` (broadcast bool, `PairwiseMath.vb`) |
| `BroadcastGreaterThan` | `broadcast.bool` | broadcast bool | `broadcast_greaterthan` | none | yes: `Pairwise` (broadcast bool, `PairwiseMath.vb`) |
| `BroadcastGreaterThanOrEqual` | `broadcast.bool` | broadcast bool | `broadcast_greaterthanorequal` | none | yes: `Pairwise` (broadcast bool, `PairwiseMath.vb`) |
| `BroadcastLessThan` | `broadcast.bool` | broadcast bool | `broadcast_lessthan` | none | yes: `Pairwise` (broadcast bool, `PairwiseMath.vb`) |
| `BroadcastLessThanOrEqual` | `broadcast.bool` | broadcast bool | `broadcast_lessthanorequal` | none | yes: `Pairwise` (broadcast bool, `PairwiseMath.vb`) |
| `BroadcastNotEqual` | `broadcast.bool` | broadcast bool | `broadcast_notequal` | none | yes: `Pairwise` (broadcast bool, `PairwiseMath.vb`) |

## Reduce float

| Java class | Package | Op kind | `opName()` | Extra args | Implemented |
| --- | --- | --- | --- | --- | --- |
| `AMean` | `reduce.floating` | reduce float | `amean` | none | yes: `Reduce` (reduce float, `ReduceMath.vb`) |
| `Entropy` | `reduce.floating` | reduce float | `entropy` | none | yes: `Reduce` (reduce float, `ReduceMath.vb`) |
| `LogEntropy` | `reduce.floating` | reduce float | `logentropy` | none | yes: `Reduce` (reduce float, `ReduceMath.vb`) |
| `Mean` | `reduce.floating` | reduce float | `reduce_mean` | none | yes: `Reduce` (reduce float, `ReduceMath.vb`) |
| `Norm1` | `reduce.floating` | reduce float | `reduce_norm1` | none | yes: `Reduce` (reduce float, `ReduceMath.vb`) |
| `Norm2` | `reduce.floating` | reduce float | `reduce_norm2` | none | yes: `Reduce` (reduce float, `ReduceMath.vb`) |
| `NormMax` | `reduce.floating` | reduce float | `reduce_normmax` | none | yes: `Reduce` (reduce float, `ReduceMath.vb`) |
| `ShannonEntropy` | `reduce.floating` | reduce float | `shannonentropy` | none | yes: `Reduce` (reduce float, `ReduceMath.vb`) |
| `SquaredNorm` | `reduce.floating` | reduce float | `reduce_sqnorm` | none | yes: `Reduce` (reduce float, `ReduceMath.vb`) |

## Reduce same

| Java class | Package | Op kind | `opName()` | Extra args | Implemented |
| --- | --- | --- | --- | --- | --- |
| `AMax` | `reduce.same` | reduce same | `amax` | none | yes: `Reduce` (reduce same, `ReduceMath.vb`) |
| `AMin` | `reduce.same` | reduce same | `amin` | none | yes: `Reduce` (reduce same, `ReduceMath.vb`) |
| `ASum` | `reduce.same` | reduce same | `asum` | none | yes: `Reduce` (reduce same, `ReduceMath.vb`) |
| `Max` | `reduce.same` | reduce same | `reduce_max` | none | yes: `Reduce` (reduce same, `ReduceMath.vb`) |
| `Min` | `reduce.same` | reduce same | `reduce_min` | none | yes: `Reduce` (reduce same, `ReduceMath.vb`) |
| `Prod` | `reduce.same` | reduce same | `reduce_prod` | none | yes: `Reduce` (reduce same, `ReduceMath.vb`) |
| `Sum` | `reduce.same` | reduce same | `reduce_sum` | none | yes: `Reduce` (reduce same, `ReduceMath.vb`) |

## Reduce long

| Java class | Package | Op kind | `opName()` | Extra args | Implemented |
| --- | --- | --- | --- | --- | --- |
| `CountNonZero` | `reduce.longer` | reduce long | `countNonZero` | none | yes: `Reduce` (reduce long, `ReduceMath.vb`) |
| `CountZero` | `reduce.longer` | reduce long | `countZero` | none | yes: `Reduce` (reduce long, `ReduceMath.vb`) |
| `MatchCondition` | `reduce.longer` | reduce long | `match_condition` | extraArgs {compare, eps, mode} | yes: `Reduce` (reduce long, `ReduceMath.vb`) |

## Reduce bool

| Java class | Package | Op kind | `opName()` | Extra args | Implemented |
| --- | --- | --- | --- | --- | --- |
| `All` | `reduce.bool` | reduce bool | `all` | none | yes: `Reduce` (reduce bool, `ReduceMath.vb`) |
| `Any` | `reduce.bool` | reduce bool | `any` | none | yes: `Reduce` (reduce bool, `ReduceMath.vb`) |
| `IsInf` | `reduce.bool` | reduce bool | `hasinf` | none | yes: `Reduce` (reduce bool, `ReduceMath.vb`) |
| `IsNaN` | `reduce.bool` | reduce bool | `hasnan` | none | yes: `Reduce` (reduce bool, `ReduceMath.vb`) |

## Reduce3

| Java class | Package | Op kind | `opName()` | Extra args | Implemented |
| --- | --- | --- | --- | --- | --- |
| `CosineDistance` | `reduce3` | reduce3 | `cosinedistance` | extraArgs {0, 0} (two accumulator slots) | yes: `Reduce` (reduce3, `Reduce3Math.vb`) |
| `CosineSimilarity` | `reduce3` | reduce3 | `cosinesimilarity` | extraArgs {0, 0} (two accumulator slots) | yes: `Reduce` (reduce3, `Reduce3Math.vb`) |
| `Dot` | `reduce3` | reduce3 | `dot` | none | yes: `Reduce` (reduce3, `Reduce3Math.vb`) |
| `EqualsWithEps` | `reduce3` | reduce3 | `equals_with_eps` | extraArgs {0, 0, eps} | yes: `Reduce` (reduce3, `Reduce3Math.vb`) |
| `EuclideanDistance` | `reduce3` | reduce3 | `euclidean` | extraArgs {0, 0} (unused) | yes: `Reduce` (reduce3, `Reduce3Math.vb`) |
| `HammingDistance` | `reduce3` | reduce3 | `hammingdistance` | extraArgs {0, 0} (unused) | yes: `Reduce` (reduce3, `Reduce3Math.vb`) |
| `JaccardDistance` | `reduce3` | reduce3 | `jaccarddistance` | extraArgs {0, 0} (two accumulator slots) | yes: `Reduce` (reduce3, `Reduce3Math.vb`) |
| `ManhattanDistance` | `reduce3` | reduce3 | `manhattan` | extraArgs {0, 0} (unused) | yes: `Reduce` (reduce3, `Reduce3Math.vb`) |

## Index reduce

| Java class | Package | Op kind | `opName()` | Extra args | Implemented |
| --- | --- | --- | --- | --- | --- |
| `FirstIndex` | `indexaccum` | index reduce | `first_index` | extraArgs {compare, eps, mode} | yes: `IndexReduce` (index reduce, `IndexReduceMath.vb`) |
| `LastIndex` | `indexaccum` | index reduce | `last_index` | extraArgs {compare, eps, mode} | yes: `IndexReduce` (index reduce, `IndexReduceMath.vb`) |

## Summary stats

| Java class | Package | Op kind | `opName()` | Extra args | Implemented |
| --- | --- | --- | --- | --- | --- |
| `StandardDeviation` | `summarystats` | summary stats | `std` | biasCorrected flag (op property, passed separately) | yes: `SummaryStats` (summary stats, `SummaryStatsMath.vb`) |
| `Variance` | `summarystats` | summary stats | `var` | biasCorrected flag (op property, passed separately) | yes: `SummaryStats` (summary stats, `SummaryStatsMath.vb`) |

## Custom ops (`DynamicCustomOp`)

Milestones follow [PHASE2_DESIGN.md](../PHASE2_DESIGN.md): M1 element-wise math (registered here; the executioner
dispatches the custom op by name to the broadcast/transform kernel), M2 reductions, index reductions and matrix
products, M3 shape, indexing, segment, sorting, linear-algebra and bitwise ops, M4 neural-network ops and gradients, M5
SameDiff internals. The registry family of each M1/M2 op with math is given in parentheses.

**M1** (47): `ClipByValue` (TransformSame), `Pow` (Pairwise), `add` (Pairwise), `assign` (Pairwise), `boolean_and` (PairwiseBool), `boolean_not` (TransformBool), `boolean_or` (PairwiseBool), `boolean_xor` (PairwiseBool), `cube_bp` (Pairwise), `divide` (Pairwise), `elu` (TransformStrict), `elu_bp` (Pairwise), `equals` (PairwiseBool), `floordiv` (Pairwise), `floormod` (Pairwise), `greater` (PairwiseBool), `greater_equal` (PairwiseBool), `hardsigmoid_bp` (Pairwise), `hardtanh_bp` (Pairwise), `identity` (TransformSame), `less` (PairwiseBool), `less_equal` (PairwiseBool), `lrelu_bp` (Pairwise), `maximum` (Pairwise), `minimum` (Pairwise), `mod` (Pairwise), `multiply` (Pairwise), `not_equals` (PairwiseBool), `prelu` (Pairwise), `rationaltanh_bp` (Pairwise), `realdiv` (Pairwise), `rectifiedtanh_bp` (Pairwise), `relu6_bp` (Pairwise), `relu_bp` (Pairwise), `reversedivide` (Pairwise), `reversesubtract` (Pairwise), `selu_bp` (Pairwise), `sigmoid_bp` (Pairwise), `softplus_bp` (Pairwise), `softsign_bp` (Pairwise), `squaredsubtract` (Pairwise), `subtract` (Pairwise), `tanh_bp` (Pairwise), `tf_atan2` (Pairwise), `thresholdedrelu` (TransformSame), `thresholdedrelu_bp` (Pairwise), `truncatediv` (Pairwise).

**M2** (21): `argamax` (IndexReduce), `argamin` (IndexReduce), `argmax` (IndexReduce), `argmin` (IndexReduce), `batched_gemm`, `bits_hamming_distance`, `cumprod`, `cumsum`, `hashcode`, `is_non_decreasing`, `is_strictly_increasing`, `matmul`, `matmul_bp`, `moments`, `normalize_moments`, `reduce_logsumexp` (ReduceFloat), `sufficient_statistics`, `tensordot`, `tensormmul_bp`, `trace`, `zero_fraction`.

**M3** (58): `Assert`, `ParallelConcat`, `batch_to_space`, `batch_to_space_nd`, `bincount`, `bitwise_and`, `bitwise_or`, `bitwise_xor`, `broadcast_to`, `cast`, `cholesky`, `choose`, `cyclic_rshift_bits`, `cyclic_shift_bits`, `dynamic_partition`, `dynamic_stitch`, `fill`, `histogram`, `histogram_fixed_width`, `identity_n`, `in_top_k`, `invert_permutation`, `is_numeric_tensor`, `listdiff`, `log_matrix_determinant`, `matrix_determinant`, `matrix_diag`, `matrix_diag_part`, `matrix_inverse`, `matrix_set_diag`, `mergeadd`, `mirror_pad`, `nth_element`, `pad`, `qr`, `reverse`, `reverse_sequence`, `reverse_v2`, `rshift_bits`, `segment_max`, `segment_mean`, `segment_min`, `segment_prod`, `segment_sum`, `shift_bits`, `space_to_batch`, `space_to_batch_nd`, `svd`, `top_k`, `unique`, `unique_with_counts`, `unsorted_segment_max`, `unsorted_segment_mean`, `unsorted_segment_min`, `unsorted_segment_prod`, `unsorted_segment_sqrt_n`, `unsorted_segment_sum`, `zeros_like`.

**M4** (68): `Pow_bp`, `add_bp`, `biasadd`, `biasadd_bp`, `check_numerics`, `clipbyavgnorm`, `clipbynorm`, `clipbynorm_bp`, `crelu`, `crelu_bp`, `cumprod_bp`, `cumsum_bp`, `dilation2d`, `divide_bp`, `dot_product_attention`, `dot_product_attention_bp`, `dynamic_partition_bp`, `fake_quant_with_min_max_args`, `fake_quant_with_min_max_vars`, `floordiv_bp`, `floormod_bp`, `ismax`, `layer_norm`, `layer_norm_bp`, `log_softmax`, `log_softmax_bp`, `maximum_bp`, `mergeadd_bp`, `mod_bp`, `multi_head_dot_product_attention`, `multi_head_dot_product_attention_bp`, `multiply_bp`, `prelu_bp`, `reduce_dot_bp`, `reduce_max_bp`, `reduce_mean_bp`, `reduce_min_bp`, `reduce_norm1_bp`, `reduce_norm2_bp`, `reduce_norm_max_bp`, `reduce_prod_bp`, `reduce_sqnorm_bp`, `reduce_stdev_bp`, `reduce_sum_bp`, `reduce_variance_bp`, `relu_layer`, `reverse_bp`, `reversedivide_bp`, `reversesubtract_bp`, `segment_max_bp`, `segment_mean_bp`, `segment_min_bp`, `segment_prod_bp`, `segment_sum_bp`, `softmax`, `softmax_bp`, `squaredsubtract_bp`, `standardize`, `standardize_bp`, `subtract_bp`, `unsorted_segment_max_bp`, `unsorted_segment_mean_bp`, `unsorted_segment_min_bp`, `unsorted_segment_prod_bp`, `unsorted_segment_sqrt_n_bp`, `unsorted_segment_sum_bp`, `xw_plus_b`, `zeta`.

**M5** (1): `gradientbackwards`.
