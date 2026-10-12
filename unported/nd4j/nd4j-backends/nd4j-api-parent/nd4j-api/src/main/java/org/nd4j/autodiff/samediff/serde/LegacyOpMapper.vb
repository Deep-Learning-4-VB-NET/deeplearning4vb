Imports System
Imports Op = org.nd4j.linalg.api.ops.Op
Imports Aggregate = org.nd4j.linalg.api.ops.aggregates.Aggregate
Imports org.nd4j.linalg.api.ops.aggregates.impl
Imports BroadcastAMax = org.nd4j.linalg.api.ops.impl.broadcast.BroadcastAMax
Imports BroadcastAMin = org.nd4j.linalg.api.ops.impl.broadcast.BroadcastAMin
Imports BroadcastMax = org.nd4j.linalg.api.ops.impl.broadcast.BroadcastMax
Imports BroadcastMin = org.nd4j.linalg.api.ops.impl.broadcast.BroadcastMin
Imports org.nd4j.linalg.api.ops.impl.indexaccum
Imports All = org.nd4j.linalg.api.ops.impl.reduce.bool.All
Imports Any = org.nd4j.linalg.api.ops.impl.reduce.bool.Any
Imports IsInf = org.nd4j.linalg.api.ops.impl.reduce.bool.IsInf
Imports IsNaN = org.nd4j.linalg.api.ops.impl.reduce.bool.IsNaN
Imports LogSumExp = org.nd4j.linalg.api.ops.impl.reduce.custom.LogSumExp
Imports org.nd4j.linalg.api.ops.impl.reduce.floating
Imports CountNonZero = org.nd4j.linalg.api.ops.impl.reduce.longer.CountNonZero
Imports CountZero = org.nd4j.linalg.api.ops.impl.reduce.longer.CountZero
Imports MatchCondition = org.nd4j.linalg.api.ops.impl.reduce.longer.MatchCondition
Imports AMax = org.nd4j.linalg.api.ops.impl.reduce.same.AMax
Imports AMin = org.nd4j.linalg.api.ops.impl.reduce.same.AMin
Imports org.nd4j.linalg.api.ops.impl.reduce.same
Imports Max = org.nd4j.linalg.api.ops.impl.reduce.same.Max
Imports Min = org.nd4j.linalg.api.ops.impl.reduce.same.Min
Imports org.nd4j.linalg.api.ops.impl.reduce3
Imports org.nd4j.linalg.api.ops.impl.scalar
Imports Pow = org.nd4j.linalg.api.ops.impl.scalar.Pow
Imports org.nd4j.linalg.api.ops.impl.scalar.comparison
Imports StandardDeviation = org.nd4j.linalg.api.ops.impl.summarystats.StandardDeviation
Imports Variance = org.nd4j.linalg.api.ops.impl.summarystats.Variance
Imports IsMax = org.nd4j.linalg.api.ops.impl.transforms.any.IsMax
Imports org.nd4j.linalg.api.ops.impl.transforms.comparison
Imports org.nd4j.linalg.api.ops.impl.transforms.custom
Imports RSqrt = org.nd4j.linalg.api.ops.impl.transforms.floating.RSqrt
Imports Sqrt = org.nd4j.linalg.api.ops.impl.transforms.floating.Sqrt
Imports org.nd4j.linalg.api.ops.impl.transforms.gradient
Imports BinaryMinimalRelativeError = org.nd4j.linalg.api.ops.impl.transforms.pairwise.BinaryMinimalRelativeError
Imports BinaryRelativeError = org.nd4j.linalg.api.ops.impl.transforms.pairwise.BinaryRelativeError
Imports RelativeError = org.nd4j.linalg.api.ops.impl.transforms.pairwise.RelativeError
Imports [Set] = org.nd4j.linalg.api.ops.impl.transforms.pairwise.Set
Imports org.nd4j.linalg.api.ops.impl.transforms.pairwise.arithmetic
Imports [And] = org.nd4j.linalg.api.ops.impl.transforms.pairwise.bool.And
Imports [Not] = org.nd4j.linalg.api.ops.impl.transforms.pairwise.bool.Not
Imports [Or] = org.nd4j.linalg.api.ops.impl.transforms.pairwise.bool.Or
Imports [Xor] = org.nd4j.linalg.api.ops.impl.transforms.pairwise.bool.Xor
Imports org.nd4j.linalg.api.ops.impl.transforms.same
Imports org.nd4j.linalg.api.ops.impl.transforms.strict
Imports TanhDerivative = org.nd4j.linalg.api.ops.impl.transforms.strict.TanhDerivative
Imports org.nd4j.linalg.api.ops.random.impl

