Imports System
Imports System.Collections.Generic
Imports NoOpNameFoundException = DeepLearning4VB.org.nd4vb.imports.NoOpNameFoundException
Imports INDArray = DeepLearning4VB.org.nd4vb.linalg.api.ndarray.INDArray
Imports BaseScalarOp = DeepLearning4VB.org.nd4vb.linalg.api.ops.BaseScalarOp
#If SAMEDIFF Then
Imports SDVariable = DeepLearning4VB.org.nd4vb.autodiff.samediff.SDVariable
Imports SameDiff = DeepLearning4VB.org.nd4vb.autodiff.samediff.SameDiff
Imports TFGraphMapper = DeepLearning4VB.org.nd4vb.imports.graphmapper.tf.TFGraphMapper
Imports DataType = DeepLearning4VB.org.nd4vb.linalg.api.buffer.DataType
Imports Relu6Derivative = DeepLearning4VB.org.nd4vb.linalg.api.ops.impl.transforms.gradient.Relu6Derivative
Imports Nd4j = DeepLearning4VB.org.nd4vb.linalg.factory.Nd4vb
Imports AttrValue = org.tensorflow.framework.AttrValue
Imports GraphDef = org.tensorflow.framework.GraphDef
Imports NodeDef = org.tensorflow.framework.NodeDef
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

Namespace org.nd4vb.linalg.api.ops.impl.scalar

    ''' <summary>The legacy scalar op <c>relu6</c> (Java <c>org.nd4j.linalg.api.ops.impl.scalar.Relu6</c>).</summary>
    Public Class Relu6
        Inherits BaseScalarOp

#If SAMEDIFF Then
		Public Sub New(ByVal sameDiff As SameDiff, ByVal i_v As SDVariable, ByVal inPlace As Boolean, ByVal cutoff As Double)
			MyBase.New(sameDiff, i_v, cutoff, inPlace)
		End Sub

		Public Sub New(ByVal sameDiff As SameDiff, ByVal i_v As SDVariable, ByVal cutoff As Double)
			Me.New(sameDiff, i_v, False, cutoff)
		End Sub
#End If

        Public Sub New()
            '
        End Sub

        Public Sub New(ByVal x As INDArray, ByVal z As INDArray, ByVal cutoff As Double)
            MyBase.New(x,Nothing, z, cutoff)
        End Sub
        Public Sub New(ByVal x As INDArray, ByVal cutoff As Double)
            MyBase.New(x, cutoff)
        End Sub

        Public Sub New(ByVal x As INDArray, ByVal z As INDArray)
            MyBase.New(x, Nothing, z,0.0f)
        End Sub

        Public Sub New(ByVal x As INDArray)
            Me.New(x, 0.0f)
        End Sub

        Public Overrides Function opNum() As Integer
            Return 40
        End Function

        Public Overrides Function opName() As String
            Return "relu6"
        End Function

        Public Overrides Function onnxName() As String
            Throw New NoOpNameFoundException("No onnx op opName found for " & opName())
        End Function

        Public Overrides Function tensorflowName() As String
            Return "Relu6"
        End Function

#If SAMEDIFF Then
		Public Overrides Sub initFromTensorFlow(ByVal nodeDef As NodeDef, ByVal initWith As SameDiff, ByVal attributesForNode As IDictionary(Of String, AttrValue), ByVal graph As GraphDef)
			'TF cutoff is always 0.0. Need to make sure scalar type is same as input type (due to scalar op 'same type' exec restrictions)
			If attributesForNode.ContainsKey("T") Then
				attributesForNode("T").getType()
				Dim dt As DataType = TFGraphMapper.convertType(attributesForNode("T").getType())
				scalarValue = Nd4j.scalar(dt, 0.0)
			End If
		End Sub

		Public Overrides Function doDiff(ByVal i_v As IList(Of SDVariable)) As IList(Of SDVariable)
			Dim dLdOut As SDVariable = i_v(0)
			Return (New Relu6Derivative(sameDiff, arg(), dLdOut, scalarValue.getDouble(0))).outputs()
		End Function
#End If

    End Class

End Namespace
