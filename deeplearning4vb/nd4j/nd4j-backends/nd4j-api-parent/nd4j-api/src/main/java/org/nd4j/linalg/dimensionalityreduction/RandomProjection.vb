Imports System
Imports System.Collections
Imports System.Collections.Generic
Imports System.Linq
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports GaussianDistribution = org.nd4j.linalg.api.ops.random.impl.GaussianDistribution
Imports Random = org.nd4j.linalg.api.rng.Random
Imports ND4JIllegalStateException = org.nd4j.linalg.exception.ND4JIllegalStateException
Imports Nd4j = org.nd4j.linalg.factory.Nd4j

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

Namespace org.nd4j.linalg.dimensionalityreduction




	Public Class RandomProjection

		Private components As Integer
		Private rng As Random
		Private eps As Double
		Private autoMode As Boolean

		Public Sub New(ByVal eps As Double, ByVal rng As Random)
			Me.rng = rng
			Me.eps = eps
			Me.autoMode = True
		End Sub

		Public Sub New(ByVal eps As Double)
			Me.New(eps, Nd4j.Random)
		End Sub

		Public Sub New(ByVal components As Integer, ByVal rng As Random)
			Me.rng = rng
			Me.components = components
			Me.autoMode = False
		End Sub

		Public Sub New(ByVal components As Integer)
			Me.New(components, Nd4j.Random)
		End Sub

		''' <summary>
		''' Find a safe number of components to project this to, through
		''' the Johnson-Lindenstrauss lemma
		''' The minimum number n' of components to guarantee the eps-embedding is
		''' given by:
		''' 
		''' n' >= 4 log(n) / (eps² / 2 - eps³ / 3)
		''' 
		''' see http://cseweb.ucsd.edu/~dasgupta/papers/jl.pdf §2.1 </summary>
		''' <param name="n"> Number of samples. If an array is given, it will compute
		'''        a safe number of components array-wise. </param>
		''' <param name="eps"> Maximum distortion rate as defined by the Johnson-Lindenstrauss lemma.
		'''            Will compute array-wise if an array is given.
		''' @return </param>
		Public Shared Function johnsonLindenstraussMinDim(ByVal n() As Integer, ParamArray ByVal eps() As Double) As IList(Of Integer)
			Dim basicCheck As Boolean? = n Is Nothing OrElse n.Length = 0 OrElse eps Is Nothing OrElse eps.Length = 0
			If basicCheck Then
				Throw New System.ArgumentException("Johnson-Lindenstrauss dimension estimation requires > 0 components and at least a relative error")
			End If
			For Each epsilon As Double In eps
				If epsilon <= 0 OrElse epsilon >= 1 Then
					Throw New System.ArgumentException("A relative error should be in ]0, 1[")
				End If
			Next epsilon
			Dim res As IList(Of Integer) = New ArrayList(n.Length * eps.Length)
			For Each epsilon As Double In eps
'JAVA TO VB CONVERTER WARNING: Java to VB Converter cannot determine whether both operands of this division are integer types - if they are then you should use the VB integer division operator:
				Dim denom As Double = (Math.Pow(epsilon, 2) / 2 - Math.Pow(epsilon, 3) / 3)
				For Each components As Integer In n
'JAVA TO VB CONVERTER WARNING: Java to VB Converter cannot determine whether both operands of this division are integer types - if they are then you should use the VB integer division operator:
					res.Add(CInt(Math.Truncate(4 * Math.Log(components) / denom)))
				Next components
			Next epsilon
			Return res
		End Function

		Public Shared Function johnsonLindenstraussMinDim(ByVal n() As Long, ParamArray ByVal eps() As Double) As IList(Of Long)
			Dim basicCheck As Boolean? = n Is Nothing OrElse n.Length = 0 OrElse eps Is Nothing OrElse eps.Length = 0
			If basicCheck Then
				Throw New System.ArgumentException("Johnson-Lindenstrauss dimension estimation requires > 0 components and at least a relative error")
			End If
			For Each epsilon As Double In eps
				If epsilon <= 0 OrElse epsilon >= 1 Then
					Throw New System.ArgumentException("A relative error should be in ]0, 1[")
				End If
			Next epsilon
			Dim res As IList(Of Long) = New ArrayList(n.Length * eps.Length)
			For Each epsilon As Double In eps
'JAVA TO VB CONVERTER WARNING: Java to VB Converter cannot determine whether both operands of this division are integer types - if they are then you should use the VB integer division operator:
				Dim denom As Double = (Math.Pow(epsilon, 2) / 2 - Math.Pow(epsilon, 3) / 3)
				For Each components As Long In n
