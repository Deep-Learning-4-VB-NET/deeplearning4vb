Imports System
Imports System.Collections.Generic
Imports NoOpNameFoundException = DeepLearning4VB.org.nd4vb.imports.NoOpNameFoundException
Imports INDArray = DeepLearning4VB.org.nd4vb.linalg.api.ndarray.INDArray
Imports DynamicCustomOp = DeepLearning4VB.org.nd4vb.linalg.api.ops.DynamicCustomOp
Imports Op = DeepLearning4VB.org.nd4vb.linalg.api.ops.Op
Imports OpContext = DeepLearning4VB.org.nd4vb.linalg.api.ops.OpContext
Imports LongShapeDescriptor = DeepLearning4VB.org.nd4vb.linalg.api.shape.LongShapeDescriptor
Imports Nd4vbT = DeepLearning4VB.org.nd4vb.linalg.factory.Nd4vb
#If SAMEDIFF Then
Imports SDVariable = DeepLearning4VB.org.nd4vb.autodiff.samediff.SDVariable
Imports VariableType = DeepLearning4VB.org.nd4vb.autodiff.samediff.VariableType
Imports Preconditions = DeepLearning4VB.org.nd4vb.common.base.Preconditions
Imports DataType = DeepLearning4VB.org.nd4vb.linalg.api.buffer.DataType
Imports Nd4j = DeepLearning4VB.org.nd4vb.linalg.factory.Nd4vb
#End If
#If MODEL_IMPORT Then
Imports Onnx = onnx.Onnx
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

