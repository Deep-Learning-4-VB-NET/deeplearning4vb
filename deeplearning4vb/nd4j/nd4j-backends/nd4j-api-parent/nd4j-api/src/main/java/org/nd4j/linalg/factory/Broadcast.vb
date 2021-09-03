Imports Preconditions = org.nd4j.common.base.Preconditions
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports org.nd4j.linalg.api.ops.impl.broadcast
Imports org.nd4j.linalg.api.ops.impl.broadcast.bool
Imports EqualTo = org.nd4j.linalg.api.ops.impl.transforms.custom.EqualTo
Imports GreaterThan = org.nd4j.linalg.api.ops.impl.transforms.custom.GreaterThan
Imports GreaterThanOrEqual = org.nd4j.linalg.api.ops.impl.transforms.custom.GreaterThanOrEqual
Imports LessThan = org.nd4j.linalg.api.ops.impl.transforms.custom.LessThan
Imports LessThanOrEqual = org.nd4j.linalg.api.ops.impl.transforms.custom.LessThanOrEqual
Imports Max = org.nd4j.linalg.api.ops.impl.transforms.custom.Max
Imports Min = org.nd4j.linalg.api.ops.impl.transforms.custom.Min
Imports NotEqualTo = org.nd4j.linalg.api.ops.impl.transforms.custom.NotEqualTo
Imports org.nd4j.linalg.api.ops.impl.transforms.pairwise.arithmetic
Imports AMax = org.nd4j.linalg.api.ops.impl.transforms.same.AMax
Imports AMin = org.nd4j.linalg.api.ops.impl.transforms.same.AMin

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

