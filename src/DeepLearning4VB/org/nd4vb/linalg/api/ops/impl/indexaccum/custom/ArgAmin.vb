Imports System
Imports System.Collections.Generic
Imports NoOpNameFoundException = DeepLearning4VB.org.nd4vb.imports.NoOpNameFoundException
Imports INDArray = DeepLearning4VB.org.nd4vb.linalg.api.ndarray.INDArray
Imports BaseDynamicCustomIndexReduction = DeepLearning4VB.org.nd4vb.linalg.api.ops.impl.reduce.custom.BaseDynamicCustomIndexReduction
#If SAMEDIFF Then
Imports SDVariable = DeepLearning4VB.org.nd4vb.autodiff.samediff.SDVariable
#End If
#If MODEL_IMPORT Then
Imports AttrValue = org.tensorflow.framework.AttrValue
Imports GraphDef = org.tensorflow.framework.GraphDef
Imports NodeDef = org.tensorflow.framework.NodeDef
#End If
#If SAMEDIFF OrElse MODEL_IMPORT Then
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

Namespace org.nd4vb.linalg.api.ops.impl.indexaccum.custom

    ''' <summary>Java <c>org.nd4j.linalg.api.ops.impl.indexaccum.custom.ArgAmin</c>.</summary>
    Public Class ArgAmin
        Inherits BaseDynamicCustomIndexReduction

        Public Sub New()
        End Sub

#If SAMEDIFF Then
		Public Sub New(ByVal sameDiff As SameDiff, ByVal args() As SDVariable, ByVal keepDims As Boolean)
			MyBase.New(sameDiff, args, keepDims)
		End Sub

		Public Sub New(ByVal sameDiff As SameDiff, ByVal args() As SDVariable, ByVal keepDims As Boolean, ByVal dimensions() As Integer)
			MyBase.New(sameDiff, args, keepDims, dimensions)
		End Sub
#End If

        Public Sub New(ByVal inputs() As INDArray)
            MyBase.New(inputs, Nothing)
        End Sub

        Public Sub New(ByVal inputs() As INDArray, ByVal outputs() As INDArray)
            MyBase.New(inputs, outputs)
        End Sub

        Public Sub New(ByVal inputs() As INDArray, ByVal outputs() As INDArray, ByVal keepDims As Boolean)
            MyBase.New(inputs, outputs, keepDims)
        End Sub

        Public Sub New(ByVal inputs() As INDArray, ByVal outputs() As INDArray, ByVal keepDims As Boolean, ParamArray ByVal dimensions() As Integer)
            MyBase.New(inputs, outputs, keepDims, dimensions)
        End Sub

        Public Sub New(ByVal inputs() As INDArray, ByVal [dim]() As Integer)
            Me.New(inputs,Nothing,False,[dim])
        End Sub

        Public Overrides Function opName() As String
            Return "argamin"
        End Function

        Public Overrides Function tensorflowName() As String
            Throw New NoOpNameFoundException("No tensorflow op opName found for " & opName())
        End Function

#If MODEL_IMPORT Then
		Public Overrides Sub initFromTensorFlow(ByVal nodeDef As NodeDef, ByVal initWith As SameDiff, ByVal attributesForNode As IDictionary(Of String, AttrValue), ByVal graph As GraphDef)

		End Sub
#End If

        ' ---------------------------------------------------------------- Lombok members

        ''' <summary>Lombok equals over the fields.</summary>
        Public Overrides Function Equals(ByVal o As Object) As Boolean
            If o Is Me Then
                Return True
            End If
            Dim other As ArgAmin = TryCast(o, ArgAmin)
            If other Is Nothing Then
                Return False
            End If
            Return True
        End Function

        ''' <summary>Lombok hashCode over the fields.</summary>
        Public Overrides Function GetHashCode() As Integer
            Dim result As Integer = 1
            Return result
        End Function

        ''' <summary>Lombok toString over the fields.</summary>
        Public Overrides Function ToString() As String
            Return "ArgAmin()"
        End Function

    End Class

End Namespace