Namespace org.nd4vb.linalg.api.ops.impl.layers

    ''' <summary>The custom op <c>ExternalErrorsFn</c> (Java <c>org.nd4j.linalg.api.ops.impl.layers.ExternalErrorsFunction</c>).</summary>
    Public Class ExternalErrorsFunction
        Inherits DynamicCustomOp

        Public Const OP_NAME As String = "ExternalErrorsFn"

        ''' <summary>
        ''' The output shape: a scalar of the default data type. Java computes it when the class loads; here it is
        ''' computed on first use, so loading the class does not need the ND4VB factory.
        ''' </summary>
        Private Shared ReadOnly outShape As New Lazy(Of IList(Of LongShapeDescriptor))(
            Function() JavaCollections.singletonList(LongShapeDescriptor.fromShape(New Long() {}, Nd4vbT.dataType())),
            System.Threading.LazyThreadSafetyMode.PublicationOnly)

        Private Shared ReadOnly Property OUT_SHAPE As IList(Of LongShapeDescriptor)
            Get
                Return outShape.Value
            End Get
        End Property

        Private gradients As IDictionary(Of String, INDArray)
#If SAMEDIFF Then
		Private gradVariables As IDictionary(Of String, SDVariable)
		Private [out] As SDVariable
#End If

        Private id As String

#If SAMEDIFF Then
		Public Sub New(ByVal sd As SameDiff, ByVal inputs As IList(Of SDVariable), ByVal gradients As IDictionary(Of String, INDArray))
			MyBase.New(sd, CType(inputs, List(Of SDVariable)).ToArray())
			If gradients Is Nothing Then
				gradients = New Dictionary(Of String, INDArray)()
			End If
			Me.gradients = gradients
			Me.id = System.Guid.randomUUID().ToString()
		End Sub
#End If

        Public Sub New()
        End Sub

#If SAMEDIFF Then
		Public Overridable ReadOnly Property GradPlaceholderName As String
			Get
				Return arg().name() & "-grad"
			End Get
		End Property

		Public Overrides Function outputVariables(ByVal baseName As String) As SDVariable()
			If [out] Is Nothing Then
				If id Is Nothing Then
					Me.id = System.Guid.randomUUID().ToString()
				End If
				Dim name As String = "dummyOutput-" & id
				If sameDiff.hasVariable(name) Then
					[out] = sameDiff.getVariable(name)
				Else
					[out] = sameDiff.zero(name, Nd4j.dataType(), 1)
					sameDiff.getOps().get(getOwnName()).setOutputsOfOp(Collections.singletonList([out].name()))
					sameDiff.getVariables().get(name).setOutputOfOp(getOwnName())
				End If
			End If
			Return New SDVariable(){[out]}
		End Function

		Public Overrides Function doDiff(ByVal f1 As IList(Of SDVariable)) As IList(Of SDVariable)
			Dim [out] As IList(Of SDVariable) = New List(Of SDVariable)()
			If gradVariables Is Nothing Then
				gradVariables = New Dictionary(Of String, SDVariable)()
				For Each arg As SDVariable In args()
					Dim gradArr As INDArray = gradients(arg.name())
					Dim grad As SDVariable
					Dim dt As DataType = arg.dataType()
					Dim n As String = GradPlaceholderName
					If gradArr IsNot Nothing Then
						Dim shape() As Long = CType(gradArr.shape().Clone(), Long())
						shape(0) = -1
						grad = sameDiff.var(n, VariableType.PLACEHOLDER, Nothing, dt, shape)
					Else
						grad = sameDiff.var(n, VariableType.PLACEHOLDER, Nothing, dt)
					End If
					gradVariables(arg.name()) = grad
					[out].Add(grad)
				Next arg
			End If
			Return [out]
		End Function
#End If

#If SAMEDIFF Then
		Public Overridable Sub updateBeforeExecution()
			Preconditions.checkState(gradVariables IsNot Nothing, "Variables list is null - doDiff has not been called?")

			'Update external gradients ready for execution
			For Each e As KeyValuePair(Of String, SDVariable) In gradVariables.SetOfKeyValuePairs()
				Dim extGradArray As INDArray = gradients(e.Key)
				If extGradArray Is Nothing Then
					Throw New System.InvalidOperationException("Cannot execute SameDiff instance with external errors: external gradient " & "for variable " & e.Key & " has not been defined")
				End If
				gradVariables(e.Key).setArray(extGradArray)
			Next e
		End Sub
#Else
        Public Overridable Sub updateBeforeExecution()
            Throw New NotSupportedException("updateBeforeExecution needs SameDiff (phase 2 milestone M5)")
        End Sub
#End If

#If MODEL_IMPORT Then
		Public Overrides Sub initFromTensorFlow(ByVal nodeDef As NodeDef, ByVal initWith As SameDiff, ByVal attributesForNode As IDictionary(Of String, AttrValue), ByVal graph As GraphDef)

		End Sub

		Public Overrides Sub initFromOnnx(ByVal node As Onnx.NodeProto, ByVal initWith As SameDiff, ByVal attributesForNode As IDictionary(Of String, Onnx.AttributeProto), ByVal graph As Onnx.GraphProto)

		End Sub
#End If

        Public Overrides Function onnxName() As String
            Throw New NoOpNameFoundException("Not supported: " & opName())
        End Function

        Public Overrides Function tensorflowName() As String
            Throw New NoOpNameFoundException("Not supported: " & opName())
        End Function

        Public Overrides Function opName() As String
            Return OP_NAME
        End Function

        Public Overrides Function ToString() As String
#If SAMEDIFF Then
            Return "ExternalErrorsFunction(" & (If(gradVariables IsNot Nothing, gradVariables.Keys, "")) & ")"
#Else
            Return "ExternalErrorsFunction()"
#End If
        End Function

        Public Overrides Function calculateOutputShape() As IList(Of LongShapeDescriptor)
            Return OUT_SHAPE
        End Function

        Public Overrides Function calculateOutputShape(ByVal oc As OpContext) As IList(Of LongShapeDescriptor)
            Return OUT_SHAPE
        End Function

        Public Overrides Function opType() As Op.Type
            Return Op.Type.LOGIC
        End Function

    End Class

End Namespace
