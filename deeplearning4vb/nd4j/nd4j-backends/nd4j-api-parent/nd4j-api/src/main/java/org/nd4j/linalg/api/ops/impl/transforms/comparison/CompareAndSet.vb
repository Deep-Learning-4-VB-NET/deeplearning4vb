Imports System.Collections.Generic
Imports SDVariable = org.nd4j.autodiff.samediff.SDVariable
Imports SameDiff = org.nd4j.autodiff.samediff.SameDiff
Imports NoOpNameFoundException = org.nd4j.imports.NoOpNameFoundException
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports BaseTransformSameOp = org.nd4j.linalg.api.ops.BaseTransformSameOp
Imports Condition = org.nd4j.linalg.indexing.conditions.Condition

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

Namespace org.nd4j.linalg.api.ops.impl.transforms.comparison


	Public Class CompareAndSet
		Inherits BaseTransformSameOp

		Private condition As Condition
		Private compare As Double
		Private set As Double
		Private eps As Double
		Private mode As Integer

		Public Sub New(ByVal sameDiff As SameDiff, ByVal [to] As SDVariable, ByVal set As Number, ByVal condition As Condition)
			MyBase.New(sameDiff, [to], False)
			Me.condition = condition
			Me.compare = condition.getValue()
			Me.set = set.doubleValue()
			Me.mode = condition.condtionNum()
			Me.eps = condition.epsThreshold()
			Me.extraArgs = New Object() {compare, set, eps, CDbl(mode)}
		End Sub

		Public Sub New()

		End Sub

		Public Sub New(ByVal x As INDArray, ByVal compare As Double, ByVal set As Double, ByVal eps As Double)
			Me.New(x, compare, set, eps, Nothing)
		End Sub

		Public Sub New(ByVal x As INDArray, ByVal compare As Double, ByVal set As Double, ByVal eps As Double, ByVal condition As Condition)
			MyBase.New(x)
			Me.compare = compare
			Me.set = set
			Me.eps = eps
			If condition Is Nothing Then
				Me.mode = 0
			Else
				Me.mode = condition.condtionNum()
			End If

			Me.extraArgs = New Object(){compare, set, eps, CDbl(mode)}
		End Sub


		''' <summary>
		''' With this constructor, op will check each X element against given Condition, and if condition met, element will be replaced with Set value
		''' 
		''' 
		''' Pseudocode:
		''' z[i] = condition(x[i]) ? set : x[i];
		''' 
		''' PLEASE NOTE: X will be modified inplace.
		''' </summary>
		''' <param name="x"> </param>
		''' <param name="set"> </param>
		''' <param name="condition"> </param>
		Public Sub New(ByVal x As INDArray, ByVal set As Double, ByVal condition As Condition)
			Me.New(x, x, set, condition)
		End Sub


		''' <summary>
		''' With this constructor, op will check each X element against given Condition, and if condition met, element will be replaced with Set value
		''' 
		''' Pseudocode:
		''' z[i] = condition(x[i]) ? set : x[i];
		''' </summary>
		''' <param name="x"> </param>
		''' <param name="set"> </param>
		''' <param name="condition"> </param>
		Public Sub New(ByVal x As INDArray, ByVal z As INDArray, ByVal set As Double, ByVal condition As Condition)
			MyBase.New(x, Nothing, z)
			Me.compare = condition.getValue()
			Me.set = set
			Me.eps = condition.epsThreshold()
			Me.mode = condition.condtionNum()
			Me.extraArgs = New Object(){compare, set, eps, CDbl(mode)}
		End Sub

		''' <summary>
		''' With this constructor, op will check each Y element against given Condition, and if condition met, element Z will be set to Y value, and X otherwise
		''' 
		''' PLEASE NOTE: X will be modified inplace.
		''' 
		''' Pseudocode:
		''' z[i] = condition(y[i]) ? y[i] : x[i];
		''' </summary>
		''' <param name="x"> </param>
		''' <param name="y"> </param>
		''' <param name="condition"> </param>
		Public Sub New(ByVal x As INDArray, ByVal y As INDArray, ByVal condition As Condition)
			Me.New(x, y, x, condition)
		End Sub


		''' <summary>
		''' With this constructor, op will check each Y element against given Condition, and if condition met, element Z will be set to Y value, and X otherwise
		''' 
		''' Pseudocode:
		''' z[i] = condition(y[i]) ? y[i] : x[i];
		''' </summary>
		''' <param name="x"> </param>
		''' <param name="y"> </param>
		''' <param name="z"> </param>
		''' <param name="condition"> </param>
		Public Sub New(ByVal x As INDArray, ByVal y As INDArray, ByVal z As INDArray, ByVal condition As Condition)
			MyBase.New(x, y, z)
			Me.compare = condition.getValue()
			Me.set = 0
			Me.eps = condition.epsThreshold()
			Me.mode = condition.condtionNum()
			Me.extraArgs = New Object(){compare, set, eps, CDbl(mode)}
		End Sub

		''' <summary>
		''' This constructor is shortcut to epsEquals.
		''' </summary>
		''' <param name="x"> </param>
		''' <param name="z"> </param>
		''' <param name="compare"> </param>
		''' <param name="set"> </param>
		''' <param name="eps"> </param>
		Public Sub New(ByVal x As INDArray, ByVal z As INDArray, ByVal compare As Double, ByVal set As Double, ByVal eps As Double)
			MyBase.New(x, z)
			Me.compare = compare
			Me.set = set
			Me.eps = eps
			Me.mode = 0
			Me.extraArgs = New Object(){compare, set, eps, CDbl(mode)}
		End Sub

		Public Overrides Function propertiesForFunction() As IDictionary(Of String, Object)
			Dim ret As IDictionary(Of String, Object) = New LinkedHashMap(Of String, Object)()
			ret("compare") = compare
			ret("set") = set
			ret("eps") = eps
			ret("mode") = mode
			Return ret
		End Function


		Public Overrides Function opNum() As Integer
			If y() Is Nothing Then
				Return 13
			Else
				Return 12
			End If
		End Function

		Public Overrides Function opName() As String
			Return "cas"
		End Function

		Public Overrides Function onnxName() As String
			Throw New NoOpNameFoundException("No onnx op opName found for " & opName())
		End Function

		Public Overrides Function tensorflowName() As String
			Throw New NoOpNameFoundException("No tensorflow op opName found for " & opName())
		End Function

		Public Overrides Function doDiff(ByVal gradient As IList(Of SDVariable)) As IList(Of SDVariable)
			'Pass through gradient where condition is NOT matched (condition matched: output replaced by scalar)
			Dim maskNotMatched As SDVariable = sameDiff.matchCondition(arg(), condition).castTo(arg().dataType()).rsub(1.0)
			Dim gradAtIn As SDVariable = gradient(0).mul(maskNotMatched)
			Return Collections.singletonList(gradAtIn)
		End Function
	End Class


End Namespace