'
' *  ******************************************************************************
' *  *
' *  *
' *  * This program and the accompanying materials are made available under the
' *  * terms of the Apache License, Version 2.0 which is available at
' *  * https://www.apache.org/licenses/LICENSE-2.0.
' *  *
' *  *  See the NOTICE file distributed with this work for additional
' *  *  information regarding copyright ownership.
' *  * Unless required by applicable law or agreed to in writing, software
' *  * distributed under the License is distributed on an "AS IS" BASIS, WITHOUT
' *  * WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the
' *  * License for the specific language governing permissions and limitations
' *  * under the License.
' *  *
' *  * SPDX-License-Identifier: Apache-2.0
' *  *****************************************************************************
' 

Namespace org.nd4j.autodiff.samediff.serde



	Public Class LegacyOpMapper

		Private Sub New()

		End Sub

		Public Shared Function getLegacyOpClassForId(ByVal opType As Op.Type, ByVal opNum As Integer) As Type
			Select Case opType
				Case Op.Type.SCALAR
					Return scalarOpClass(opNum)
				Case Op.Type.SCALAR_BOOL
					Return scalarBoolOpClass(opNum)
				Case Op.Type.TRANSFORM_SAME
					Return transformSameOpClass(opNum)
				Case Op.Type.TRANSFORM_STRICT
					Return transformStrictOpClass(opNum)
				Case Op.Type.PAIRWISE
					Return pairwiseOpClass(opNum)
				Case Op.Type.PAIRWISE_BOOL
					Return pairwiseBoolOpClass(opNum)
				Case Op.Type.BROADCAST
					Return broadcastOpClass(opNum)
				Case Op.Type.REDUCE_FLOAT
					Return reduceFloatOpClass(opNum)
				Case Op.Type.REDUCE_BOOL
					Return reduceBoolOpClass(opNum)
				Case Op.Type.REDUCE_SAME
					Return reduceSameOpClass(opNum)
				Case Op.Type.REDUCE_LONG
					Return reduceLongOpClass(opNum)
				Case Op.Type.INDEXREDUCE
					Return indexReduceClass(opNum)
				Case Op.Type.REDUCE3
					Return reduce3OpClass(opNum)
				Case Op.Type.RANDOM
					Return randomOpClass(opNum)
				Case Op.Type.AGGREGATION
					Return aggregateOpClass(opNum)
				Case Op.Type.VARIANCE, SUMMARYSTATS 'Intentional fall-through
					Return varianceOpClass(opNum)
				Case Op.Type.TRANSFORM_BOOL
					Return transformBoolOpClass(opNum)
				Case Op.Type.TRANSFORM_ANY
					Return transformAnyOpClass(opNum)
				Case Op.Type.TRANSFORM_FLOAT
					Return transformFloatingOpClass(opNum)

				Case Else
					Throw New System.NotSupportedException("Unable to map op " & opNum & " of type " & opType)
			End Select
		End Function

		Public Shared Function aggregateOpClass(ByVal opNum As Integer) As Type
			Select Case opNum

				Case 2
					Return GetType(AggregateAxpy)
				Case 5
					Return GetType(AggregateGEMM)
				Case Else
					Throw New System.NotSupportedException("No known aggregate op for op number: " & opNum)
			End Select
		End Function

		Public Shared Function broadcastOpClass(ByVal opNum As Integer) As Type
			Select Case opNum
				Case 0
					Return GetType(AddOp)
				Case 1
					Return GetType(SubOp)
				Case 2
					Return GetType(MulOp)
				Case 3
					Return GetType(DivOp)
				Case 4
					Return GetType(RDivOp)
				Case 5
					Return GetType(RSubOp)
				Case 6
					Return GetType(CopyOp)
				Case 7
					Return GetType(EqualTo)
				Case 8
					Return GetType(GreaterThan)
				Case 9
					Return GetType(GreaterThanOrEqual)
				Case 10
					Return GetType(LessThan)
				Case 11
					Return GetType(LessThanOrEqual)
				Case 12
					Return GetType(NotEqualTo)
				Case 13
					Return GetType(BroadcastMin)
				Case 14
					Return GetType(BroadcastMax)
				Case 15
					Return GetType(BroadcastAMin)
				Case 16
					Return GetType(BroadcastAMax)
				Case 17
					Return GetType(SquaredDifferenceOp)
				Case 18
					Return GetType(FloorModOp)
				Case 19
					Return GetType(FloorDivOp)
				Case 23
					Return GetType(TruncateDivOp)
				Case 24

					Return GetType([And])
				Case 25
					Return GetType([Or])
				Case 26
					Throw New System.NotSupportedException("OldATan2 (op number " & opNum & ") is no longer supported.")
				Case 27
					Return GetType(LogicalOr)
				Case 28
					Return GetType(LogicalXor)
				Case 29
					Return GetType([Not])
				Case 30
					Return GetType(LogicalAnd)
				Case Else
					Throw New System.NotSupportedException("No known broadcast op for op number: " & opNum)
			End Select
		End Function

		Public Shared Function transformSameOpClass(ByVal opNum As Integer) As Type
			Select Case opNum
				Case 0
					Return GetType(Abs)
				Case 1
					Return GetType(Sign)
				Case 3
					Return GetType(Negative)
				Case 4
					Return GetType(Round)
				Case 5
					Return GetType(TimesOneMinus)
				Case 6
					Return GetType(Cube)
				Case 7
					Return GetType(OneMinus)
				Case 8
					Return GetType(org.nd4j.linalg.api.ops.impl.transforms.same.Min)
				Case 11
					Return GetType(Reciprocal)
				Case 12
					Return GetType(Square)
				Case 13
					Return GetType(CompareAndSet)
				Case 15
					Return GetType(FModOp)
				Case 17
					Return GetType(Ceil)
				Case 18
					Return GetType(Floor)
				Case 20
					Throw New System.NotSupportedException("OldReverse (op number " & opNum & ") is no longer supported.")
				Case Else
					Throw New System.NotSupportedException("No known transform same op for op number: " & opNum)
			End Select
		End Function

		Public Shared Function transformStrictOpClass(ByVal opNum As Integer) As Type
			Select Case opNum
				Case 0
					Return GetType(Abs)
				Case 2
					Return GetType(LogSoftMax)
				Case 4
					Return GetType(TanhDerivative)
				Case 5
					Return GetType(HardTanhDerivative)
				Case 6
					Return GetType(org.nd4j.linalg.api.ops.impl.transforms.strict.SigmoidDerivative)
				Case 7
					Return GetType(SoftSignDerivative)
				Case 8
					Return GetType(TanhDerivative)
				Case 9
					Return GetType(SELUDerivative)
				Case 10
					Return GetType(HardSigmoidDerivative)
				Case 11
					Return GetType(RationalTanhDerivative)
				Case 12
					Return GetType(RectifiedTanhDerivative)
				Case 13
					Return GetType(SwishDerivative)
				Case 19
					Return GetType(Stabilize)
				Case 21
					Return GetType(CubeDerivative)
				Case 22
					Return GetType(Cos)
				Case 23
					Return GetType(Exp)
				Case 24
					Return GetType(Log)
				Case 25
					Return GetType(SetRange)
				Case 26
					Return GetType(Sigmoid)
				Case 27
					Return GetType(Sin)
				Case 28
					Return GetType(SoftPlus)
				Case 29
					Return GetType(Tanh)
				Case 30
					Return GetType(ACos)
				Case 31
					Return GetType(ASin)
				Case 32
					Return GetType(ATan)
				Case 33
					Return GetType(HardTanh)
				Case 34
					Return GetType(SoftSign)
				Case 35
					Return GetType(ELU)
				Case 36
					Return GetType(HardSigmoid)
				Case 37
					Return GetType(RationalTanh)
				Case 38
					Return GetType(RectifiedTanh)
				Case 39
					Return GetType(Sinh)
				Case 40
					Return GetType(Cosh)
				Case 41
					Return GetType(Tan)
				Case 42
					Return GetType(SELU)
				Case 43
					Return GetType(Swish)
				Case 44
					Return GetType(Log1p)
				Case 45
					Return GetType(Erf)
				Case 46
					Return GetType(ACosh)
				Case 47
					Return GetType(ASinh)
				Case 48
					Return GetType(Rint)
				Case 49
					Return GetType(LogSigmoid)
				Case 50
					Return GetType(Erfc)
				Case 51
					Return GetType(Expm1)
				Case 52
					Return GetType(ATanh)
				Case 53
					Return GetType(GELU)
				Case 54
					Return GetType(GELUDerivative)
				Case 55
					Return GetType(PreciseGELU)
				Case 56
					Return GetType(PreciseGELUDerivative)
				Case Else
					Throw New System.NotSupportedException("No known transform strict op for op number: " & opNum)
			End Select
		End Function

		Public Shared Function scalarOpClass(ByVal opNum As Integer) As Type
			Select Case opNum
				Case 0
					Return GetType(ScalarAdd)
				Case 1
					Return GetType(ScalarSubtraction)
				Case 2
					Return GetType(ScalarMultiplication)
				Case 3
					Return GetType(ScalarDivision)
				Case 4
					Return GetType(ScalarReverseDivision)
				Case 5
					Return GetType(ScalarReverseSubtraction)
				Case 6
					Return GetType(ScalarMax)
				Case 7
					Return GetType(ScalarLessThan)
				Case 8
					Return GetType(ScalarGreaterThan)
				Case 9
					Return GetType(ScalarEquals)
				Case 10
					Return GetType(ScalarLessThanOrEqual)
				Case 11
					Return GetType(ScalarNotEquals)
				Case 13
					Return GetType(ScalarMin)
				Case 14
					Return GetType(ScalarSet)
				Case 16
					Return GetType(ScalarGreaterThanOrEqual)
				Case 17
					Return GetType(ScalarRemainder)
				Case 18
					Return GetType(ScalarFMod)
				Case 31
					Return GetType(Pow)
				Case 32
					Return GetType(PowDerivative)
				Case 35
					Return GetType(LeakyReLU)
				Case 37
					Return GetType(ReplaceNans)
				Case 38
					Return GetType(LogX)
				Case 39
					Return GetType(RectifiedLinear)
				Case 40
					Return GetType(Relu6)
				Case 41
					Return GetType([Step])
				Case Else
					Throw New System.NotSupportedException("No known scalar op for op number: " & opNum)
			End Select
		End Function

		Public Shared Function scalarBoolOpClass(ByVal opNum As Integer) As Type
			Select Case opNum
				Case 0
					Return GetType(ScalarEquals)
				Case 1
					Return GetType(ScalarGreaterThan)
				Case 2
					Return GetType(ScalarLessThan)
				Case 3
					Return GetType(ScalarEps)
				Case 4
					Return GetType(ScalarGreaterThanOrEqual)
				Case 5
					Return GetType(MatchCondition)
				Case 6
					Return GetType(ScalarNotEquals)
				Case 7
					Return GetType(ScalarAdd)
				Case 8
					Return GetType(ScalarOr)
				Case 9
					Return GetType(ScalarXor)
				Case 10
					Return GetType(ScalarNot)
				Case 11
					Return GetType(ScalarLessThanOrEqual)
				Case Else
					Throw New System.NotSupportedException("No known scalar bool op for op number: " & opNum)
			End Select
		End Function

		Public Shared Function reduce3OpClass(ByVal opNum As Integer) As Type
			Select Case opNum
				Case 0
					Return GetType(ManhattanDistance)
				Case 1
					Return GetType(EuclideanDistance)
				Case 2
					Return GetType(CosineSimilarity)
				Case 3
					Return GetType(Dot)
				Case 4
					Return GetType(EqualsWithEps)
				Case 5
					Return GetType(CosineDistance)
				Case 6
					Return GetType(JaccardDistance)
				Case 7
					Return GetType(HammingDistance)
				Case Else
					Throw New System.NotSupportedException("No known reduce3 op for op number: " & opNum)
			End Select
		End Function

		Public Shared Function reduceFloatOpClass(ByVal opNum As Integer) As Type
			Select Case opNum
				Case 0
					Return GetType(Mean)
				Case 1
					Return GetType(AMean)
				Case 2
					Return GetType(Norm1)
				Case 3
					Return GetType(Norm2)
				Case 4
					Return GetType(NormMax)
				Case 7
					Return GetType(SquaredNorm)
				Case 8
					Return GetType(Entropy)
				Case 9
					Return GetType(LogEntropy)
				Case 10
					Return GetType(ShannonEntropy)
				Case 11
					Return GetType(LogSumExp)
				Case Else
					Throw New System.NotSupportedException("No known reduce float op for op number: " & opNum)
			End Select
		End Function

		Public Shared Function reduceSameOpClass(ByVal opNum As Integer) As Type
			Select Case opNum
				Case 0
					Return GetType(Sum)
				Case 1
					Return GetType(Max)
				Case 2
					Return GetType(Min)
				Case 3
					Return GetType(Prod)
				Case 4
					Return GetType(ASum)
				Case 5
					Return GetType(AMax)
				Case 6
					Return GetType(AMin)
				Case Else
					Throw New System.NotSupportedException("No known reduce same op for op number: " & opNum)
			End Select
		End Function

		Public Shared Function reduceLongOpClass(ByVal opNum As Integer) As Type
			Select Case opNum
				Case 0
					Return GetType(CountNonZero)
				Case 1
					Return GetType(CountZero)
				Case 2
					Return GetType(MatchCondition)
				Case Else
					Throw New System.NotSupportedException("No known reduce long op for op number: " & opNum)
			End Select
		End Function

		Public Shared Function reduceBoolOpClass(ByVal opNum As Integer) As Type
			Select Case opNum
				Case 0
					Return GetType(Any)
				Case 1
					Return GetType(All)
				Case 4
					Return GetType(IsNaN)
				Case 5
					Return GetType(IsInf)
				Case Else
					Throw New System.NotSupportedException("No known reduce bool op for op number: " & opNum)
			End Select
		End Function

		Public Shared Function randomOpClass(ByVal opNum As Integer) As Type
			Select Case opNum
				Case 0
					Return GetType(UniformDistribution)
				Case 1
					Return GetType(DropOut)
				Case 2
					Return GetType(DropOutInverted)
				Case 3
					Return GetType(ProbablisticMerge)
				Case 4
					Return GetType(Linspace)
				Case 5
					Return GetType(Choice)
				Case 6
					Return GetType(GaussianDistribution)
				Case 7
					Return GetType(BernoulliDistribution)
				Case 8
					Return GetType(BinomialDistribution)
				Case 9
					Return GetType(BinomialDistributionEx)
				Case 10
					Return GetType(LogNormalDistribution)
				Case 11
					Return GetType(TruncatedNormalDistribution)
				Case 12
					Return GetType(AlphaDropOut)
				Case Else
					Throw New System.NotSupportedException("No known random op for op number: " & opNum)
	'            case 13:
	'                return ExponentialDistribution.class;
	'            case 14:
	'                return ExponentialDistributionInv.class;
			End Select
		End Function

		Public Shared Function pairwiseOpClass(ByVal opNum As Integer) As Type
			Select Case opNum
			Case 0
					Throw New System.NotSupportedException("OldFModOp (op number " & opNum & ") is no longer supported.")
			Case 1
					Return GetType(CopyOp)
			Case 2
					Throw New System.NotSupportedException("OldDivOp (op number " & opNum & ") is no longer supported.")
			Case 3
					Throw New System.NotSupportedException("OldEqualTo (op number " & opNum & ") is no longer supported.")
			Case 4
					Throw New System.NotSupportedException("OldGreaterThan (op number " & opNum & ") is no longer supported.")
			Case 5
					Throw New System.NotSupportedException("OldLessThan (op number " & opNum & ") is no longer supported.")
			Case 6
					Throw New System.NotSupportedException("OldMulOp (op number " & opNum & ") is no longer supported.")
			Case 7
					Return GetType(Pow)
			Case 8
					Return GetType(RSubOp)
			Case 9
					Return GetType(SubOp)
			Case 10
					Return GetType(Eps)
			Case 11
					Throw New System.NotSupportedException("OldGreaterThanOrEqual (op number " & opNum & ") is no longer supported.")
			Case 12
					Throw New System.NotSupportedException("OldLessThanOrEqual (op number " & opNum & ") is no longer supported.")
			Case 13
					Throw New System.NotSupportedException("OldMax (op number " & opNum & ") is no longer supported.")
			Case 14
					Throw New System.NotSupportedException("OldMin (op number " & opNum & ") is no longer supported.")
			Case 15
					Throw New System.NotSupportedException("OldNotEqualTo (op number " & opNum & ") is no longer supported.")
			Case 16
					Return GetType([Set])
			Case 17
					Return GetType(Axpy)
			Case 18
					Return GetType(RDivOp)
			Case 45
					Return GetType(CompareAndSet)
			Case 46
					Return GetType(CompareAndReplace)
			Case 56
					Return GetType([And])
			Case 57
					Return GetType([Or])
			Case 58
					Return GetType([Xor])
			Case 59
					Return GetType(RemainderOp)
			Case 60
					Throw New System.NotSupportedException("OldFModOp (op number " & opNum & ") is no longer supported.")
			Case 69
					Throw New System.NotSupportedException("OldATan2 (op number " & opNum & ") is no longer supported.")
			Case 20
					Throw New System.NotSupportedException("OldFloorDivOp (op number " & opNum & ") is no longer supported.")
			Case 26
					Return GetType(RelativeError)
			Case 27
					Return GetType(BinaryRelativeError)
			Case 28
					Return GetType(BinaryMinimalRelativeError)
			Case 92
					Return GetType(PowDerivative)
			Case Else
				Throw New System.NotSupportedException("No known pairwise op for op number: " & opNum)

	'        case 19:
	'            return TruncateDiv.class;
	'            case 21:
	'                return FloorMod.class;
	'            case 22:
	'                return SquaredSubtract.class;
	'            case 23:
	'                return ReverseMod.class;
	'            case 24:
	'                return SafeDivide.class;
	'            case 25:
	'                return Mod.class;
	'            case 29:
	'                return LogicalOr.class;
	'            case 30:
	'                return LogicalXor.class;
	'            case 31:
	'                return LogicalNot.class;
	'            case 32:
	'                return LogicalAnd.class;
	'            case 93:
	'                return LogPoisonLoss.class;
	'            case 94:
	'                return LogPoisonLossFull.class;
			End Select
		End Function

		Public Shared Function pairwiseBoolOpClass(ByVal opNum As Integer) As Type
			Select Case opNum
				Case 7
					Return GetType([And])
				Case 8
					Return GetType([Or])
				Case 9
					Return GetType([Xor])
				Case Else
					Throw New System.NotSupportedException("No known pairwise bool op for op number: " & opNum)
			End Select
		End Function

		Public Shared Function indexReduceClass(ByVal opNum As Integer) As Type
			Select Case opNum
				Case 4
					Return GetType(FirstIndex)
				Case 5
					Return GetType(LastIndex)
				Case Else
					Throw New System.NotSupportedException("No known index reduce op for op number: " & opNum)
			End Select
		End Function

		Public Shared Function varianceOpClass(ByVal opNum As Integer) As Type
			Select Case opNum
				Case 0
					Return GetType(Variance)
				Case 1
					Return GetType(StandardDeviation)
				Case Else
					Throw New System.NotSupportedException("No known variance op for op number: " & opNum)
			End Select
		End Function

		Public Shared Function transformBoolOpClass(ByVal opNum As Integer) As Type
			Select Case opNum
				Case 1
					Return GetType(org.nd4j.linalg.api.ops.impl.transforms.bool.IsInf)
				Case 2
					Return GetType(org.nd4j.linalg.api.ops.impl.transforms.bool.IsNaN)
				Case 3
					Return GetType(org.nd4j.linalg.api.ops.impl.transforms.bool.IsFinite)
				Case 5
					Return GetType(org.nd4j.linalg.api.ops.impl.transforms.bool.MatchConditionTransform)
				Case 7
					Return GetType(org.nd4j.linalg.api.ops.impl.transforms.bool.BooleanNot)
				Case Else
					Throw New System.NotSupportedException("No known transform bool op for op number: " & opNum)
			End Select
		End Function

		Public Shared Function transformAnyOpClass(ByVal opNum As Integer) As Type
			Select Case opNum
				Case 0
					Return GetType(Assign)
				Case 1
					Return GetType(IsMax)
				Case Else
					Throw New System.NotSupportedException("No known transform any op for op number: " & opNum)
			End Select
		End Function

		Public Shared Function transformFloatingOpClass(ByVal opNum As Integer) As Type
			Select Case opNum
				Case 1
					Return GetType(Sqrt)
				Case 3
					Return GetType(RSqrt)
				Case Else
					Throw New System.NotSupportedException("No known transform floating op for op number: " & opNum)
			End Select
		End Function

	End Class

End Namespace