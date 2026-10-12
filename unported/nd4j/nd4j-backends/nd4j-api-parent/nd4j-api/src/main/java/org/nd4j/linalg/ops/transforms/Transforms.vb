Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports NonNull = lombok.NonNull
Imports val = lombok.val
Imports Preconditions = org.nd4j.common.base.Preconditions
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports CustomOp = org.nd4j.linalg.api.ops.CustomOp
Imports ScalarOp = org.nd4j.linalg.api.ops.ScalarOp
Imports TransformOp = org.nd4j.linalg.api.ops.TransformOp
Imports org.nd4j.linalg.api.ops.impl.reduce3
Imports org.nd4j.linalg.api.ops.impl.scalar
Imports ScalarNot = org.nd4j.linalg.api.ops.impl.scalar.comparison.ScalarNot
Imports Cross = org.nd4j.linalg.api.ops.impl.shape.Cross
Imports BooleanNot = org.nd4j.linalg.api.ops.impl.transforms.bool.BooleanNot
Imports IsMax = org.nd4j.linalg.api.ops.impl.transforms.any.IsMax
Imports ATan2 = org.nd4j.linalg.api.ops.impl.transforms.custom.ATan2
Imports GreaterThanOrEqual = org.nd4j.linalg.api.ops.impl.transforms.custom.GreaterThanOrEqual
Imports LessThanOrEqual = org.nd4j.linalg.api.ops.impl.transforms.custom.LessThanOrEqual
Imports Reverse = org.nd4j.linalg.api.ops.impl.transforms.custom.Reverse
Imports SoftMax = org.nd4j.linalg.api.ops.impl.transforms.custom.SoftMax
Imports org.nd4j.linalg.api.ops.impl.transforms.floating
Imports org.nd4j.linalg.api.ops.impl.transforms.comparison
Imports EluBp = org.nd4j.linalg.api.ops.impl.transforms.gradient.EluBp
Imports HardTanhDerivative = org.nd4j.linalg.api.ops.impl.transforms.gradient.HardTanhDerivative
Imports LeakyReLUDerivative = org.nd4j.linalg.api.ops.impl.transforms.gradient.LeakyReLUDerivative
Imports SoftSignDerivative = org.nd4j.linalg.api.ops.impl.transforms.gradient.SoftSignDerivative
Imports PowPairwise = org.nd4j.linalg.api.ops.impl.transforms.pairwise.arithmetic.PowPairwise
Imports [And] = org.nd4j.linalg.api.ops.impl.transforms.pairwise.bool.And
Imports [Or] = org.nd4j.linalg.api.ops.impl.transforms.pairwise.bool.Or
Imports [Xor] = org.nd4j.linalg.api.ops.impl.transforms.pairwise.bool.Xor
Imports org.nd4j.linalg.api.ops.impl.transforms.same
Imports org.nd4j.linalg.api.ops.impl.transforms.strict
Imports LongShapeDescriptor = org.nd4j.linalg.api.shape.LongShapeDescriptor
Imports Shape = org.nd4j.linalg.api.shape.Shape
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports InvertMatrix = org.nd4j.linalg.inverse.InvertMatrix

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

