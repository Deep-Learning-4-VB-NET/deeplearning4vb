Imports System
Imports System.Collections.Generic
Imports Preconditions = DeepLearning4VB.org.nd4vb.common.base.Preconditions
Imports DataType = DeepLearning4VB.org.nd4vb.linalg.api.buffer.DataType
Imports INDArray = DeepLearning4VB.org.nd4vb.linalg.api.ndarray.INDArray
Imports DynamicCustomOp = DeepLearning4VB.org.nd4vb.linalg.api.ops.DynamicCustomOp
Imports Nd4vbT = DeepLearning4VB.org.nd4vb.linalg.factory.Nd4vb
Imports ArrayUtil = DeepLearning4VB.org.nd4vb.common.util.ArrayUtil
Imports DeepLearning4VB.org.nd4vb.linalg.api.buffer
#If SAMEDIFF Then
Imports SDVariable = DeepLearning4VB.org.nd4vb.autodiff.samediff.SDVariable
#End If
#If MODEL_IMPORT Then
Imports Onnx = onnx.Onnx
Imports SameDiffOp = DeepLearning4VB.org.nd4vb.autodiff.samediff.internal.SameDiffOp
Imports TFGraphMapper = DeepLearning4VB.org.nd4vb.imports.graphmapper.tf.TFGraphMapper
Imports Conv1DConfig = DeepLearning4VB.org.nd4vb.linalg.api.ops.impl.layers.convolution.config.Conv1DConfig
Imports Conv2DConfig = DeepLearning4VB.org.nd4vb.linalg.api.ops.impl.layers.convolution.config.Conv2DConfig
Imports Conv3DConfig = DeepLearning4VB.org.nd4vb.linalg.api.ops.impl.layers.convolution.config.Conv3DConfig
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