Namespace org.nd4j.linalg.factory


	Public Class Broadcast

		Private Sub New()
		End Sub

		''' <summary>
		''' Broadcast add op. See: <seealso cref="BroadcastAddOp"/>
		''' </summary>
		Public Shared Function add(ByVal x As INDArray, ByVal y As INDArray, ByVal z As INDArray, ParamArray ByVal dimensions() As Integer) As INDArray
			If dimensions Is Nothing OrElse dimensions.Length = 0 Then
				validateShapesNoDimCase(x,y,z)
				Return Nd4j.Executioner.exec(New AddOp(x,y,z))(0)
			End If

			Return Nd4j.Executioner.exec(New BroadcastAddOp(x,y,z,dimensions))
		End Function

		''' <summary>
		''' Broadcast copy op. See: <seealso cref="BroadcastCopyOp"/>
		''' </summary>
		Public Shared Function copy(ByVal x As INDArray, ByVal y As INDArray, ByVal z As INDArray, ParamArray ByVal dimensions() As Integer) As INDArray
			If dimensions Is Nothing OrElse dimensions.Length = 0 Then
				validateShapesNoDimCase(x,y,z)
				Return Nd4j.Executioner.exec(New CopyOp(x,y,z))
			End If

			Return Nd4j.Executioner.exec(New BroadcastCopyOp(x,y,z,dimensions))
		End Function

		''' <summary>
		''' Broadcast divide op. See: <seealso cref="BroadcastDivOp"/>
		''' </summary>
		Public Shared Function div(ByVal x As INDArray, ByVal y As INDArray, ByVal z As INDArray, ParamArray ByVal dimensions() As Integer) As INDArray
			If dimensions Is Nothing OrElse dimensions.Length = 0 Then
				validateShapesNoDimCase(x,y,z)
				Return Nd4j.Executioner.exec(New DivOp(x,y,z))(0)
			End If

			Return Nd4j.Executioner.exec(New BroadcastDivOp(x,y,z,dimensions))
		End Function

		''' <summary>
		''' Broadcast equal to op. See: <seealso cref="BroadcastEqualTo"/>
		''' </summary>
		Public Shared Function eq(ByVal x As INDArray, ByVal y As INDArray, ByVal z As INDArray, ParamArray ByVal dimensions() As Integer) As INDArray
			If dimensions Is Nothing OrElse dimensions.Length = 0 Then
				validateShapesNoDimCase(x,y,z)
				Return Nd4j.Executioner.exec(New EqualTo(x,y,z))(0)
			End If
			Return Nd4j.Executioner.exec(New BroadcastEqualTo(x,y,z,dimensions))
		End Function

		''' <summary>
		''' Broadcast greater than op. See: <seealso cref="BroadcastGreaterThan"/>
		''' </summary>
		Public Shared Function gt(ByVal x As INDArray, ByVal y As INDArray, ByVal z As INDArray, ParamArray ByVal dimensions() As Integer) As INDArray
			If dimensions Is Nothing OrElse dimensions.Length = 0 Then
				validateShapesNoDimCase(x,y,z)
				Return Nd4j.Executioner.exec(New GreaterThan(x,y,z))(0)
			End If

			Return Nd4j.Executioner.exec(New BroadcastGreaterThan(x,y,z,dimensions))
		End Function

		''' <summary>
		''' Broadcast greater than or equal to op. See: <seealso cref="BroadcastGreaterThanOrEqual"/>
		''' </summary>
		Public Shared Function gte(ByVal x As INDArray, ByVal y As INDArray, ByVal z As INDArray, ParamArray ByVal dimensions() As Integer) As INDArray
			If dimensions Is Nothing OrElse dimensions.Length = 0 Then
				validateShapesNoDimCase(x,y,z)
				Return Nd4j.Executioner.exec(New GreaterThanOrEqual(x,y,z))(0)
			End If

			Return Nd4j.Executioner.exec(New BroadcastGreaterThanOrEqual(x,y,z,dimensions))
		End Function

		''' <summary>
		''' Broadcast less than op. See: <seealso cref="BroadcastLessThan"/>
		''' </summary>
		Public Shared Function lt(ByVal x As INDArray, ByVal y As INDArray, ByVal z As INDArray, ParamArray ByVal dimensions() As Integer) As INDArray
			If dimensions Is Nothing OrElse dimensions.Length = 0 Then
				validateShapesNoDimCase(x,y,z)
				Return Nd4j.Executioner.exec(New LessThan(x,y,z))(0)
			End If

			Return Nd4j.Executioner.exec(New BroadcastLessThan(x,y,z,dimensions))
		End Function

		''' <summary>
		''' Broadcast less than or equal to op. See: <seealso cref="BroadcastLessThanOrEqual"/>
		''' </summary>
		Public Shared Function lte(ByVal x As INDArray, ByVal y As INDArray, ByVal z As INDArray, ParamArray ByVal dimensions() As Integer) As INDArray
			If dimensions Is Nothing OrElse dimensions.Length = 0 Then
				validateShapesNoDimCase(x,y,z)
				Return Nd4j.Executioner.exec(New LessThanOrEqual(x,y,z))(0)
			End If

			Return Nd4j.Executioner.exec(New BroadcastLessThanOrEqual(x,y,z,dimensions))
		End Function

		''' <summary>
		''' Broadcast element-wise multiply op. See: <seealso cref="BroadcastMulOp"/>
		''' </summary>
		Public Shared Function mul(ByVal x As INDArray, ByVal y As INDArray, ByVal z As INDArray, ParamArray ByVal dimensions() As Integer) As INDArray
			If dimensions Is Nothing OrElse dimensions.Length = 0 Then
				validateShapesNoDimCase(x,y,z)
				Return Nd4j.Executioner.exec(New MulOp(x,y,z))(0)
			End If

			Return Nd4j.Executioner.exec(New BroadcastMulOp(x,y,z,dimensions))
		End Function

		''' <summary>
		''' Broadcast not equal to op. See: <seealso cref="BroadcastNotEqual"/>
		''' </summary>
		Public Shared Function neq(ByVal x As INDArray, ByVal y As INDArray, ByVal z As INDArray, ParamArray ByVal dimensions() As Integer) As INDArray
			If dimensions Is Nothing OrElse dimensions.Length = 0 Then
				validateShapesNoDimCase(x,y,z)
				Return Nd4j.Executioner.exec(New NotEqualTo(x,y,z))(0)
			End If

			Return Nd4j.Executioner.exec(New BroadcastNotEqual(x,y,z,dimensions))
		End Function

		''' <summary>
		''' Broadcast reverse division op. See: <seealso cref="BroadcastRDivOp"/>
		''' </summary>
		Public Shared Function rdiv(ByVal x As INDArray, ByVal y As INDArray, ByVal z As INDArray, ParamArray ByVal dimensions() As Integer) As INDArray
			If dimensions Is Nothing OrElse dimensions.Length = 0 Then
				validateShapesNoDimCase(x,y,z)
				Return Nd4j.Executioner.exec(New RDivOp(x,y,z))(0)
			End If

			Return Nd4j.Executioner.exec(New BroadcastRDivOp(x,y,z,dimensions))
		End Function

		''' <summary>
		''' Broadcast reverse subtraction op. See: <seealso cref="BroadcastRSubOp"/>
		''' </summary>
		Public Shared Function rsub(ByVal x As INDArray, ByVal y As INDArray, ByVal z As INDArray, ParamArray ByVal dimensions() As Integer) As INDArray
			If dimensions Is Nothing OrElse dimensions.Length = 0 Then
				validateShapesNoDimCase(x,y,z)
				Return Nd4j.Executioner.exec(New SubOp(x,y,z))(0)
			End If

			Return Nd4j.Executioner.exec(New BroadcastRSubOp(x,y,z,dimensions))
		End Function

		''' <summary>
		''' Broadcast subtraction op. See: <seealso cref="BroadcastSubOp"/>
		''' </summary>
		Public Shared Function [sub](ByVal x As INDArray, ByVal y As INDArray, ByVal z As INDArray, ParamArray ByVal dimensions() As Integer) As INDArray
			If dimensions Is Nothing OrElse dimensions.Length = 0 Then
				validateShapesNoDimCase(x,y,z)
				Return Nd4j.Executioner.exec(New SubOp(x,y,z))(0)
			End If

			Return Nd4j.Executioner.exec(New BroadcastSubOp(x,y,z,dimensions))
		End Function

		''' <summary>
		''' Broadcast max op. See: <seealso cref="BroadcastMax"/>
		''' </summary>
		Public Shared Function max(ByVal x As INDArray, ByVal y As INDArray, ByVal z As INDArray, ParamArray ByVal dimensions() As Integer) As INDArray
			If dimensions Is Nothing OrElse dimensions.Length = 0 Then
				validateShapesNoDimCase(x,y,z)
				Return Nd4j.Executioner.exec(New Max(x,y,z))(0)
			End If


			Return Nd4j.Executioner.exec(New BroadcastMax(x,y,z,dimensions))
		End Function

		''' <summary>
		''' Broadcast min op. See: <seealso cref="BroadcastMin"/>
		''' </summary>
		Public Shared Function min(ByVal x As INDArray, ByVal y As INDArray, ByVal z As INDArray, ParamArray ByVal dimensions() As Integer) As INDArray
			If dimensions Is Nothing OrElse dimensions.Length = 0 Then
				validateShapesNoDimCase(x,y,z)
				Return Nd4j.Executioner.exec(New Min(x,y,z))(0)
			End If


			Return Nd4j.Executioner.exec(New BroadcastMin(x,y,z,dimensions))
		End Function

		''' <summary>
		''' Broadcast absolute max op. See: <seealso cref="BroadcastAMax"/>
		''' </summary>
		Public Shared Function amax(ByVal x As INDArray, ByVal y As INDArray, ByVal z As INDArray, ParamArray ByVal dimensions() As Integer) As INDArray
			If dimensions Is Nothing OrElse dimensions.Length = 0 Then
				validateShapesNoDimCase(x,y,z)
				Return Nd4j.Executioner.exec(New AMax(x,y,z))
			End If

			Return Nd4j.Executioner.exec(New BroadcastAMax(x,y,z,dimensions))
		End Function

		''' <summary>
		''' Broadcast absolute min op. See: <seealso cref="BroadcastAMax"/>
		''' </summary>
		Public Shared Function amin(ByVal x As INDArray, ByVal y As INDArray, ByVal z As INDArray, ParamArray ByVal dimensions() As Integer) As INDArray
			If dimensions Is Nothing OrElse dimensions.Length = 0 Then
				validateShapesNoDimCase(x,y,z)
				Return Nd4j.Executioner.exec(New AMin(x,y,z))
			End If

			Return Nd4j.Executioner.exec(New BroadcastAMin(x,y,z,dimensions))
		End Function

		Public Shared Sub validateShapesNoDimCase(ByVal x As INDArray, ByVal y As INDArray, ByVal z As INDArray)
			Preconditions.checkArgument(x.equalShapes(y), "When no dimensions are provided, X and Y shapes must be" & " equal (x shape: %s, y shape: %s)", x.shape(), y.shape())
			Preconditions.checkArgument(x.equalShapes(z), "When no dimensions are provided, X and Z (result) shapes must be" & " equal (x shape: %s, z shape: %s)", x.shape(), z.shape())
		End Sub

		''' <summary>
		''' Validate the broadcast dimensions for manual broadcast ops such as <seealso cref="BroadcastMulOp"/>.
		''' Here, the dimensions are those that the arrays match on WRT X.
		''' For example, mul([a,b,c], [a,c], 0,2)
		''' </summary>
		Public Shared Sub validateBroadcastDims(ByVal x As INDArray, ByVal y As INDArray, ByVal z As INDArray, ParamArray ByVal dimensions() As Integer)
			Preconditions.checkArgument(x Is z OrElse x.equalShapes(z), "X and Z arrays must be equal shape. X shape: %s, Z shape: %s", x.shape(), z.shape())
			Dim sx() As Long = x.shape()
			Dim sy() As Long = y.shape()
			'Possibility 1: equal ranks - dimensions must match
			If dimensions.Length = 1 AndAlso sy.Length = 2 AndAlso (sy(0) = 1 OrElse sy(1) = 1) Then
				'Edge case: x=[a,b,c], y=[1,b], dim=1 etc
				Dim d2 As Integer = If(dimensions(0) < 0, dimensions(0) + sx.Length, dimensions(0)) 'Handle negative dimensions
				If sy(0) = 1 Then
					Preconditions.checkState(sx(d2) = sy(1), "Shapes do not match: dimensions[0] - x[%s] must match y[%s], x shape %s, y shape %s, dimensions %s", dimensions(0), 1, sx, sy, dimensions)
				Else
					Preconditions.checkState(sx(d2) = sy(0), "Shapes do not match: dimensions[0] - x[%s] must match y[%s], x shape %s, y shape %s, dimensions %s", dimensions(0), 0, sx, sy, dimensions)
				End If
			ElseIf sx.Length = sy.Length Then
				For Each d As Integer In dimensions
					Dim d2 As Integer = If(d < 0, d + sx.Length, d) 'Handle negative dimensions
					Preconditions.checkState(sx(d2) = sy(d2), "Dimensions mismatch on dimension %s: x shape %s, y shape %s", d, sx, sy)
				Next d
			ElseIf dimensions.Length = sy.Length Then
				'Possibility 2: different ranks - for example, mul([a,b,c],[a,c], [0,2]) - dimensions refer to x
				Dim i As Integer = 0
				Do While i < dimensions.Length
					Dim d2 As Integer = If(dimensions(i) < 0, dimensions(i) + sx.Length, dimensions(i)) 'Handle negative dimensions
					Preconditions.checkState(sx(d2) = sy(i), "Shapes do not match: dimensions[%s] - x[%s] must match y[%s], x shape %s, y shape %s, dimensions %s", i, d2, i, sx, sy, dimensions)
					i += 1
				Loop
			Else
				Throw New System.InvalidOperationException("Invalid broadcast dimensions: x shape " & Arrays.toString(sx) & ", y shape " & Arrays.toString(sy) & ", dimensions " & Arrays.toString(dimensions))
			End If
		End Sub

	End Class

End Namespace