Namespace org.nd4j.linalg.ops.transforms


	Public Class Transforms


		Private Sub New()
		End Sub

		''' <summary>
		''' Cosine similarity
		''' </summary>
		''' <param name="d1"> the first vector </param>
		''' <param name="d2"> the second vector </param>
		''' <returns> the cosine similarities between the 2 arrays </returns>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public static double cosineSim(@NonNull INDArray d1, @NonNull INDArray d2)
		Public Shared Function cosineSim(ByVal d1 As INDArray, ByVal d2 As INDArray) As Double
			Return Nd4j.Executioner.exec(New CosineSimilarity(d1, d2)).getDouble(0)
		End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public static double cosineDistance(@NonNull INDArray d1, @NonNull INDArray d2)
		Public Shared Function cosineDistance(ByVal d1 As INDArray, ByVal d2 As INDArray) As Double
			Return Nd4j.Executioner.exec(New CosineDistance(d1, d2)).getDouble(0)
		End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public static double hammingDistance(@NonNull INDArray d1, @NonNull INDArray d2)
		Public Shared Function hammingDistance(ByVal d1 As INDArray, ByVal d2 As INDArray) As Double
			Return Nd4j.Executioner.exec(New HammingDistance(d1, d2)).getDouble(0)
		End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public static double jaccardDistance(@NonNull INDArray d1, @NonNull INDArray d2)
		Public Shared Function jaccardDistance(ByVal d1 As INDArray, ByVal d2 As INDArray) As Double
			Return Nd4j.Executioner.exec(New JaccardDistance(d1, d2)).getDouble(0)
		End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public static org.nd4j.linalg.api.ndarray.INDArray allCosineSimilarities(@NonNull INDArray d1, @NonNull INDArray d2, int... dimensions)
		Public Shared Function allCosineSimilarities(ByVal d1 As INDArray, ByVal d2 As INDArray, ParamArray ByVal dimensions() As Integer) As INDArray
			Return Nd4j.Executioner.exec(New CosineSimilarity(d1, d2, True, dimensions))
		End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public static org.nd4j.linalg.api.ndarray.INDArray allCosineDistances(@NonNull INDArray d1, @NonNull INDArray d2, int... dimensions)
		Public Shared Function allCosineDistances(ByVal d1 As INDArray, ByVal d2 As INDArray, ParamArray ByVal dimensions() As Integer) As INDArray
			Return Nd4j.Executioner.exec(New CosineDistance(d1, d2, True, dimensions))
		End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public static org.nd4j.linalg.api.ndarray.INDArray allEuclideanDistances(@NonNull INDArray d1, @NonNull INDArray d2, int... dimensions)
		Public Shared Function allEuclideanDistances(ByVal d1 As INDArray, ByVal d2 As INDArray, ParamArray ByVal dimensions() As Integer) As INDArray
			Return Nd4j.Executioner.exec(New EuclideanDistance(d1, d2, True, dimensions))
		End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public static org.nd4j.linalg.api.ndarray.INDArray allManhattanDistances(@NonNull INDArray d1, @NonNull INDArray d2, int... dimensions)
		Public Shared Function allManhattanDistances(ByVal d1 As INDArray, ByVal d2 As INDArray, ParamArray ByVal dimensions() As Integer) As INDArray
			Return Nd4j.Executioner.exec(New ManhattanDistance(d1, d2, True, dimensions))
		End Function


		Public Shared Function reverse(ByVal x As INDArray, ByVal dup As Boolean) As INDArray
			Return Nd4j.Executioner.exec(New Reverse(x,If(dup, x.ulike(), x)))(0)
		End Function

		''' <summary>
		''' Dot product, new INDArray instance will be returned.<br>
		''' Note that the Nd4J design is different from Numpy. Numpy dot on 2d arrays is matrix multiplication. Nd4J is
		''' full array dot product reduction.
		''' </summary>
		''' <param name="x"> the first vector </param>
		''' <param name="y"> the second vector </param>
		''' <returns> the dot product between the 2 arrays </returns>
		Public Shared Function dot(ByVal x As INDArray, ByVal y As INDArray) As INDArray
			Return Nd4j.Executioner.exec(New Dot(x, y))
		End Function

		Public Shared Function cross(ByVal x As INDArray, ByVal y As INDArray) As INDArray
			Dim c As New Cross(x, y, Nothing)
			Dim shape As IList(Of LongShapeDescriptor) = c.calculateOutputShape()
			Dim [out] As INDArray = Nd4j.create(shape(0))
			c.addOutputArgument([out])
			Nd4j.Executioner.exec(c)
			Return [out]
		End Function

		''' <param name="d1"> </param>
		''' <param name="d2">
		''' @return </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public static double manhattanDistance(@NonNull INDArray d1, @NonNull INDArray d2)
		Public Shared Function manhattanDistance(ByVal d1 As INDArray, ByVal d2 As INDArray) As Double
			Return d1.distance1(d2)
		End Function

		''' <summary>
		''' Atan2 operation, new INDArray instance will be returned
		''' Note the order of x and y parameters is opposite to that of <seealso cref="java.lang.Math.atan2(Double, Double)"/>
		''' </summary>
		''' <param name="x"> the abscissa coordinate </param>
		''' <param name="y"> the ordinate coordinate </param>
		''' <returns> the theta from point (r, theta) when converting (x,y) from to cartesian to polar coordinates </returns>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public static org.nd4j.linalg.api.ndarray.INDArray atan2(@NonNull INDArray x, @NonNull INDArray y)
		Public Shared Function atan2(ByVal x As INDArray, ByVal y As INDArray) As INDArray
			' Switched on purpose, to match OldATan2 (which the javadoc was written for)
			Return Nd4j.Executioner.exec(New ATan2(y, x, x.ulike()))(0)
		End Function

		''' <param name="d1"> </param>
		''' <param name="d2">
		''' @return </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public static double euclideanDistance(@NonNull INDArray d1, @NonNull INDArray d2)
		Public Shared Function euclideanDistance(ByVal d1 As INDArray, ByVal d2 As INDArray) As Double
			Return d1.distance2(d2)
		End Function


		''' <summary>
		''' Normalize data to zero mean and unit variance
		''' substract by the mean and divide by the standard deviation
		''' </summary>
		''' <param name="toNormalize"> the ndarray to normalize </param>
		''' <returns> the normalized ndarray </returns>
		Public Shared Function normalizeZeroMeanAndUnitVariance(ByVal toNormalize As INDArray) As INDArray
			Dim columnMeans As INDArray = toNormalize.mean(0)
			Dim columnStds As INDArray = toNormalize.std(0)

			toNormalize.subiRowVector(columnMeans)
			'padding for non zero
			columnStds.addi(Nd4j.EPS_THRESHOLD)
			toNormalize.diviRowVector(columnStds)
			Return toNormalize
		End Function


		''' <summary>
		''' Scale by 1 / norm2 of the matrix
		''' </summary>
		''' <param name="toScale"> the ndarray to scale </param>
		''' <returns> the scaled ndarray </returns>
		Public Shared Function unitVec(ByVal toScale As INDArray) As INDArray
			Dim length As Double = toScale.norm2Number().doubleValue()

			If length > 0 Then
				If toScale.data().dataType() = CType(Then, DataType.FLOAT)
					Return Nd4j.BlasWrapper.scal(1.0f / CSng(length), toScale)
				Else
					Return Nd4j.BlasWrapper.scal(1.0 / length, toScale)
				End If

			End If
			Return toScale
		End Function


		''' <summary>
		''' Returns the negative of an ndarray
		''' </summary>
		''' <param name="ndArray"> the ndarray to take the negative of </param>
		''' <returns> the negative of the ndarray </returns>
		Public Shared Function neg(ByVal ndArray As INDArray) As INDArray
			Return neg(ndArray, True)
		End Function


		''' <summary>
		''' Binary matrix of whether the number at a given index is greater than
		''' </summary>
		''' <param name="ndArray">
		''' @return </param>
		Public Shared Function floor(ByVal ndArray As INDArray) As INDArray
			Return floor(ndArray, True)

		End Function

		''' <summary>
		''' Binary matrix of whether the number at a given index is greater than
		''' </summary>
		''' <param name="ndArray">
		''' @return </param>
		Public Shared Function ceiling(ByVal ndArray As INDArray) As INDArray
			Return ceiling(ndArray, True)

		End Function

		''' <summary>
		''' Ceiling function
		''' </summary>
		''' <param name="ndArray"> </param>
		''' <param name="copyOnOps">
		''' @return </param>
		Public Shared Function ceiling(ByVal ndArray As INDArray, ByVal copyOnOps As Boolean) As INDArray
			Return exec(If(copyOnOps, New Ceil(ndArray, ndArray.ulike()), New Ceil(ndArray, ndArray)))
		End Function

		''' <summary>
		''' Signum function of this ndarray
		''' </summary>
		''' <param name="toSign">
		''' @return </param>
		Public Shared Function sign(ByVal toSign As INDArray) As INDArray
			Return sign(toSign, True)
		End Function


		''' <param name="ndArray"> </param>
		''' <param name="k">
		''' @return </param>
		Public Shared Function stabilize(ByVal ndArray As INDArray, ByVal k As Double) As INDArray
			Return stabilize(ndArray, k, True)
		End Function

		''' <summary>
		''' Sin function
		''' </summary>
		''' <param name="in">
		''' @return </param>
		Public Shared Function sin(ByVal [in] As INDArray) As INDArray
			Return sin([in], True)
		End Function

		''' <summary>
		''' Sin function
		''' </summary>
		''' <param name="in"> </param>
		''' <param name="copy">
		''' @return </param>
		Public Shared Function sin(ByVal [in] As INDArray, ByVal copy As Boolean) As INDArray
			Return Nd4j.Executioner.exec(New Sin([in], (If(copy, [in].ulike(), [in]))))
		End Function


		''' <summary>
		''' Sin function
		''' </summary>
		''' <param name="in">
		''' @return </param>
		Public Shared Function atanh(ByVal [in] As INDArray) As INDArray
			Return atanh([in], True)
		End Function

		''' <summary>
		''' Sin function
		''' </summary>
		''' <param name="in"> </param>
		''' <param name="copy">
		''' @return </param>
		Public Shared Function atanh(ByVal [in] As INDArray, ByVal copy As Boolean) As INDArray
			Return Nd4j.Executioner.exec(New ATanh([in], (If(copy, [in].ulike(), [in]))))
		End Function

		''' <summary>
		''' Sinh function
		''' </summary>
		''' <param name="in">
		''' @return </param>
		Public Shared Function sinh(ByVal [in] As INDArray) As INDArray
			Return sinh([in], True)
		End Function

		''' <summary>
		''' Sinh function
		''' </summary>
		''' <param name="in"> </param>
		''' <param name="copy">
		''' @return </param>
		Public Shared Function sinh(ByVal [in] As INDArray, ByVal copy As Boolean) As INDArray
			Return Nd4j.Executioner.exec(New Sinh([in], (If(copy, [in].ulike(), [in]))))
		End Function

		''' <param name="in">
		''' @return </param>
		Public Shared Function cos(ByVal [in] As INDArray) As INDArray
			Return cos([in], True)
		End Function

		''' <param name="in"> </param>
		''' <param name="copy">
		''' @return </param>
		Public Shared Function cosh(ByVal [in] As INDArray, ByVal copy As Boolean) As INDArray
			Return Nd4j.Executioner.exec(New Cosh([in], (If(copy, [in].ulike(), [in]))))
		End Function

		''' <param name="in">
		''' @return </param>
		Public Shared Function cosh(ByVal [in] As INDArray) As INDArray
			Return cosh([in], True)
		End Function

		''' <param name="in"> </param>
		''' <param name="copy">
		''' @return </param>
		Public Shared Function cos(ByVal [in] As INDArray, ByVal copy As Boolean) As INDArray
			Return Nd4j.Executioner.exec(New Cos([in], (If(copy, [in].ulike(), [in]))))
		End Function


		Public Shared Function acos(ByVal arr As INDArray) As INDArray
			Return acos(arr, True)
		End Function


		Public Shared Function acos(ByVal [in] As INDArray, ByVal copy As Boolean) As INDArray
			Return Nd4j.Executioner.exec(New ACos([in], (If(copy, [in].ulike(), [in]))))
		End Function


		Public Shared Function asin(ByVal arr As INDArray) As INDArray
			Return asin(arr, True)
		End Function


		Public Shared Function asin(ByVal [in] As INDArray, ByVal copy As Boolean) As INDArray
			Return Nd4j.Executioner.exec(New ASin([in], (If(copy, [in].ulike(), [in]))))
		End Function

		Public Shared Function atan(ByVal arr As INDArray) As INDArray
			Return atan(arr, True)
		End Function


		Public Shared Function atan(ByVal [in] As INDArray, ByVal copy As Boolean) As INDArray
			Return Nd4j.Executioner.exec(New ATan([in], (If(copy, [in].ulike(), [in]))))
		End Function

		Public Shared Function ceil(ByVal arr As INDArray) As INDArray
			Return ceil(arr, True)
		End Function


		Public Shared Function ceil(ByVal [in] As INDArray, ByVal copy As Boolean) As INDArray
			Return Nd4j.Executioner.exec(New Ceil([in], (If(copy, [in].ulike(), [in]))))
		End Function


		Public Shared Function relu(ByVal arr As INDArray) As INDArray
			Return relu(arr, True)
		End Function


		Public Shared Function relu(ByVal [in] As INDArray, ByVal copy As Boolean) As INDArray
			Return Nd4j.Executioner.exec(New RectifiedLinear([in], (If(copy, [in].ulike(), [in]))))
		End Function

		Public Shared Function relu6(ByVal arr As INDArray) As INDArray
			Return relu6(arr, True)
		End Function


		Public Shared Function relu6(ByVal [in] As INDArray, ByVal copy As Boolean) As INDArray
			Return Nd4j.Executioner.exec(New Relu6([in], (If(copy, [in].ulike(), [in]))))
		End Function


		Public Shared Function leakyRelu(ByVal arr As INDArray) As INDArray
			Return leakyRelu(arr, True)
		End Function


		Public Shared Function leakyRelu(ByVal [in] As INDArray, ByVal copy As Boolean) As INDArray
			Return Nd4j.Executioner.exec(New LeakyReLU([in], (If(copy, [in].ulike(), [in]))))
		End Function

		Public Shared Function elu(ByVal arr As INDArray) As INDArray
			Return elu(arr, True)
		End Function


		Public Shared Function elu(ByVal [in] As INDArray, ByVal copy As Boolean) As INDArray
			Return Nd4j.Executioner.exec(New ELU([in], (If(copy, [in].ulike(), [in]))))(0)
		End Function

		Public Shared Function eluDerivative(ByVal arr As INDArray, ByVal grad As INDArray) As INDArray
			Return eluDerivative(arr, grad,True)
		End Function


		Public Shared Function eluDerivative(ByVal [in] As INDArray, ByVal grad As INDArray, ByVal copy As Boolean) As INDArray
			Return Nd4j.Executioner.exec(New EluBp([in], grad, (If(copy, [in].ulike(), [in]))))(0)
		End Function


		Public Shared Function leakyRelu(ByVal arr As INDArray, ByVal cutoff As Double) As INDArray
			Return leakyRelu(arr, cutoff, True)
		End Function


		Public Shared Function leakyRelu(ByVal [in] As INDArray, ByVal cutoff As Double, ByVal copy As Boolean) As INDArray
			Return Nd4j.Executioner.exec(New LeakyReLU([in], (If(copy, [in].ulike(), [in])), cutoff))
		End Function

		Public Shared Function leakyReluDerivative(ByVal arr As INDArray, ByVal cutoff As Double) As INDArray
			Return leakyReluDerivative(arr, cutoff, True)
		End Function


		Public Shared Function leakyReluDerivative(ByVal [in] As INDArray, ByVal cutoff As Double, ByVal copy As Boolean) As INDArray
			Return Nd4j.Executioner.exec(New LeakyReLUDerivative([in], (If(copy, [in].ulike(), [in])), cutoff))
		End Function


		Public Shared Function softPlus(ByVal arr As INDArray) As INDArray
			Return softPlus(arr, True)
		End Function


		Public Shared Function softPlus(ByVal [in] As INDArray, ByVal copy As Boolean) As INDArray
			Return Nd4j.Executioner.exec(New SoftPlus([in], (If(copy, [in].ulike(), [in]))))
		End Function

		Public Shared Function [step](ByVal arr As INDArray) As INDArray
			Return [step](arr, True)
		End Function


		Public Shared Function [step](ByVal [in] As INDArray, ByVal copy As Boolean) As INDArray
			Return Nd4j.Executioner.exec(New [Step]([in], (If(copy, [in].ulike(), [in]))))
		End Function


		Public Shared Function softsign(ByVal arr As INDArray) As INDArray
			Return softsign(arr, True)
		End Function


		Public Shared Function softsign(ByVal [in] As INDArray, ByVal copy As Boolean) As INDArray
			Return Nd4j.Executioner.exec(New SoftSign([in], (If(copy, [in].ulike(), [in]))))
		End Function


		Public Shared Function softsignDerivative(ByVal arr As INDArray) As INDArray
			Return softsignDerivative(arr, True)
		End Function


		Public Shared Function softsignDerivative(ByVal [in] As INDArray, ByVal copy As Boolean) As INDArray
			Return Nd4j.Executioner.exec(New SoftSignDerivative([in], (If(copy, [in].ulike(), [in]))))
		End Function


		Public Shared Function softmax(ByVal arr As INDArray) As INDArray
			Return softmax(arr, True)
		End Function


		''' <param name="in"> </param>
		''' <param name="copy">
		''' @return </param>
		Public Shared Function softmax(ByVal [in] As INDArray, ByVal copy As Boolean) As INDArray
			Return Nd4j.Executioner.exec(DirectCast(New SoftMax([in], (If(copy, [in].ulike(), [in])), -1), CustomOp))(0)
		End Function

		''' <summary>
		''' out = in * (1-in)
		''' </summary>
		''' <param name="in">   Input array </param>
		''' <param name="copy"> If true: copy. False: apply in-place
		''' @return </param>
		Public Shared Function timesOneMinus(ByVal [in] As INDArray, ByVal copy As Boolean) As INDArray
			Return Nd4j.Executioner.exec(New TimesOneMinus([in], (If(copy, [in].ulike(), [in]))))
		End Function

		''' <summary>
		''' Abs function
		''' </summary>
		''' <param name="ndArray">
		''' @return </param>
		Public Shared Function abs(ByVal ndArray As INDArray) As INDArray
			Return abs(ndArray, True)
		End Function


		''' <summary>
		''' Run the exp operation
		''' </summary>
		''' <param name="ndArray">
		''' @return </param>
		Public Shared Function exp(ByVal ndArray As INDArray) As INDArray
			Return exp(ndArray, True)
		End Function


		Public Shared Function hardTanh(ByVal ndArray As INDArray) As INDArray
			Return hardTanh(ndArray, True)

		End Function

		''' <summary>
		''' Hard tanh
		''' </summary>
		''' <param name="ndArray"> the input </param>
		''' <param name="dup">     whether to duplicate the ndarray and return it as the result </param>
		''' <returns> the output </returns>
		Public Shared Function hardTanh(ByVal ndArray As INDArray, ByVal dup As Boolean) As INDArray
			Return exec(If(dup, New HardTanh(ndArray, ndArray.ulike()), New HardTanh(ndArray)))
		End Function

		Public Shared Function hardSigmoid(ByVal arr As INDArray, ByVal dup As Boolean) As INDArray
			Return Nd4j.Executioner.exec(New HardSigmoid(arr, (If(dup, arr.ulike(), arr))))
		End Function


		Public Shared Function hardTanhDerivative(ByVal ndArray As INDArray) As INDArray
			Return hardTanhDerivative(ndArray, True)

		End Function

		''' <summary>
		''' Hard tanh
		''' </summary>
		''' <param name="ndArray"> the input </param>
		''' <param name="dup">     whether to duplicate the ndarray and return it as the result </param>
		''' <returns> the output </returns>
		Public Shared Function hardTanhDerivative(ByVal ndArray As INDArray, ByVal dup As Boolean) As INDArray
			Return exec(If(dup, New HardTanhDerivative(ndArray, ndArray.ulike()), New HardTanhDerivative(ndArray)))
		End Function


		''' <param name="ndArray">
		''' @return </param>
		Public Shared Function identity(ByVal ndArray As INDArray) As INDArray
			Return identity(ndArray, True)
		End Function


		''' <summary>
		''' Pow function
		''' </summary>
		''' <param name="ndArray"> the ndarray to raise hte power of </param>
		''' <param name="power">   the power to raise by </param>
		''' <returns> the ndarray raised to this power </returns>
		Public Shared Function pow(ByVal ndArray As INDArray, ByVal power As Number) As INDArray
			Return pow(ndArray, power, True)

		End Function


		''' <summary>
		''' Element-wise power function - x^y, performed element-wise.
		''' Not performed in-place: the input arrays are not modified.
		''' </summary>
		''' <param name="ndArray"> the ndarray to raise to the power of </param>
		''' <param name="power">   the power to raise by </param>
		''' <returns> a copy of the ndarray raised to the specified power (element-wise) </returns>
		Public Shared Function pow(ByVal ndArray As INDArray, ByVal power As INDArray) As INDArray
			Return pow(ndArray, power, True)
		End Function

		''' <summary>
		''' Element-wise power function - x^y, performed element-wise
		''' </summary>
		''' <param name="ndArray"> the ndarray to raise to the power of </param>
		''' <param name="power">   the power to raise by </param>
		''' <param name="dup">     if true: </param>
		''' <returns> the ndarray raised to this power </returns>
		Public Shared Function pow(ByVal ndArray As INDArray, ByVal power As INDArray, ByVal dup As Boolean) As INDArray
			Dim result As INDArray = (If(dup, ndArray.ulike(), ndArray))
			Return exec(New PowPairwise(ndArray, power, result))
		End Function

		''' <summary>
		''' Rounding function
		''' </summary>
		''' <param name="ndArray">
		''' @return </param>
		Public Shared Function round(ByVal ndArray As INDArray) As INDArray
			Return round(ndArray, True)
		End Function

		''' <summary>
		''' Sigmoid function
		''' </summary>
		''' <param name="ndArray">
		''' @return </param>
		Public Shared Function sigmoid(ByVal ndArray As INDArray) As INDArray
			Return sigmoid(ndArray, True)
		End Function

		''' <summary>
		''' Sigmoid function
		''' </summary>
		''' <param name="ndArray"> </param>
		''' <param name="dup">
		''' @return </param>
		Public Shared Function sigmoid(ByVal ndArray As INDArray, ByVal dup As Boolean) As INDArray
			Return exec(If(dup, New Sigmoid(ndArray, ndArray.ulike()), New Sigmoid(ndArray)))
		End Function

		''' <summary>
		''' Sigmoid function
		''' </summary>
		''' <param name="ndArray">
		''' @return </param>
		Public Shared Function sigmoidDerivative(ByVal ndArray As INDArray) As INDArray
			Return sigmoidDerivative(ndArray, True)
		End Function

		''' <summary>
		''' Sigmoid function
		''' </summary>
		''' <param name="ndArray"> </param>
		''' <param name="dup">
		''' @return </param>
		Public Shared Function sigmoidDerivative(ByVal ndArray As INDArray, ByVal dup As Boolean) As INDArray
			Return exec(If(dup, New SigmoidDerivative(ndArray, ndArray.ulike()), New SigmoidDerivative(ndArray)))
		End Function


		''' <summary>
		''' Sqrt function
		''' </summary>
		''' <param name="ndArray">
		''' @return </param>
		Public Shared Function sqrt(ByVal ndArray As INDArray) As INDArray
			Return sqrt(ndArray, True)
		End Function


		''' <summary>
		''' Element-wise tan function. Copies the array
		''' </summary>
		''' <param name="ndArray"> Input array </param>
		Public Shared Function tan(ByVal ndArray As INDArray) As INDArray
			Return tan(ndArray, True)
		End Function

		''' <summary>
		''' Element-wise tan function. Copies the array
		''' </summary>
		''' <param name="ndArray"> Input array </param>
		Public Shared Function tan(ByVal ndArray As INDArray, ByVal dup As Boolean) As INDArray
			Return exec(If(dup, New Tan(ndArray, ndArray.ulike()), New Tan(ndArray)))
		End Function

		''' <summary>
		''' Tanh function
		''' </summary>
		''' <param name="ndArray">
		''' @return </param>
		Public Shared Function tanh(ByVal ndArray As INDArray) As INDArray
			Return tanh(ndArray, True)
		End Function

		''' <summary>
		''' Log on arbitrary base
		''' </summary>
		''' <param name="ndArray"> </param>
		''' <param name="base">
		''' @return </param>
		Public Shared Function log(ByVal ndArray As INDArray, ByVal base As Double) As INDArray
			Return log(ndArray, base, True)
		End Function

		''' <summary>
		''' Log on arbitrary base
		''' </summary>
		''' <param name="ndArray"> </param>
		''' <param name="base">
		''' @return </param>
		Public Shared Function log(ByVal ndArray As INDArray, ByVal base As Double, ByVal duplicate As Boolean) As INDArray
			Return Nd4j.Executioner.exec(New LogX(ndArray,If(duplicate, ndArray.ulike(), ndArray), base))
		End Function

		Public Shared Function log(ByVal ndArray As INDArray) As INDArray
			Return log(ndArray, True)
		End Function

		Public Shared Function eps(ByVal ndArray As INDArray) As INDArray
			Return exec(New Eps(ndArray))
		End Function

		''' <summary>
		''' 1 if greater than or equal to 0 otherwise (at each element)
		''' </summary>
		''' <param name="first"> </param>
		''' <param name="ndArray">
		''' @return </param>
		Public Shared Function greaterThanOrEqual(ByVal first As INDArray, ByVal ndArray As INDArray) As INDArray
			Return greaterThanOrEqual(first, ndArray, True)
		End Function

		''' <summary>
		''' 1 if less than or equal to 0 otherwise (at each element)
		''' </summary>
		''' <param name="first"> </param>
		''' <param name="ndArray">
		''' @return </param>
		Public Shared Function lessThanOrEqual(ByVal first As INDArray, ByVal ndArray As INDArray) As INDArray
			Return lessThanOrEqual(first, ndArray, True)
		End Function


		''' <summary>
		''' Eps function
		''' </summary>
		''' <param name="ndArray">
		''' @return </param>
		Public Shared Function lessThanOrEqual(ByVal first As INDArray, ByVal ndArray As INDArray, ByVal dup As Boolean) As INDArray
			Dim op As val = If(dup, New LessThanOrEqual(first, ndArray, Nd4j.createUninitialized(DataType.BOOL, first.shape(), first.ordering())), New LessThanOrEqual(first, ndArray))
			Return Nd4j.Executioner.exec(op)(0)
		End Function


		''' <summary>
		''' Eps function
		''' </summary>
		''' <param name="ndArray">
		''' @return </param>
		Public Shared Function greaterThanOrEqual(ByVal first As INDArray, ByVal ndArray As INDArray, ByVal dup As Boolean) As INDArray
			Dim op As val = If(dup, New GreaterThanOrEqual(first, ndArray, Nd4j.createUninitialized(DataType.BOOL, first.shape(), first.ordering())), New GreaterThanOrEqual(first, ndArray))
			Return Nd4j.Executioner.exec(op)(0)

		End Function


		''' <summary>
		''' Floor function
		''' </summary>
		''' <param name="ndArray">
		''' @return </param>
		Public Shared Function floor(ByVal ndArray As INDArray, ByVal dup As Boolean) As INDArray
			Return exec(If(dup, New Floor(ndArray, ndArray.ulike()), New Floor(ndArray)))

		End Function


		''' <summary>
		''' Signum function of this ndarray
		''' </summary>
		''' <param name="toSign">
		''' @return </param>
		Public Shared Function sign(ByVal toSign As INDArray, ByVal dup As Boolean) As INDArray
			Return exec(If(dup, New Sign(toSign, toSign.ulike()), New Sign(toSign)))
		End Function

		''' <summary>
		''' Maximum function with a scalar
		''' </summary>
		''' <param name="ndArray"> tbe ndarray </param>
		''' <param name="k"> </param>
		''' <param name="dup">
		''' @return </param>
		Public Shared Function max(ByVal ndArray As INDArray, ByVal k As Double, ByVal dup As Boolean) As INDArray
			Return exec(If(dup, New ScalarMax(ndArray, Nothing, ndArray.ulike(), k), New ScalarMax(ndArray, k)))
		End Function

		''' <summary>
		''' Maximum function with a scalar
		''' </summary>
		''' <param name="ndArray"> tbe ndarray </param>
		''' <param name="k">
		''' @return </param>
		Public Shared Function max(ByVal ndArray As INDArray, ByVal k As Double) As INDArray
			Return max(ndArray, k, True)
		End Function

		''' <summary>
		''' Element wise maximum function between 2 INDArrays
		''' </summary>
		''' <param name="first"> </param>
		''' <param name="second"> </param>
		''' <param name="dup">
		''' @return </param>
		Public Shared Function max(ByVal first As INDArray, ByVal second As INDArray, ByVal dup As Boolean) As INDArray
			Dim outShape() As Long = broadcastResultShape(first, second) 'Also validates
			Preconditions.checkState(dup OrElse outShape.SequenceEqual(first.shape()), "Cannot do inplace max operation when first input is not equal to result shape (%ndShape vs. result %s)", first, outShape)
			Dim [out] As INDArray = If(dup, Nd4j.create(first.dataType(), outShape), first)
			Return Nd4j.exec(New org.nd4j.linalg.api.ops.impl.transforms.custom.Max(first, second, [out]))(0)
		End Function

		''' <summary>
		''' Element wise maximum function between 2 INDArrays
		''' </summary>
		''' <param name="first"> </param>
		''' <param name="second">
		''' @return </param>
		Public Shared Function max(ByVal first As INDArray, ByVal second As INDArray) As INDArray
			Return max(first, second, True)
		End Function

		''' <summary>
		''' Minimum function with a scalar
		''' </summary>
		''' <param name="ndArray"> tbe ndarray </param>
		''' <param name="k"> </param>
		''' <param name="dup">
		''' @return </param>
		Public Shared Function min(ByVal ndArray As INDArray, ByVal k As Double, ByVal dup As Boolean) As INDArray
			Return exec(If(dup, New ScalarMin(ndArray, Nothing, ndArray.ulike(), k), New ScalarMin(ndArray, k)))
		End Function

		''' <summary>
		''' Maximum function with a scalar
		''' </summary>
		''' <param name="ndArray"> tbe ndarray </param>
		''' <param name="k">
		''' @return </param>
		Public Shared Function min(ByVal ndArray As INDArray, ByVal k As Double) As INDArray
			Return min(ndArray, k, True)
		End Function

		''' <summary>
		''' Element wise minimum function between 2 INDArrays
		''' </summary>
		''' <param name="first"> </param>
		''' <param name="second"> </param>
		''' <param name="dup">
		''' @return </param>
		Public Shared Function min(ByVal first As INDArray, ByVal second As INDArray, ByVal dup As Boolean) As INDArray
			Dim outShape() As Long = broadcastResultShape(first, second) 'Also validates
			Preconditions.checkState(dup OrElse outShape.SequenceEqual(first.shape()), "Cannot do inplace min operation when first input is not equal to result shape (%ndShape vs. result %s)", first, outShape)
			Dim [out] As INDArray = If(dup, Nd4j.create(first.dataType(), outShape), first)
			Return Nd4j.exec(New org.nd4j.linalg.api.ops.impl.transforms.custom.Min(first, second, [out]))(0)
		End Function

		''' <summary>
		''' Element wise minimum function between 2 INDArrays
		''' </summary>
		''' <param name="first"> </param>
		''' <param name="second">
		''' @return </param>
		Public Shared Function min(ByVal first As INDArray, ByVal second As INDArray) As INDArray
			Return min(first, second, True)
		End Function


		''' <summary>
		''' Stabilize to be within a range of k
		''' </summary>
		''' <param name="ndArray"> tbe ndarray </param>
		''' <param name="k"> </param>
		''' <param name="dup">
		''' @return </param>
		Public Shared Function stabilize(ByVal ndArray As INDArray, ByVal k As Double, ByVal dup As Boolean) As INDArray
			Return exec(If(dup, New Stabilize(ndArray, ndArray.ulike(), k), New Stabilize(ndArray, k)))
		End Function


		''' <summary>
		''' Abs function
		''' </summary>
		''' <param name="ndArray"> </param>
		''' <param name="dup">
		''' @return </param>
		Public Shared Function abs(ByVal ndArray As INDArray, ByVal dup As Boolean) As INDArray
			Return exec(If(dup, New Abs(ndArray, ndArray.ulike()), New Abs(ndArray)))

		End Function

		''' <summary>
		''' Exp function
		''' </summary>
		''' <param name="ndArray"> </param>
		''' <param name="dup">
		''' @return </param>
		Public Shared Function exp(ByVal ndArray As INDArray, ByVal dup As Boolean) As INDArray
			Return exec(If(dup, New Exp(ndArray, ndArray.ulike()), New Exp(ndArray)))
		End Function


		''' <summary>
		''' Elementwise exponential - 1 function
		''' </summary>
		''' <param name="ndArray"> </param>
		''' <param name="dup">
		''' @return </param>
		Public Shared Function expm1(ByVal ndArray As INDArray, ByVal dup As Boolean) As INDArray
			Return exec(If(dup, New Expm1(ndArray, ndArray.ulike()), New Expm1(ndArray)))
		End Function


		''' <summary>
		''' Identity function
		''' </summary>
		''' <param name="ndArray"> </param>
		''' <param name="dup">
		''' @return </param>
		Public Shared Function identity(ByVal ndArray As INDArray, ByVal dup As Boolean) As INDArray
			Return Nd4j.Executioner.exec(If(dup, New Identity(ndArray, ndArray.ulike()), New Identity(ndArray, ndArray)))(0)
		End Function

		Public Shared Function isMax(ByVal input As INDArray, ByVal dataType As DataType) As INDArray
			Return isMax(input, Nd4j.createUninitialized(dataType, input.shape(), input.ordering()))
		End Function


		Public Shared Function isMax(ByVal input As INDArray) As INDArray
			Return isMax(input, input)
		End Function

		Public Shared Function isMax(ByVal input As INDArray, ByVal output As INDArray) As INDArray
			Nd4j.Executioner.exec(New IsMax(input, output))
			Return output
		End Function


		''' <summary>
		''' Pow function
		''' </summary>
		''' <param name="ndArray"> </param>
		''' <param name="power"> </param>
		''' <param name="dup">
		''' @return </param>
		Public Shared Function pow(ByVal ndArray As INDArray, ByVal power As Number, ByVal dup As Boolean) As INDArray
			Return exec(If(dup, New Pow(ndArray, ndArray.ulike(), power.doubleValue()), New Pow(ndArray, power.doubleValue())))
		End Function

		''' <summary>
		''' Rounding function
		''' </summary>
		''' <param name="ndArray"> the ndarray </param>
		''' <param name="dup">
		''' @return </param>
		Public Shared Function round(ByVal ndArray As INDArray, ByVal dup As Boolean) As INDArray
			Return exec(If(dup, New Round(ndArray, ndArray.ulike()), New Round(ndArray)))
		End Function


		''' <summary>
		''' Sqrt function
		''' </summary>
		''' <param name="ndArray"> </param>
		''' <param name="dup">
		''' @return </param>
		Public Shared Function sqrt(ByVal ndArray As INDArray, ByVal dup As Boolean) As INDArray
			Return exec(If(dup, New Sqrt(ndArray, ndArray.ulike()), New Sqrt(ndArray, ndArray)))
		End Function

		''' <summary>
		''' Tanh function
		''' </summary>
		''' <param name="ndArray"> </param>
		''' <param name="dup">
		''' @return </param>
		Public Shared Function tanh(ByVal ndArray As INDArray, ByVal dup As Boolean) As INDArray
			Return exec(If(dup, New Tanh(ndArray, ndArray.ulike()), New Tanh(ndArray)))
		End Function

		''' <summary>
		''' Log function
		''' </summary>
		''' <param name="ndArray"> </param>
		''' <param name="dup">
		''' @return </param>
		Public Shared Function log(ByVal ndArray As INDArray, ByVal dup As Boolean) As INDArray
			Return exec(If(dup, New Log(ndArray, ndArray.ulike()), New Log(ndArray)))
		End Function


		''' <summary>
		''' Log of x + 1 function
		''' </summary>
		''' <param name="ndArray"> </param>
		''' <param name="dup">
		''' @return </param>
		Public Shared Function log1p(ByVal ndArray As INDArray, ByVal dup As Boolean) As INDArray
			Return exec(If(dup, New Log1p(ndArray, ndArray.ulike()), New Log1p(ndArray)))
		End Function

		''' <summary>
		''' Negative
		''' </summary>
		''' <param name="ndArray"> </param>
		''' <param name="dup">
		''' @return </param>
		Public Shared Function neg(ByVal ndArray As INDArray, ByVal dup As Boolean) As INDArray
			Return exec(If(dup, New Negative(ndArray, ndArray.ulike()), New Negative(ndArray)))
		End Function

		Public Shared Function [and](ByVal x As INDArray, ByVal y As INDArray) As INDArray
			Dim z As INDArray = Nd4j.createUninitialized(DataType.BOOL, x.shape(), x.ordering())
			Nd4j.Executioner.exec(New [And](x, y, z, 0.0))
			Return z
		End Function

		Public Shared Function [or](ByVal x As INDArray, ByVal y As INDArray) As INDArray
			Dim z As INDArray = Nd4j.createUninitialized(DataType.BOOL, x.shape(), x.ordering())
			Nd4j.Executioner.exec(New [Or](x, y, z, 0.0))
			Return z
		End Function

		Public Shared Function [xor](ByVal x As INDArray, ByVal y As INDArray) As INDArray
			Dim z As INDArray = Nd4j.createUninitialized(DataType.BOOL, x.shape(), x.ordering())
			Nd4j.Executioner.exec(New [Xor](x, y, z, 0.0))
			Return z
		End Function

		Public Shared Function [not](ByVal x As INDArray) As INDArray
			Dim z As val = Nd4j.createUninitialized(DataType.BOOL, x.shape(), x.ordering())
			If x.B Then
				Nd4j.Executioner.exec(New BooleanNot(x, z))
			Else
				Nd4j.Executioner.exec(New ScalarNot(x, z, 0.0f))
			End If
			Return z
		End Function


		''' <summary>
		''' Apply the given elementwise op
		''' </summary>
		''' <param name="op"> the factory to create the op </param>
		''' <returns> the new ndarray </returns>
		Private Shared Function exec(ByVal op As ScalarOp) As INDArray
			Return Nd4j.Executioner.exec(op)
		End Function

		''' <summary>
		''' Apply the given elementwise op
		''' </summary>
		''' <param name="op"> the factory to create the op </param>
		''' <returns> the new ndarray </returns>
		Private Shared Function exec(ByVal op As TransformOp) As INDArray
			Return Nd4j.Executioner.exec(op)
		End Function

		''' <summary>
		''' Raises a square matrix to a power <i>n</i>, which can be positive, negative, or zero.
		''' The behavior is similar to the numpy matrix_power() function.  The algorithm uses
		''' repeated squarings to minimize the number of mmul() operations needed
		''' <para>If <i>n</i> is zero, the identity matrix is returned.</para>
		''' <para>If <i>n</i> is negative, the matrix is inverted and raised to the abs(n) power.</para>
		''' </summary>
		''' <param name="in">  A square matrix to raise to an integer power, which will be changed if dup is false. </param>
		''' <param name="n">   The integer power to raise the matrix to. </param>
		''' <param name="dup"> If dup is true, the original input is unchanged. </param>
		''' <returns> The result of raising <i>in</i> to the <i>n</i>th power. </returns>
		Public Shared Function mpow(ByVal [in] As INDArray, ByVal n As Integer, ByVal dup As Boolean) As INDArray
			Preconditions.checkState([in].Matrix AndAlso [in].Square, "Input must be a square matrix: got input with shape %s", [in].shape())
			If n = 0 Then
				If dup Then
					Return Nd4j.eye([in].rows())
				Else
					Return [in].assign(Nd4j.eye([in].rows()))
				End If
			End If
			Dim temp As INDArray
			If n < 0 Then
				temp = InvertMatrix.invert([in], Not dup)
				n = -n
			Else
				temp = [in].dup()
			End If
			Dim result As INDArray = temp.dup()
			If n < 4 Then
				For i As Integer = 1 To n - 1
					result.mmuli(temp)
				Next i
				If dup Then
					Return result
				Else
					Return [in].assign(result)
				End If
			Else
				' lets try to optimize by squaring itself a bunch of times
'JAVA TO VB CONVERTER WARNING: Java to VB Converter cannot determine whether both operands of this division are integer types - if they are then you should use the VB integer division operator:
				Dim squares As Integer = CInt(Math.Truncate(Math.Log(n) / Math.Log(2.0)))
				For i As Integer = 0 To squares - 1
					result = result.mmul(result)
				Next i
				Dim diff As Integer = CInt(CLng(Math.Round(n - Math.Pow(2.0, squares), MidpointRounding.AwayFromZero)))
				For i As Integer = 0 To diff - 1
					result.mmuli(temp)
				Next i
				If dup Then
					Return result
				Else
					Return [in].assign(result)
				End If
			End If
		End Function


		Protected Friend Shared Function broadcastResultShape(ByVal first As INDArray, ByVal second As INDArray) As Long()
			If first.equalShapes(second) Then
				Return first.shape()
			ElseIf Shape.areShapesBroadcastable(first.shape(), second.shape()) Then
				Return Shape.broadcastOutputShape(first.shape(), second.shape())
			Else
				Throw New System.InvalidOperationException("Array shapes are not broadcastable: " & Arrays.toString(first.shape()) & " vs. " & Arrays.toString(second.shape()))
			End If
		End Function
	End Class

End Namespace