'JAVA TO VB CONVERTER WARNING: Java to VB Converter cannot determine whether both operands of this division are integer types - if they are then you should use the VB integer division operator:
					res.Add(CLng(Math.Truncate(4 * Math.Log(components) / denom)))
				Next components
			Next epsilon
			Return res
		End Function

		Public Shared Function johnsonLindenStraussMinDim(ByVal n As Integer, ParamArray ByVal eps() As Double) As IList(Of Integer)
			Return johnsonLindenstraussMinDim(New Integer(){n}, eps)
		End Function

		Public Shared Function johnsonLindenStraussMinDim(ByVal n As Long, ParamArray ByVal eps() As Double) As IList(Of Long)
			Return johnsonLindenstraussMinDim(New Long(){n}, eps)
		End Function

		''' <summary>
		''' Generate a dense Gaussian random matrix.
		''' 
		'''   The n' components of the random matrix are drawn from
		'''       N(0, 1.0 / n').
		''' </summary>
		''' <param name="shape"> </param>
		''' <param name="rng">
		''' @return </param>
		Private Function gaussianRandomMatrix(ByVal shape() As Long, ByVal rng As Random) As INDArray
			Nd4j.checkShapeValues(shape)
			Dim res As INDArray = Nd4j.create(shape)

			Dim op1 As New GaussianDistribution(res, 0.0, 1.0 / Math.Sqrt(shape(0)))
			Nd4j.Executioner.exec(op1, rng)
			Return res
		End Function

		Private projectionMatrixShape() As Long
		Private _projectionMatrix As INDArray

		Private Function getProjectionMatrix(ByVal shape() As Long, ByVal rng As Random) As INDArray
			If Not projectionMatrixShape.SequenceEqual(shape) OrElse _projectionMatrix Is Nothing Then
				_projectionMatrix = gaussianRandomMatrix(shape, rng)
			End If
			Return _projectionMatrix
		End Function

		''' 
		''' <summary>
		''' Compute the target shape of the projection matrix </summary>
		''' <param name="shape"> the shape of the data tensor </param>
		''' <param name="eps"> the relative error used in the Johnson-Lindenstrauss estimation </param>
		''' <param name="auto"> whether to use JL estimation for user specification </param>
		''' <param name="targetDimension"> the target size for the
		'''  </param>
		Private Shared Function targetShape(ByVal shape() As Integer, ByVal eps As Double, ByVal targetDimension As Integer, ByVal auto As Boolean) As Integer()
			Dim components As Integer = targetDimension
			If auto Then
				components = johnsonLindenStraussMinDim(shape(0), eps)(0)
			End If
			' JL or user spec edge cases
			If auto AndAlso (components <= 0 OrElse components > shape(1)) Then
				Throw New ND4JIllegalStateException(String.Format("Estimation led to a target dimension of {0:D}, which is invalid", components))
			End If
			Return New Integer(){ shape(1), components}
		End Function

		Private Shared Function targetShape(ByVal shape() As Long, ByVal eps As Double, ByVal targetDimension As Integer, ByVal auto As Boolean) As Long()
			Dim components As Long = targetDimension
			If auto Then
				components = johnsonLindenStraussMinDim(shape(0), eps)(0)
			End If
			' JL or user spec edge cases
			If auto AndAlso (components <= 0 OrElse components > shape(1)) Then
				Throw New ND4JIllegalStateException(String.Format("Estimation led to a target dimension of {0:D}, which is invalid", components))
			End If
			Return New Long(){ shape(1), components}
		End Function


		''' <summary>
		''' Compute the target shape of a suitable projection matrix </summary>
		''' <param name="X"> the Data tensor </param>
		''' <param name="eps"> the relative error used in the Johnson-Lindenstrauss estimation </param>
		''' <returns> the shape of the projection matrix to use </returns>
		Protected Friend Shared Function targetShape(ByVal X As INDArray, ByVal eps As Double) As Long()
			Return targetShape(X.shape(), eps, -1, True)
		End Function

		''' <summary>
		''' Compute the target shape of a suitable projection matrix </summary>
		''' <param name="X"> the Data Tensor </param>
		''' <param name="targetDimension"> a desired dimension </param>
		''' <returns> the shape of the projection matrix to use </returns>
		Protected Friend Shared Function targetShape(ByVal X As INDArray, ByVal targetDimension As Integer) As Long()
			Return targetShape(X.shape(), -1, targetDimension, False)
		End Function


		''' <summary>
		''' Create a copy random projection by using matrix product with a random matrix </summary>
		''' <param name="data"> </param>
		''' <returns> the projected matrix </returns>
		Public Overridable Function project(ByVal data As INDArray) As INDArray
			Dim tShape() As Long = targetShape(data.shape(), eps, components, autoMode)
			Return data.mmul(getProjectionMatrix(tShape, Me.rng))
		End Function

		''' <summary>
		''' Create a copy random projection by using matrix product with a random matrix
		''' </summary>
		''' <param name="data"> </param>
		''' <param name="result"> a placeholder result
		''' @return </param>
		Public Overridable Function project(ByVal data As INDArray, ByVal result As INDArray) As INDArray
			Dim tShape() As Long = targetShape(data.shape(), eps, components, autoMode)
			Return data.mmuli(getProjectionMatrix(tShape, Me.rng), result)
		End Function

		''' <summary>
		''' Create an in-place random projection by using in-place matrix product with a random matrix </summary>
		''' <param name="data"> </param>
		''' <returns> the projected matrix </returns>
		Public Overridable Function projecti(ByVal data As INDArray) As INDArray
			Dim tShape() As Long = targetShape(data.shape(), eps, components, autoMode)
			Return data.mmuli(getProjectionMatrix(tShape, Me.rng))
		End Function

		''' <summary>
		''' Create an in-place random projection by using in-place matrix product with a random matrix
		''' </summary>
		''' <param name="data"> </param>
		''' <param name="result"> a placeholder result
		''' @return </param>
		Public Overridable Function projecti(ByVal data As INDArray, ByVal result As INDArray) As INDArray
			Dim tShape() As Long = targetShape(data.shape(), eps, components, autoMode)
			Return data.mmuli(getProjectionMatrix(tShape, Me.rng), result)
		End Function



	End Class

End Namespace