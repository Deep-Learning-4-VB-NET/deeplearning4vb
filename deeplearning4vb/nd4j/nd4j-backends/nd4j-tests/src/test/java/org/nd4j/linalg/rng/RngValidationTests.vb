Imports System
Imports System.Collections.Generic
import static org.junit.jupiter.api.Assertions.assertEquals
import static org.junit.jupiter.api.Assertions.fail
Imports Builder = lombok.Builder
Imports Data = lombok.Data
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports Disabled = org.junit.jupiter.api.Disabled
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports Execution = org.junit.jupiter.api.parallel.Execution
Imports ExecutionMode = org.junit.jupiter.api.parallel.ExecutionMode
Imports ParameterizedTest = org.junit.jupiter.params.ParameterizedTest
Imports MethodSource = org.junit.jupiter.params.provider.MethodSource
Imports Preconditions = org.nd4j.common.base.Preconditions
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports ArrayUtil = org.nd4j.common.util.ArrayUtil
Imports BaseNd4jTestWithBackends = org.nd4j.linalg.BaseNd4jTestWithBackends
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports CustomOp = org.nd4j.linalg.api.ops.CustomOp
Imports DynamicCustomOp = org.nd4j.linalg.api.ops.DynamicCustomOp
Imports Op = org.nd4j.linalg.api.ops.Op
Imports MatchConditionTransform = org.nd4j.linalg.api.ops.impl.transforms.bool.MatchConditionTransform
Imports RandomStandardNormal = org.nd4j.linalg.api.ops.random.compat.RandomStandardNormal
Imports DistributionUniform = org.nd4j.linalg.api.ops.random.custom.DistributionUniform
Imports RandomBernoulli = org.nd4j.linalg.api.ops.random.custom.RandomBernoulli
Imports RandomExponential = org.nd4j.linalg.api.ops.random.custom.RandomExponential
Imports AlphaDropOut = org.nd4j.linalg.api.ops.random.impl.AlphaDropOut
Imports BernoulliDistribution = org.nd4j.linalg.api.ops.random.impl.BernoulliDistribution
Imports BinomialDistribution = org.nd4j.linalg.api.ops.random.impl.BinomialDistribution
Imports Choice = org.nd4j.linalg.api.ops.random.impl.Choice
Imports DropOut = org.nd4j.linalg.api.ops.random.impl.DropOut
Imports DropOutInverted = org.nd4j.linalg.api.ops.random.impl.DropOutInverted
Imports GaussianDistribution = org.nd4j.linalg.api.ops.random.impl.GaussianDistribution
Imports Linspace = org.nd4j.linalg.api.ops.random.impl.Linspace
Imports LogNormalDistribution = org.nd4j.linalg.api.ops.random.impl.LogNormalDistribution
Imports ProbablisticMerge = org.nd4j.linalg.api.ops.random.impl.ProbablisticMerge
Imports TruncatedNormalDistribution = org.nd4j.linalg.api.ops.random.impl.TruncatedNormalDistribution
Imports UniformDistribution = org.nd4j.linalg.api.ops.random.impl.UniformDistribution
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports Nd4jBackend = org.nd4j.linalg.factory.Nd4jBackend
Imports Conditions = org.nd4j.linalg.indexing.conditions.Conditions

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

Namespace org.nd4j.linalg.rng


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j @NativeTag @Tag(TagNames.RNG) @Execution(ExecutionMode.SAME_THREAD) public class RngValidationTests extends org.nd4j.linalg.BaseNd4jTestWithBackends
	Public Class RngValidationTests
		Inherits BaseNd4jTestWithBackends

		Public Overrides Function ordering() As Char
			Return "c"c
		End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Builder(builderClassName = "TestCaseBuilder") @Data public static class TestCase
		Public Class TestCase
			Friend opType As String
			Friend dataType As DataType
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Builder.@Default private long rngSeed = 12345;
			Friend rngSeed As Long = 12345
			Friend shape() As Long
			Friend minValue As Double
			Friend maxValue As Double
			Friend minValueInclusive As Boolean
			Friend maxValueInclusive As Boolean
			Friend expectedMean As Double?
			Friend expectedStd As Double?
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Builder.@Default private double meanRelativeErrorTolerance = 0.01;
			Friend meanRelativeErrorTolerance As Double = 0.01
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Builder.@Default private double stdRelativeErrorTolerance = 0.01;
			Friend stdRelativeErrorTolerance As Double = 0.01
			Friend meanMinAbsErrorTolerance As Double? 'Consider relative error between 0 and 0.001: relative error is 1.0, but absolute error is small
			Friend stdMinAbsErrorTolerance As Double?
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Builder.@Default private static java.util.Map<String,Object> args = new java.util.LinkedHashMap<>();
			Friend Shared args As IDictionary(Of String, Object) = New LinkedHashMap(Of String, Object)()

			Public Class TestCaseBuilder

