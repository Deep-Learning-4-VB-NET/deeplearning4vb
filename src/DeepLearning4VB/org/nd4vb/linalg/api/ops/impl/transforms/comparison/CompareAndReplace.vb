Imports System
Imports System.Collections.Generic
Imports Preconditions = DeepLearning4VB.org.nd4vb.common.base.Preconditions
Imports NoOpNameFoundException = DeepLearning4VB.org.nd4vb.imports.NoOpNameFoundException
Imports DataType = DeepLearning4VB.org.nd4vb.linalg.api.buffer.DataType
Imports INDArray = DeepLearning4VB.org.nd4vb.linalg.api.ndarray.INDArray
Imports BaseTransformSameOp = DeepLearning4VB.org.nd4vb.linalg.api.ops.BaseTransformSameOp
Imports Condition = DeepLearning4VB.org.nd4vb.linalg.indexing.conditions.Condition
#If SAMEDIFF Then
Imports SDVariable = DeepLearning4VB.org.nd4vb.autodiff.samediff.SDVariable
Imports SameDiff = DeepLearning4VB.org.nd4vb.autodiff.samediff.SameDiff
#End If

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

Namespace org.nd4vb.linalg.api.ops.impl.transforms.comparison

    ''' <summary>The legacy transform same op <c>car</c> (Java <c>org.nd4j.linalg.api.ops.impl.transforms.comparison.CompareAndReplace</c>).</summary>
    Public Class CompareAndReplace
        Inherits BaseTransformSameOp

        Private condition As Condition
        Private compare As Double
        Private [set] As Double
        Private eps As Double
        Private mode As Integer

#If SAMEDIFF Then
		Public Sub New(ByVal sameDiff As SameDiff, ByVal [to] As SDVariable, ByVal from As SDVariable, ByVal condition As Condition)
			MyBase.New(sameDiff, [to], from, False)
			Me.condition = condition
			Me.compare = condition.getValue()
			Me.set = 0
			Me.mode = condition.condtionNum()
			Me.eps = condition.epsThreshold()
			Me.extraArgs = New Object() {compare, set, eps, CDbl(mode)}
		End Sub
#End If

        Public Sub New()

        End Sub

        ''' <summary>
        ''' With this constructor, op will check each X element against given Condition, and if condition met, element Z will be set to Y value, and X otherwise
        ''' 
        ''' PLEASE NOTE: X will be modified inplace.
        ''' 
        ''' Pseudocode:
        ''' z[i] = condition(x[i]) ? y[i] : x[i];
        ''' </summary>
        ''' <param name="x"> </param>
        ''' <param name="y"> </param>
        ''' <param name="condition"> </param>
        Public Sub New(ByVal x As INDArray, ByVal y As INDArray, ByVal condition As Condition)
            Me.New(x, y, Nothing, condition)
        End Sub

        ''' <summary>
        ''' With this constructor, op will check each X element against given Condition, and if condition met, element Z will be set to Y value, and X otherwise
        ''' 
        ''' Pseudocode:
        ''' z[i] = condition(x[i]) ? y[i] : x[i];
        ''' </summary>
        ''' <param name="x"> </param>
        ''' <param name="y"> </param>
        ''' <param name="z"> </param>
        ''' <param name="condition"> </param>
        Public Sub New(ByVal x As INDArray, ByVal y As INDArray, ByVal z As INDArray, ByVal condition As Condition)
            MyBase.New(x, y, z)
            Me.compare = condition.Value
            Me.set = 0
            Me.mode = condition.condtionNum()
            Me.eps = condition.epsThreshold()
            Me.extraArgsField = New Object() {compare, [set], eps, CDbl(mode)}
        End Sub

        Public Overrides Function propertiesForFunction() As IDictionary(Of String, Object)
            Dim ret As IDictionary(Of String, Object) = New JavaCompat.LinkedHashMap(Of String, Object)()
            ret("compare") = compare
            ret("set") = [set]
            ret("eps") = eps
            ret("mode") = mode
            Return ret
        End Function

        Public Overrides Function opNum() As Integer
            Return 13
        End Function

        Public Overrides Function opName() As String
            Return "car"
        End Function

        Public Overrides Function onnxName() As String
            Throw New NoOpNameFoundException("No onnx op opName found for " & opName())
        End Function

        Public Overrides Function tensorflowName() As String
            Throw New NoOpNameFoundException("No tensorflow op opName found for " & opName())
        End Function

#If SAMEDIFF Then
		Public Overrides Function doDiff(ByVal grad As IList(Of SDVariable)) As IList(Of SDVariable)
			'2 inputs: 'to' and 'from'
			'Pass through gradient for 'to' where condition is NOT satisfied
			'Pass through gradient for 'from' where condition IS satisfied
			Dim maskMatched As SDVariable = sameDiff.matchCondition(arg(0), condition).castTo(arg().dataType())
			Dim maskNotMatched As SDVariable = maskMatched.rsub(1.0)

			Return New List(Of SDVariable) From {grad(0).mul(maskNotMatched), grad(0).mul(maskMatched)}
		End Function
#End If

        Public Overrides Function calculateOutputDataTypes(ByVal dataTypes As IList(Of DataType)) As IList(Of DataType)
            Preconditions.checkState(dataTypes IsNot Nothing AndAlso dataTypes.Count = 2, "Expected exactly 2 input datatypes for %s, got input %s", Me.GetType(), dataTypes)
            Preconditions.checkState(dataTypes(0) = dataTypes(1), "Input data types must be the same: got %s", dataTypes)
            Return New List(Of DataType) From {dataTypes(0)}
        End Function

    End Class

End Namespace