Namespace org.nd4vb.linalg.api.ops.impl.layers.convolution

    ''' <summary>The custom op <c>batchnorm</c> (Java <c>org.nd4j.linalg.api.ops.impl.layers.convolution.BatchNorm</c>).</summary>
    Public Class BatchNorm
        Inherits DynamicCustomOp

        Public Sub New()
        End Sub

        Private applyGamma As Boolean
        Private applyBeta As Boolean
        Private epsilon As Double
        Private jaxis() As Integer

#If SAMEDIFF Then
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Builder(builderMethodName = "builder") public BatchNorm(org.nd4j.autodiff.samediff.SameDiff sameDiff, org.nd4j.autodiff.samediff.SDVariable[] inputFunctions, org.nd4j.linalg.api.ndarray.INDArray[] inputArrays, org.nd4j.linalg.api.ndarray.INDArray[] outputArrays, boolean inPlace, boolean applyGamma, boolean applyBeta, double epsilon, int[] axis)
		Public Sub New(ByVal sameDiff As SameDiff, ByVal inputFunctions() As SDVariable, ByVal inputArrays() As INDArray, ByVal outputArrays() As INDArray, ByVal inPlace As Boolean, ByVal applyGamma As Boolean, ByVal applyBeta As Boolean, ByVal epsilon As Double, ByVal axis() As Integer)
			MyBase.New(Nothing,sameDiff, inputFunctions, inPlace)
			Preconditions.checkState(axis IsNot Nothing AndAlso axis.Length > 0, "Invalid axis argument: axis must be specified" & "and length > 0. Got %s", axis)
			Me.sameDiff = sameDiff

			Me.applyGamma = applyGamma
			Me.applyBeta = applyBeta
			Me.epsilon = epsilon
			Me.jaxis = axis
			If inputArrays IsNot Nothing Then
				addInputArgument(inputArrays)
			End If
			If outputArrays IsNot Nothing Then
				addOutputArgument(outputArrays)
			End If
			addArgs()
		End Sub

		Public Sub New(ByVal sameDiff As SameDiff, ByVal input As SDVariable, ByVal mean As SDVariable, ByVal variance As SDVariable, ByVal gamma As SDVariable, ByVal beta As SDVariable, ByVal epsilon As Double, ByVal axis() As Integer)
			MyBase.New(Nothing,sameDiff, wrapFilterNull(input, mean, variance, gamma, beta), False)
			Preconditions.checkState(axis IsNot Nothing AndAlso axis.Length > 0, "Invalid axis argument: axis must be specified" & "and length > 0. Got %s", axis)
			Me.sameDiff = sameDiff
			Me.applyBeta = beta IsNot Nothing
			Me.applyGamma = gamma IsNot Nothing
			Me.epsilon = epsilon
			Me.jaxis = axis
			addArgs()
		End Sub
#End If

        Public Sub New(ByVal input As INDArray, ByVal mean As INDArray, ByVal variance As INDArray, ByVal gamma As INDArray, ByVal beta As INDArray, ByVal epsilon As Double, ParamArray ByVal axis() As Integer)
            MyBase.New(wrapFilterNull(input, mean, variance, gamma, beta), Nothing)
            Me.jaxis = axis
            Me.applyBeta = beta IsNot Nothing
            Me.applyGamma = gamma IsNot Nothing
            Me.epsilon = epsilon
            addArgs()
        End Sub

        Public Overridable Sub addArgs()
            addIArgument(ArrayUtil.fromBoolean(applyGamma))
            addIArgument(ArrayUtil.fromBoolean(applyBeta))
            If jaxis IsNot Nothing Then
                'If null: op defaults to last dimension
                axis.Clear()
                For Each v In jaxis
                    axis.Add(v)
                Next v
                addIArgument(jaxis)
            End If
            addTArgument(epsilon)
        End Sub

        Public Overrides Function propertiesForFunction() As IDictionary(Of String, Object)
            Dim ret As IDictionary(Of String, Object) = New JavaCompat.LinkedHashMap(Of String, Object)()
            ret("applyGamma") = applyGamma
            ret("applyBeta") = applyBeta
            ret("epsilon") = epsilon
            ret("axis") = axis
            Return ret
        End Function

#If MODEL_IMPORT Then
		Public Overrides Sub initFromTensorFlow(ByVal nodeDef As NodeDef, ByVal initWith As SameDiff, ByVal attributesForNode As IDictionary(Of String, AttrValue), ByVal graph As GraphDef)
			TFGraphMapper.initFunctionFromProperties(nodeDef.getOp(), Me, attributesForNode, nodeDef, graph)
			'Switch order: TF uses [input, gamma, beta, mean, variance]; libnd4j expects [input, mean, variance, gamma, beta]
			Dim op As SameDiffOp = initWith.getOps().get(Me.getOwnName())
			Dim list As IList(Of String) = op.getInputsToOp()
			Dim newList As IList(Of String) = New List(Of String) From {list(0), list(3), list(4), list(1), list(2)}
			op.InputsToOp = newList

			Me.applyGamma = True
			Me.applyBeta = True
			Me.epsilon = attributesForNode("epsilon").getF()

			If attributesForNode.ContainsKey("data_format") Then
				Dim dataFormat As String = attributesForNode("data_format").getS().toStringUtf8()
				'TODO not sure if these conv1d/3d cases appear. But BN definitely uses "NCHW" or "NHWC"
				If dataFormat.Equals(Conv2DConfig.NCHW, StringComparison.OrdinalIgnoreCase) OrElse dataFormat.Equals(Conv1DConfig.NCW, StringComparison.OrdinalIgnoreCase) OrElse dataFormat.Equals(Conv3DConfig.NCDHW_Conflict, StringComparison.OrdinalIgnoreCase) Then
					jaxis = New Integer(){1}
				ElseIf dataFormat.Equals(Conv2DConfig.NHWC_Conflict, StringComparison.OrdinalIgnoreCase) Then
					jaxis = New Integer(){3}
				ElseIf dataFormat.Equals(Conv1DConfig.NWC_Conflict, StringComparison.OrdinalIgnoreCase) Then
					jaxis = New Integer(){2}
				ElseIf dataFormat.Equals(Conv3DConfig.NDHWC, StringComparison.OrdinalIgnoreCase) Then
					jaxis = New Integer(){4}
				Else
					Throw New System.InvalidOperationException("Unknown data format: """ & dataFormat & """")
				End If
			End If

			addArgs()
		End Sub

		Public Overrides Sub initFromOnnx(ByVal node As Onnx.NodeProto, ByVal initWith As SameDiff, ByVal attributesForNode As IDictionary(Of String, Onnx.AttributeProto), ByVal graph As Onnx.GraphProto)

		End Sub
#End If

        Public Overrides Function opName() As String
            Return "batchnorm"
        End Function

        Public Overrides Function onnxName() As String
            Return "BatchNormalization"
        End Function

        Public Overrides Function tensorflowName() As String
            Return "FusedBatchNorm"
        End Function

#If SAMEDIFF Then
		Public Overrides Function doDiff(ByVal f1 As IList(Of SDVariable)) As IList(Of SDVariable)
			Dim ret As IList(Of SDVariable) = New List(Of SDVariable)()
			Dim inputs As IList(Of SDVariable) = New List(Of SDVariable) From {args()}
			inputs.Add(f1(0))
			Dim batchNormDerivative As BatchNormDerivative = BatchNormDerivative.derivativeBuilder().sameDiff(sameDiff).inputFunctions(CType(inputs, List(Of SDVariable)).ToArray()).applyGamma(applyGamma).applyBeta(applyBeta).epsilon(epsilon).axis(jaxis).build()
			CType(ret, List(Of SDVariable)).AddRange(New List(Of SDVariable) From {batchNormDerivative.outputVariables()})
			Return ret
		End Function
#End If

        Public Overrides Function calculateOutputDataTypes(ByVal inputDataTypes As IList(Of DataType)) As IList(Of DataType)
            Preconditions.checkState(inputDataTypes IsNot Nothing AndAlso inputDataTypes.Count >= 3 AndAlso inputDataTypes.Count <= 5, "Expected 3 to 5 input datatypes for %s, got %s", Me.GetType(), inputDataTypes)
            If inputDataTypes(0).isFPType() Then
                Return New List(Of DataType) From {inputDataTypes(0)}
            End If
            Return New List(Of DataType) From {Nd4vbT.defaultFloatingPointType()}
        End Function

        ' ---------------------------------------------------------------- Lombok members

        ''' <summary>Lombok getter of <c>applyGamma</c>.</summary>
        Public Overridable Function isApplyGamma() As Boolean
            Return applyGamma
        End Function

        ''' <summary>Lombok getter of <c>applyBeta</c>.</summary>
        Public Overridable Function isApplyBeta() As Boolean
            Return applyBeta
        End Function

        ''' <summary>Lombok getter of <c>epsilon</c>.</summary>
        Public Overridable Function getEpsilon() As Double
            Return epsilon
        End Function

        ''' <summary>Lombok getter of <c>jaxis</c>.</summary>
        Public Overridable Function getJaxis() As Integer()
            Return jaxis
        End Function

    End Class

End Namespace