'JAVA TO VB CONVERTER NOTE: The parameter arg was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
				Public Overridable Function arg(ByVal arg_Conflict As String, ByVal value As Object) As TestCaseBuilder
					If args Is Nothing Then
						args = New LinkedHashMap(Of String, Object)()
					End If
					args(arg_Conflict) = value
					Return Me
				End Function

'JAVA TO VB CONVERTER NOTE: The parameter shape was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
				Public Overridable Function shape(ParamArray ByVal shape_Conflict() As Long) As TestCaseBuilder
					Me.shape = shape_Conflict
					Return Me
				End Function
			End Class

			Public Overridable Function arr() As INDArray
				Preconditions.checkState(shape IsNot Nothing, "Shape is null")
'JAVA TO VB CONVERTER NOTE: The local variable arr was renamed since Visual Basic will not allow local variables with the same name as their enclosing function or property:
				Dim arr_Conflict As INDArray = Nd4j.createUninitialized(dataType, shape)
				arr_Conflict.assign(Double.NaN) 'Assign NaNs to help detect implementation issues
				Return arr_Conflict
			End Function

			Public Overridable Function prop(Of T)(ByVal s As String) As T
				Preconditions.checkState(args IsNot Nothing AndAlso args.ContainsKey(s), "Property ""%s"" not found. All properties: %s", s, args)
				Return DirectCast(args(s), T)
			End Function
		End Class


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") @Disabled @Tag(org.nd4j.common.tests.tags.TagNames.NEEDS_VERIFY) public void validateRngDistributions(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub validateRngDistributions(ByVal backend As Nd4jBackend)
			Dim testCases As IList(Of TestCase) = New List(Of TestCase)()
			For Each type As DataType In New DataType(){DataType.DOUBLE, DataType.FLOAT, DataType.HALF}
				'Legacy (non-custom) RNG ops:
				testCases.Add(TestCase.builder().opType("bernoulli").dataType(type).shape(New Long(){}).minValue(0).maxValue(1).minValueInclusive(True).maxValueInclusive(True).arg("prob", 0.5).build()) 'Don't check mean/std for 1 element
				testCases.Add(TestCase.builder().opType("bernoulli").dataType(type).shape(1000).minValue(0).maxValue(1).minValueInclusive(True).maxValueInclusive(True).arg("prob", 0.5).expectedMean(0.5).expectedStd(Math.Sqrt(0.5*0.5)).build())
				testCases.Add(TestCase.builder().opType("bernoulli").dataType(type).shape(100,10000).minValue(0).maxValue(1).minValueInclusive(True).maxValueInclusive(True).arg("prob", 0.2).expectedMean(0.2).expectedStd(Math.Sqrt(0.2*(1-0.2))).meanRelativeErrorTolerance(0.005).stdRelativeErrorTolerance(0.01).build())

				testCases.Add(TestCase.builder().opType("uniform").dataType(type).shape(New Long(){}).minValue(0).maxValue(1).minValueInclusive(True).maxValueInclusive(True).arg("min", 0.0).arg("max", 1.0).build()) 'Don't check mean/std for 1 element
				testCases.Add(TestCase.builder().opType("uniform").dataType(type).shape(1000).minValue(1).maxValue(2).minValueInclusive(True).maxValueInclusive(True).arg("min", 1.0).arg("max",2.0).expectedMean((1+2)/2.0).expectedStd(Math.Sqrt(1/12.0 * Math.Pow(2.0-1.0, 2))).build())
				testCases.Add(TestCase.builder().opType("uniform").dataType(type).shape(100,10000).minValue(-4).maxValue(-2).minValueInclusive(True).maxValueInclusive(True).arg("min", -4.0).arg("max",-2.0).expectedMean(-3.0).expectedStd(Math.Sqrt(1/12.0 * Math.Pow(-4.0+2.0, 2))).meanRelativeErrorTolerance(0.005).stdRelativeErrorTolerance(0.01).build())

				testCases.Add(TestCase.builder().opType("gaussian").dataType(type).shape(New Long(){}).minValue(minValue(type)).maxValue(maxValue(type)).minValueInclusive(True).maxValueInclusive(True).arg("mean", 0.0).arg("std", 1.0).build()) 'Don't check mean/std for 1 element
				testCases.Add(TestCase.builder().opType("gaussian").dataType(type).shape(1000).minValue(minValue(type)).maxValue(maxValue(type)).minValueInclusive(True).maxValueInclusive(True).arg("mean", 0.0).arg("std", 1.0).expectedMean(0.0).expectedStd(1.0).stdRelativeErrorTolerance(0.03).meanMinAbsErrorTolerance(0.1).stdMinAbsErrorTolerance(0.1).build())
				testCases.Add(TestCase.builder().opType("gaussian").dataType(type).shape(100,1000).minValue(minValue(type)).maxValue(maxValue(type)).minValueInclusive(True).maxValueInclusive(True).arg("mean", 2.0).arg("std", 0.5).expectedMean(2.0).expectedStd(0.5).meanRelativeErrorTolerance(0.01).stdRelativeErrorTolerance(0.01).meanMinAbsErrorTolerance(0.001).build())

				testCases.Add(TestCase.builder().opType("binomial").dataType(type).shape(New Long(){}).minValue(0).maxValue(5).minValueInclusive(True).maxValueInclusive(True).arg("n", 5).arg("p",0.5).build()) 'Don't check mean/std for 1 element
				testCases.Add(TestCase.builder().opType("binomial").dataType(type).shape(1000).minValue(0).maxValue(10).minValueInclusive(True).maxValueInclusive(True).arg("n", 10).arg("p",0.5).stdRelativeErrorTolerance(0.02).expectedMean(10*0.5).expectedStd(Math.Sqrt(10*0.5*(1-0.5))).build())
				testCases.Add(TestCase.builder().opType("binomial").dataType(type).shape(100,10000).minValue(0).maxValue(20).minValueInclusive(True).maxValueInclusive(True).arg("n", 20).arg("p",0.2).expectedMean(20*0.2).expectedStd(Math.Sqrt(20*0.2*(1-0.2))).meanRelativeErrorTolerance(0.001).stdRelativeErrorTolerance(0.01).build())

				'truncated normal clips at (mean-2*std, mean+2*std). Mean for equal 2 sided clipping about mean is same as original mean. Variance is difficult to calculate...
				'Assume variance is similar to non-truncated normal (should be a bit less in practice) but use large relative error here
				testCases.Add(TestCase.builder().opType("truncated_normal").dataType(type).shape(New Long(){}).minValue(-2.0).maxValue(2.0).minValueInclusive(True).maxValueInclusive(True).arg("mean", 0.0).arg("std", 1.0).build()) 'Don't check mean/std for 1 element
				testCases.Add(TestCase.builder().opType("truncated_normal").dataType(type).shape(1000).minValue(-2.0).maxValue(2.0).minValueInclusive(True).maxValueInclusive(True).arg("mean", 0.0).arg("std", 1.0).expectedMean(0.0).expectedStd(1.0).stdRelativeErrorTolerance(0.2).meanMinAbsErrorTolerance(0.1).build())
				testCases.Add(TestCase.builder().opType("truncated_normal").dataType(type).shape(100,10000).minValue(1.0).maxValue(3.0).minValueInclusive(True).maxValueInclusive(True).arg("mean", 2.0).arg("std", 0.5).expectedMean(2.0).expectedStd(0.5).meanRelativeErrorTolerance(0.001).stdRelativeErrorTolerance(0.2).meanMinAbsErrorTolerance(0.001).build())

				'Dropout (non-inverted): same as bernoulli distribution, when dropout applied to "ones" array
				testCases.Add(TestCase.builder().opType("dropout").dataType(type).shape(New Long(){}).minValue(0).maxValue(1).minValueInclusive(True).maxValueInclusive(True).arg("p", 0.5).build()) 'Don't check mean/std for 1 element
				testCases.Add(TestCase.builder().opType("dropout").dataType(type).shape(1000).minValue(0).maxValue(1).minValueInclusive(True).maxValueInclusive(True).arg("p", 0.4).expectedMean(0.4).expectedStd(Math.Sqrt(0.4*(1-0.4))).meanMinAbsErrorTolerance(0.05).stdMinAbsErrorTolerance(0.05).build())
				testCases.Add(TestCase.builder().opType("dropout").dataType(type).shape(100,10000).minValue(0).maxValue(1).minValueInclusive(True).maxValueInclusive(True).arg("p", 0.3).expectedMean(0.3).expectedStd(Math.Sqrt(0.3*(1-0.3))).meanRelativeErrorTolerance(0.005).stdRelativeErrorTolerance(0.01).build())

				'Dropout (inverted): basically bernoulli distribution * 2, when inverted dropout applied to "ones" array
				testCases.Add(TestCase.builder().opType("dropout_inverted").dataType(type).shape(New Long(){}).minValue(0).maxValue(1).minValueInclusive(True).maxValueInclusive(True).arg("p", 0.5).build()) 'Don't check mean/std for 1 element
				testCases.Add(TestCase.builder().opType("dropout_inverted").dataType(type).shape(1000).minValue(0).maxValue(1.0/0.4).minValueInclusive(True).maxValueInclusive(True).arg("p", 0.4).expectedMean(1.0).expectedStd(1/0.4*Math.Sqrt(0.4*(1-0.4))).meanMinAbsErrorTolerance(0.05).stdMinAbsErrorTolerance(0.05).build())
				testCases.Add(TestCase.builder().opType("dropout_inverted").dataType(type).shape(100,10000).minValue(0).maxValue(1.0/0.3).minValueInclusive(True).maxValueInclusive(True).arg("p", 0.3).expectedMean(1.0).expectedStd(1/0.3*Math.Sqrt(0.3*(1-0.3))).meanRelativeErrorTolerance(0.005).stdRelativeErrorTolerance(0.01).build())

				'Linspace: we'll treat is as basically a uniform distribution for the purposes of these tests...
				testCases.Add(TestCase.builder().opType("linspace").dataType(type).shape(1000).minValue(1).maxValue(2).minValueInclusive(True).maxValueInclusive(True).arg("from", 1.0).arg("to",2.0).expectedMean(1.5).expectedStd(Math.Sqrt(1/12.0 * Math.Pow(2.0-1.0, 2))).build())

				'Log normal distribution: parameterized such that if X~lognormal(m,s) then mean(log(X))=m and std(log(X))=s
				'mean is given by exp(mu+s^2/2), variance [exp(s^2)-1]*[exp(2*mu+s^2)]
				testCases.Add(TestCase.builder().opType("lognormal").dataType(type).shape(New Long(){}).minValue(0).maxValue(maxValue(type)).minValueInclusive(True).maxValueInclusive(True).arg("mu", 0.0).arg("s", 1.0).build()) 'Don't check mean/std for 1 element
				testCases.Add(TestCase.builder().opType("lognormal").dataType(type).shape(1000).minValue(0).maxValue(maxValue(type)).minValueInclusive(True).maxValueInclusive(True).arg("mu", 0.0).arg("s", 1.0).expectedMean(Math.Exp(0.0 + 1.0/2.0)).expectedStd(Math.Sqrt((Math.Exp(1.0)-1)*Math.Exp(1.0))).meanRelativeErrorTolerance(0.1).stdRelativeErrorTolerance(0.1).meanMinAbsErrorTolerance(0.1).stdMinAbsErrorTolerance(0.1).build())
				testCases.Add(TestCase.builder().opType("lognormal").dataType(type).shape(100,10000).minValue(0).maxValue(maxValue(type)).minValueInclusive(True).maxValueInclusive(True).arg("mu", 2.0).arg("s", 0.5).expectedMean(Math.Exp(2.0 + 0.5*0.5/2.0)).expectedStd(Math.Sqrt((Math.Exp(0.5*0.5)-1)*Math.Exp(2.0*2.0+0.5*0.5))).meanRelativeErrorTolerance(0.01).stdRelativeErrorTolerance(0.01).meanMinAbsErrorTolerance(0.001).build())

				'Choice op. For the purposes of this test, use discrete uniform distribution with values 0 to 10 inclusive
				testCases.Add(TestCase.builder().opType("choice").dataType(type).shape(New Long(){}).minValue(0).maxValue(10).minValueInclusive(True).maxValueInclusive(True).build()) 'Don't check mean/std for 1 element
'JAVA TO VB CONVERTER WARNING: Java to VB Converter cannot determine whether both operands of this division are integer types - if they are then you should use the VB integer division operator:
				testCases.Add(TestCase.builder().opType("choice").dataType(type).shape(1000).minValue(0).maxValue(10).minValueInclusive(True).maxValueInclusive(True).expectedMean(5.0).expectedStd(Math.Sqrt((Math.Pow(10-0+1,2)-1)/12.0)).meanRelativeErrorTolerance(0.05).stdRelativeErrorTolerance(0.05).meanMinAbsErrorTolerance(0.05).stdMinAbsErrorTolerance(0.05).build())
'JAVA TO VB CONVERTER WARNING: Java to VB Converter cannot determine whether both operands of this division are integer types - if they are then you should use the VB integer division operator:
				testCases.Add(TestCase.builder().opType("choice").dataType(type).shape(100,10000).minValue(0).maxValue(10).minValueInclusive(True).maxValueInclusive(True).expectedMean(5.0).expectedStd(Math.Sqrt((Math.Pow(10-0+1,2)-1)/12.0)).meanRelativeErrorTolerance(0.01).stdRelativeErrorTolerance(0.01).meanMinAbsErrorTolerance(0.001).build())

				'Probabilistic merge: use 0 and 1, 0.5 probability. Then it's same as bernoulli distribution
				testCases.Add(TestCase.builder().opType("probabilisticmerge").dataType(type).shape(New Long(){}).minValue(0).maxValue(1).minValueInclusive(True).maxValueInclusive(True).arg("prob", 0.5).build()) 'Don't check mean/std for 1 element
				testCases.Add(TestCase.builder().opType("probabilisticmerge").dataType(type).shape(1000).minValue(0).maxValue(1).minValueInclusive(True).maxValueInclusive(True).arg("prob", 0.5).expectedMean(0.5).expectedStd(Math.Sqrt(0.5*0.5)).build())
				testCases.Add(TestCase.builder().opType("probabilisticmerge").dataType(type).shape(100,10000).minValue(0).maxValue(1).minValueInclusive(True).maxValueInclusive(True).arg("prob", 0.2).expectedMean(0.2).expectedStd(Math.Sqrt(0.2*(1-0.2))).meanRelativeErrorTolerance(0.005).stdRelativeErrorTolerance(0.01).build())

				'Range: x to y in N steps - essentially same statistical properties as uniform distribution
				testCases.Add(TestCase.builder().opType("range").dataType(type).shape(10).minValue(0).maxValue(1).minValueInclusive(True).maxValueInclusive(True).arg("min", 0.0).arg("max", 1.0).build()) 'Don't check mean/std for 1 element
				testCases.Add(TestCase.builder().opType("range").dataType(type).shape(1000).minValue(1).maxValue(2).minValueInclusive(True).maxValueInclusive(True).arg("min", 1.0).arg("max",2.0).expectedMean((1+2)/2.0).expectedStd(Math.Sqrt(1/12.0 * Math.Pow(2.0-1.0, 2))).build())

				'AlphaDropout: implements a * (x * d + alphaPrime * (1-d)) + b, where d ~ Bernoulli(p), i.e., d \in {0,1}.
				'For ones input and p=0.5, this should give us values (a+b or a*alphaPrime+b) with probability 0.5
				'Mean should be same as input - i.e., 1
				testCases.Add(TestCase.builder().opType("alphaDropout").dataType(type).shape(New Long(){}).maxValue(alphaDropoutA(0.5)+alphaDropoutB(0.5)).minValue(alphaDropoutA(0.5)*ALPHA_PRIME+alphaDropoutB(0.5)).minValueInclusive(True).maxValueInclusive(True).arg("p", 0.5).build()) 'Don't check mean/std for 1 element
				testCases.Add(TestCase.builder().opType("alphaDropout").dataType(type).shape(1000).maxValue(alphaDropoutA(0.4)+alphaDropoutB(0.4)).minValue(alphaDropoutA(0.4)*ALPHA_PRIME+alphaDropoutB(0.4)).minValueInclusive(True).maxValueInclusive(True).arg("p", 0.4).expectedMean(1.0).build())
				testCases.Add(TestCase.builder().opType("alphaDropout").dataType(type).shape(100,10000).maxValue(alphaDropoutA(0.3)+alphaDropoutB(0.3)).minValue(alphaDropoutA(0.3)*ALPHA_PRIME+alphaDropoutB(0.3)).minValueInclusive(True).maxValueInclusive(True).arg("p", 0.3).expectedMean(1.0).meanRelativeErrorTolerance(0.005).stdRelativeErrorTolerance(0.01).build())


				'--- Custom ops ---
				'DistributionUniform, RandomBernoulli, RandomExponential, RandomNormal, RandomStandardNormal
				testCases.Add(TestCase.builder().opType("distributionuniform").dataType(type).shape(New Long(){}).minValue(0).maxValue(1).minValueInclusive(True).maxValueInclusive(True).arg("min", 0.0).arg("max", 1.0).build()) 'Don't check mean/std for 1 element
				testCases.Add(TestCase.builder().opType("distributionuniform").dataType(type).shape(1000).minValue(1).maxValue(2).minValueInclusive(True).maxValueInclusive(True).arg("min", 1.0).arg("max",2.0).expectedMean((1+2)/2.0).expectedStd(Math.Sqrt(1/12.0 * Math.Pow(2.0-1.0, 2))).build())
				testCases.Add(TestCase.builder().opType("distributionuniform").dataType(type).shape(100,10000).minValue(-4).maxValue(-2).minValueInclusive(True).maxValueInclusive(True).arg("min", -4.0).arg("max",-2.0).expectedMean(-3.0).expectedStd(Math.Sqrt(1/12.0 * Math.Pow(-4.0+2.0, 2))).meanRelativeErrorTolerance(0.005).stdRelativeErrorTolerance(0.01).build())

				testCases.Add(TestCase.builder().opType("randombernoulli").dataType(type).shape(New Long(){}).minValue(0).maxValue(1).minValueInclusive(True).maxValueInclusive(True).arg("prob", 0.5).build()) 'Don't check mean/std for 1 element
				testCases.Add(TestCase.builder().opType("randombernoulli").dataType(type).shape(1000).minValue(0).maxValue(1).minValueInclusive(True).maxValueInclusive(True).arg("prob", 0.5).expectedMean(0.5).expectedStd(Math.Sqrt(0.5*0.5)).build())
				testCases.Add(TestCase.builder().opType("randombernoulli").dataType(type).shape(100,10000).minValue(0).maxValue(1).minValueInclusive(True).maxValueInclusive(True).arg("prob", 0.2).expectedMean(0.2).expectedStd(Math.Sqrt(0.2*(1-0.2))).meanRelativeErrorTolerance(0.005).stdRelativeErrorTolerance(0.01).build())

				'3 cases: lambda = 1, 1, 0.4
				testCases.Add(TestCase.builder().opType("randomexponential").dataType(type).shape(New Long(){}).minValue(0).maxValue(maxValue(type)).minValueInclusive(False).maxValueInclusive(True).arg("lambda", 1.0).build()) 'Don't check mean/std for 1 element
				testCases.Add(TestCase.builder().opType("randomexponential").dataType(type).shape(1000).minValue(0.0).maxValue(maxValue(type)).minValueInclusive(False).maxValueInclusive(True).arg("lambda", 1.0).expectedMean(1.0).expectedStd(1.0).build())
				testCases.Add(TestCase.builder().opType("randomexponential").dataType(type).shape(100,10000).minValue(0.0).maxValue(maxValue(type)).minValueInclusive(False).maxValueInclusive(True).arg("lambda", 0.4).expectedMean(1.0 / 0.4).expectedStd(1.0 / Math.Pow(0.4, 2)).meanRelativeErrorTolerance(0.005).stdRelativeErrorTolerance(0.01).build())

				testCases.Add(TestCase.builder().opType("randomnormal").dataType(type).shape(New Long(){}).minValue(minValue(type)).maxValue(maxValue(type)).minValueInclusive(True).maxValueInclusive(True).arg("mean", 0.0).arg("std", 1.0).build()) 'Don't check mean/std for 1 element
				testCases.Add(TestCase.builder().opType("randomnormal").dataType(type).shape(1000).minValue(minValue(type)).maxValue(maxValue(type)).minValueInclusive(True).maxValueInclusive(True).arg("mean", 0.0).arg("std", 1.0).expectedMean(0.0).expectedStd(1.0).meanMinAbsErrorTolerance(0.05).stdMinAbsErrorTolerance(0.05).build())
				testCases.Add(TestCase.builder().opType("randomnormal").dataType(type).shape(100,1000).minValue(minValue(type)).maxValue(maxValue(type)).minValueInclusive(True).maxValueInclusive(True).arg("mean", 2.0).arg("std", 0.5).expectedMean(2.0).expectedStd(0.5).meanRelativeErrorTolerance(0.01).stdRelativeErrorTolerance(0.01).meanMinAbsErrorTolerance(0.001).build())

				testCases.Add(TestCase.builder().opType("randomstandardnormal").dataType(type).shape(New Long(){}).minValue(minValue(type)).maxValue(maxValue(type)).minValueInclusive(True).maxValueInclusive(True).build()) 'Don't check mean/std for 1 element
				testCases.Add(TestCase.builder().opType("randomstandardnormal").dataType(type).shape(1000).minValue(minValue(type)).maxValue(maxValue(type)).minValueInclusive(True).maxValueInclusive(True).expectedMean(0.0).expectedStd(1.0).meanMinAbsErrorTolerance(0.05).stdMinAbsErrorTolerance(0.05).build())
				testCases.Add(TestCase.builder().opType("randomstandardnormal").dataType(type).shape(100,1000).minValue(minValue(type)).maxValue(maxValue(type)).minValueInclusive(True).maxValueInclusive(True).expectedMean(0.0).expectedStd(1.0).meanRelativeErrorTolerance(0.01).stdRelativeErrorTolerance(0.01).meanMinAbsErrorTolerance(0.001).build())
			Next type


			Dim count As Integer = 1
			For Each tc As TestCase In testCases
				log.info("Starting test case: {} of {}", count, testCases.Count)
				log.info("{}", tc)

				Dim op As Object = getOp(tc)
				Dim z As INDArray = Nothing
				Nd4j.Random.setSeed(tc.getRngSeed())
				If TypeOf op Is Op Then
					Dim o As Op = DirectCast(op, Op)
					Nd4j.Executioner.exec(o)
					z = o.z()
				Else
					Dim o As CustomOp = DirectCast(op, CustomOp)
					Nd4j.Executioner.exec(o)
					z = o.getOutputArgument(0)
				End If

				'Check for NaNs, Infs, etc
				Dim countNaN As Integer = Nd4j.Executioner.exec(New MatchConditionTransform(z, Nd4j.create(DataType.BOOL, z.shape()), Conditions.Nan)).castTo(DataType.INT).sumNumber().intValue()
				Dim countInf As Integer = Nd4j.Executioner.exec(New MatchConditionTransform(z, Nd4j.create(DataType.BOOL, z.shape()), Conditions.Infinite)).castTo(DataType.INT).sumNumber().intValue()
				assertEquals(0, countNaN,"NaN - expected 0 values")
				assertEquals(0, countInf,"Infinite - expected 0 values")

				'Check min/max values
				Dim min As Double = z.minNumber().doubleValue()
				If (tc.isMinValueInclusive() AndAlso min < tc.getMinValue()) OrElse (Not tc.isMinValueInclusive() AndAlso min <= tc.getMinValue()) Then
					fail("Minimum value (" & min & ") is less than allowed minimum value (" & tc.getMinValue() & ", inclusive=" & tc.isMinValueInclusive() & "): test case: " & tc)
				End If

				Dim max As Double = z.maxNumber().doubleValue()
				If (tc.isMaxValueInclusive() AndAlso max > tc.getMaxValue()) OrElse (Not tc.isMaxValueInclusive() AndAlso max >= tc.getMaxValue()) Then
					fail("Maximum value (" & max & ") is greater than allowed maximum value (" & tc.getMaxValue() & ", inclusive=" & tc.isMaxValueInclusive() & "): test case: " & tc)
				End If

				'Check RNG seed repeatability
				Dim op2 As Object = getOp(tc)
				Nd4j.Random.setSeed(tc.getRngSeed())
				Dim z2 As INDArray
				If TypeOf op2 Is Op Then
					Dim o As Op = DirectCast(op2, Op)
					Nd4j.Executioner.exec(o)
					z2 = o.z()
				Else
					Dim o As CustomOp = DirectCast(op2, CustomOp)
					Nd4j.Executioner.exec(o)
					z2 = o.getOutputArgument(0)
				End If
				assertEquals(z, z2)

				'Check mean, stdev
				If tc.getExpectedMean() IsNot Nothing Then
					Dim mean As Double = z.meanNumber().doubleValue()
					Dim re As Double = relError(tc.getExpectedMean(), mean)
					Dim ae As Double = Math.Abs(tc.getExpectedMean() - mean)
					If re > tc.getMeanRelativeErrorTolerance() AndAlso (tc.getMeanMinAbsErrorTolerance() Is Nothing OrElse ae > tc.getMeanMinAbsErrorTolerance()) Then
						fail("Relative error for mean (" & re & ") exceeds maximum (" & tc.getMeanRelativeErrorTolerance() & ") - expected mean = " & tc.getExpectedMean() & " vs. observed mean = " & mean & " - test: " & tc)
					End If
				End If
				If tc.getExpectedStd() IsNot Nothing Then
					Dim std As Double = z.std(True).getDouble(0)
					Dim re As Double = relError(tc.getExpectedStd(), std)
					Dim ae As Double = Math.Abs(tc.getExpectedStd() - std)
					If re > tc.getStdRelativeErrorTolerance() AndAlso (tc.getStdMinAbsErrorTolerance() Is Nothing OrElse ae > tc.getStdMinAbsErrorTolerance()) Then
	'                    
	'                    //Histogram for debugging
	'                    INDArray range = Nd4j.create(new double[]{z.minNumber().doubleValue(), z.maxNumber().doubleValue()}).castTo(tc.getDataType());
	'                    INDArray n = Nd4j.scalar(DataType.INT,100);
	'                    INDArray out = Nd4j.create(DataType.INT, 100);
	'                    DynamicCustomOp histogram = DynamicCustomOp.builder("histogram_fixed_width")
	'                            .addInputs(z, range, n)
	'                            .addOutputs(out)
	'                            .build();
	'                    Nd4j.getExecutioner().exec(histogram);
	'                    System.out.println(range);
	'                    System.out.println(out.toString().replaceAll("\\s", ""));
	'                    
						fail("Relative error for stdev (" & re & ") exceeds maximum (" & tc.getStdRelativeErrorTolerance() & ") - expected stdev = " & tc.getExpectedStd() & " vs. observed stdev = " & std & " - test: " & tc)
					End If
				End If

				count += 1
			Next tc


		End Sub

		Private Shared Function minValue(ByVal dataType As DataType) As Double
			Select Case dataType.innerEnumValue
				Case DataType.InnerEnum.DOUBLE
					Return -Double.MaxValue
				Case DataType.InnerEnum.FLOAT
					Return -Single.MaxValue
				Case DataType.InnerEnum.HALF
					Return -65504.0
				Case Else
					Throw New Exception("Dtype not supported: " & dataType)
			End Select
		End Function

		Private Shared Function maxValue(ByVal dataType As DataType) As Double
			Select Case dataType.innerEnumValue
				Case DataType.InnerEnum.DOUBLE
					Return Double.MaxValue
				Case DataType.InnerEnum.FLOAT
					Return Single.MaxValue
				Case DataType.InnerEnum.HALF
					Return 65504.0
				Case Else
					Throw New Exception("Dtype not supported: " & dataType)
			End Select
		End Function


		Private Shared Function getOp(ByVal tc As TestCase) As Object

			Select Case tc.getOpType()
				'Legacy (non-custom) RNG ops
				Case "bernoulli"
					Return New BernoulliDistribution(tc.arr(), CDbl(tc.prop("prob")))
				Case "uniform"
					Return New UniformDistribution(tc.arr(), tc.prop("min"), tc.prop("max"))
				Case "gaussian"
					Return New GaussianDistribution(tc.arr(), CDbl(tc.prop("mean")), tc.prop("std"))
				Case "binomial"
					Return New BinomialDistribution(tc.arr(), tc.prop("n"), CDbl(tc.prop("p")))
				Case "truncated_normal"
					Return New TruncatedNormalDistribution(tc.arr(), CDbl(tc.prop("mean")), tc.prop("std"))
				Case "dropout"
					Dim z As INDArray = tc.arr()
					z.assign(1.0)
					Return New DropOut(z, tc.prop("p"))
				Case "dropout_inverted"
					Dim z2 As INDArray = tc.arr()
					z2.assign(1.0)
					Return New DropOutInverted(z2, tc.prop("p"))
				Case "linspace"
					Return New Linspace(tc.arr(), tc.prop("from"), tc.prop("to"))
				Case "lognormal"
					Return New LogNormalDistribution(tc.arr(), CDbl(tc.prop("mu")), tc.prop("s"))
				Case "choice"
					Dim source As INDArray = Nd4j.linspace(0, 10, 11, tc.getDataType())
					Dim probs As INDArray = Nd4j.ones(11).divi(11)
					Return New Choice(source, probs, tc.arr())
				Case "probabilisticmerge"
					Dim x As INDArray = Nd4j.zeros(tc.getDataType(), tc.getShape())
					Dim y As INDArray = Nd4j.ones(tc.getDataType(), tc.getShape())
					Return New ProbablisticMerge(x, y, tc.arr(), tc.prop("prob"))
				Case "range"
					Dim rMin As Double = tc.prop("min")
					Dim rMax As Double = tc.prop("max")
					Dim [step] As Double = (rMax - rMin) / CDbl(ArrayUtil.prodLong(tc.shape))
					Dim op As DynamicCustomOp = DynamicCustomOp.builder("range").addFloatingPointArguments(rMin, rMax, [step]).addOutputs(tc.arr()).build()
					Return op
				Case "alphaDropout"
					Dim alpha As Double = alphaDropoutA(tc.prop("p"))
					Dim beta As Double = alphaDropoutB(tc.prop("p"))
					Return New AlphaDropOut(Nd4j.ones(tc.getDataType(), tc.shape), tc.arr(), tc.prop("p"), alpha, ALPHA_PRIME, beta)
				Case "distributionuniform"
					Dim shape As INDArray = If(tc.getShape().length = 0, Nd4j.empty(DataType.LONG), Nd4j.create(ArrayUtil.toDouble(tc.shape)).castTo(DataType.LONG))
					Return New DistributionUniform(shape, tc.arr(), tc.prop("min"), tc.prop("max"))
				Case "randombernoulli"
					Dim shape2 As INDArray = If(tc.getShape().length = 0, Nd4j.empty(DataType.LONG), Nd4j.create(ArrayUtil.toDouble(tc.shape)).castTo(DataType.LONG))
					Return New RandomBernoulli(shape2, tc.arr(), tc.prop("prob"))
				Case "randomexponential"
					Dim shape3 As INDArray = If(tc.getShape().length = 0, Nd4j.empty(DataType.LONG), Nd4j.create(ArrayUtil.toDouble(tc.shape)).castTo(DataType.LONG))
					Return New RandomExponential(shape3, tc.arr(), tc.prop("lambda"))
				Case "randomnormal"
					Dim shape4 As INDArray = If(tc.getShape().length = 0, Nd4j.empty(DataType.LONG), Nd4j.create(ArrayUtil.toDouble(tc.shape)).castTo(DataType.LONG))
					Return DynamicCustomOp.builder("randomnormal").addFloatingPointArguments(tc.prop("mean"), tc.prop("std")).addInputs(shape4).addOutputs(tc.arr()).build()
				Case "randomstandardnormal"
					Dim shape5 As INDArray = If(tc.getShape().length = 0, Nd4j.empty(DataType.LONG), Nd4j.create(ArrayUtil.toDouble(tc.shape)).castTo(DataType.LONG))
					Return New RandomStandardNormal(shape5, Nd4j.create(tc.getDataType(), tc.getShape()))
				Case Else
					Throw New Exception("Not yet implemented: " & tc.getOpType())
			End Select
		End Function

		Private Shared Function relError(ByVal x As Double, ByVal y As Double) As Double
			Return Math.Abs(x-y) / (Math.Abs(x) + Math.Abs(y))
		End Function

		Public Const DEFAULT_ALPHA As Double = 1.6732632423543772
		Public Const DEFAULT_LAMBDA As Double = 1.0507009873554804
		Public Shared ReadOnly ALPHA_PRIME As Double = -DEFAULT_LAMBDA * DEFAULT_ALPHA
		Public Shared Function alphaDropoutA(ByVal p As Double) As Double
			Return 1.0 / Math.Sqrt(p + ALPHA_PRIME*ALPHA_PRIME * p * (1-p))
		End Function

		Public Shared Function alphaDropoutB(ByVal p As Double) As Double
			Dim alphaPrime As Double = -DEFAULT_LAMBDA * DEFAULT_ALPHA
			Return -alphaDropoutA(p) * (1-p)*alphaPrime
		End Function
	End Class

End Namespace